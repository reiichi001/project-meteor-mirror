/*
===========================================================================
Copyright (C) 2015-2019 Project Meteor Dev Team

This file is part of Project Meteor Server.

Project Meteor Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Project Meteor Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with Project Meteor Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/


using Meteor.Common;
using Meteor.Map.actors.area;
using Meteor.Map.actors.group;
using Meteor.Map.Actors;
using Meteor.Map.lua;
using Meteor.Map.packets.send.actor;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;

namespace Meteor.Map.actors.director
{
    class Director : Actor
    {
        private uint directorId;
        private string directorScriptPath;
        private List<Actor> members = new List<Actor>();
        protected ContentGroup contentGroup;
        private bool isCreated = false;
        private bool isDeleted = false;
        private bool isDeleting = false;

        private Script directorScript;
        private Coroutine currentCoroutine;

        public Director(uint id, Area zone, string directorPath, bool hasContentGroup, params object[] args)
            : base((6 << 28 | zone.actorId << 19 | (uint)id))
        {
            directorId = id;
            this.zone = zone;
            this.zoneId = zone.actorId;
            directorScriptPath = directorPath;

            LoadLuaScript();

            if (hasContentGroup)
                contentGroup = Server.GetWorldManager().CreateContentGroup(this, GetMembers());

            eventConditions = new EventList();
            eventConditions.noticeEventConditions = new List<EventList.NoticeEventCondition>();
            eventConditions.noticeEventConditions.Add(new EventList.NoticeEventCondition("noticeEvent",  0xE,0x0));
            eventConditions.noticeEventConditions.Add(new EventList.NoticeEventCondition("noticeRequest", 0x0, 0x1));
            eventConditions.noticeEventConditions.Add(new EventList.NoticeEventCondition("reqForChild", 0x0, 0x1));            
        }       

        public override SubPacket CreateScriptBindPacket()
        {
            List<LuaParam> actualLParams = new List<LuaParam>();
            actualLParams.Insert(0, new LuaParam(2, classPath));
            actualLParams.Insert(1, new LuaParam(4, 4));
            actualLParams.Insert(2, new LuaParam(4, 4));
            actualLParams.Insert(3, new LuaParam(4, 4));
            actualLParams.Insert(4, new LuaParam(4, 4));
            actualLParams.Insert(5, new LuaParam(4, 4));

            List<LuaParam> lparams = LuaEngine.GetInstance().CallLuaFunctionForReturn(null, this, "init", false);
            for (int i = 1; i < lparams.Count; i++)
                actualLParams.Add(lparams[i]);

            return ActorInstantiatePacket.BuildPacket(actorId, actorName, className, actualLParams);
        }

        public override List<SubPacket> GetSpawnPackets(ushort spawnType = 1)
        {
            List<SubPacket> subpackets = new List<SubPacket>();
            subpackets.Add(CreateAddActorPacket(0));
            subpackets.AddRange(GetEventConditionPackets());
            subpackets.Add(CreateSpeedPacket());
            subpackets.Add(CreateSpawnPositonPacket(0));
            subpackets.Add(CreateNamePacket());
            subpackets.Add(CreateStatePacket());
            subpackets.Add(CreateIsZoneingPacket());
            subpackets.Add(CreateScriptBindPacket());
            return subpackets;
        }

        public override List<SubPacket> GetInitPackets()
        {
            List<SubPacket> subpackets = new List<SubPacket>();
            SetActorPropetyPacket initProperties = new SetActorPropetyPacket("/_init");
            initProperties.AddTarget();
            subpackets.Add(initProperties.BuildPacket(actorId));
            return subpackets;
        }

        public void OnTalkEvent(Player player, Npc npc)
        {
            LuaEngine.GetInstance().CallLuaFunction(player, this, "onTalkEvent", false, npc);
        }

        public void OnCommandEvent(Player player, Command command)
        {
            LuaEngine.GetInstance().CallLuaFunction(player, this, "onCommandEvent", false, command);
        }   

        public void StartDirector(bool spawnImmediate, params object[] args)
        {
            object[] args2 = new object[args.Length + 1];
            args2[0] = this;
            Array.Copy(args, 0, args2, 1, args.Length);

            List<LuaParam> lparams = CallLuaScript("init", args2);
            
            if (lparams != null && lparams.Count >= 1 && lparams[0].value is string)
            {
                classPath = (string)lparams[0].value;
                className = classPath.Substring(classPath.LastIndexOf("/") + 1);
                GenerateActorName((int)directorId);
                isCreated = true;
            }

            if (isCreated && spawnImmediate)
            {
                if (contentGroup != null)
                    contentGroup.Start();

                foreach (Player p in GetPlayerMembers())
                {
                    p.QueuePackets(GetSpawnPackets());
                    p.QueuePackets(GetInitPackets());
                }
            }

            if (this is GuildleveDirector)
            {               
                ((GuildleveDirector)this).LoadGuildleve();
            }

            CallLuaScript("main", this, contentGroup);
        }

        public void StartContentGroup()
        {
            if (contentGroup != null)
                contentGroup.Start();
        }

        public void EndDirector()
        {
            isDeleting = true;

            if (contentGroup != null)
                contentGroup.DeleteGroup();

            if (this is GuildleveDirector)
                ((GuildleveDirector)this).EndGuildleveDirector();

            List<Actor> players = GetPlayerMembers();
            foreach (Actor player in players)
                ((Player)player).RemoveDirector(this);
            members.Clear();
            isDeleted = true;
            Server.GetWorldManager().GetZone(zoneId).DeleteDirector(actorId);
        }
        
        public void AddMember(Actor actor)
        {
            if (!members.Contains(actor))
            {
                members.Add(actor);

                if (actor is Player)
                    ((Player)actor).AddDirector(this);

                if (contentGroup != null)
                    contentGroup.AddMember(actor);
            }
        }

        public void RemoveMember(Actor actor)
        {
            if (members.Contains(actor))
                members.Remove(actor);
            if (contentGroup != null)
                contentGroup.RemoveMember(actor.actorId);
            if (GetPlayerMembers().Count == 0 && !isDeleting)
                EndDirector();
        }

        public List<Actor> GetMembers()
        {
            return members;
        }

        public List<Actor> GetPlayerMembers()
        {
            return members.FindAll(s => s is Player);
        }

        public List<Actor> GetNpcMembers()
        {
            return members.FindAll(s => s is Npc);
        }

        public bool IsCreated()
        {
            return isCreated;
        }

        public bool IsDeleted()
        {
            return isDeleted;
        }

        public bool HasContentGroup()
        {
            return contentGroup != null;
        }

        public ContentGroup GetContentGroup()
        {
            return contentGroup;
        }

        public void GenerateActorName(int actorNumber)
        {            
            //Format Class Name
            string className = this.className;
                
            className = Char.ToLowerInvariant(className[0]) + className.Substring(1);

            //Format Zone Name
            string zoneName = zone.zoneName.Replace("Field", "Fld")
                                           .Replace("Dungeon", "Dgn")
                                           .Replace("Town", "Twn")
                                           .Replace("Battle", "Btl")
                                           .Replace("Test", "Tes")
                                           .Replace("Event", "Evt")
                                           .Replace("Ship", "Shp")
                                           .Replace("Office", "Ofc");
            if (zone is PrivateArea)
            {
                //Check if "normal"
                zoneName = zoneName.Remove(zoneName.Length - 1, 1) + "P";
            }
            zoneName = Char.ToLowerInvariant(zoneName[0]) + zoneName.Substring(1);

            try
            {
                className = className.Substring(0, 20 - zoneName.Length);
            }
            catch (ArgumentOutOfRangeException)
            { }

            //Convert actor number to base 63
            string classNumber = Utils.ToStringBase63(actorNumber);

            //Get stuff after @
            uint zoneId = zone.actorId;
            uint privLevel = 0;
            if (zone is PrivateArea)
                privLevel = ((PrivateArea)zone).GetPrivateAreaType();

            actorName = String.Format("{0}_{1}_{2}@{3:X3}{4:X2}", className, zoneName, classNumber, zoneId, privLevel);
        }

        public string GetScriptPath()
        {
            return directorScriptPath;
        }

        private void LoadLuaScript()
        {
            string luaPath = String.Format(LuaEngine.FILEPATH_DIRECTORS, GetScriptPath());
            directorScript = LuaEngine.LoadScript(luaPath);
            if (directorScript == null)
                Program.Log.Error("Could not find script for director {0}.", GetName());
        }

        private List<LuaParam> CallLuaScript(string funcName, params object[] args)
        {
            if (directorScript != null)
            {
                directorScript = LuaEngine.LoadScript(String.Format(LuaEngine.FILEPATH_DIRECTORS, directorScriptPath));
                if (!directorScript.Globals.Get(funcName).IsNil())
                {
                    DynValue result = directorScript.Call(directorScript.Globals[funcName], args);
                    List<LuaParam> lparams = LuaUtils.CreateLuaParamList(result);
                    return lparams;
                }
                else
                    Program.Log.Error("Could not find script for director {0}.", GetName());
            }
            return null;
        }

        private List<LuaParam> StartCoroutine(string funcName, params object[] args)
        {
            if (directorScript != null)
            {
                if (!directorScript.Globals.Get(funcName).IsNil())
                {
                    currentCoroutine = directorScript.CreateCoroutine(directorScript.Globals[funcName]).Coroutine;
                    DynValue value = currentCoroutine.Resume(args);
                    LuaEngine.GetInstance().ResolveResume(null, currentCoroutine, value);
                }
                else
                    Program.Log.Error("Could not find script for director {0}.", GetName());
            }
            return null;
        }

        public void OnEventStart(Player player, object[] args)
        {
            object[] args2 = new object[args.Length + (player == null ? 1 : 2)];
            Array.Copy(args, 0, args2, (player == null ? 1 : 2), args.Length);
            if (player != null)
            {
                args2[0] = player;
                args2[1] = this;
            }
            else
                args2[0] = this;

            Coroutine coroutine = directorScript.CreateCoroutine(directorScript.Globals["onEventStarted"]).Coroutine;
            DynValue value = coroutine.Resume(args2);
            LuaEngine.GetInstance().ResolveResume(player, coroutine, value);
        }
    }    
}

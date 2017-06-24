
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.area;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send.actor;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.IO;

namespace FFXIVClassic_Map_Server.actors.director
{
    class Director : Actor
    {
        private uint directorId;
        private string directorScriptPath;
        private List<Actor> members = new List<Actor>();
        private bool isCreated = false;

        private Script directorScript;
        private Coroutine currentCoroutine;

        public Director(uint id, Area zone, string directorPath, params object[] args)
            : base((6 << 28 | zone.actorId << 19 | (uint)id))
        {
            directorId = id;
            this.zone = zone;
            directorScriptPath = directorPath;

            LoadLuaScript();

            eventConditions = new EventList();
            eventConditions.noticeEventConditions = new List<EventList.NoticeEventCondition>();
            eventConditions.noticeEventConditions.Add(new EventList.NoticeEventCondition("noticeEvent",  0xE,0x0));
            eventConditions.noticeEventConditions.Add(new EventList.NoticeEventCondition("noticeRequest", 0x0, 0x1));
            eventConditions.noticeEventConditions.Add(new EventList.NoticeEventCondition("reqForChild", 0x0, 0x1));            
        }       

        public override SubPacket CreateScriptBindPacket(uint playerActorId)
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

            return ActorInstantiatePacket.BuildPacket(actorId, playerActorId, actorName, className, actualLParams);
        }

        public override BasePacket GetSpawnPackets(uint playerActorId, ushort spawnType)
        {
            List<SubPacket> subpackets = new List<SubPacket>();
            subpackets.Add(CreateAddActorPacket(playerActorId, 0));
            subpackets.AddRange(GetEventConditionPackets(playerActorId));
            subpackets.Add(CreateSpeedPacket(playerActorId));
            subpackets.Add(CreateSpawnPositonPacket(playerActorId, 0));
            subpackets.Add(CreateNamePacket(playerActorId));
            subpackets.Add(CreateStatePacket(playerActorId));
            subpackets.Add(CreateIsZoneingPacket(playerActorId));
            subpackets.Add(CreateScriptBindPacket(playerActorId));
            return BasePacket.CreatePacket(subpackets, true, false);
        }        

        public override BasePacket GetInitPackets(uint playerActorId)
        {
            SetActorPropetyPacket initProperties = new SetActorPropetyPacket("/_init");
            initProperties.AddTarget();
            return BasePacket.CreatePacket(initProperties.BuildPacket(playerActorId, actorId), true, false);
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
            
            if (lparams.Count >= 1 && lparams[0].value is string)
            {
                classPath = (string)lparams[0].value;
                className = classPath.Substring(classPath.LastIndexOf("/") + 1);
                GenerateActorName((int)directorId);
                isCreated = true;
            }

            if (isCreated && spawnImmediate)
            {
                foreach (Player p in GetPlayerMembers())
                {
                    GetSpawnPackets(actorId).DebugPrintPacket();
                    p.QueuePacket(GetSpawnPackets(actorId));
                    p.QueuePacket(GetInitPackets(actorId));
                }
            }
        }

        public void AddMember(Actor actor)
        {
            if (!members.Contains(actor))
                members.Add(actor);
        }

        public void RemoveMember(Actor actor)
        {
            if (members.Contains(actor))
                members.Remove(actor);
            if (members.Count == 0)
                Server.GetWorldManager().GetZone(zoneId).DeleteDirector(actorId);
        }

        public void RemoveMembers()
        {
            members.Clear();
            Server.GetWorldManager().GetZone(zoneId).DeleteDirector(actorId);
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
            catch (ArgumentOutOfRangeException e)
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

    }    
}

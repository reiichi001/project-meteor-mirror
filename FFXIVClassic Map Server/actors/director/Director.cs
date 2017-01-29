
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
        private List<Actor> childrenOwners = new List<Actor>();
        private bool isCreated = false;

        public Director(uint id, Area zone, string directorPath)
            : base((6 << 28 | zone.actorId << 19 | (uint)id))
        {
            directorId = id;
            this.zone = zone;
            directorScriptPath = directorPath;            
            DoActorInit(directorScriptPath);
            GenerateActorName((int)id);

            eventConditions = new EventList();
            eventConditions.noticeEventConditions = new List<EventList.NoticeEventCondition>();
            eventConditions.noticeEventConditions.Add(new EventList.NoticeEventCondition("noticeEvent",  0xE,0x0));
            eventConditions.noticeEventConditions.Add(new EventList.NoticeEventCondition("noticeRequest",0x0,0x1));
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
            actualLParams.Insert(6, new LuaParam(0, (int)0x13883));

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

        public void OnTalked(Player player, Npc npc)
        {
            LuaEngine.DoDirectorOnTalked(this, player, npc);
        }

        public void OnCommand(Player player, Command command)
        {
            LuaEngine.DoDirectorOnCommand(this, player, command);
        }

        public void AddChild(Actor actor)
        {
            if (!childrenOwners.Contains(actor))
                childrenOwners.Add(actor);
        }

        public void RemoveChild(Actor actor)
        {
            if (childrenOwners.Contains(actor))
                childrenOwners.Remove(actor);
            if (childrenOwners.Count == 0)
                Server.GetWorldManager().GetZone(zoneId).DeleteDirector(actorId);
        }

        public void RemoveChildren()
        {
            childrenOwners.Clear();
            Server.GetWorldManager().GetZone(zoneId).DeleteDirector(actorId);
        }

        public void DoActorInit(string directorPath)
        {
            LuaScript script = null;

            if (File.Exists("./scripts/directors/" + directorPath + ".lua"))
                script = LuaEngine.LoadScript("./scripts/directors/" + directorPath + ".lua");
            else
                return;

            DynValue result;

            if (script != null && script.Globals["init"] != null)
                result = script.Call(script.Globals["init"], this);
            else
                return;

            List<LuaParam> lparams = LuaUtils.CreateLuaParamList(result);

            if (lparams.Count == 1 && lparams[0].value is string)
            {
                classPath = (string)lparams[0].value;
                className = classPath.Substring(classPath.LastIndexOf("/") + 1);
                isCreated = true;
            }
        }

        public Coroutine GetEventStartCoroutine(Player player)
        {
            LuaScript script = null;

            if (File.Exists("./scripts/directors/" + directorScriptPath + ".lua"))
                script = LuaEngine.LoadScript("./scripts/directors/" + directorScriptPath + ".lua");
            else
                return null;
            
            //Run Script
            Coroutine coroutine;

            if (script != null && !script.Globals.Get("onEventStarted").IsNil())
                coroutine = script.CreateCoroutine(script.Globals["onEventStarted"]).Coroutine;            
            else
                return null;

            return coroutine;
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
                privLevel = ((PrivateArea)zone).GetPrivateAreaLevel();

            actorName = String.Format("{0}_{1}_{2}@{3:X3}{4:X2}", className, zoneName, classNumber, zoneId, privLevel);
        }

    }    
}

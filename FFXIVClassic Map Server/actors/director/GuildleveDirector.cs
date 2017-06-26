using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.area;
using FFXIVClassic_Map_Server.actors.director.Work;
using FFXIVClassic_Map_Server.actors.group;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.director
{
    class GuildleveDirector : Director
    {
        public uint guildleveId;
        public Player guildleveOwner;        
        public byte selectedDifficulty;
        public ContentGroup contentGroup;

        public GuildleveData guildleveData;
        public GuildleveWork guildleveWork = new GuildleveWork();

        public bool isEnded = false;
        public uint completionTime = 0;

        public GuildleveDirector(uint id, Area zone, string directorPath, uint guildleveId, byte selectedDifficulty, Player guildleveOwner, params object[] args)
            : base(id, zone, directorPath, args)
        {
            this.guildleveId = guildleveId;
            this.selectedDifficulty = selectedDifficulty;
            this.guildleveData = Server.GetGuildleveGamedata(guildleveId);
            this.guildleveOwner = guildleveOwner;

            guildleveWork.aimNum[0] = guildleveData.aimNum[0];
            guildleveWork.aimNum[1] = guildleveData.aimNum[1];
            guildleveWork.aimNum[2] = guildleveData.aimNum[2];
            guildleveWork.aimNum[3] = guildleveData.aimNum[3];

            if (guildleveWork.aimNum[0] != 0)
                guildleveWork.uiState[0] = 1;
            if (guildleveWork.aimNum[1] != 0)
                guildleveWork.uiState[1] = 1;
            if (guildleveWork.aimNum[2] != 0)
                guildleveWork.uiState[2] = 1;
            if (guildleveWork.aimNum[3] != 0)
                guildleveWork.uiState[3] = 1;

            guildleveWork.aimNumNow[0] = guildleveWork.aimNumNow[1] = guildleveWork.aimNumNow[2] = guildleveWork.aimNumNow[3] = 0;
        }

        public void LoadGuildleve()
        {
            contentGroup = Server.GetWorldManager().CreateGLContentGroup(this, GetMembers());
        }

        public void StartGuildleve()
        {
            foreach (Actor p in GetPlayerMembers())
            {
                Player player = (Player) p;

                //Set music
                if (guildleveData.location == 1)
                    player.ChangeMusic(22);
                else if (guildleveData.location == 2)
                    player.ChangeMusic(14);
                else if (guildleveData.location == 3)
                    player.ChangeMusic(26);
                else if (guildleveData.location == 4)
                    player.ChangeMusic(16);

                //Show Start Messages
                player.SendGameMessage(Server.GetWorldManager().GetActor(), 50022, 0x20, guildleveId, selectedDifficulty);
                player.SendDataPacket("attention", Server.GetWorldManager().GetActor(), "", 50022, guildleveId, selectedDifficulty);
                player.SendGameMessage(Server.GetWorldManager().GetActor(), 50026, 0x20, (object)(int)guildleveData.timeLimit);
            }

            guildleveWork.startTime = Utils.UnixTimeStampUTC();
            ActorPropertyPacketUtil propertyBuilder = new ActorPropertyPacketUtil("guildleveWork/start", this, actorId);
            propertyBuilder.AddProperty("guildleveWork.startTime");
            SendPacketsToPlayers(propertyBuilder.Done());            
        }

        public void EndGuildleve(bool wasCompleted)
        {
            if (isEnded)
                return;
            isEnded = true;

            completionTime = Utils.UnixTimeStampUTC() - guildleveWork.startTime;

            if (wasCompleted)
            {
                foreach (Actor a in GetPlayerMembers())
                {
                    Player player = (Player)a;
                    player.MarkGuildleve(guildleveId, true, true);
                    player.PlayAnimation(0x02000002, true);
                    player.ChangeMusic(81);
                    player.SendGameMessage(Server.GetWorldManager().GetActor(), 50023, 0x20, (object)(int)guildleveId);
                    player.SendDataPacket("attention", Server.GetWorldManager().GetActor(), "", 50023, (object)(int)guildleveId);
                }
            }

            foreach (Actor a in GetNpcMembers())
            {
                Npc npc = (Npc)a;
                npc.Despawn();
                RemoveMember(a);
            }

            guildleveWork.startTime = 0;
            guildleveWork.signal = -1;
            ActorPropertyPacketUtil propertyBuilder = new ActorPropertyPacketUtil("guildleveWork/signal", this, actorId);
            propertyBuilder.AddProperty("guildleveWork.signal");
            propertyBuilder.NewTarget("guildleveWork/start");
            propertyBuilder.AddProperty("guildleveWork.startTime");
            SendPacketsToPlayers(propertyBuilder.Done());
            
            if (wasCompleted)
            {
                Npc aetheryteNode = zone.SpawnActor(1200040, String.Format("{0}:warpExit", guildleveOwner.actorName), guildleveOwner.positionX, guildleveOwner.positionY, guildleveOwner.positionZ);
                contentGroup.AddMember(aetheryteNode);
            }
        }   
        
        public void AbandonGuildleve()
        {
            foreach (Actor p in GetPlayerMembers())
            {
                Player player = (Player)p;                
                player.SendGameMessage(Server.GetWorldManager().GetActor(), 50147, 0x20, (object)guildleveId);
                player.MarkGuildleve(guildleveId, true, false);
            }

            EndGuildleve(false);
            EndDirector();
        }

        //Delete ContentGroup, change music back
        public void EndGuildleveDirector()
        {
            contentGroup.DeleteGroup();
            foreach (Actor p in GetPlayerMembers())
            {
                Player player = (Player)p;
                player.ChangeMusic(player.GetZone().bgmDay);
            }
        }

        public void SyncAllInfo()
        {
            ActorPropertyPacketUtil propertyBuilder = new ActorPropertyPacketUtil("guildleveWork/infoVariable", this, actorId);

            if (guildleveWork.aimNum[0] != 0)
                propertyBuilder.AddProperty("guildleveWork.aimNum[0]");
            if (guildleveWork.aimNum[1] != 0)
                propertyBuilder.AddProperty("guildleveWork.aimNum[1]");
            if (guildleveWork.aimNum[2] != 0)
                propertyBuilder.AddProperty("guildleveWork.aimNum[2]");
            if (guildleveWork.aimNum[3] != 0)
                propertyBuilder.AddProperty("guildleveWork.aimNum[3]");

            if (guildleveWork.aimNumNow[0] != 0)
                propertyBuilder.AddProperty("guildleveWork.aimNumNow[0]");
            if (guildleveWork.aimNumNow[1] != 0)
                propertyBuilder.AddProperty("guildleveWork.aimNumNow[1]");
            if (guildleveWork.aimNumNow[2] != 0)
                propertyBuilder.AddProperty("guildleveWork.aimNumNow[2]");
            if (guildleveWork.aimNumNow[3] != 0)
                propertyBuilder.AddProperty("guildleveWork.aimNumNow[3]");

            if (guildleveWork.uiState[0] != 0)
                propertyBuilder.AddProperty("guildleveWork.uiState[0]");
            if (guildleveWork.uiState[1] != 0)
                propertyBuilder.AddProperty("guildleveWork.uiState[1]");
            if (guildleveWork.uiState[2] != 0)
                propertyBuilder.AddProperty("guildleveWork.uiState[2]");
            if (guildleveWork.uiState[3] != 0)
                propertyBuilder.AddProperty("guildleveWork.uiState[3]");

            SendPacketsToPlayers(propertyBuilder.Done());
        }

        public void UpdateAimNumNow(int index, sbyte value)
        {
            guildleveWork.aimNumNow[index] = value;
            ActorPropertyPacketUtil propertyBuilder = new ActorPropertyPacketUtil("guildleveWork/infoVariable", this, actorId);
            propertyBuilder.AddProperty(String.Format("guildleveWork.aimNumNow[{0}]", index));
            SendPacketsToPlayers(propertyBuilder.Done());
        }

        public void UpdateUiState(int index, sbyte value)
        {
            guildleveWork.uiState[index] = value;
            ActorPropertyPacketUtil propertyBuilder = new ActorPropertyPacketUtil("guildleveWork/infoVariable", this, actorId);
            propertyBuilder.AddProperty(String.Format("guildleveWork.uiState[{0}]", index));
            SendPacketsToPlayers(propertyBuilder.Done());
        }

        public void UpdateMarkers(int markerIndex, float x, float y, float z)
        {
            guildleveWork.markerX[markerIndex] = x;
            guildleveWork.markerY[markerIndex] = y;
            guildleveWork.markerZ[markerIndex] = z;
            ActorPropertyPacketUtil propertyBuilder = new ActorPropertyPacketUtil("guildleveWork/marker", this, actorId);
            propertyBuilder.AddProperty(String.Format("guildleveWork.markerX[{0}]", markerIndex));
            propertyBuilder.AddProperty(String.Format("guildleveWork.markerY[{0}]", markerIndex));
            propertyBuilder.AddProperty(String.Format("guildleveWork.markerZ[{0}]", markerIndex));
            SendPacketsToPlayers(propertyBuilder.Done());
        }

        public void SendPacketsToPlayers(List<SubPacket> packets)
        {
            List<Actor> players = GetPlayerMembers();
            foreach (Actor p in players)
            {
                ((Player)p).QueuePackets(packets);
            }
        }

        public static uint GlBorderIconIDToAnimID(uint iconId)
        {
	        return iconId - 20000;
        }

        public static uint GlPlateIconIDToAnimID(uint iconId)
        {
	        return iconId - 20020;
        }

        public static uint GetGLStartAnimationFromSheet(uint border, uint plate, bool isBoost)
        {
	        return GetGLStartAnimation(GlBorderIconIDToAnimID(border), GlPlateIconIDToAnimID(plate), isBoost);
        }

        public static uint GetGLStartAnimation(uint border, uint plate, bool isBoost)
        {
            uint borderBits = border;
	        uint plateBits = plate << 7;

            uint boostBits = isBoost ? (uint)0x8000 : (uint) 0;
	        
	        return 0x0B000000 | boostBits | plateBits | borderBits;
        }
	
    }
}

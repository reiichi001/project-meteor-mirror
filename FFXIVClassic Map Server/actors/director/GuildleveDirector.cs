using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.director.Work;
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
        public GuildleveData guildleveData;
        public GuildleveWork guildleveWork = new GuildleveWork();

        public GuildleveDirector(uint id, Area zone, string directorPath, uint guildleveId, params object[] args)
            : base(id, zone, directorPath, args)
        {
            this.guildleveId = guildleveId;
            this.guildleveData = Server.GetGuildleveGamedata(guildleveId);

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

        public void StartGuildleve()
        {
            guildleveWork.startTime = Utils.UnixTimeStampUTC();
            ActorPropertyPacketUtil propertyBuilder = new ActorPropertyPacketUtil("guildleveWork/start", this, actorId);
            propertyBuilder.AddProperty("guildleveWork.startTime");
            SendPacketsToPlayers(propertyBuilder.Done());            
        }

        public void EndGuildleve()
        {
            guildleveWork.startTime = 0;
            guildleveWork.signal = -1;
            ActorPropertyPacketUtil propertyBuilder = new ActorPropertyPacketUtil("guildleveWork/signal", this, actorId);
            propertyBuilder.AddProperty("guildleveWork.signal");
            propertyBuilder.NewTarget("guildleveWork/start");
            propertyBuilder.AddProperty("guildleveWork.startTime");
            SendPacketsToPlayers(propertyBuilder.Done());
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
            ActorPropertyPacketUtil propertyBuilder = new ActorPropertyPacketUtil("guildleveWork/infoVariable", this, actorId);
            propertyBuilder.AddProperty(String.Format("guildleveWork.aimNumNow[{0}]", index));
            SendPacketsToPlayers(propertyBuilder.Done());
        }

        public void UpdateUiState(int index, sbyte value)
        {
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

    }
}

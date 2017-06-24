using FFXIVClassic_Map_Server.actors.director.Work;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.dataobjects;
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

            guildleveWork.aimNumNow[0] = guildleveWork.aimNumNow[1] = guildleveWork.aimNumNow[2] = guildleveWork.aimNumNow[3] = 0;
        }

        public void UpdateAimNum(int index, sbyte value)
        {

        }

        public void updateUiState(int index, sbyte value)
        {

        }

    }
}

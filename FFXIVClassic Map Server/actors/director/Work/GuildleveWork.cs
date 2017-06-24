using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.director.Work
{

    class GuildleveWork
    {
        public uint startTime = 0;
        public sbyte[] aimNum = new sbyte[4];
        public sbyte[] aimNumNow = new sbyte[4];
        public sbyte[] uiState = new sbyte[4];
        public float[] markerX = new float[3];
        public float[] markerY = new float[3];
        public float[] markerZ = new float[3];
    }

}

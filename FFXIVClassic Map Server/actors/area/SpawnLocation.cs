using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.area
{
    class SpawnLocation
    {
        public uint classId;
        public uint zoneId;
        public string privAreaName;
        public uint privAreaLevel;
        public float x;
        public float y;
        public float z;
        public float rot;
        public ushort state;
        public uint animId;

        public SpawnLocation(uint classId, uint zoneId, string privAreaName, uint privAreaLevel, float x, float y, float z, float rot, ushort state, uint animId)
        {
            this.classId = classId;
            this.zoneId = zoneId;
            this.privAreaName = privAreaName;
            this.privAreaLevel = privAreaLevel;
            this.x = x;
            this.y = y;
            this.z = z;
            this.rot = rot;
            this.state = state;
            this.animId = animId;
        }
    }
}

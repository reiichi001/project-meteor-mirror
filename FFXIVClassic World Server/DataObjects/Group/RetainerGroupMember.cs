using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_World_Server.DataObjects.Group
{
    class RetainerGroupMember
    {
        public uint id;
        public string name;
        public uint classActorId;
        public byte cdIDOffset;
        public ushort placeName;
        public byte conditions;
        public byte level;

        public RetainerGroupMember(uint id, string name, uint classActorId, byte cdIDOffset, ushort placeName, byte conditions, byte level)
        {
            this.id = id;
            this.name = name;
            this.classActorId = classActorId;
            this.cdIDOffset = cdIDOffset;
            this.placeName = placeName;
            this.conditions = conditions;
            this.level = level;
        }
    }
}

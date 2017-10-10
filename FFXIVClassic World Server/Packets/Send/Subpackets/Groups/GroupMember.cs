using FFXIVClassic.Common;

namespace FFXIVClassic_World_Server.Packets.Send.Subpackets.Groups
{
    class GroupMember
    {
        public uint actorId;
        public int localizedName;
        public uint unknown2;
        public bool flag1;
        public bool isOnline;
        public string name;

        public GroupMember(uint actorId, int localizedName, uint unknown2, bool flag1, bool isOnline, string name)
        {
            this.actorId = actorId;
            this.localizedName = localizedName;
            this.unknown2 = unknown2;
            this.flag1 = flag1;
            this.isOnline = isOnline;
            this.name = name;
        }
    }
}

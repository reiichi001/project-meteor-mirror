namespace FFXIVClassic_Map_Server.Actors.Chara
{
    class NpcWork
    {
        public static byte HATE_TYPE_NONE = 0;
        public static byte HATE_TYPE_ENGAGED = 2;
        public static byte HATE_TYPE_ENGAGED_PARTY = 3;

        public ushort pushCommand;
        public int pushCommandSub;
        public byte pushCommandPriority;
        public byte hateType = 1;                
    }
}

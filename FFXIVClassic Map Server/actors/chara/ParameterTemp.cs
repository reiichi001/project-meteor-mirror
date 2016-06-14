namespace FFXIVClassic_Map_Server.Actors.Chara
{
    class ParameterTemp
    {
        public short tp = 0;

        public int tarGetInformation = 0;

        public ushort[] maxCommandRecastTime = new ushort[40];

        public float[] forceControl_float_forClientSelf = { 1.0f, 1.0f, 0.0f, 0.0f};
        public short[] forceControl_int16_forClientSelf = { -1, -1 };

        public byte[] otherClassAbilityCount = new byte[2];
        public byte[] giftCount = new byte[2];
    }
}

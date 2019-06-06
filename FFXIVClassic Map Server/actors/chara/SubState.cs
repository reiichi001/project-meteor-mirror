namespace FFXIVClassic_Map_Server.actors.chara
{
    class SubState
    {
        public byte breakage = 0;
        public byte chantId = 0;
        public byte guard = 0;
        public byte waste = 0;
        public byte mode = 0;
        public ushort motionPack = 0;

        public void toggleBreak(int index, bool toggle)
        {
            if (index > 7 || index < 0)
                return;

            if (toggle)
                breakage = (byte)(breakage | (1 << index)); 
            else
                breakage = (byte)(breakage & ~(1 << index)); 
        }

        public void setChant(byte chant) {
            chantId = chant;
        }

        public void setGuard(byte guard)
        {
            if (guard >= 0 && guard <= 3)
                this.guard = guard;
        }

        public void setWaste(byte waste)
        {
            if (waste >= 0 && waste <= 3)
                this.waste = waste;
        }

        public void setMode(byte bitfield)
        {
            mode = bitfield;
        }

        public void setMotionPack(ushort mp)
        {
            motionPack = mp;
        }

    }
}

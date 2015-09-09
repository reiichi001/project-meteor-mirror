using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Lobby_Server.dataobjects
{
    class CharaInfo
    {
        public uint tribe = 0;
        public uint size = 0;
        public uint voice = 0;
        public uint skinColor = 0;

        public uint hairStyle = 0;
        public uint hairColor = 0;
        public uint eyeColor = 0;

        public uint faceType = 0;
        public uint faceBrow = 0;
        public uint faceEye = 0;
        public uint faceIris = 0;
        public uint faceNose = 0;
        public uint faceMouth = 0;
        public uint faceJaw = 0;
        public uint faceCheek = 0;
        public uint faceOption1 = 0;
        public uint faceOption2 = 0;

        public uint guardian = 0;
        public uint birthMonth = 0;
        public uint birthDay = 0;
        public uint allegiance = 0;

        public uint weapon1 = 0;
        public uint weapon2 = 0;

        public uint headGear = 0;
        public uint bodyGear = 0;
        public uint legsGear = 0;
        public uint handsGear = 0;
        public uint feetGear = 0;
        public uint waistGear = 0;
        public uint rightEarGear = 0;
        public uint leftEarGear = 0;
        public uint rightFingerGear = 0;
        public uint leftFingerGear = 0;
        
        public byte[] toBytes()
        {
            byte[] bytes = new byte[0x120];
            return bytes;
        }

    }
}

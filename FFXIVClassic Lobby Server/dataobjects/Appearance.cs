using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Lobby_Server.dataobjects
{
    class Appearance
    {
        ////////////
        //Chara Info
        public byte size = 0;
        public byte voice = 0;
        public ushort skinColor = 0;

        public ushort hairStyle = 0;
        public ushort hairColor = 0;
        public ushort hairHighlightColor = 0;
        public ushort hairVariation = 0;
        public ushort eyeColor = 0;
        public byte characteristicsColor = 0;

        public byte faceType = 0;
        public byte faceEyebrows = 0;
        public byte faceEyeShape = 0;
        public byte faceIrisSize = 0;
        public byte faceNose = 0;
        public byte faceMouth = 0;
        public byte faceFeatures = 0;
        public byte characteristics = 0;
        public byte ears = 0;

        public uint mainHand = 0;
        public uint offHand = 0;

        public uint head = 0;
        public uint body = 0;
        public uint legs = 0;
        public uint hands = 0;
        public uint feet = 0;
        public uint waist = 0;
        public uint rightEar = 0;
        public uint leftEar = 0;
        public uint rightFinger = 0;
        public uint leftFinger = 0;
        //Chara Info
        ////////////
    }
}

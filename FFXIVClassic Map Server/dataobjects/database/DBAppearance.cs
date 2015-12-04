using FFXIVClassic_Lobby_Server.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Lobby_Server.dataobjects
{
    class DBAppearance
    {
        ////////////
        //Chara Info
        public byte tribe = 0;
        public byte size = 0;
        public byte voice = 0;
        public ushort skinColor = 0;

        public ushort hairStyle = 0;
        public ushort hairColor = 0;
        public ushort hairHighlightColor = 0;
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

        public struct FaceInfo
        {
            [BitfieldLength(5)]
            public uint characteristics;
            [BitfieldLength(3)]
            public uint characteristicsColor;
            [BitfieldLength(6)]
            public uint type;
            [BitfieldLength(2)]
            public uint ears;
            [BitfieldLength(2)]
            public uint mouth;
            [BitfieldLength(2)]
            public uint features;
            [BitfieldLength(3)]
            public uint nose;
            [BitfieldLength(3)]
            public uint eyeShape;
            [BitfieldLength(1)]
            public uint irisSize;
            [BitfieldLength(3)]
            public uint eyebrows;
            [BitfieldLength(2)]
            public uint unknown;
        }

        public FaceInfo getFaceInfo()
        {
            FaceInfo faceInfo = new FaceInfo();
            faceInfo.characteristics = characteristics;
            faceInfo.characteristicsColor = characteristicsColor;
            faceInfo.type = faceType;
            faceInfo.ears = ears;
            faceInfo.features = faceFeatures;
            faceInfo.eyebrows = faceEyebrows;
            faceInfo.eyeShape = faceEyeShape;
            faceInfo.irisSize = faceIrisSize;
            faceInfo.mouth = faceMouth;
            faceInfo.nose = faceNose;
            return faceInfo;
        }

        public static UInt32 getTribeModel(byte tribe)
        {
            switch (tribe)
            {
                //Hyur Midlander Male
                case 1:
                default:
                    return 1;

                //Hyur Midlander Female
                case 2:
                    return 2;

                //Elezen Male
                case 4:
                case 6:
                    return 3;

                //Elezen Female
                case 5:
                case 7:
                    return 4;

                //Lalafell Male
                case 8:
                case 10:
                    return 5;

                //Lalafell Female
                case 9:
                case 11:
                    return 6;

                //Miqo'te Female
                case 12:
                case 13:
                    return 8;

                //Roegadyn Male
                case 14:
                case 15:
                    return 7;

                //Hyur Highlander Male
                case 3:
                    return 9;
            }
        }

    }
}

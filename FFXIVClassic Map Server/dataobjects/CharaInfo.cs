using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Lobby_Server.common;
using System.IO;

namespace FFXIVClassic_Lobby_Server.dataobjects
{
    class CharaInfo
    {
        public uint tribe = 0;
        public uint size = 0;
        public uint voice = 0;
        public ushort skinColor = 0;

        public ushort hairStyle = 0;
        public ushort hairColor = 0;
        public ushort hairHighlightColor = 0;
        public ushort eyeColor = 0;
        public ushort characteristicsColor = 0;

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

        public uint faceType = 0;
        public uint faceEyebrows = 0;
        public uint faceEyeShape = 0;
        public uint faceIrisSize = 0;
        public uint faceNose = 0;
        public uint faceMouth = 0;
        public uint faceFeatures = 0;
        public uint characteristics = 0;
        public uint ears = 0;

        public uint guardian = 0;
        public uint birthMonth = 0;
        public uint birthDay = 0;
        public uint currentClass = 0;
        public uint currentJob = 0;
        public uint allegiance = 0;

        public uint mainHand = 0;
        public uint offHand = 0;

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

        public uint currentLevel = 1;

        public static CharaInfo getFromNewCharRequest(String encoded)
        {
            byte[] data = Convert.FromBase64String(encoded.Replace('-', '+').Replace('_', '/'));
            CharaInfo info = new CharaInfo();

            using (MemoryStream stream = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    uint version = reader.ReadUInt32();
                    uint unknown1 = reader.ReadUInt32();
                    info.tribe = reader.ReadByte();
                    info.size = reader.ReadByte();
                    info.hairStyle = reader.ReadUInt16();
                    info.hairHighlightColor = reader.ReadUInt16();
                    info.faceType = reader.ReadByte();
                    info.characteristics = reader.ReadByte();
                    info.characteristicsColor = reader.ReadByte();

                    reader.ReadUInt32();

                    info.faceEyebrows = reader.ReadByte();
                    info.faceIrisSize = reader.ReadByte();
                    info.faceEyeShape = reader.ReadByte();
                    info.faceNose = reader.ReadByte();
                    info.faceFeatures = reader.ReadByte();
                    info.faceMouth = reader.ReadByte();
                    info.ears = reader.ReadByte();
                    info.hairColor = reader.ReadUInt16();

                    reader.ReadUInt32();

                    info.skinColor = reader.ReadUInt16();
                    info.eyeColor = reader.ReadUInt16();

                    info.voice = reader.ReadByte();
                    info.guardian = reader.ReadByte();
                    info.birthMonth = reader.ReadByte();
                    info.birthDay = reader.ReadByte();
                    info.currentClass = reader.ReadUInt16();

                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    reader.ReadUInt32();

                    reader.BaseStream.Seek(0x10, SeekOrigin.Current);

                    info.allegiance = reader.ReadByte();

                }
            }



            return info;
        }

        public String buildForCharaList(DBCharacter chara)
        {
            byte[] data;

            mainHand = 79707136;
            offHand = 32509954;
            headGear = 43008;
            bodyGear = 43008;
            legsGear = 43008;
            handsGear = 43008;
            feetGear = 43008;

            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    //Build faceinfo for later
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


                    string location1 = "prv0Inn01\0";
                    string location2 = "defaultTerritory\0";

                    writer.Write((UInt32)0x000004c0);
                    writer.Write((UInt32)0x232327ea);
                    writer.Write((UInt32)System.Text.Encoding.UTF8.GetBytes(chara.name + '\0').Length);
                    writer.Write(System.Text.Encoding.UTF8.GetBytes(chara.name + '\0'));
                    writer.Write((UInt32)0x1c);
                    writer.Write((UInt32)0x04);
                    writer.Write((UInt32)getTribeModel());
                    writer.Write((UInt32)size);
                    uint colorVal = skinColor | (uint)(hairColor << 10) | (uint)(eyeColor << 20);
                    writer.Write((UInt32)colorVal);

                    var bitfield = PrimitiveConversion.ToUInt32(faceInfo);

                    writer.Write((UInt32)bitfield); //FACE, Figure this out!
                    uint hairVal = hairHighlightColor | (uint)(hairStyle << 10) | (uint)(characteristicsColor << 20);
                    writer.Write((UInt32)hairVal);
                    writer.Write((UInt32)voice);                    
                    writer.Write((UInt32)mainHand);
                    writer.Write((UInt32)offHand);

                    writer.Write((UInt32)0);
                    writer.Write((UInt32)0);
                    writer.Write((UInt32)0);
                    writer.Write((UInt32)0);
                    writer.Write((UInt32)0);

                    writer.Write((UInt32)headGear);
                    writer.Write((UInt32)bodyGear);
                    writer.Write((UInt32)legsGear);
                    writer.Write((UInt32)handsGear);
                    writer.Write((UInt32)feetGear);
                    writer.Write((UInt32)waistGear);

                    writer.Write((UInt32)0);

                    writer.Write((UInt32)rightEarGear);
                    writer.Write((UInt32)leftEarGear);

                    writer.Write((UInt32)0);
                    writer.Write((UInt32)0);

                    writer.Write((UInt32)rightFingerGear);
                    writer.Write((UInt32)leftFingerGear);

                    for (int i = 0; i < 0x8; i++)
                        writer.Write((byte)0);

                    writer.Write((UInt32)1);
                    writer.Write((UInt32)1);

                    writer.Write((byte)currentClass);
                    writer.Write((UInt16)currentLevel);
                    writer.Write((byte)currentJob);
                    writer.Write((UInt16)1);
                    writer.Write((byte)tribe);

                    writer.Write((UInt32)0xe22222aa);

                    writer.Write((UInt32)System.Text.Encoding.UTF8.GetBytes(location1).Length);
                    writer.Write(System.Text.Encoding.UTF8.GetBytes(location1));
                    writer.Write((UInt32)System.Text.Encoding.UTF8.GetBytes(location2).Length);
                    writer.Write(System.Text.Encoding.UTF8.GetBytes(location2));

                    writer.Write((byte)guardian);
                    writer.Write((byte)birthMonth);
                    writer.Write((byte)birthDay);

                    writer.Write((UInt16)0x17);
                    writer.Write((UInt32)4);
                    writer.Write((UInt32)4);

                    writer.BaseStream.Seek(0x10, SeekOrigin.Current);

                    writer.Write((UInt32)allegiance);
                    writer.Write((UInt32)allegiance);
                }

                data = stream.GetBuffer();
            }

            return Convert.ToBase64String(data).Replace('+', '-').Replace('/', '_');
        }

        public static String debug()
        {
            byte[] bytes = File.ReadAllBytes("./packets/charaInfo.bin");

            Console.WriteLine(Utils.ByteArrayToHex(bytes));

            return Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_');
        }

        public UInt32 getTribeModel()
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

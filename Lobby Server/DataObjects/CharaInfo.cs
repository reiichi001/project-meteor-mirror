/*
===========================================================================
Copyright (C) 2015-2019 Project Meteor Dev Team

This file is part of Project Meteor Server.

Project Meteor Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Project Meteor Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with Project Meteor Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

using System;
using System.IO;
using Meteor.Common;

namespace Meteor.Lobby.DataObjects
{
    class CharaInfo
    {
        public Appearance appearance;

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

        public uint guardian = 0;
        public uint birthMonth = 0;
        public uint birthDay = 0;
        public uint currentClass = 0;
        public uint currentJob = 0;
        public uint initialTown = 0;
        public uint tribe = 0;

        public ushort zoneId;
        public float x, y, z, rot;

        public uint currentLevel = 1;

        public uint weapon1;
        public uint weapon2;
        public uint head;
        public uint body;
        public uint hands;
        public uint legs;
        public uint feet;
        public uint belt;

        public static CharaInfo GetFromNewCharRequest(String encoded)
        {
            byte[] data = Convert.FromBase64String(encoded.Replace('-', '+').Replace('_', '/'));

            CharaInfo info = new CharaInfo();
            Appearance appearance = new Appearance();

            using (MemoryStream stream = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    uint version = reader.ReadUInt32();
                    uint unknown1 = reader.ReadUInt32();
                    info.tribe = reader.ReadByte();
                    appearance.size = reader.ReadByte();
                    appearance.hairStyle = reader.ReadUInt16();
                    appearance.hairHighlightColor = reader.ReadByte();
                    appearance.hairVariation = reader.ReadByte();
                    appearance.faceType = reader.ReadByte();
                    appearance.characteristics = reader.ReadByte();
                    appearance.characteristicsColor = reader.ReadByte();

                    reader.ReadUInt32();

                    appearance.faceEyebrows = reader.ReadByte();
                    appearance.faceIrisSize = reader.ReadByte();
                    appearance.faceEyeShape = reader.ReadByte();
                    appearance.faceNose = reader.ReadByte();
                    appearance.faceFeatures = reader.ReadByte();
                    appearance.faceMouth = reader.ReadByte();
                    appearance.ears = reader.ReadByte();
                    appearance.hairColor = reader.ReadUInt16();

                    reader.ReadUInt32();

                    appearance.skinColor = reader.ReadUInt16();
                    appearance.eyeColor = reader.ReadUInt16();

                    appearance.voice = reader.ReadByte();
                    info.guardian = reader.ReadByte();
                    info.birthMonth = reader.ReadByte();
                    info.birthDay = reader.ReadByte();
                    info.currentClass = reader.ReadUInt16();

                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    reader.ReadUInt32();

                    reader.BaseStream.Seek(0x10, SeekOrigin.Current);

                    info.initialTown = reader.ReadByte();

                }
            }

            info.appearance = appearance;

            return info;
        }

        public static String BuildForCharaList(Character chara, Appearance appearance)
        {
            byte[] data;
            
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    //Build faceinfo for later
                    FaceInfo faceInfo = new FaceInfo();
                    faceInfo.characteristics = appearance.characteristics;
                    faceInfo.characteristicsColor = appearance.characteristicsColor;
                    faceInfo.type = appearance.faceType;
                    faceInfo.ears = appearance.ears;
                    faceInfo.features = appearance.faceFeatures;
                    faceInfo.eyebrows = appearance.faceEyebrows;
                    faceInfo.eyeShape = appearance.faceEyeShape;
                    faceInfo.irisSize = appearance.faceIrisSize;
                    faceInfo.mouth = appearance.faceMouth;
                    faceInfo.nose = appearance.faceNose;

                    string location1 = "prv0Inn01\0";
                    string location2 = "defaultTerritory\0";

                    writer.Write((UInt32)0x000004c0);
                    writer.Write((UInt32)0x232327ea);
                    writer.Write((UInt32)System.Text.Encoding.UTF8.GetBytes(chara.name + '\0').Length);
                    writer.Write(System.Text.Encoding.UTF8.GetBytes(chara.name + '\0'));
                    writer.Write((UInt32)0x1c);
                    writer.Write((UInt32)0x04);
                    writer.Write((UInt32)GetTribeModel(chara.tribe));
                    writer.Write((UInt32)appearance.size);
                    uint colorVal = appearance.skinColor | (uint)(appearance.hairColor << 10) | (uint)(appearance.eyeColor << 20);
                    writer.Write((UInt32)colorVal);

                    var bitfield = PrimitiveConversion.ToUInt32(faceInfo);

                    writer.Write((UInt32)bitfield); //FACE, Figure this out!
                    uint hairVal = appearance.hairHighlightColor | (uint)(appearance.hairVariation << 5) | (uint)(appearance.hairStyle << 10);
                    writer.Write((UInt32)hairVal);
                    writer.Write((UInt32)appearance.voice);
                    writer.Write((UInt32)appearance.mainHand);
                    writer.Write((UInt32)appearance.offHand);

                    writer.Write((UInt32)0);
                    writer.Write((UInt32)0);
                    writer.Write((UInt32)0);
                    writer.Write((UInt32)0);
                    writer.Write((UInt32)0);

                    writer.Write((UInt32)appearance.head);
                    writer.Write((UInt32)appearance.body);
                    writer.Write((UInt32)appearance.legs);
                    writer.Write((UInt32)appearance.hands);
                    writer.Write((UInt32)appearance.feet);
                    writer.Write((UInt32)appearance.waist);

                    writer.Write((UInt32)appearance.neck);
                    writer.Write((UInt32)appearance.rightEar);
                    writer.Write((UInt32)appearance.leftEar);
                    writer.Write((UInt32)appearance.rightIndex);
                    writer.Write((UInt32)appearance.leftIndex);
                    writer.Write((UInt32)appearance.rightFinger);
                    writer.Write((UInt32)appearance.leftFinger);

                    for (int i = 0; i < 0x8; i++)
                        writer.Write((byte)0);

                    writer.Write((UInt32)1);
                    writer.Write((UInt32)1);

                    writer.Write((byte)chara.currentClass);
                    writer.Write((UInt16)chara.currentLevel);
                    writer.Write((byte)chara.currentJob);
                    writer.Write((UInt16)1);
                    writer.Write((byte)chara.tribe);

                    writer.Write((UInt32)0xe22222aa);

                    writer.Write((UInt32)System.Text.Encoding.UTF8.GetBytes(location1).Length);
                    writer.Write(System.Text.Encoding.UTF8.GetBytes(location1));
                    writer.Write((UInt32)System.Text.Encoding.UTF8.GetBytes(location2).Length);
                    writer.Write(System.Text.Encoding.UTF8.GetBytes(location2));

                    writer.Write((byte)chara.guardian);
                    writer.Write((byte)chara.birthMonth);
                    writer.Write((byte)chara.birthDay);

                    writer.Write((UInt16)0x17);
                    writer.Write((UInt32)4);
                    writer.Write((UInt32)4);

                    writer.BaseStream.Seek(0x10, SeekOrigin.Current);

                    writer.Write((UInt32)chara.initialTown);
                    writer.Write((UInt32)chara.initialTown);
                }

                data = stream.GetBuffer();
            }

            return Convert.ToBase64String(data).Replace('+', '-').Replace('/', '_');
        }

        public static String Debug()
        {
            byte[] bytes = File.ReadAllBytes("./packets/charaappearance.bin");

            Program.Log.Debug(Common.Utils.ByteArrayToHex(bytes));

            return Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_');
        }

        public static UInt32 GetTribeModel(byte tribe)
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

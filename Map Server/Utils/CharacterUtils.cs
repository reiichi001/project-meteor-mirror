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

using Meteor.Common;
using System;

namespace Meteor.Map.utils
{
    class CharacterUtils
    {
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
        
        public static FaceInfo GetFaceInfo(byte characteristics, byte characteristicsColor, byte faceType, byte ears, byte faceMouth, byte faceFeatures, byte faceNose, byte faceEyeShape, byte faceIrisSize, byte faceEyebrows)
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

        public static string GetClassNameForId(short id)
        {
            switch (id)
            {
                case 2: return "pug";
                case 3: return "gla";
                case 4: return "mrd";
                case 7: return "arc";
                case 8: return "lnc";
                case 22: return "thm";
                case 23: return "cnj";
                case 29: return "crp";
                case 30: return "bsm";
                case 31: return "arm";
                case 32: return "gsm";
                case 33: return "ltw";
                case 34: return "wvr";
                case 35: return "alc";
                case 36: return "cul";
                case 39: return "min";
                case 40: return "btn";
                case 41: return "fsh";
                default: return "undefined";
            }
        }

    }
}

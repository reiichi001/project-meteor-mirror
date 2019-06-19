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

using System.Collections.Generic;

namespace Meteor.Lobby
{
    class CharacterCreatorUtils
    {
        private static readonly Dictionary<uint, uint[]> equipmentAppearance = new Dictionary<uint, uint[]>
        {
            { 2, new uint[]{60818432,60818432,0,0,0,0,0,0,10656,10560,1024,25824,6144,0,0,0,0,0,0,0,0,0} },         //PUG
            { 3, new uint[]{79692890,0,0,0,0,0,0,0,31776,4448,1024,25824,6144,0,0,0,0,0,0,0,0,0} },                 //GLA
            { 4, new uint[]{147850310,0,0,0,0,0,0,23713,0,10016,5472,1152,6144,0,0,0,0,0,0,0,0,0} },                //MRD
            { 7, new uint[]{210764860,236979210,0,0,0,231736320,0,0,9888,9984,1024,25824,6144,0,0,0,0,0,0,0,0,0} }, //ARC
            { 8, new uint[]{168823858,0,0,0,0,0,0,0,13920,7200,1024,10656,6144,0,0,0,0,0,0,0,0,0} },                //LNC

            { 22, new uint[]{294650980,0,0,0,0,0,0,0,7744,5472,1024,5504,4096,0,0,0,0,0,0,0,0,0} },                 //THM
            { 23, new uint[]{347079700,0,0,0,0,0,0,0,4448,2240,1024,4416,4096,0,0,0,0,0,0,0,0,0} },                 //CNJ

            { 29, new uint[]{705692672,0,0,0,0,0,0,0,0,10016,10656,9632,2048,0,0,0,0,0,0,0,0,0} },                  //CRP
            { 30, new uint[]{721421372,0,0,0,0,0,0,0,0,2241,2336,2304,2048,0,0,0,0,0,0,0,0,0} },                    //BSM
            { 31, new uint[]{737149962,0,0,0,0,0,0,0,32992,2240,1024,2272,2048,0,0,0,0,0,0,0,0,0} },                //ARM
            { 32, new uint[]{752878592,0,0,0,0,0,0,0,2368,3424,1024,10656,2048,0,0,0,0,0,0,0,0,0} },                //GSM
            { 33, new uint[]{768607252,0,0,0,0,0,0,4448,4449,1792,1024,21888,2048,0,0,0,0,0,0,0,0,0} },             //LTW
            { 34, new uint[]{784335922,0,0,0,0,0,0,0,5505,5473,1024,5505,2048,0,0,0,0,0,0,0,0,0} },                 //WVR
            { 35, new uint[]{800064522,0,0,0,0,0,0,20509,5504,2241,1024,1152,2048,0,0,0,0,0,0,0,0,0} },             //ALC
            { 36, new uint[]{815793192,0,0,0,0,0,0,5632,34848,1792,1024,25825,2048,0,0,0,0,0,0,0,0,0} },            //CUL

            { 39, new uint[]{862979092,0,0,0,0,0,0,0,1184,2242,6464,6528,14336,0,0,0,0,0,0,0,0,0} },                //MIN
            { 40, new uint[]{878707732,0,0,0,0,0,0,6304,6624,6560,1024,1152,14336,0,0,0,0,0,0,0,0,0} },             //BOT
            { 41, new uint[]{894436372,0,0,0,0,0,0,6400,1184,9984,1024,6529,14336,0,0,0,0,0,0,0,0,0} },             //FSH
        };

        public static uint[] GetEquipmentForClass(uint charClass)
        {
            if (equipmentAppearance.ContainsKey(charClass))
                return equipmentAppearance[charClass];
            else
                return null;
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

        public static uint GetUndershirtForTribe(uint tribe)
        {
            uint graphicId;
            switch (tribe)
            {
                case 1:
                    graphicId = 1184;
                    break;
                case 2:
                    graphicId = 1186;
                    break;
                case 3:
                    graphicId = 1187;
                    break;
                case 4:
                    graphicId = 1184;
                    break;
                case 5:
                    graphicId = 1024;
                    break;
                case 6:
                    graphicId = 1187;
                    break;
                case 7:
                    graphicId = 1505;
                    break;
                case 8:
                    graphicId = 1184;
                    break;
                case 9:
                    graphicId = 1185;
                    break;
                case 10:
                    graphicId = 1504;
                    break;
                case 11:
                    graphicId = 1505;
                    break;
                case 12:
                    graphicId = 1216;
                    break;
                case 13:
                    graphicId = 1186;
                    break;
                case 14:
                    graphicId = 1184;
                    break;
                case 15:
                    graphicId = 1186;
                    break;
                default:
                    graphicId = 0;
                    break;
            }

            return graphicId;
        }

    }
}









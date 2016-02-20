using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Lobby_Server.utils
{
    class CharacterCreatorUtils
    {
        private static readonly Dictionary<uint, uint[]> equipmentAppearance = new Dictionary<uint, uint[]>
        {
            { 2, new uint[]{1} }, //PUG
            { 3, new uint[]{1} }, //GLA
            { 4, new uint[]{1} }, //MRD
            { 7, new uint[]{1} }, //ARC

            { 22, new uint[]{1} }, //THM
            { 23, new uint[]{1} }, //CNJ

            { 29, new uint[]{1} }, //CRP
            { 30, new uint[]{1} }, //BSM
            { 31, new uint[]{1} }, //ARM
            { 32, new uint[]{1} }, //GSM
            { 33, new uint[]{1} }, //LTW
            { 34, new uint[]{1} }, //WVR
            { 35, new uint[]{1} }, //ALC
            { 36, new uint[]{1} }, //CUL

            { 39, new uint[]{1} }, //MIN
            { 40, new uint[]{1} }, //BOT
            { 41, new uint[]{1} }, //FSH
        };

        public static uint[] getEquipmentForClass(uint charClass)
        {
            if (equipmentAppearance.ContainsKey(charClass))
                return equipmentAppearance[charClass];
            else
                return null;
        }

    }
}

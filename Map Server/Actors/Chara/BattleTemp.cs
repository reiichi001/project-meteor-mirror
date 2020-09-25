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

namespace Meteor.Map.Actors.Chara
{
    class BattleTemp
    {
        //Are these right?
        public const uint NAMEPLATE_SHOWN = 0;
        public const uint TARGETABLE = 1;
        public const uint NAMEPLATE_SHOWN2 = 2;
        //public const uint NAMEPLATE_SHOWN2 = 3;

        public const uint STAT_STRENGTH = 3;
        public const uint STAT_VITALITY = 4;
        public const uint STAT_DEXTERITY = 5;
        public const uint STAT_INTELLIGENCE = 6;
        public const uint STAT_MIND = 7;
        public const uint STAT_PIETY = 8;

        public const uint STAT_RESISTANCE_FIRE = 9;
        public const uint STAT_RESISTANCE_ICE = 10;
        public const uint STAT_RESISTANCE_WIND = 11;
        public const uint STAT_RESISTANCE_LIGHTNING = 12;
        public const uint STAT_RESISTANCE_EARTH = 13;
        public const uint STAT_RESISTANCE_WATER = 14;

        public const uint STAT_ATTACK = 17;
        public const uint STAT_ACCURACY = 15;
        public const uint STAT_NORMALDEFENSE = 18;
        public const uint STAT_EVASION = 16;
        public const uint STAT_ATTACK_MAGIC = 23;
        public const uint STAT_HEAL_MAGIC = 24;
        public const uint STAT_ENCHANCEMENT_MAGIC_POTENCY = 25;
        public const uint STAT_ENFEEBLING_MAGIC_POTENCY = 26;

        public const uint STAT_MAGIC_ACCURACY = 27;
        public const uint STAT_MAGIC_EVASION = 28;

        public const uint STAT_CRAFT_PROCESSING = 30;
        public const uint STAT_CRAFT_MAGIC_PROCESSING = 31;
        public const uint STAT_CRAFT_PROCESS_CONTROL = 32;

        public const uint STAT_HARVEST_POTENCY = 33;
        public const uint STAT_HARVEST_LIMIT = 34;
        public const uint STAT_HARVEST_RATE = 35;

        public float[] castGauge_speed = { 1.0f, 0.25f};
        public bool[]   timingCommandFlag = new bool[4];
        public short[] generalParameter = new short[35];
    }
}

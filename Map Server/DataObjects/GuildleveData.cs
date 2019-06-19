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

using MySql.Data.MySqlClient;

namespace Meteor.Map.dataobjects
{
    class GuildleveData
    {
        public readonly uint id;
        public readonly uint classType;
        public readonly uint location;
        public readonly ushort factionCreditRequired;
        public readonly ushort level;
        public readonly uint aetheryte;
        public readonly uint plateId;
        public readonly uint borderId;
        public readonly uint objective;
        public readonly byte timeLimit;
        public readonly uint skill;
        public readonly byte favorCount;

        public readonly sbyte[] aimNum = new sbyte[4];
        public readonly uint[] itemTarget = new uint[4];
        public readonly uint[] mobTarget = new uint[4];

        public GuildleveData(MySqlDataReader reader)
        {
            id = reader.GetUInt32("id");
            classType = reader.GetUInt32("classType");
            location = reader.GetUInt32("location");
            factionCreditRequired = reader.GetUInt16("factionCreditRequired");
            level = reader.GetUInt16("level");
            aetheryte = reader.GetUInt32("aetheryte");
            plateId = reader.GetUInt32("plateId");
            borderId = reader.GetUInt32("borderId");
            objective = reader.GetUInt32("objective");
            timeLimit = reader.GetByte("timeLimit");
            skill = reader.GetUInt32("skill");
            favorCount = reader.GetByte("favorCount");

            aimNum[0] = reader.GetSByte("aimNum1");
            aimNum[1] = reader.GetSByte("aimNum2");
            aimNum[2] = reader.GetSByte("aimNum3");
            aimNum[3] = reader.GetSByte("aimNum4");

            itemTarget[0] = reader.GetUInt32("item1");
            itemTarget[1] = reader.GetUInt32("item2");
            itemTarget[2] = reader.GetUInt32("item3");
            itemTarget[3] = reader.GetUInt32("item4");

            mobTarget[0] = reader.GetUInt32("mob1");
            mobTarget[1] = reader.GetUInt32("mob2");
            mobTarget[2] = reader.GetUInt32("mob3");
            mobTarget[3] = reader.GetUInt32("mob4");
        }

    }
}

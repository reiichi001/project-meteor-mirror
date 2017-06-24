using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.dataobjects
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
            factionCreditRequired = reader.GetByte("factionCreditRequired");
            level = reader.GetByte("level");
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

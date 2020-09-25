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
using System.IO;

namespace Meteor.Map.packets.send.player
{
    class SetCompletedAchievementsPacket
    {     
        //Achievenments are +1 and up, except for Quests and GCs which is +2
        public const int CATEGORY_BATTLE =             000;
        public const int CATEGORY_CHARACTER =          050;
        public const int CATEGORY_CURRENCY =           200;
        public const int CATEGORY_ITEMS =              250;
        public const int CATEGORY_SYNTHESIS =          300;
        public const int CATEGORY_GATHERING =          400;
        public const int CATEGORY_MATERIA =            550;
        public const int CATEGORY_QUESTS =             600;
        public const int CATEGORY_SEASONAL_EVENTS =    700;
        public const int CATEGORY_DUNGEONS =           750;
        public const int CATEGORY_EXPLORATION =        800;
        public const int CATEGORY_GRAND_COMPANY =      820;
        
        public const ushort OPCODE = 0x019A;
        public const uint PACKET_SIZE = 0xA0;

        public bool[] achievementFlags = new bool[1024];

        public SubPacket BuildPacket(uint sourceActorId)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    byte[] binStream = Utils.ConvertBoolArrayToBinaryStream(achievementFlags);
                    if (binStream.Length <= PACKET_SIZE - 0x20)
                        binWriter.Write(binStream);
                    else                    
                        Program.Log.Error("Failed making SetCompletedAchievements packet. Bin Stream was too big!");                    
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
        
    }
}

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
using System.IO;

namespace Meteor.Map.packets.send.actor.battle
{
    // see xtx_command
    enum CommandResultX01PacketCommand : ushort
    {
        Disengage = 12002,
        Attack = 22104,
    }

    class CommandResultX01Packet
    {
        public const ushort OPCODE = 0x0139;
        public const uint PACKET_SIZE = 0x58;

        public static SubPacket BuildPacket(uint sourceActorId, uint animationId, ushort commandId, CommandResult action)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)sourceActorId);

                    binWriter.Write((UInt32)animationId);

                    //Missing... last value is float, string in here as well?

                    binWriter.Seek(0x20, SeekOrigin.Begin);
                    binWriter.Write((UInt32)1); //Num actions (always 1 for this)
                    binWriter.Write((UInt16)commandId);
                    binWriter.Write((UInt16)0x810); //?

                    binWriter.Write((UInt32)action.targetId);

                    binWriter.Write((UInt16)action.amount);
                    binWriter.Write((UInt16)action.worldMasterTextId);

                    binWriter.Write((UInt32)action.effectId);
                    binWriter.Write((Byte)action.param);
                    binWriter.Write((Byte)action.hitNum);
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

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

using System.Collections.Generic;

namespace  Meteor.Map.packets.send.actor.battle
{
    class CommandResultX10Packet
    {
        public const ushort OPCODE = 0x013A;
        public const uint PACKET_SIZE = 0xD8;
        
        public static SubPacket BuildPacket(uint sourceActorId, uint animationId, ushort commandId, CommandResult[] actionList, ref int listOffset)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    int max;
                    if (actionList.Length - listOffset <= 10)
                        max = actionList.Length - listOffset;
                    else
                        max = 10;

                    binWriter.Write((UInt32)sourceActorId);
                    binWriter.Write((UInt32)animationId);

                    //Missing... last value is float, string in here as well?

                    binWriter.Seek(0x20, SeekOrigin.Begin);
                    binWriter.Write((UInt32)max); //Num actions
                    binWriter.Write((UInt16)commandId);
                    binWriter.Write((UInt16)0x810); //?

                    //binWriter.Seek(0x20, SeekOrigin.Begin);
                    for (int i = 0; i < max; i++)
                        binWriter.Write((UInt32)actionList[listOffset + i].targetId);

                    binWriter.Seek(0x50, SeekOrigin.Begin);
                    for (int i = 0; i < max; i++)
                        binWriter.Write((UInt16)actionList[listOffset + i].amount);

                    binWriter.Seek(0x64, SeekOrigin.Begin);
                    for (int i = 0; i < max; i++)
                        binWriter.Write((UInt16)actionList[listOffset + i].worldMasterTextId);

                    binWriter.Seek(0x78, SeekOrigin.Begin);
                    for (int i = 0; i < max; i++)
                        binWriter.Write((UInt32)actionList[listOffset + i].effectId);

                    binWriter.Seek(0xA0, SeekOrigin.Begin);
                    for (int i = 0; i < max; i++)
                        binWriter.Write((Byte)actionList[listOffset + i].param);

                    binWriter.Seek(0xAA, SeekOrigin.Begin);
                    for (int i = 0; i < max; i++)
                        binWriter.Write((Byte)actionList[listOffset + i].hitNum);

                    listOffset += max;
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }

        public static SubPacket BuildPacket(uint sourceActorId, uint animationId, ushort commandId, List<CommandResult> actionList, ref int listOffset)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    int max;
                    if (actionList.Count - listOffset <= 10)
                        max = actionList.Count - listOffset;
                    else
                        max = 10;

                    binWriter.Write((UInt32)sourceActorId);
                    binWriter.Write((UInt32)animationId);

                    //Missing... last value is float, string in here as well?

                    binWriter.Seek(0x20, SeekOrigin.Begin);
                    binWriter.Write((UInt32)max); //Num actions
                    binWriter.Write((UInt16)commandId);
                    binWriter.Write((UInt16)0x810); //?

                    //binWriter.Seek(0x20, SeekOrigin.Begin);
                    for (int i = 0; i < max; i++)
                        binWriter.Write((UInt32)actionList[listOffset + i].targetId);

                    binWriter.Seek(0x50, SeekOrigin.Begin);
                    for (int i = 0; i < max; i++)
                        binWriter.Write((UInt16)actionList[listOffset + i].amount);

                    binWriter.Seek(0x64, SeekOrigin.Begin);
                    for (int i = 0; i < max; i++)
                        binWriter.Write((UInt16)actionList[listOffset + i].worldMasterTextId);

                    binWriter.Seek(0x78, SeekOrigin.Begin);
                    for (int i = 0; i < max; i++)
                    {
                        binWriter.Write((UInt32)actionList[listOffset + i].effectId);
                    }

                    binWriter.Seek(0xA0, SeekOrigin.Begin);
                    for (int i = 0; i < max; i++)
                        binWriter.Write((Byte)actionList[listOffset + i].param);

                    binWriter.Seek(0xAA, SeekOrigin.Begin);
                    for (int i = 0; i < max; i++)
                        binWriter.Write((Byte) actionList[listOffset + i].hitNum);

                    listOffset += max;
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }

    }
}

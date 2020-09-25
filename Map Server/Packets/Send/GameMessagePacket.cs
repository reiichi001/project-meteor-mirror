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

using Meteor.Map.lua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Meteor.Common;

namespace Meteor.Map.packets.send
{
    class GameMessagePacket
    {        
        private const ushort OPCODE_GAMEMESSAGE_WITH_ACTOR1 = 0x157;
        private const ushort OPCODE_GAMEMESSAGE_WITH_ACTOR2 = 0x158;
        private const ushort OPCODE_GAMEMESSAGE_WITH_ACTOR3 = 0x159;
        private const ushort OPCODE_GAMEMESSAGE_WITH_ACTOR4 = 0x15a;
        private const ushort OPCODE_GAMEMESSAGE_WITH_ACTOR5 = 0x15b;

        private const ushort OPCODE_GAMEMESSAGE_WITH_CUSTOM_SENDER1 = 0x15c;
        private const ushort OPCODE_GAMEMESSAGE_WITH_CUSTOM_SENDER2 = 0x15d;
        private const ushort OPCODE_GAMEMESSAGE_WITH_CUSTOM_SENDER3 = 0x15e;
        private const ushort OPCODE_GAMEMESSAGE_WITH_CUSTOM_SENDER4 = 0x15f;
        private const ushort OPCODE_GAMEMESSAGE_WITH_CUSTOM_SENDER5 = 0x160;

        private const ushort OPCODE_GAMEMESSAGE_WITH_DISPID_SENDER1 = 0x161;
        private const ushort OPCODE_GAMEMESSAGE_WITH_DISPID_SENDER2 = 0x162;
        private const ushort OPCODE_GAMEMESSAGE_WITH_DISPID_SENDER3 = 0x163;
        private const ushort OPCODE_GAMEMESSAGE_WITH_DISPID_SENDER4 = 0x164;
        private const ushort OPCODE_GAMEMESSAGE_WITH_DISPID_SENDER5 = 0x165;

        private const ushort OPCODE_GAMEMESSAGE_WITHOUT_ACTOR1 = 0x166;
        private const ushort OPCODE_GAMEMESSAGE_WITHOUT_ACTOR2 = 0x167;
        private const ushort OPCODE_GAMEMESSAGE_WITHOUT_ACTOR3 = 0x168;
        private const ushort OPCODE_GAMEMESSAGE_WITHOUT_ACTOR4 = 0x169;
        private const ushort OPCODE_GAMEMESSAGE_WITHOUT_ACTOR5 = 0x16a;

        private const ushort SIZE_GAMEMESSAGE_WITH_ACTOR1 = 0x30;
        private const ushort SIZE_GAMEMESSAGE_WITH_ACTOR2 = 0x38;
        private const ushort SIZE_GAMEMESSAGE_WITH_ACTOR3 = 0x40;
        private const ushort SIZE_GAMEMESSAGE_WITH_ACTOR4 = 0x50;
        private const ushort SIZE_GAMEMESSAGE_WITH_ACTOR5 = 0x70;

        private const ushort SIZE_GAMEMESSAGE_WITH_CUSTOM_SENDER1 = 0x48;
        private const ushort SIZE_GAMEMESSAGE_WITH_CUSTOM_SENDER2 = 0x58;
        private const ushort SIZE_GAMEMESSAGE_WITH_CUSTOM_SENDER3 = 0x68;
        private const ushort SIZE_GAMEMESSAGE_WITH_CUSTOM_SENDER4 = 0x78;
        private const ushort SIZE_GAMEMESSAGE_WITH_CUSTOM_SENDER5 = 0x98;

        private const ushort SIZE_GAMEMESSAGE_WITH_DISPID_SENDER1 = 0x30;
        private const ushort SIZE_GAMEMESSAGE_WITH_DISPID_SENDER2 = 0x38;
        private const ushort SIZE_GAMEMESSAGE_WITH_DISPID_SENDER3 = 0x40;
        private const ushort SIZE_GAMEMESSAGE_WITH_DISPID_SENDER4 = 0x50;
        private const ushort SIZE_GAMEMESSAGE_WITH_DISPID_SENDER5 = 0x60;

        private const ushort SIZE_GAMEMESSAGE_WITHOUT_ACTOR1 = 0x28;
        private const ushort SIZE_GAMEMESSAGE_WITHOUT_ACTOR2 = 0x38;
        private const ushort SIZE_GAMEMESSAGE_WITHOUT_ACTOR3 = 0x38;
        private const ushort SIZE_GAMEMESSAGE_WITHOUT_ACTOR4 = 0x48;
        private const ushort SIZE_GAMEMESSAGE_WITHOUT_ACTOR5 = 0x68;

        public static SubPacket BuildPacket(uint sourceActorId, uint actorId, uint textOwnerActorId, ushort textId, byte log)
        {
            byte[] data = new byte[SIZE_GAMEMESSAGE_WITH_ACTOR1 - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {                    
                    binWriter.Write((UInt32)actorId);
                    binWriter.Write((UInt32)textOwnerActorId);
                    binWriter.Write((UInt16)textId);
                    binWriter.Write((UInt16)log);
                }
            }

            return new SubPacket(OPCODE_GAMEMESSAGE_WITH_ACTOR1, sourceActorId, data);
        }

        public static SubPacket BuildPacket(uint sourceActorId, uint actorId, uint textOwnerActorId, ushort textId, byte log, List<LuaParam> lParams)
        {
            int lParamsSize = findSizeOfParams(lParams);
            byte[] data;
            ushort opcode;

            if (lParamsSize <= 0x8)
            {
                data = new byte[SIZE_GAMEMESSAGE_WITH_ACTOR2 - 0x20];
                opcode = OPCODE_GAMEMESSAGE_WITH_ACTOR2;
            }
            else if (lParamsSize <= 0x10)
            {
                data = new byte[SIZE_GAMEMESSAGE_WITH_ACTOR3 - 0x20];
                opcode = OPCODE_GAMEMESSAGE_WITH_ACTOR3;
            }
            else if (lParamsSize <= 0x20)
            {
                data = new byte[SIZE_GAMEMESSAGE_WITH_ACTOR4 - 0x20];
                opcode = OPCODE_GAMEMESSAGE_WITH_ACTOR4;
            }
            else
            {
                data = new byte[SIZE_GAMEMESSAGE_WITH_ACTOR5 - 0x20];
                opcode = OPCODE_GAMEMESSAGE_WITH_ACTOR5;
            }

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)actorId);
                    binWriter.Write((UInt32)textOwnerActorId);
                    binWriter.Write((UInt16)textId);
                    binWriter.Write((UInt16)log);
                    LuaUtils.WriteLuaParams(binWriter, lParams);

                    if (lParamsSize <= 0x14-12)
                    {
                        binWriter.Seek(0x14, SeekOrigin.Begin);
                        binWriter.Write((UInt32)8);
                    }
                }
            }

            return new SubPacket(opcode, sourceActorId, data);
        }

        public static SubPacket BuildPacket(uint sourceActorId, uint textOwnerActorId, ushort textId, string sender, byte log)
        {
            byte[] data = new byte[SIZE_GAMEMESSAGE_WITH_CUSTOM_SENDER1 - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)textOwnerActorId);
                    binWriter.Write((UInt16)textId);
                    binWriter.Write((UInt16)log);
                    binWriter.Write(Encoding.ASCII.GetBytes(sender), 0, Encoding.ASCII.GetByteCount(sender) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(sender));
                }
            }

            return new SubPacket(OPCODE_GAMEMESSAGE_WITH_CUSTOM_SENDER1, sourceActorId, data);
        }

        public static SubPacket BuildPacket(uint sourceActorId, uint textOwnerActorId, ushort textId, string sender, byte log, List<LuaParam> lParams)
        {
            int lParamsSize = findSizeOfParams(lParams);
            byte[] data;
            ushort opcode;

            if (lParamsSize <= 0x8)
            {
                data = new byte[SIZE_GAMEMESSAGE_WITH_CUSTOM_SENDER2 - 0x20];
                opcode = OPCODE_GAMEMESSAGE_WITH_CUSTOM_SENDER2;
            }
            else if (lParamsSize <= 0x10)
            {
                data = new byte[SIZE_GAMEMESSAGE_WITH_CUSTOM_SENDER3 - 0x20];
                opcode = OPCODE_GAMEMESSAGE_WITH_CUSTOM_SENDER3;
            }
            else if (lParamsSize <= 0x20)
            {
                data = new byte[SIZE_GAMEMESSAGE_WITH_CUSTOM_SENDER4 - 0x20];
                opcode = OPCODE_GAMEMESSAGE_WITH_CUSTOM_SENDER4;
            }
            else
            {
                data = new byte[SIZE_GAMEMESSAGE_WITH_CUSTOM_SENDER5 - 0x20];
                opcode = OPCODE_GAMEMESSAGE_WITH_CUSTOM_SENDER5;
            }

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)textOwnerActorId);
                    binWriter.Write((UInt16)textId);
                    binWriter.Write((UInt16)log);
                    binWriter.Write(Encoding.ASCII.GetBytes(sender), 0, Encoding.ASCII.GetByteCount(sender) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(sender));
                    LuaUtils.WriteLuaParams(binWriter, lParams);

                    if (lParamsSize <= 0x14 - 12)
                    {
                        binWriter.Seek(0x30, SeekOrigin.Begin);
                        binWriter.Write((UInt32)8);
                    }
                }
            }

            return new SubPacket(opcode, sourceActorId, data);
        }

        public static SubPacket BuildPacket(uint sourceActorId, uint textOwnerActorId, ushort textId, uint senderDisplayNameId, byte log)
        {
            byte[] data = new byte[SIZE_GAMEMESSAGE_WITH_DISPID_SENDER1 - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)senderDisplayNameId);
                    binWriter.Write((UInt32)textOwnerActorId);
                    binWriter.Write((UInt16)textId);
                    binWriter.Write((UInt16)log);
                }
            }

            return new SubPacket(OPCODE_GAMEMESSAGE_WITH_DISPID_SENDER1, sourceActorId, data);
        }

        public static SubPacket BuildPacket(uint sourceActorId, uint textOwnerActorId, ushort textId, uint senderDisplayNameId, byte log, List<LuaParam> lParams)
        {
            int lParamsSize = findSizeOfParams(lParams);
            byte[] data;
            ushort opcode;

            if (lParamsSize <= 0x8)
            {
                data = new byte[SIZE_GAMEMESSAGE_WITH_DISPID_SENDER2 - 0x20];
                opcode = OPCODE_GAMEMESSAGE_WITH_DISPID_SENDER2;
            }
            else if (lParamsSize <= 0x10)
            {
                data = new byte[SIZE_GAMEMESSAGE_WITH_DISPID_SENDER3 - 0x20];
                opcode = OPCODE_GAMEMESSAGE_WITH_DISPID_SENDER3;
            }
            else if (lParamsSize <= 0x20)
            {
                data = new byte[SIZE_GAMEMESSAGE_WITH_DISPID_SENDER4 - 0x20];
                opcode = OPCODE_GAMEMESSAGE_WITH_DISPID_SENDER4;
            }
            else
            {
                data = new byte[SIZE_GAMEMESSAGE_WITH_DISPID_SENDER5 - 0x20];
                opcode = OPCODE_GAMEMESSAGE_WITH_DISPID_SENDER5;
            }

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)senderDisplayNameId);
                    binWriter.Write((UInt32)textOwnerActorId);
                    binWriter.Write((UInt16)textId);
                    binWriter.Write((UInt16)log);
                    LuaUtils.WriteLuaParams(binWriter, lParams);

                    if (lParamsSize <= 0x14 - 12)
                    {
                        binWriter.Seek(0x14, SeekOrigin.Begin);
                        binWriter.Write((UInt32)8);
                    }
                }
            }

            return new SubPacket(opcode, sourceActorId, data);
        }

        public static SubPacket BuildPacket(uint sourceActorId, uint textOwnerActorId, ushort textId, byte log)
        {
            byte[] data = new byte[SIZE_GAMEMESSAGE_WITHOUT_ACTOR1 - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)textOwnerActorId);
                    binWriter.Write((UInt16)textId);
                    binWriter.Write((UInt16)log);
                }
            }

            return new SubPacket(OPCODE_GAMEMESSAGE_WITHOUT_ACTOR1, sourceActorId, data);
        }

        public static SubPacket BuildPacket(uint sourceActorId, uint textOwnerActorId, ushort textId, byte log, List<LuaParam> lParams)
        {
            int lParamsSize = findSizeOfParams(lParams);
            byte[] data;
            ushort opcode;

            if (lParamsSize <= 0x8)
            {
                data = new byte[SIZE_GAMEMESSAGE_WITHOUT_ACTOR2 - 0x20];
                opcode = OPCODE_GAMEMESSAGE_WITHOUT_ACTOR2;
            }
            else if (lParamsSize <= 0x10)
            {
                data = new byte[SIZE_GAMEMESSAGE_WITHOUT_ACTOR3 - 0x20];
                opcode = OPCODE_GAMEMESSAGE_WITHOUT_ACTOR3;
            }
            else if (lParamsSize <= 0x20)
            {
                data = new byte[SIZE_GAMEMESSAGE_WITHOUT_ACTOR4 - 0x20];
                opcode = OPCODE_GAMEMESSAGE_WITHOUT_ACTOR4;
            }
            else
            {
                data = new byte[SIZE_GAMEMESSAGE_WITHOUT_ACTOR5 - 0x20];
                opcode = OPCODE_GAMEMESSAGE_WITHOUT_ACTOR5;
            }

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)textOwnerActorId);
                    binWriter.Write((UInt16)textId);
                    binWriter.Write((UInt16)log);
                    LuaUtils.WriteLuaParams(binWriter, lParams);

                    if (lParamsSize <= 0x8)
                    {
                        binWriter.Seek(0x10, SeekOrigin.Begin);
                        binWriter.Write((UInt32)8);
                    }
                }
            }

            return new SubPacket(opcode, sourceActorId, data);
        }

        private static int findSizeOfParams(List<LuaParam> lParams)
        {
            int total = 0;
            foreach (LuaParam l in lParams)
            {
                switch (l.typeID)
                {
                    case 0:
                    case 1:
                        total += 5;
                        break;
                    case 2:
                        total += 20;
                        break;
                    case 6:
                        total += 5;
                        break;
                    case 3:
                    case 4:
                    case 5:
                        total += 1;
                        break;
                }
            }
            return total + 1;
        }
    }
}

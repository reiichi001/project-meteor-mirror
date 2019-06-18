/*
===========================================================================
Copyright (C) 2015-2019 FFXIV Classic Server Dev Team

This file is part of FFXIV Classic Server.

FFXIV Classic Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

FFXIV Classic Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with FFXIV Classic Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send.supportdesk
{
    class StartGMTicketPacket
    {
        public const ushort OPCODE = 0x01D3;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint playerActorID, bool startGM)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            data[0] = (byte)(startGM ? 1 : 0);
            return new SubPacket(OPCODE, playerActorID, data);
        }
    }
}

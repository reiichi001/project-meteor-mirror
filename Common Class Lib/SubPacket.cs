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

using System;
using System.Runtime.InteropServices;

using NLog;
using NLog.Targets;

namespace Meteor.Common
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SubPacketHeader
    {
        public ushort subpacketSize;
        public ushort type;
        public uint sourceId;
        public uint targetId;
        public uint unknown1;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GameMessageHeader
    {
        public ushort unknown4; //Always 0x14
        public ushort opcode;
        public uint unknown5;
        public uint timestamp;
        public uint unknown6;
    }

    public class SubPacket
    {
        public const int SUBPACKET_SIZE = 0x10;
        public const int GAMEMESSAGE_SIZE = 0x10;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public byte[] data;
        public GameMessageHeader gameMessage;

        public SubPacketHeader header;

        public unsafe SubPacket(byte[] bytes, ref int offset)
        {
            if (bytes.Length < offset + SUBPACKET_SIZE)
                throw new OverflowException("Packet Error: Subpacket was too small");

            fixed (byte* pdata = &bytes[offset])
            {
                header = (SubPacketHeader) Marshal.PtrToStructure(new IntPtr(pdata), typeof(SubPacketHeader));
            }

            if (header.type == 0x3)
            {
                fixed (byte* pdata = &bytes[offset + SUBPACKET_SIZE])
                {
                    gameMessage =
                        (GameMessageHeader) Marshal.PtrToStructure(new IntPtr(pdata), typeof(GameMessageHeader));
                }
            }

            if (bytes.Length < offset + header.subpacketSize)
                throw new OverflowException("Packet Error: Subpacket size didn't equal subpacket data");

            if (header.type == 0x3)
            {
                data = new byte[header.subpacketSize - SUBPACKET_SIZE - GAMEMESSAGE_SIZE];
                Array.Copy(bytes, offset + SUBPACKET_SIZE + GAMEMESSAGE_SIZE, data, 0, data.Length);
            }
            else
            {
                data = new byte[header.subpacketSize - SUBPACKET_SIZE];
                Array.Copy(bytes, offset + SUBPACKET_SIZE, data, 0, data.Length);
            }

            offset += header.subpacketSize;
        }

        public SubPacket(ushort opcode, uint sourceId, byte[] data) : this(true, opcode, sourceId, data) { }

        public SubPacket(bool isGameMessage, ushort opcode, uint sourceId, byte[] data)
        {
            header = new SubPacketHeader();

            if (isGameMessage)
            {
                gameMessage = new GameMessageHeader();
                gameMessage.opcode = opcode;
                gameMessage.timestamp = Utils.UnixTimeStampUTC();
                gameMessage.unknown4 = 0x14;
                gameMessage.unknown5 = 0x00;
                gameMessage.unknown6 = 0x00;
            }

            header.sourceId = sourceId;
            header.targetId = 0;

            if (isGameMessage)
                header.type = 0x03;
            else
                header.type = opcode;

            header.unknown1 = 0x00;

            this.data = data;

            header.subpacketSize = (ushort) (SUBPACKET_SIZE + data.Length);

            if (isGameMessage)
                header.subpacketSize += GAMEMESSAGE_SIZE;
        }

        public SubPacket(SubPacket original, uint newTargetId)
        {
            header = new SubPacketHeader();
            gameMessage = original.gameMessage;
            header.subpacketSize = original.header.subpacketSize;
            header.type = original.header.type;
            header.sourceId = original.header.sourceId;
            header.targetId = newTargetId;
            data = original.data;
        }

        public void SetTargetId(uint target)
        {
            this.header.targetId = target;
        }

        public byte[] GetHeaderBytes()
        {
            var size = Marshal.SizeOf(header);
            var arr = new byte[size];

            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(header, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        public byte[] GetGameMessageBytes()
        {
            var size = Marshal.SizeOf(gameMessage);
            var arr = new byte[size];

            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(gameMessage, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        public byte[] GetBytes()
        {
            var outBytes = new byte[header.subpacketSize];
            Array.Copy(GetHeaderBytes(), 0, outBytes, 0, SUBPACKET_SIZE);

            if (header.type == 0x3)
                Array.Copy(GetGameMessageBytes(), 0, outBytes, SUBPACKET_SIZE, GAMEMESSAGE_SIZE);

            Array.Copy(data, 0, outBytes, SUBPACKET_SIZE + (header.type == 0x3 ? GAMEMESSAGE_SIZE : 0), data.Length);
            return outBytes;
        }

        /// <summary>
        /// Builds a packet from the incoming buffer + offset. If a packet can be built, it is returned else null.
        /// </summary>
        /// <param name="offset">Current offset in buffer.</param>
        /// <param name="buffer">Incoming buffer.</param>
        /// <returns>Returns either a BasePacket or null if not enough data.</returns>
        public static SubPacket CreatePacket(ref int offset, byte[] buffer, int bytesRead)
        {
            SubPacket newPacket = null;

            //Too small to even get length
            if (bytesRead <= offset)
                return null;

            ushort packetSize = BitConverter.ToUInt16(buffer, offset);

            //Too small to whole packet
            if (bytesRead < offset + packetSize)
                return null;

            if (buffer.Length < offset + packetSize)
                return null;

            try
            {
                newPacket = new SubPacket(buffer, ref offset);
            }
            catch (OverflowException)
            {
                return null;
            }

            return newPacket;
        }

        public void DebugPrintSubPacket()
        {
#if DEBUG
            logger.ColorDebug(
                string.Format("Size:0x{0:X} Opcode:0x{1:X}{2}{3}", header.subpacketSize, gameMessage.opcode,
                    Environment.NewLine,
                    Utils.ByteArrayToHex(GetHeaderBytes())), ConsoleOutputColor.DarkRed);

            if (header.type == 0x03)
            {
                logger.ColorDebug(Utils.ByteArrayToHex(GetGameMessageBytes(), SUBPACKET_SIZE),
                    ConsoleOutputColor.DarkRed);

                logger.ColorDebug(Utils.ByteArrayToHex(data, SUBPACKET_SIZE + GAMEMESSAGE_SIZE),
                    ConsoleOutputColor.DarkMagenta);
            }
            else
                logger.ColorDebug(Utils.ByteArrayToHex(data, SUBPACKET_SIZE),
                    ConsoleOutputColor.DarkMagenta);
#endif
        }        
    }
}

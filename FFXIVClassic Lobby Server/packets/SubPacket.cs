using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using FFXIVClassic_Lobby_Server;
using FFXIVClassic_Lobby_Server.common;

namespace FFXIVClassic_Lobby_Server.packets
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

        public SubPacketHeader header;
        public GameMessageHeader gameMessage;
        public byte[] data;

        public unsafe SubPacket(byte[] bytes, ref int offset)
        {
            if (bytes.Length < offset + SUBPACKET_SIZE)
                throw new OverflowException("Packet Error: Subpacket was too small");

            fixed (byte* pdata = &bytes[offset])
            {
                header = (SubPacketHeader)Marshal.PtrToStructure(new IntPtr(pdata), typeof(SubPacketHeader));
            }

            if (header.type == 0x3)
            {
                fixed (byte* pdata = &bytes[offset + SUBPACKET_SIZE])
                {
                    gameMessage = (GameMessageHeader)Marshal.PtrToStructure(new IntPtr(pdata), typeof(GameMessageHeader));
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

        public SubPacket(ushort opcode, uint sourceId, uint targetId, byte[] data)
        {
            this.header = new SubPacketHeader();
            this.gameMessage = new GameMessageHeader();

            gameMessage.opcode = opcode;
            header.sourceId = sourceId;
            header.targetId = targetId;

            gameMessage.timestamp = Utils.UnixTimeStampUTC();

            header.type = 0x03;
            header.unknown1 = 0x00;
            gameMessage.unknown4 = 0x14;
            gameMessage.unknown5 = 0x00;
            gameMessage.unknown6 = 0x00;

            this.data = data;

            header.subpacketSize = (ushort)(SUBPACKET_SIZE + GAMEMESSAGE_SIZE + data.Length);
        }

        public SubPacket(SubPacket original, uint newTargetId)
        {
            this.header = new SubPacketHeader();
            this.gameMessage = original.gameMessage;
            header.subpacketSize = original.header.subpacketSize;
            header.type = original.header.type;
            header.sourceId = original.header.sourceId;
            header.targetId = newTargetId;
            data = original.data;
        }

        public byte[] getHeaderBytes()
        {
            int size = Marshal.SizeOf(header);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(header, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        public byte[] getGameMessageBytes()
        {
            int size = Marshal.SizeOf(gameMessage);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(gameMessage, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        public byte[] getBytes()
        {
            byte[] outBytes = new byte[header.subpacketSize];
            Array.Copy(getHeaderBytes(), 0, outBytes, 0, SUBPACKET_SIZE);

            if (header.type == 0x3)
                Array.Copy(getGameMessageBytes(), 0, outBytes, SUBPACKET_SIZE, GAMEMESSAGE_SIZE);

            Array.Copy(data, 0, outBytes, SUBPACKET_SIZE + (header.type == 0x3 ? GAMEMESSAGE_SIZE : 0), data.Length);
            return outBytes;
        }

        public void debugPrintSubPacket()
        {
#if DEBUG
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Size: 0x{0:X}", header.subpacketSize);
            if (header.type == 0x03)
                Console.WriteLine("Opcode: 0x{0:X}", gameMessage.opcode);
            Console.WriteLine("{0}", Utils.ByteArrayToHex(getHeaderBytes()));
            if (header.type == 0x03)
                Console.WriteLine("{0}", Utils.ByteArrayToHex(getGameMessageBytes()));
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("{0}", Utils.ByteArrayToHex(data));
            Console.BackgroundColor = ConsoleColor.Black;
#endif
        }

    }
}

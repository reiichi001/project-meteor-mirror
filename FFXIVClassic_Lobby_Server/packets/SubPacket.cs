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
        public ushort       subpacketSize;
        public ushort       unknown0; //Always 0x03
        public uint         sourceId; 
        public uint         targetId;
        public uint         unknown1;
        public ushort       unknown4; //Always 0x14
        public ushort       opcode;
        public uint         unknown5; 
        public uint         timestamp;
        public uint         unknown6;
    }

    public class SubPacket
    {
        public const int SUBPACKET_SIZE = 0x20;

        public SubPacketHeader  header;
        public byte[]           data;

        public unsafe SubPacket(byte[] bytes, ref int offset)
        {
            if (bytes.Length < offset + SUBPACKET_SIZE)
                throw new OverflowException("Packet Error: Subpacket was too small");

            fixed (byte* pdata = &bytes[offset])
            {
                header = (SubPacketHeader)Marshal.PtrToStructure(new IntPtr(pdata), typeof(SubPacketHeader));
            }

            if (bytes.Length < offset + header.subpacketSize)
                throw new OverflowException("Packet Error: Subpacket size didn't equal subpacket data");

            data = new byte[header.subpacketSize - SUBPACKET_SIZE];
            Array.Copy(bytes, offset + SUBPACKET_SIZE, data, 0, data.Length);

            offset += header.subpacketSize;
        }

        public SubPacket(ushort opcode, uint sourceId, uint targetId, byte[] data)
        {
            this.header = new SubPacketHeader();
            header.opcode = opcode;
            header.sourceId = sourceId;
            header.targetId = targetId;

            UInt32 unixTimestamp = (UInt32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            header.timestamp = unixTimestamp;

            header.unknown0 = 0x03;
            header.unknown1 = 0x00;
            header.unknown4 = 0x14;
            header.unknown5 = 0x00;
            header.unknown6 = 0x00;

            this.data = data;

            header.subpacketSize = (ushort)(0x20 + data.Length);
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

        public byte[] getBytes()
        {
            byte[] outBytes = new byte[header.subpacketSize];
            Array.Copy(getHeaderBytes(), 0, outBytes, 0, SUBPACKET_SIZE);
            Array.Copy(data, 0, outBytes, SUBPACKET_SIZE, data.Length);
            return outBytes;
        }

        public void debugPrintSubPacket()
        {
#if DEBUG
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Size: 0x{0:X}, Opcode: 0x{1:X}", header.subpacketSize, header.opcode);            
            Console.WriteLine("{0}", Utils.ByteArrayToHex(getHeaderBytes()));
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("{0}", Utils.ByteArrayToHex(data));
            Console.BackgroundColor = ConsoleColor.Black;
#endif
        }

    }
}

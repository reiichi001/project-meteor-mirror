using System;
using System.Runtime.InteropServices;
using FFXIVClassic.Common;
using NLog;
using NLog.Targets;

namespace FFXIVClassic_Map_Server.packets
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

        public SubPacket(ushort opcode, uint sourceId, uint targetId, byte[] data)
        {
            header = new SubPacketHeader();
            gameMessage = new GameMessageHeader();

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

            header.subpacketSize = (ushort) (SUBPACKET_SIZE + GAMEMESSAGE_SIZE + data.Length);
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
#endif
        }
    }
}
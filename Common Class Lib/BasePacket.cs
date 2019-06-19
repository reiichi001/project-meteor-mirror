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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

using Ionic.Zlib;
using NLog;
using NLog.Targets;

namespace Meteor.Common
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BasePacketHeader
    {
        public byte isAuthenticated;
        public byte isCompressed;
        public ushort connectionType;
        public ushort packetSize;
        public ushort numSubpackets;
        public ulong timestamp; //Miliseconds
    }

    public class BasePacket
    {
        public const int TYPE_ZONE = 1;
        public const int TYPE_CHAT = 2;
        public const int BASEPACKET_SIZE = 0x10;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public byte[] data;

        public BasePacketHeader header;

        //Loads a sniffed packet from a file
        public unsafe BasePacket(string path)
        {
            var bytes = File.ReadAllBytes(path);

            if (bytes.Length < BASEPACKET_SIZE)
                throw new OverflowException("Packet Error: Packet was too small");

            fixed (byte* pdata = &bytes[0])
            {
                header = (BasePacketHeader) Marshal.PtrToStructure(new IntPtr(pdata), typeof(BasePacketHeader));
            }

            if (bytes.Length < header.packetSize)
                throw new OverflowException("Packet Error: Packet size didn't equal given size");

            int packetSize = header.packetSize;

            if (packetSize - BASEPACKET_SIZE != 0)
            {
                data = new byte[packetSize - BASEPACKET_SIZE];
                Array.Copy(bytes, BASEPACKET_SIZE, data, 0, packetSize - BASEPACKET_SIZE);
            }
            else
                data = new byte[0];
        }

        //Loads a sniffed packet from a byte array
        public unsafe BasePacket(byte[] bytes)
        {
            if (bytes.Length < BASEPACKET_SIZE)
                throw new OverflowException("Packet Error: Packet was too small");

            fixed (byte* pdata = &bytes[0])
            {
                header = (BasePacketHeader) Marshal.PtrToStructure(new IntPtr(pdata), typeof(BasePacketHeader));
            }

            if (bytes.Length < header.packetSize)
                throw new OverflowException("Packet Error: Packet size didn't equal given size");

            int packetSize = header.packetSize;

            data = new byte[packetSize - BASEPACKET_SIZE];
            Array.Copy(bytes, BASEPACKET_SIZE, data, 0, packetSize - BASEPACKET_SIZE);
        }

        public unsafe BasePacket(byte[] bytes, ref int offset)
        {
            if (bytes.Length < offset + BASEPACKET_SIZE)
                throw new OverflowException("Packet Error: Packet was too small");

            fixed (byte* pdata = &bytes[offset])
            {
                header = (BasePacketHeader) Marshal.PtrToStructure(new IntPtr(pdata), typeof(BasePacketHeader));
            }

            int packetSize = header.packetSize;

            if (bytes.Length < offset + header.packetSize)
                throw new OverflowException("Packet Error: Packet size didn't equal given size");

            data = new byte[packetSize - BASEPACKET_SIZE];
            Array.Copy(bytes, offset + BASEPACKET_SIZE, data, 0, packetSize - BASEPACKET_SIZE);

            offset += packetSize;
        }

        public BasePacket(BasePacketHeader header, byte[] data)
        {
            this.header = header;
            this.data = data;
        }

        public List<SubPacket> GetSubpackets()
        {
            var subpackets = new List<SubPacket>(header.numSubpackets);

            var offset = 0;

            while (offset < data.Length)
                subpackets.Add(new SubPacket(data, ref offset));

            return subpackets;
        }

        public static unsafe BasePacketHeader GetHeader(byte[] bytes)
        {
            BasePacketHeader header;
            if (bytes.Length < BASEPACKET_SIZE)
                throw new OverflowException("Packet Error: Packet was too small");

            fixed (byte* pdata = &bytes[0])
            {
                header = (BasePacketHeader) Marshal.PtrToStructure(new IntPtr(pdata), typeof(BasePacketHeader));
            }

            return header;
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

        public byte[] GetPacketBytes()
        {
            var outBytes = new byte[header.packetSize];
            Array.Copy(GetHeaderBytes(), 0, outBytes, 0, BASEPACKET_SIZE);
            Array.Copy(data, 0, outBytes, BASEPACKET_SIZE, data.Length);
            return outBytes;
        }

        //Replaces all instances of the sniffed actorID with the given one
        public void ReplaceActorID(uint actorID)
        {
            using (var mem = new MemoryStream(data))
            {
                using (var binWriter = new BinaryWriter(mem))
                {
                    using (var binreader = new BinaryReader(mem))
                    {
                        while (binreader.BaseStream.Position + 4 < data.Length)
                        {
                            var read = binreader.ReadUInt32();
                            if (read == 0x029B2941 || read == 0x02977DC7 || read == 0x0297D2C8 || read == 0x0230d573 ||
                                read == 0x23317df || read == 0x23344a3 || read == 0x1730bdb || read == 0x6c)
                                //Original ID
                            {
                                binWriter.BaseStream.Seek(binreader.BaseStream.Position - 0x4, SeekOrigin.Begin);
                                binWriter.Write(actorID);
                            }
                        }
                    }
                }
            }
        }

        //Replaces all instances of the sniffed actorID with the given one
        public void ReplaceActorID(uint fromActorID, uint actorID)
        {
            using (var mem = new MemoryStream(data))
            {
                using (var binWriter = new BinaryWriter(mem))
                {
                    using (var binreader = new BinaryReader(mem))
                    {
                        while (binreader.BaseStream.Position + 4 < data.Length)
                        {
                            var read = binreader.ReadUInt32();
                            if (read == fromActorID) //Original ID
                            {
                                binWriter.BaseStream.Seek(binreader.BaseStream.Position - 0x4, SeekOrigin.Begin);
                                binWriter.Write(actorID);
                            }
                        }
                    }
                }
            }
        }

        public void DebugPrintPacket()
        {
#if DEBUG
            logger.ColorDebug(
                string.Format("IsAuth:{0} IsEncrypted:{1}, Size:0x{2:X}, NumSubpackets:{3}{4}{5}",
                    header.isAuthenticated, header.isCompressed, header.packetSize, header.numSubpackets,
                    Environment.NewLine, Utils.ByteArrayToHex(GetHeaderBytes())), ConsoleOutputColor.DarkYellow);

            foreach (var sub in GetSubpackets())
            {
                sub.DebugPrintSubPacket();
            }
#endif
        }

        #region Utility Functions

        public static BasePacket CreatePacket(List<SubPacket> subpackets, bool isAuthed, bool isCompressed)
        {
            //Create Header
            var header = new BasePacketHeader();
            byte[] data = null;

            header.isAuthenticated = isAuthed ? (byte) 1 : (byte) 0;
            header.isCompressed = isCompressed ? (byte) 1 : (byte) 0;
            header.numSubpackets = (ushort) subpackets.Count;
            header.packetSize = BASEPACKET_SIZE;
            header.timestamp = Utils.MilisUnixTimeStampUTC();

            //Get packet size
            foreach (var subpacket in subpackets)
                header.packetSize += subpacket.header.subpacketSize;

            data = new byte[header.packetSize - 0x10];

            //Add Subpackets
            var offset = 0;
            foreach (var subpacket in subpackets)
            {
                var subpacketData = subpacket.GetBytes();
                Array.Copy(subpacketData, 0, data, offset, subpacketData.Length);
                offset += (ushort)subpacketData.Length;
            }

            //Compress this array into a new one if needed
            if (isCompressed)
            {
                data = CompressData(data);
                header.packetSize = (ushort)(BASEPACKET_SIZE + data.Length);
            }

            Debug.Assert(data != null && offset == data.Length && header.packetSize == 0x10 + offset);

            var packet = new BasePacket(header, data);
            return packet;
        }

        public static BasePacket CreatePacket(SubPacket subpacket, bool isAuthed, bool isCompressed)
        {
            //Create Header
            var header = new BasePacketHeader();
            byte[] data = null;

            header.isAuthenticated = isAuthed ? (byte) 1 : (byte) 0;
            header.isCompressed = isCompressed ? (byte) 1 : (byte) 0;
            header.numSubpackets = 1;
            header.packetSize = BASEPACKET_SIZE;
            header.timestamp = Utils.MilisUnixTimeStampUTC();

            //Get packet size
            header.packetSize += subpacket.header.subpacketSize;

            data = new byte[header.packetSize - 0x10];

            //Add Subpackets
            byte[] subpacketData = subpacket.GetBytes();

            //Compress this array into a new one if needed
            if (isCompressed)
            {
                subpacketData = CompressData(subpacketData);
                header.packetSize = (ushort)(BASEPACKET_SIZE + data.Length);
            }

            Array.Copy(subpacketData, 0, data, 0, subpacketData.Length);

            Debug.Assert(data != null);

            var packet = new BasePacket(header, data);
            return packet;
        }

        public static BasePacket CreatePacket(byte[] data, bool isAuthed, bool isCompressed)
        {
            Debug.Assert(data != null);

            //Create Header
            var header = new BasePacketHeader();

            header.isAuthenticated = isAuthed ? (byte) 1 : (byte) 0;
            header.isCompressed = isCompressed ? (byte) 1 : (byte) 0;
            header.numSubpackets = 1;
            header.packetSize = BASEPACKET_SIZE;
            header.timestamp = Utils.MilisUnixTimeStampUTC();

            //Get packet size
            header.packetSize += (ushort) data.Length;

            //Compress this array into a new one if needed
            if (isCompressed)
            {
                data = CompressData(data);
                header.packetSize = (ushort)(BASEPACKET_SIZE + data.Length);
            }

            var packet = new BasePacket(header, data);
            return packet;
        }

        /// <summary>
        /// Builds a packet from the incoming buffer + offset. If a packet can be built, it is returned else null.
        /// </summary>
        /// <param name="offset">Current offset in buffer.</param>
        /// <param name="buffer">Incoming buffer.</param>
        /// <returns>Returns either a BasePacket or null if not enough data.</returns>
        public static BasePacket CreatePacket(ref int offset, byte[] buffer, int bytesRead)
        {
            BasePacket newPacket = null;

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
                newPacket = new BasePacket(buffer, ref offset);
            }
            catch (OverflowException)
            {
                return null;
            }

            return newPacket;
        }

        public static unsafe void EncryptPacket(Blowfish blowfish, BasePacket packet)
        {
            var data = packet.data;
            int size = packet.header.packetSize;

            var offset = 0;
            while (offset < data.Length)
            {
                if (data.Length < offset + SubPacket.SUBPACKET_SIZE)
                    throw new OverflowException("Packet Error: Subpacket was too small");

                SubPacketHeader header;
                fixed (byte* pdata = &data[offset])
                {
                    header = (SubPacketHeader) Marshal.PtrToStructure(new IntPtr(pdata), typeof(SubPacketHeader));
                }

                if (data.Length < offset + header.subpacketSize)
                    throw new OverflowException("Packet Error: Subpacket size didn't equal subpacket data");

                blowfish.Encipher(data, offset + 0x10, header.subpacketSize - 0x10);

                offset += header.subpacketSize;
            }
        }

        public static unsafe void DecryptPacket(Blowfish blowfish, ref BasePacket packet)
        {
            var data = packet.data;
            int size = packet.header.packetSize;

            var offset = 0;
            while (offset < data.Length)
            {
                if (data.Length < offset + SubPacket.SUBPACKET_SIZE)
                    throw new OverflowException("Packet Error: Subpacket was too small");

                SubPacketHeader header;
                fixed (byte* pdata = &data[offset])
                {
                    header = (SubPacketHeader) Marshal.PtrToStructure(new IntPtr(pdata), typeof(SubPacketHeader));
                }

                if (data.Length < offset + header.subpacketSize)
                    throw new OverflowException("Packet Error: Subpacket size didn't equal subpacket data");

                blowfish.Decipher(data, offset + 0x10, header.subpacketSize - 0x10);

                offset += header.subpacketSize;
            }
        }

        public static unsafe void DecompressPacket(ref BasePacket packet)
        {
            using (var compressedStream = new MemoryStream(packet.data))
            using (var zipStream = new ZlibStream(compressedStream, Ionic.Zlib.CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                packet.data = resultStream.ToArray();
                packet.header.isCompressed = 0;
                packet.header.packetSize = (ushort)(BASEPACKET_SIZE + packet.data.Length);
            }
        }

        public static unsafe BasePacket CompressPacket(BasePacket uncompressedPacket)
        {
            using (var compressedStream = new MemoryStream(uncompressedPacket.data))
            using (var zipStream = new ZlibStream(compressedStream, Ionic.Zlib.CompressionMode.Compress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                BasePacket compressedPacket = BasePacket.CreatePacket(resultStream.ToArray(), uncompressedPacket.header.isAuthenticated == 1, true);
                return compressedPacket;
            }
        }

        public static unsafe byte[] CompressData(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new ZlibStream(compressedStream, Ionic.Zlib.CompressionMode.Compress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }

        #endregion
    }

    public static class LoggerExtensions
    {
        public static void ColorDebug(this Logger logger, string message, ConsoleOutputColor color)
        {
            var logEvent = new LogEventInfo(LogLevel.Debug, logger.Name, message);
            logEvent.Properties["color"] = (int) color;
            logger.Log(logEvent);
        }
    }
}

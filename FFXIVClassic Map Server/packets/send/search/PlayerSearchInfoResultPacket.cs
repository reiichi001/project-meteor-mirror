using System.IO;
using System.Text;

using FFXIVClassic.Common;
using System;

namespace FFXIVClassic_Map_Server.packets.send.search
{
    class PlayerSearchInfoResultPacket
    {
        public const ushort OPCODE = 0x01DF;
        public const uint PACKET_SIZE = 0x3C8;

        public static SubPacket BuildPacket(uint sourceActorId, uint searchSessionId, byte resultCode, PlayerSearchResult[] results, ref int offset)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            byte count;

            if (results.Length - offset < 8)
                count = (byte)(results.Length - offset);
            else
                count = 8;

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    for (int i = offset; i < count; i++)
                    {
                        long start = binWriter.BaseStream.Position;
                        binWriter.Write((Byte)results[i].preferredClass);
                        binWriter.Write((Byte)0);
                        binWriter.Write((Byte)results[i].clientLanguage);
                        binWriter.Write((UInt16)results[i].currentZone);
                        binWriter.Write((Byte)results[i].initialTown);
                        binWriter.Write((Byte)0);
                        binWriter.Write((Byte)results[i].status);
                        binWriter.Write((Byte)results[i].currentClass);
                        binWriter.Write((Byte)0);
                        binWriter.Write(Encoding.ASCII.GetBytes(results[i].name), 0, Encoding.ASCII.GetByteCount(results[i].name) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(results[i].name));
                        binWriter.Seek((int)(start + 30), SeekOrigin.Begin);
                        //classes
                        binWriter.Seek((int)(start + 30 + 20), SeekOrigin.Begin);
                        //jobs
                    }
                }
            }

            offset += count;

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

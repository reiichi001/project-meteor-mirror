using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.list
{
    class ListStartPacket
    {
        public const uint TYPEID_RETAINER = 0x13881;
        public const uint TYPEID_PARTY = 0x2711;
        public const uint TYPEID_LINKSHELL = 0x4E22;

        public const ushort OPCODE = 0x017C;
        public const uint PACKET_SIZE = 0x98;

        public static SubPacket buildPacket(uint playerActorID, uint locationCode, ulong sequenceId, ulong listId, uint listTypeId, int numEntries)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    //Temp stuff
                    string name = "";

                    //Write list header
                    binWriter.Write((UInt64)locationCode);
                    binWriter.Write((UInt64)sequenceId);

                    //Write list id
                    binWriter.Write((UInt64)3);
                    binWriter.Write((UInt64)listId);
                    binWriter.Write((UInt64)0);
                    binWriter.Write((UInt64)listId);

                    //This seems to change depending on what the list is for
                    binWriter.Write((UInt32)listTypeId);
                    binWriter.Seek(0x40, SeekOrigin.Begin);

                    //This is for Linkshell
                    binWriter.Write((UInt32)0xFFFFFFFF);
                    binWriter.Write(Encoding.ASCII.GetBytes(name), 0, Encoding.ASCII.GetByteCount(name) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(name));

                    binWriter.Seek(0x64, SeekOrigin.Begin);

                    binWriter.Write((UInt32)0x6D);
                    binWriter.Write((UInt32)0x6D);
                    binWriter.Write((UInt32)0x6D);
                    binWriter.Write((UInt32)0x6D);

                    binWriter.Write((UInt32)numEntries);
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

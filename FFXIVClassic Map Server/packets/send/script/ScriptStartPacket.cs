using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.script
{
    class ScriptStartPacket
    {
        public const ushort OPCODE = 0x0130;
        public const uint PACKET_SIZE = 0xB0;

        //Known types
        public const byte TYPE_END = 0xF;
        public const byte TYPE_UINT = 0x6;
        public const byte TYPE_BYTE = 0x5;        
        public const byte TYPE_STRING = 0x2;
        public const byte TYPE_STRING_20B = 0x1;
        public const byte TYPE_STRING_NULLTERM = 0x0;

        public static SubPacket buildPacket(uint playerActorID, uint scriptOwnerActorID, string startName, string scriptName)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((uint)playerActorID);
                    binWriter.Write((uint)scriptOwnerActorID);
                    binWriter.Write((byte)1);
                    binWriter.Write(Encoding.Unicode.GetBytes(startName));
                    binWriter.Seek(0x29, SeekOrigin.Begin);
                    binWriter.Write(Encoding.Unicode.GetBytes(scriptName));
                    binWriter.Seek(0x29 + 0x20, SeekOrigin.Begin);
                    binWriter.Write((byte)6);

                    byte[] actorID = BitConverter.GetBytes(playerActorID);
                    Array.Reverse(actorID);
                    binWriter.Write(actorID);

                    binWriter.Write((byte)0x0F);
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

using FFXIVClassic.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_World_Server.Packets.Send
{
    class _0x2Packet
    {
        public const ushort OPCODE = 0x0002;
        public const uint PACKET_SIZE = 0x38;

        public static SubPacket BuildPacket(uint actorID)
        {
            byte[] data =  {                                           
                                            0x6c, 0x00, 0x00, 0x00, 0xC8, 0xD6, 0xAF, 0x2B, 0x38, 0x2B, 0x5F, 0x26, 0xB8, 0x8D, 0xF0, 0x2B,
                                            0xC8, 0xFD, 0x85, 0xFE, 0xA8, 0x7C, 0x5B, 0x09, 0x38, 0x2B, 0x5F, 0x26, 0xC8, 0xD6, 0xAF, 0x2B,
                                            0xB8, 0x8D, 0xF0, 0x2B, 0x88, 0xAF, 0x5E, 0x26
                                        };

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    try
                    {
                        binWriter.Seek(0, SeekOrigin.Begin);
                        binWriter.Write((UInt32)actorID);                        
                    }
                    catch (Exception)
                    { }
                }
            }

            /*
            0x6c, 0x00, 0x00, 0x00, 0xC8, 0xD6, 0xAF, 0x2B, 0x38, 0x2B, 0x5F, 0x26, 0xB8, 0x8D, 0xF0, 0x2B,
            0xC8, 0xFD, 0x85, 0xFE, 0xA8, 0x7C, 0x5B, 0x09, 0x38, 0x2B, 0x5F, 0x26, 0xC8, 0xD6, 0xAF, 0x2B,
            0xB8, 0x8D, 0xF0, 0x2B, 0x88, 0xAF, 0x5E, 0x26
             */

            return new SubPacket(false, OPCODE, 0, 0, data);
        }
    }
}

using FFXIVClassic.Common;
using FFXIVClassic_World_Server.DataObjects.Group;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_World_Server.Packets.Send.Subpackets.Groups
{
    class SetActiveLinkshellPacket
    {
      
        public const ushort OPCODE = 0x018A;
        public const uint PACKET_SIZE = 0x98;

        public static SubPacket BuildPacket(uint sessionId, ulong groupId)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    //Write the LS groupId
                    binWriter.Write((UInt64)groupId);
                    
                    //Write the LS group type
                    binWriter.Seek(0x40, SeekOrigin.Begin);
                    binWriter.Write((UInt32)Group.CompanyGroup);

                    //Seems to be a index but can only set one active so /shrug
                    binWriter.Seek(0x60, SeekOrigin.Begin);
                    binWriter.Write((UInt32)(groupId == 0 ? 0 : 1));
                }
            }

            return new SubPacket(OPCODE, sessionId, sessionId, data);
        }
    }
}

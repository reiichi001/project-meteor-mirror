using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.dataobjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.recruitment
{
    class CurrentRecruitmentDetailsPacket
    {
        public const ushort OPCODE = 0x01C8;
        public const uint PACKET_SIZE = 0x218;

        public static SubPacket buildPacket(uint playerActorID, RecruitmentDetails details)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    if (details == null)
                    {
                        return new SubPacket(OPCODE, playerActorID, playerActorID, data);
                    }

                    binWriter.Write((UInt32)details.purposeId);
                    binWriter.Write((UInt32)details.locationId);
                    binWriter.Write((UInt32)details.subTaskId);
                    binWriter.Write((UInt32)details.timeSinceStart);

                    for (int i = 0; i < 4; i++)
                    {
                        binWriter.Write((UInt32)details.discipleId[i]);
                        binWriter.Write((UInt32)details.classjobId[i]);
                        binWriter.Write((byte)details.minLvl[i]);
                        binWriter.Write((byte)details.maxLvl[i]);
                        binWriter.Write((byte)details.num[i]);
                        binWriter.Write((byte)0);
                    }

                    binWriter.Write(Encoding.ASCII.GetBytes(details.comment), 0, Encoding.ASCII.GetByteCount(details.comment) >= 0x168 ? 0x168 : Encoding.ASCII.GetByteCount(details.comment));
                    binWriter.Seek(0x1C0, SeekOrigin.Begin);
                    binWriter.Write(Encoding.ASCII.GetBytes(details.recruiterName), 0, Encoding.ASCII.GetByteCount(details.recruiterName) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(details.recruiterName));
                    binWriter.Seek(0x1E0, SeekOrigin.Begin);
                    binWriter.Write((byte)1);
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

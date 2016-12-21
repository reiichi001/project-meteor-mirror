using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.dataobjects;
using System;
using System.IO;
using System.Text;

namespace FFXIVClassic_Map_Server.packets.WorldPackets.Send.Group
{
    class ModifyLinkshellPacket
    {
        public const ushort OPCODE = 0x1000;
        public const uint PACKET_SIZE = 0x60;

        public static SubPacket BuildPacket(Session session, ushort changeArg, string name, string newName, ushort crest, uint master)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write(Encoding.ASCII.GetBytes(name), 0, Encoding.ASCII.GetByteCount(name) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(name));
                    binWriter.Write((UInt16)changeArg);
                    switch (changeArg)
                    {
                        case 0:
                            binWriter.Write(Encoding.ASCII.GetBytes(newName), 0, Encoding.ASCII.GetByteCount(newName) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(newName));
                            break;
                        case 1:
                            binWriter.Write((UInt16)crest);
                            break;
                        case 2:
                            binWriter.Write((UInt32)master);
                            break;
                    }
                    
                }
            }
            return new SubPacket(true, OPCODE, 0, session.id, data);
        }      
    }
}

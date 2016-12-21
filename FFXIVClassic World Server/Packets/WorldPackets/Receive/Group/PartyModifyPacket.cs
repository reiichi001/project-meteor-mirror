using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FFXIVClassic_World_Server.Packets.WorldPackets.Receive.Group
{
    class PartyModifyPacket
    {
        public bool invalidPacket = false;

        public const ushort MODIFY_LEADER = 0;
        public const ushort MODIFY_KICKPLAYER = 1;

        public ushort command;
        public string name;

        public PartyModifyPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        command = binReader.ReadUInt16();
                        name = Encoding.ASCII.GetString(binReader.ReadBytes(0x20)).Trim(new[] { '\0' });
                    }
                    catch (Exception)
                    {
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

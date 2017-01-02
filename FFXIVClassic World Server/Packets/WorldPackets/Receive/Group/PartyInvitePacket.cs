using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FFXIVClassic_World_Server.Packets.WorldPackets.Receive.Group
{
    class PartyInvitePacket
    {
        public bool invalidPacket = false;

        public ushort command;
        public string name;
        public uint actorId;

        public PartyInvitePacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        command = binReader.ReadUInt16();

                        if (command == 1)
                            actorId = binReader.ReadUInt32();
                        else
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

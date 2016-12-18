using System;
using System.IO;
using System.Text;

namespace FFXIVClassic_World_Server.Packets.WorldPackets.Receive.Group
{
    class ModifyLinkshellPacket
    {
        public bool invalidPacket = false;

        public string currentName;
        public ushort argCode;
        public string name;
        public ushort crestid;
        public uint master;
        
        public ModifyLinkshellPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        currentName = Encoding.ASCII.GetString(binReader.ReadBytes(0x20)).Trim(new[] { '\0' });
                        argCode = binReader.ReadUInt16();

                        switch (argCode)
                        {
                            case 0:
                                name = Encoding.ASCII.GetString(binReader.ReadBytes(0x20)).Trim(new[] { '\0' });
                                break;
                            case 1:
                                crestid = binReader.ReadUInt16();
                                break;
                            case 2:
                                master = binReader.ReadUInt32();
                                break;
                        }
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

using FFXIVClassic.Common;
using FFXIVClassic_World_Server.DataObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_World_Server.Packets.WorldPackets.Receive
{
    class SessionBeginConfirmPacket
    {
        public bool invalidPacket = false;
        public uint sessionId;
        public ushort errorCode;        

        public SessionBeginConfirmPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        sessionId = binReader.ReadUInt32();
                        errorCode = binReader.ReadUInt16();
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

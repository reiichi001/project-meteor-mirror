using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.login
{
    class InitPacket
    {
        public static BasePacket buildPacket(uint unknown, uint time)
        {
            byte[] data = new byte[0x18];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    try
                    {
                        binWriter.Write((short)0x18);
                        binWriter.Write((short)0x7);
                        binWriter.Write((uint)0);
                        binWriter.Write((uint)0);
                        binWriter.Write((uint)0xFFFFFD7F);

                        binWriter.Write((uint)unknown);
                        binWriter.Write((uint)time);
                    }
                    catch (Exception)
                    {                        
                    }
                }
            }

            return BasePacket.createPacket(data, false, false);
        }
    }
}

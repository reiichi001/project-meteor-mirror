using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.lua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.actor
{
    class ActorInstantiatePacket
    {
        public const ushort OPCODE = 0x00CC;
        public const uint PACKET_SIZE = 0x128;

        public static SubPacket buildPacket(uint sourceActorID, uint targetActorID, string objectName, string className, List<LuaParam> initParams)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    int value1 = 0x00; //Instance ID?
                    int value2 = 0x3040;
                    binWriter.Write((Int16)value1);
                    binWriter.Write((Int16)value2);
                    binWriter.Write(Encoding.ASCII.GetBytes(objectName), 0, Encoding.ASCII.GetByteCount(objectName) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(objectName));
                    binWriter.BaseStream.Seek(0x24, SeekOrigin.Begin);
                    binWriter.Write(Encoding.ASCII.GetBytes(className), 0, Encoding.ASCII.GetByteCount(className) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(className));
                    binWriter.BaseStream.Seek(0x44, SeekOrigin.Begin);
                    LuaUtils.writeLuaParams(binWriter, initParams);
                }
            }

            return new SubPacket(OPCODE, sourceActorID, targetActorID, data);
        }

    }
}

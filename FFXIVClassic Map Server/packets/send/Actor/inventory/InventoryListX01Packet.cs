using FFXIVClassic_Map_Server.dataobjects;
using System.IO;

using FFXIVClassic.Common;

namespace  FFXIVClassic_Map_Server.packets.send.actor.inventory
{
    class InventoryListX01Packet
    {
        public const ushort OPCODE = 0x0148;
        public const uint PACKET_SIZE = 0x90;
        
        public static SubPacket BuildPacket(uint sourceActorId, InventoryItem item)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {                    
                        binWriter.Write(item.ToPacketBytes());                       
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }

    }
}

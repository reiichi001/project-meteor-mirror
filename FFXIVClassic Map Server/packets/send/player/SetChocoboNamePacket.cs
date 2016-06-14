using System.Text;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class SetChocoboNamePacket
    {
        public const ushort OPCODE = 0x0198;
        public const uint PACKET_SIZE = 0x40;

        public static SubPacket BuildPacket(uint playerActorID, uint tarGetActorID, string name)
        {
            if (Encoding.Unicode.GetByteCount(name) >= 0x20)
                name = "ERR: Too Long";
            return new SubPacket(OPCODE, playerActorID, tarGetActorID, Encoding.ASCII.GetBytes(name));
        }
    }
}

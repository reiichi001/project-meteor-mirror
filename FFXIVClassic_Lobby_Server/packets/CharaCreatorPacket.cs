using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Lobby_Server.packets
{
    class CharaCreatorPacket
    {
        public const ushort OPCODE = 0x0E;

        public const ushort RESERVE = 0x01;
        public const ushort MAKE = 0x02;
        public const ushort RENAME = 0x03;
        public const ushort DELETE = 0x04;
        public const ushort RENAME_RETAINER = 0x06;

        private UInt64 sequence;

        private ushort command;
        private uint pid;
        private uint cid;
        private uint type;
        private uint ticket;
        private string charaName;
        private string worldName;

        public CharaCreatorPacket(UInt64 sequence, ushort command, uint pid, uint cid, uint ticket, string charaName, string worldName)
        {
            this.sequence = sequence;
            this.command = command;
            this.pid = pid;
            this.cid = cid;
            this.type = 0x400017;
            this.ticket = ticket;
            this.charaName = charaName; 
            this.worldName = worldName;
        }        

        public SubPacket buildPacket()    
        {
            MemoryStream memStream = new MemoryStream(0x1F0);
            BinaryWriter binWriter = new BinaryWriter(memStream);

            binWriter.Write((UInt64)sequence);
            binWriter.Write((byte)1);
            binWriter.Write((byte)1);
            binWriter.Write((UInt16)command);
            binWriter.Write((UInt32)0);

            binWriter.Write((UInt32)pid); //PID
            binWriter.Write((UInt32)cid); //CID
            binWriter.Write((UInt32)type); //Type?
            binWriter.Write((UInt32)ticket); //Ticket
            binWriter.Write(Encoding.ASCII.GetBytes(charaName.PadRight(0x20, '\0'))); //Name
            binWriter.Write(Encoding.ASCII.GetBytes(worldName.PadRight(0x20, '\0'))); //World Name

            byte[] data = memStream.GetBuffer();
            binWriter.Dispose();
            memStream.Dispose();

            return new SubPacket(OPCODE, 0xe0006868, 0xe0006868, data);
        }
    }
}

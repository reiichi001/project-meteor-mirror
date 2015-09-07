using FFXIVClassic_Lobby_Server.dataobjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Lobby_Server.packets
{
    class RetainerListPacket
    {
        public const ushort OPCODE = 0x17;
        public const ushort MAXPERPACKET = 3;

        private List<Retainer> retainerList;

        public RetainerListPacket(List<Retainer> retainerList)
        {
            this.retainerList = retainerList;
        }        

        public List<SubPacket> buildPackets()
        {
            List<SubPacket> subPackets = new List<SubPacket>();

            int retainerCount = 0;
            int totalCount = 0;

            MemoryStream memStream = null;
            BinaryWriter binWriter = null;

            foreach (Retainer chara in retainerList)
            {
                if (totalCount == 0 || retainerCount % MAXPERPACKET == 0)
                {
                    memStream = new MemoryStream(0x210);
                    binWriter = new BinaryWriter(memStream);

                    //Write List Info
                    binWriter.Write((UInt64)0);
                    binWriter.Write(retainerList.Count - totalCount <= MAXPERPACKET ? (byte)(retainerList.Count + 1) : (byte)0);
                    binWriter.Write(retainerList.Count - totalCount <= MAXPERPACKET ? (UInt32)(retainerList.Count - totalCount) : (UInt32)MAXPERPACKET);
                    binWriter.Write((byte)6);
                    binWriter.Write((UInt16)5);
                }

                //Write Entries
                //binWriter.Write((ushort)world.id);
                //binWriter.Write((ushort)world.listPosition);
                //binWriter.Write((uint)world.population);
                //binWriter.Write((UInt64)0);
                //binWriter.Write(Encoding.ASCII.GetBytes(world.name.PadRight(64, '\0')));

                retainerCount++;
                totalCount++;

                //Send this chunk of character list
                if (retainerCount >= MAXPERPACKET)
                {
                    byte[] data = memStream.GetBuffer();
                    binWriter.Dispose();
                    memStream.Dispose();
                    SubPacket subpacket = new SubPacket(OPCODE, 0xe0006868, 0xe0006868, data);
                    subPackets.Add(subpacket);
                    retainerCount = 0;
                }

            }

            //If there is anything left that was missed or the list is empty
            if (retainerCount > 0 || retainerList.Count == 0)
            {
                if (retainerList.Count == 0)
                {
                    memStream = new MemoryStream(0x210);
                    binWriter = new BinaryWriter(memStream);

                    //Write Empty List Info
                    binWriter.Write((UInt64)0);
                    binWriter.Write((byte)1);
                    binWriter.Write((UInt32)0);
                    binWriter.Write((byte)0);
                    binWriter.Write((UInt16)0);
                }

                byte[] data = memStream.GetBuffer();
                binWriter.Dispose();
                memStream.Dispose();
                SubPacket subpacket = new SubPacket(OPCODE, 0xe0006868, 0xe0006868, data);
                subPackets.Add(subpacket);
            }

            return subPackets;
        }
    }
}

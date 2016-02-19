using FFXIVClassic_Lobby_Server.dataobjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Lobby_Server.packets
{
    class ImportListPacket
    {
        public const ushort OPCODE = 0x16;
        public const ushort MAXPERPACKET = 12;

        private UInt64 sequence;
        private List<String> namesList;

        public ImportListPacket(UInt64 sequence, List<String> names)
        {
            this.sequence = sequence;
            this.namesList = names;
        }        

        public List<SubPacket> buildPackets()
        {
            List<SubPacket> subPackets = new List<SubPacket>();

            int namesCount = 0;
            int totalCount = 0;

            MemoryStream memStream = null;
            BinaryWriter binWriter = null;

            foreach (String name in namesList)
            {
                if (totalCount == 0 || namesCount % MAXPERPACKET == 0)
                {
                    memStream = new MemoryStream(0x210);
                    binWriter = new BinaryWriter(memStream);

                    //Write List Info
                    binWriter.Write((UInt64)sequence);
                    byte listTracker = (byte)((MAXPERPACKET * 2) * subPackets.Count);
                    binWriter.Write(namesList.Count - totalCount <= MAXPERPACKET ? (byte)(listTracker + 1) : (byte)(listTracker));
                    binWriter.Write(namesList.Count - totalCount <= MAXPERPACKET ? (UInt32)(namesList.Count - totalCount) : (UInt32)MAXPERPACKET);
                    binWriter.Write((byte)0);
                    binWriter.Write((UInt16)0);
                }

                //Write Entries
                binWriter.Write((uint)0);
                binWriter.Write((uint)totalCount);

                if (!name.Contains(" "))
                    binWriter.Write(Encoding.ASCII.GetBytes((name+" Last").PadRight(0x20, '\0')));
                else
                    binWriter.Write(Encoding.ASCII.GetBytes(name.PadRight(0x20, '\0')));

                namesCount++;
                totalCount++;

                //Send this chunk of world list
                if (namesCount >= MAXPERPACKET)
                {
                    byte[] data = memStream.GetBuffer();
                    binWriter.Dispose();
                    memStream.Dispose();
                    SubPacket subpacket = new SubPacket(OPCODE, 0xe0006868, 0xe0006868, data);
                    subPackets.Add(subpacket);
                    namesCount = 0;
                }

            }

            //If there is anything left that was missed or the list is empty
            if (namesCount > 0 || namesList.Count == 0)
            {
                if (namesList.Count == 0)
                {
                    memStream = new MemoryStream(0x210);
                    binWriter = new BinaryWriter(memStream);

                    //Write Empty List Info
                    binWriter.Write((UInt64)sequence);
                    byte listTracker = (byte)((MAXPERPACKET * 2) * subPackets.Count);
                    binWriter.Write(namesList.Count - totalCount <= MAXPERPACKET ? (byte)(listTracker + 1) : (byte)(listTracker));
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

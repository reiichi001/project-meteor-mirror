using FFXIVClassic_Lobby_Server.dataobjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Lobby_Server.packets
{
    class WorldListPacket
    {
        private List<World> worldList;

        public WorldListPacket(List<World> serverList)
        {
            this.worldList = serverList;
        }        

        public List<SubPacket> buildPackets()
        {
            List<SubPacket> subPackets = new List<SubPacket>();

            int serverCount = 0;
            int totalCount = 0;

            MemoryStream memStream = null;
            BinaryWriter binWriter = null;

            foreach (World world in worldList)
            {
                if (totalCount == 0 || serverCount % 6 == 0)
                {
                    memStream = new MemoryStream(0x210);
                    binWriter = new BinaryWriter(memStream);

                    //Write List Info
                    binWriter.Write((UInt64)0);
                    binWriter.Write(worldList.Count - totalCount <= 6 ? (byte)(worldList.Count + 1) : (byte)0);
                    binWriter.Write(worldList.Count - totalCount <= 6 ? (UInt32)(worldList.Count - totalCount) : (UInt32)6);
                    binWriter.Write((byte)6);
                    binWriter.Write((UInt16)5);
                }

                //Write Entries
                binWriter.Write((ushort)world.id);
                binWriter.Write((ushort)world.listPosition);
                binWriter.Write((uint)world.population);
                binWriter.Write((UInt64)0);
                binWriter.Write(Encoding.ASCII.GetBytes(world.name.PadRight(64, '\0')));

                serverCount++;
                totalCount++;

                //Send this chunk of world list
                if (serverCount >= 6)
                {
                    byte[] data = memStream.GetBuffer();
                    binWriter.Dispose();
                    memStream.Dispose();
                    SubPacket subpacket = new SubPacket(0x15, 0xe0006868, 0xe0006868, data);
                    subPackets.Add(subpacket);
                    serverCount = 0;
                }

            }

            //If there is anything left that was missed or the list is empty
            if (serverCount > 0 || worldList.Count == 0)
            {
                if (worldList.Count == 0)
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
                SubPacket subpacket = new SubPacket(0x15, 0xe0006868, 0xe0006868, data);
                subPackets.Add(subpacket);
            }

            return subPackets;
        }
    }
}

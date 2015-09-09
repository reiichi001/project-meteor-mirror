using FFXIVClassic_Lobby_Server.dataobjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Lobby_Server.packets
{
    class CharacterListPacket
    {
        public const ushort OPCODE = 0x0D;
        public const ushort MAXPERPACKET = 2;

        private ulong sequence;
        private List<Character> characterList;

        public CharacterListPacket(ulong sequence, List<Character> characterList)
        {
            this.sequence = sequence;
            this.characterList = characterList;
        }        

        public List<SubPacket> buildPackets()
        {
            List<SubPacket> subPackets = new List<SubPacket>();

            int characterCount = 0;
            int totalCount = 0;

            MemoryStream memStream = null;
            BinaryWriter binWriter = null;

            foreach (Character chara in characterList)
            {
                if (totalCount == 0 || characterCount % MAXPERPACKET == 0)
                {
                    memStream = new MemoryStream(0x3D0);
                    binWriter = new BinaryWriter(memStream);

                    //Write List Info
                    binWriter.Write((UInt64)sequence);
                    binWriter.Write(characterList.Count - totalCount <= MAXPERPACKET ? (byte)(1) : (byte)0);
                    //binWriter.Write((byte)1);
                    binWriter.Write(characterList.Count - totalCount <= MAXPERPACKET ? (UInt32)(characterList.Count - totalCount) : (UInt32)MAXPERPACKET);
                    binWriter.Write((byte)0);
                    binWriter.Write((UInt16)0);
                }

                binWriter.Seek(0x10 + (0x1D0 * characterCount), SeekOrigin.Begin);

                //Write Entries
                World world = Database.getServer(chara.serverId);
                string worldname = world == null ? "Unknown" : world.name;

                binWriter.Write((uint)0); //???
                binWriter.Write((uint)chara.id); //Character Id            
                binWriter.Write((byte)totalCount); //Slot

                byte options = 0;
                if (chara.state == 2)
                    options |= 0x01;
                if (chara.doRename)
                    options |= 0x02;
                if (chara.isLegacy)
                    options |= 0x08;

                binWriter.Write((byte)options); //Options (0x01: Service Account not active, 0x72: Change Chara Name) 
                binWriter.Write((ushort)0);  
                binWriter.Write((uint)0xF4); //Logged out zone
                binWriter.Write(Encoding.ASCII.GetBytes(chara.name.PadRight(0x20, '\0'))); //Name
                binWriter.Write(Encoding.ASCII.GetBytes(worldname.PadRight(0xE, '\0'))); //World Name
                binWriter.Write("wAQAAOonIyMNAAAAV3Jlbml4IFdyb25nABwAAAAEAAAAAwAAAAMAAAA_8OADAAHQFAAEAAABAAAAABTQCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGEgAAAAMQAAQCQAAMAsAACKVAAAAPgCAAAAAAAAAAAAAAAAAAAAAAAAAAAAACQAAAAkAwAAAAAAAAAAANvb1M05AQAABBoAAAEABqoiIuIKAAAAcHJ2MElubjAxABEAAABkZWZhdWx0VGVycml0b3J5AAwJAhcABAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAIAAAAAAAAAAAAAAAA="); //Appearance Data
                
                characterCount++;
                totalCount++;                

                //Send this chunk of character list
                if (characterCount >= MAXPERPACKET)
                {
                    byte[] data = memStream.GetBuffer();
                    binWriter.Dispose();
                    memStream.Dispose();
                    SubPacket subpacket = new SubPacket(OPCODE, 0xe0006868, 0xe0006868, data);
                    subPackets.Add(subpacket);
                    characterCount = 0;
                }                

            }

            //If there is anything left that was missed or the list is empty
            if (characterCount > 0 || characterList.Count == 0)
            {
                if (characterList.Count == 0)
                {
                    memStream = new MemoryStream(0x3D0);
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

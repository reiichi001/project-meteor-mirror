using FFXIVClassic_Lobby_Server.dataobjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Lobby_Server.packets
{
    class AccountListPacket
    {
        public const ushort OPCODE = 0x0C;
        public const ushort MAXPERPACKET = 8;

        private UInt64 sequence;
        private List<Account> accountList;

        public AccountListPacket(UInt64 sequence, List<Account> accountList)
        {
            this.sequence = sequence;
            this.accountList = accountList;
        }        

        public List<SubPacket> buildPackets()
        {
            List<SubPacket> subPackets = new List<SubPacket>();

            int accountCount = 0;
            int totalCount = 0;

            MemoryStream memStream = null;
            BinaryWriter binWriter = null;

            foreach (Account account in accountList)
            {
                if (totalCount == 0 || accountCount % MAXPERPACKET == 0)
                {
                    memStream = new MemoryStream(0x280);
                    binWriter = new BinaryWriter(memStream);

                    //Write List Info
                    binWriter.Write((UInt64)sequence);
                    byte listTracker = (byte)((MAXPERPACKET * 2) * subPackets.Count);
                    binWriter.Write(accountList.Count - totalCount <= MAXPERPACKET ? (byte)(listTracker + 1) : (byte)(listTracker));
                    binWriter.Write(accountList.Count - totalCount <= MAXPERPACKET ? (UInt32)(accountList.Count - totalCount) : (UInt32)MAXPERPACKET);
                    binWriter.Write((byte)0);
                    binWriter.Write((UInt16)0);
                }

                //Write Entries
                binWriter.Write((UInt32)account.id);
                binWriter.Write((UInt32)0);
                binWriter.Write(Encoding.ASCII.GetBytes(account.name.PadRight(0x40, '\0')));

                accountCount++;
                totalCount++;

                //Send this chunk of world list
                if (accountCount >= MAXPERPACKET)
                {
                    byte[] data = memStream.GetBuffer();
                    binWriter.Dispose();
                    memStream.Dispose();
                    SubPacket subpacket = new SubPacket(OPCODE, 0xe0006868, 0xe0006868, data);
                    subPackets.Add(subpacket);
                    accountCount = 0;
                }

            }

            //If there is anything left that was missed or the list is empty
            if (accountCount > 0 || accountList.Count == 0)
            {
                if (accountList.Count == 0)
                {
                    memStream = new MemoryStream(0x210);
                    binWriter = new BinaryWriter(memStream);

                    //Write Empty List Info
                    binWriter.Write((UInt64)sequence);
                    byte listTracker = (byte)((MAXPERPACKET * 2) * subPackets.Count);
                    binWriter.Write(accountList.Count - totalCount <= MAXPERPACKET ? (byte)(listTracker + 1) : (byte)(listTracker));
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

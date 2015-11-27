using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.actor
{
    class SetActorPropetyPacket
    {

        public const ushort OPCODE = 0x0137;
        public const uint PACKET_SIZE = 0xA8;

        private const ushort MAXBYTES = 0x7D;

        //These are known property IDs
        public const uint NAMEPLATE_SHOWN = 0xFBFBCFB1;
        public const uint TARGETABLE = 0x2138FD71;
        public const uint LS_CREST = 0xBC98941D;

        public const uint HP = 0x4232BCAA;
        public const uint MP = 0x13F89710;
        public const uint TP = 0;

        public const uint HPMAX = 0x7BCDFB69;
        public const uint MPMAX = 0x3C95D5C5;
        public const uint TPMAX = 0;

        public const uint JOB = 0x7532CE24;
        public const uint LEVEL = 0x96063588;

        public const uint STAT_STRENGTH = 0x647A29A8;
        public const uint STAT_VITALITY = 0x939E884A;
        public const uint STAT_DEXTERITY = 0x416571AC;
        public const uint STAT_INTELLIGENCE = 0x2DFBC13A;
        public const uint STAT_MIND = 0x0E704141;
        public const uint STAT_PIETY = 0x6CCAF8B3;        

        public const uint STAT_ACCURACY = 0x91CD44E7;
        public const uint STAT_EVASION = 0x11B1B22D;
        public const uint STAT_ATTACK = 0xBA51C4E1;
        public const uint STAT_DEFENSE = 0x8CAE90DB;
        public const uint STAT_ATTACK_MAGIC_POTENCY = 0x1F3DACC5;
        public const uint STAT_HEALING_MAGIC_POTENCY = 0xA329599A;
        public const uint STAT_ENCHANCEMENT_MAGIC_POTENCY = 0xBA51C4E1;
        public const uint STAT_ENFEEBLING_MAGIC_POTENCY = 0xEB90BAAB;
        public const uint STAT_MAGIC_ACCURACY = 0xD57DC284;
        public const uint STAT_MAGIC_EVASION = 0x17AB37EF;

        public const uint RESISTANCE_FIRE = 0x79C7ECFF;
        public const uint RESISTANCE_ICE = 0xE17D8C7A;
        public const uint RESISTANCE_WIND = 0x204CF942;
        public const uint RESISTANCE_LIGHTNING = 0x1C2AEC73;
        public const uint RESISTANCE_EARTH = 0x5FC56D16;
        public const uint RESISTANCE_WATER = 0x64803E98;               

        public const uint TRIBE = 0x774A02BF;
        public const uint GUARDIAN = 0x5AB3D930;
        public const uint BIRTHDAY = 0x822C9556;
        public const uint BIRTHMONTH = 0x0EFB92D4;
        public const uint ALLEGIANCE = 0xAAD96353;
        //End of properties

        private ushort runningByteTotal = 0;
        private byte[] data = new byte[PACKET_SIZE - 0x20];

        private MemoryStream mem;
        private BinaryWriter binWriter;

        public SetActorPropetyPacket()
        {
            mem = new MemoryStream(data);
            binWriter = new BinaryWriter(mem);
            binWriter.Seek(1, SeekOrigin.Begin);
        }

        public void closeStreams()
        {
            binWriter.Dispose();
            mem.Dispose();
        }

        public bool addByte(uint id, byte value)
        {
            if (runningByteTotal + 6 > MAXBYTES)
                return false;

            binWriter.Write((byte)1);
            binWriter.Write((UInt32)id);
            binWriter.Write((byte)value);
            runningByteTotal+=6;

            return true;
        }

        public bool addShort(uint id, ushort value)
        {
            if (runningByteTotal + 7 > MAXBYTES)
                return false;

            binWriter.Write((byte)2);
            binWriter.Write((UInt32)id);
            binWriter.Write((UInt16)value);
            runningByteTotal+=7;

            return true;
        }

        public bool addInt(uint id, uint value)
        {
            if (runningByteTotal + 9 > MAXBYTES)
                return false;

            binWriter.Write((byte)4);
            binWriter.Write((UInt32)id);
            binWriter.Write((UInt32)value);
            runningByteTotal+=9;

            return true;
        }

        public bool addBuffer(uint id, byte[] buffer)
        {
            if (runningByteTotal + 5 + buffer.Length  > MAXBYTES)
                return false;

            binWriter.Write((byte)buffer.Length);
            binWriter.Write((UInt32)id);
            binWriter.Write(buffer);
            runningByteTotal += (ushort)(5 + buffer.Length);

            return true;
        }

        public void setTarget(string target)
        {
            binWriter.Write((byte)(0x82 + target.Length));
            binWriter.Write(target);
            runningByteTotal += (ushort)(1 + target.Length);

        }

        public SubPacket buildPacket(uint playerActorID, uint actorID)
        {
            binWriter.Seek(0, SeekOrigin.Begin);
            binWriter.Write((byte)runningByteTotal);
            
            closeStreams();

            SubPacket packet = new SubPacket(OPCODE, playerActorID, actorID, data);
            return packet;
        }       
    
    }
}

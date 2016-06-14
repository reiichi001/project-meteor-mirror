using FFXIVClassic.Common;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FFXIVClassic_Map_Server.packets.send.actor
{
    class SetActorPropetyPacket
    {

        public const ushort OPCODE = 0x0137;
        public const uint PACKET_SIZE = 0xA8;

        private const ushort MAXBYTES = 0x7D;        

        private ushort runningByteTotal = 0;
        private byte[] data = new byte[PACKET_SIZE - 0x20];

        private bool isArrayMode = false;
        private bool isMore = false;

        string currentTarget;

        private MemoryStream mem;
        private BinaryWriter binWriter;

        public SetActorPropetyPacket(string startingTarget)
        {
            mem = new MemoryStream(data);
            binWriter = new BinaryWriter(mem);
            binWriter.Seek(1, SeekOrigin.Begin);
            currentTarget = startingTarget;
        }

        public void CloseStreams()
        {
            binWriter.Dispose();
            mem.Dispose();
        }

        public bool AddByte(uint id, byte value)
        {
            if (runningByteTotal + 6 + (1 + Encoding.ASCII.GetByteCount(currentTarget)) > MAXBYTES)
                return false;

            binWriter.Write((byte)1);
            binWriter.Write((UInt32)id);
            binWriter.Write((byte)value);
            runningByteTotal+=6;

            return true;
        }

        public bool AddShort(uint id, ushort value)
        {
            if (runningByteTotal + 7 + (1 + Encoding.ASCII.GetByteCount(currentTarget)) > MAXBYTES)
                return false;

            binWriter.Write((byte)2);
            binWriter.Write((UInt32)id);
            binWriter.Write((UInt16)value);
            runningByteTotal+=7;

            return true;
        }

        public bool AddInt(uint id, uint value)
        {
            if (runningByteTotal + 9 + (1 + Encoding.ASCII.GetByteCount(currentTarget)) > MAXBYTES)
                return false;

            binWriter.Write((byte)4);
            binWriter.Write((UInt32)id);
            binWriter.Write((UInt32)value);
            runningByteTotal+=9;

            return true;
        }

        public bool AddBuffer(uint id, byte[] buffer)
        {
            if (runningByteTotal + 5 + buffer.Length + (1 + Encoding.ASCII.GetByteCount(currentTarget)) > MAXBYTES)
                return false;

            binWriter.Write((byte)buffer.Length);
            binWriter.Write((UInt32)id);
            binWriter.Write(buffer);
            runningByteTotal += (ushort)(5 + buffer.Length);

            return true;
        }

        public bool AddBuffer(uint id, byte[] buffer, int index, int length, int page)
        {
            if (runningByteTotal + 5 + length + (1 + Encoding.ASCII.GetByteCount(currentTarget)) > MAXBYTES)
                return false;

            binWriter.Write((byte)(length + 1));
            binWriter.Write((UInt32)id);
            binWriter.Write(buffer, index, length);
            binWriter.Write((byte)page);
            runningByteTotal += (ushort)(6 + length);

            return true;
        }

        public bool AddProperty(FFXIVClassic_Map_Server.Actors.Actor actor, string name)
        {
            string[] split = name.Split('.');
            int arrayIndex = 0;

            if (!(split[0].Equals("work") || split[0].Equals("charaWork") || split[0].Equals("playerWork") || split[0].Equals("npcWork")))
                return false;

            Object curObj = actor;
            for (int i = 0; i < split.Length; i++)
            {
                //For arrays
                if (split[i].Contains('['))
                {
                    if (split[i].LastIndexOf(']') - split[i].IndexOf('[') <= 0)
                        return false;

                    arrayIndex = Convert.ToInt32(split[i].Substring(split[i].IndexOf('[') + 1, split[i].LastIndexOf(']') - split[i].LastIndexOf('[')-1));
                    split[i] = split[i].Substring(0, split[i].IndexOf('['));
                }

                FieldInfo field = curObj.GetType().GetField(split[i]);
                if (field == null)
                    return false;

                curObj = field.GetValue(curObj);
                if (curObj == null)
                    return false;
            }

            if (curObj == null)
                return false;
            else
            {
                //Array, we actually care whats inside
                if (curObj.GetType().IsArray)
                {
                    if (((Array)curObj).Length <= arrayIndex)
                        return false;
                    curObj = ((Array)curObj).GetValue(arrayIndex);
                }

                if (curObj == null)
                    return false;

                //Cast to the proper object and Add to packet
                uint id = Utils.MurmurHash2(name, 0);
                if (curObj is bool)                
                    return AddByte(id, (byte)(((bool)curObj) ? 1 : 0));
                else if (curObj is byte)
                    return AddByte(id, (byte)curObj);
                else if (curObj is ushort)
                    return AddShort(id, (ushort)curObj);
                else if (curObj is short)
                    return AddShort(id, (ushort)(short)curObj);
                else if (curObj is uint)
                    return AddInt(id, (uint)curObj);
                else if (curObj is int)
                    return AddInt(id, (uint)(int)curObj);
                else if (curObj is float)
                    return AddBuffer(id, BitConverter.GetBytes((float)curObj));
                else
                    return false;
            }
        }

        public void SetIsArrayMode(bool flag)
        {
            isArrayMode = flag;
        }

        public void SetIsMore(bool flag)
        {
            isMore = flag;
        }

        public void SetTarget(string tarGet)
        {
            currentTarget = tarGet;
        }

        public void AddTarget()
        {
            if (isArrayMode)
                binWriter.Write((byte)(0xA4 + currentTarget.Length));
            else
                binWriter.Write((byte)(isMore ? 0x60 + currentTarget.Length : 0x82 + currentTarget.Length));
            binWriter.Write(Encoding.ASCII.GetBytes(currentTarget));
            runningByteTotal += (ushort)(1 + Encoding.ASCII.GetByteCount(currentTarget));
        }

        public void AddTarget(string newTarget)
        {
            binWriter.Write((byte)(isMore ? 0x60 + currentTarget.Length : 0x82 + currentTarget.Length));
            binWriter.Write(Encoding.ASCII.GetBytes(currentTarget));
            runningByteTotal += (ushort)(1 + Encoding.ASCII.GetByteCount(currentTarget));
            currentTarget = newTarget;
        }

        public SubPacket BuildPacket(uint playerActorID, uint actorID)
        {
            binWriter.Seek(0, SeekOrigin.Begin);
            binWriter.Write((byte)runningByteTotal);
            
            CloseStreams();

            SubPacket packet = new SubPacket(OPCODE, actorID, playerActorID, data);
            return packet;
        }       
    
    }
}

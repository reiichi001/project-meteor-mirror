using FFXIVClassic.Common;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FFXIVClassic_Map_Server.packets.send.actor
{
    class SetListPropetyPacket
    {
        public const ushort OPCODE = 0x017A;
        public const uint PACKET_SIZE = 0xB0;

        private const ushort MAXBYTES = 0x98;        

        private ushort runningByteTotal = 0;
        private byte[] data = new byte[PACKET_SIZE - 0x20];
        private bool isMore = false;

        private MemoryStream mem;
        private BinaryWriter binWriter;

        public SetListPropetyPacket(ulong listId)
        {
            mem = new MemoryStream(data);
            binWriter = new BinaryWriter(mem);
            binWriter.Write((UInt64)listId);
            binWriter.Seek(1, SeekOrigin.Current);
        }

        public void CloseStreams()
        {
            binWriter.Dispose();
            mem.Dispose();
        }

        public bool AcceptCallback(uint id, byte value)
        {
            if (runningByteTotal + 6 > MAXBYTES)
                return false;

            binWriter.Write((byte)1);
            binWriter.Write((UInt32)id);
            binWriter.Write((byte)value);
            runningByteTotal+=6;

            return true;
        }

        public bool AddShort(uint id, ushort value)
        {
            if (runningByteTotal + 7 > MAXBYTES)
                return false;

            binWriter.Write((byte)2);
            binWriter.Write((UInt32)id);
            binWriter.Write((UInt16)value);
            runningByteTotal+=7;

            return true;
        }

        public bool AddInt(uint id, uint value)
        {
            if (runningByteTotal + 9 > MAXBYTES)
                return false;

            binWriter.Write((byte)4);
            binWriter.Write((UInt32)id);
            binWriter.Write((UInt32)value);
            runningByteTotal+=9;

            return true;
        }

        public bool AddBuffer(uint id, byte[] buffer)
        {
            if (runningByteTotal + 5 + buffer.Length  > MAXBYTES)
                return false;

            binWriter.Write((byte)buffer.Length);
            binWriter.Write((UInt32)id);
            binWriter.Write(buffer);
            runningByteTotal += (ushort)(5 + buffer.Length);

            return true;
        }

        public void AddProperty(FFXIVClassic_Map_Server.Actors.Actor actor, string name)
        {
            string[] split = name.Split('.');
            int arrayIndex = 0;

            if (!(split[0].Equals("work") || split[0].Equals("charaWork") || split[0].Equals("playerWork") || split[0].Equals("npcWork")))
                return;

            Object curObj = actor;
            for (int i = 0; i < split.Length; i++)
            {
                //For arrays
                if (split[i].Contains('['))
                {
                    if (split[i].LastIndexOf(']') - split[i].IndexOf('[') <= 0)
                        return;

                    arrayIndex = Convert.ToInt32(split[i].Substring(split[i].IndexOf('[') + 1, split[i].Length - split[i].LastIndexOf(']')));
                    split[i] = split[i].Substring(0, split[i].IndexOf('['));
                }

                FieldInfo field = curObj.GetType().GetField(split[i]);
                if (field == null)
                    return;

                curObj = field.GetValue(curObj);
                if (curObj == null)
                    return;
            }

            if (curObj == null)
                return;
            else
            {
                //Array, we actually care whats inside
                if (curObj.GetType().IsArray)
                {
                    if (((Array)curObj).Length <= arrayIndex)
                        return;
                    curObj = ((Array)curObj).GetValue(arrayIndex);
                }

                if (curObj == null)
                    return;

                //Cast to the proper object and Add to packet
                uint id = Utils.MurmurHash2(name, 0);
                if (curObj is bool)                
                    AcceptCallback(id, (byte)(((bool)curObj) ? 1 : 0));
                else if (curObj is byte)
                    AcceptCallback(id, (byte)curObj);
                else if (curObj is ushort)
                    AddShort(id, (ushort)curObj);
                else if (curObj is short)                
                    AddShort(id, (ushort)(short)curObj);
                else if (curObj is uint)
                    AddInt(id, (uint)curObj);
                else if (curObj is int)                                    
                    AddInt(id, (uint)(int)curObj);
                else if (curObj is float)
                    AddBuffer(id, BitConverter.GetBytes((float)curObj));
                else
                    return;
            }
        }

        public void SetIsMore(bool flag)
        {
            isMore = flag;
        }

        public void SetTarget(string tarGet)
        {
            binWriter.Write((byte)(isMore ? 0x62 + tarGet.Length : 0x82 + tarGet.Length));
            binWriter.Write(Encoding.ASCII.GetBytes(tarGet));
            runningByteTotal += (ushort)(1 + Encoding.ASCII.GetByteCount(tarGet));

        }

        public SubPacket BuildPacket(uint playerActorID, uint actorID)
        {
            binWriter.Seek(0x8, SeekOrigin.Begin);
            binWriter.Write((byte)runningByteTotal);
            
            CloseStreams();

            SubPacket packet = new SubPacket(OPCODE, playerActorID, actorID, data);
            return packet;
        }       
    
    }
}

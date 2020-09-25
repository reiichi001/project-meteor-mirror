/*
===========================================================================
Copyright (C) 2015-2019 Project Meteor Dev Team

This file is part of Project Meteor Server.

Project Meteor Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Project Meteor Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with Project Meteor Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Meteor.Common;
using Meteor.World.DataObjects.Group;

namespace Meteor.World.Packets.Send.Subpackets.Groups
{
    class SynchGroupWorkValuesPacket
    {
        public const ushort OPCODE = 0x017A;
        public const uint PACKET_SIZE = 0xB0;

        private const ushort MAXBYTES = 0x98;

        private ushort runningByteTotal = 0;
        private byte[] data = new byte[PACKET_SIZE - 0x20];
        private bool isMore = false;

        private MemoryStream mem;
        private BinaryWriter binWriter;

        public SynchGroupWorkValuesPacket(ulong listId)
        {
            mem = new MemoryStream(data);
            binWriter = new BinaryWriter(mem);
            binWriter.Write((UInt64)listId);
            binWriter.Seek(1, SeekOrigin.Current);
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
            runningByteTotal += 6;

            return true;
        }

        public bool addShort(uint id, ushort value)
        {
            if (runningByteTotal + 7 > MAXBYTES)
                return false;

            binWriter.Write((byte)2);
            binWriter.Write((UInt32)id);
            binWriter.Write((UInt16)value);
            runningByteTotal += 7;

            return true;
        }

        public bool addInt(uint id, uint value)
        {
            if (runningByteTotal + 9 > MAXBYTES)
                return false;

            binWriter.Write((byte)4);
            binWriter.Write((UInt32)id);
            binWriter.Write((UInt32)value);
            runningByteTotal += 9;

            return true;
        }

        public bool addLong(uint id, ulong value)
        {
            if (runningByteTotal + 13 > MAXBYTES)
                return false;

            binWriter.Write((byte)8);
            binWriter.Write((UInt32)id);
            binWriter.Write((UInt64)value);
            runningByteTotal += 13;

            return true;
        }

        public bool addBuffer(uint id, byte[] buffer)
        {
            if (runningByteTotal + 5 + buffer.Length > MAXBYTES)
                return false;

            binWriter.Write((byte)buffer.Length);
            binWriter.Write((UInt32)id);
            binWriter.Write(buffer);
            runningByteTotal += (ushort)(5 + buffer.Length);

            return true;
        }

        public void addProperty(Group group, string name)
        {
            string[] split = name.Split('.');
            int arrayIndex = 0;

            if (!(split[0].Equals("work") || split[0].Equals("partyGroupWork")))
                return;

            Object curObj = group;
            for (int i = 0; i < split.Length; i++)
            {
                //For arrays
                if (split[i].Contains('['))
                {
                    if (split[i].LastIndexOf(']') - split[i].IndexOf('[') <= 0)
                        return;

                    arrayIndex = Convert.ToInt32(split[i].Substring(split[i].IndexOf('[') + 1, split[i].Length - split[i].LastIndexOf(']')));

                    split[i] = split[i].Substring(0, split[i].IndexOf('['));

                    if (i != split.Length - 1)
                    {
                        curObj = curObj.GetType().GetField(split[i]).GetValue(curObj);
                        curObj = ((Array)curObj).GetValue(arrayIndex);
                        i++;
                    }
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

                //Cast to the proper object and add to packet
                uint id = Utils.MurmurHash2(name, 0);
                if (curObj is bool)
                    addByte(id, (byte)(((bool)curObj) ? 1 : 0));
                else if (curObj is byte)
                    addByte(id, (byte)curObj);
                else if (curObj is ushort)
                    addShort(id, (ushort)curObj);
                else if (curObj is short)
                    addShort(id, (ushort)(short)curObj);
                else if (curObj is uint)
                    addInt(id, (uint)curObj);
                else if (curObj is int)
                    addInt(id, (uint)(int)curObj);
                else if (curObj is long)
                    addLong(id, (ulong)(long)curObj);
                else if (curObj is ulong)
                    addLong(id, (ulong)curObj);
                else if (curObj is float)
                    addBuffer(id, BitConverter.GetBytes((float)curObj));
                else
                    return;
            }
        }

        public void setIsMore(bool flag)
        {
            isMore = flag;
        }

        public void setTarget(string target)
        {
            binWriter.Write((byte)(isMore ? 0x62 + target.Length : 0x82 + target.Length));
            binWriter.Write(Encoding.ASCII.GetBytes(target));
            runningByteTotal += (ushort)(1 + Encoding.ASCII.GetByteCount(target));

        }

        public SubPacket buildPacket(uint playerActorID)
        {
            binWriter.Seek(0x8, SeekOrigin.Begin);
            binWriter.Write((byte)runningByteTotal);

            closeStreams();

            SubPacket packet = new SubPacket(OPCODE, playerActorID, data);
            return packet;
        }

    }
}


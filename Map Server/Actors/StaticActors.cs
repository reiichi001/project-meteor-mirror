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

using Meteor.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Meteor.Map.Actors
{
    class StaticActors
    {
        private Dictionary<uint, Actor> mStaticActors = new Dictionary<uint, Actor>();        

        public StaticActors(string path)
        {
            byte[] data = File.ReadAllBytes(path);

            if (data[0] == 's' && data[1] == 'a' && data[2] == 'n' && data[3] == 'e')
                data = DecryptStaticActorsFile(data);
                
            LoadStaticActors(data);
        }

        private byte[] DecryptStaticActorsFile(byte[] encoded)
        {
            byte[] decoded = new byte[encoded.Length - 13];

            MemoryStream sIn = new MemoryStream(encoded);
            MemoryStream sOut = new MemoryStream(decoded);

            BinaryReader binReader = new BinaryReader(sIn);
            BinaryWriter binWriter = new BinaryWriter(sOut);

            binReader.BaseStream.Seek(13, SeekOrigin.Begin);

            while (true)
            {
                try
                {
                    byte byteIn = binReader.ReadByte();
                    byte byteOut = (Byte)(byteIn ^ 0x73);
                    binWriter.Write((Byte)byteOut);
                }
                catch (EndOfStreamException) { break; }
            }

            binReader.Close();
            binWriter.Close();

            return decoded;
        }

        private bool LoadStaticActors(byte[] data) 
        {
            try
            {
                using (MemoryStream s = new MemoryStream(data))
                {
                    using (BinaryReader binReader = new BinaryReader(s))
                    {

                        while (binReader.BaseStream.Position != binReader.BaseStream.Length)
                        {
                            uint id = Utils.SwapEndian(binReader.ReadUInt32()) | 0xA0F00000;

                            List<byte> list = new List<byte>();
                            byte readByte;

                            while ((readByte = binReader.ReadByte()) != 0)
                                list.Add(readByte);

                            string output = Encoding.UTF8.GetString(list.ToArray());
                            string actorType = output.Split('/')[1];
                            string actorName = output.Substring(1 + output.LastIndexOf("/"));

                            if (actorType.Equals("Command"))
                                mStaticActors.Add(id, new Command(id, actorName));
                            else if (actorType.Equals("Quest"))
                                mStaticActors.Add(id, new Quest(id, actorName));
                            //else if (actorType.Equals("Status"))
                            //mStaticActors.Add(id, new Status(id, actorName));
                            else if (actorType.Equals("Judge"))
                                mStaticActors.Add(id, new Judge(id, actorName));

                        }


                    }
                }
            }
            catch(FileNotFoundException)
            { Program.Log.Error("Could not find staticactors file."); return false; }

            Program.Log.Info("Loaded {0} static actors.", mStaticActors.Count());

            return true;
        }

        public bool Exists(uint actorId)
        {
            return mStaticActors[actorId] != null;
        }

        public Actor FindStaticActor(string name)
        {
            foreach (Actor a in mStaticActors.Values)
            {
                if (a.actorName.Equals(name))
                    return a;
            }

            return null;
        }

        public Actor GetActor(uint actorId)
        {
            if (mStaticActors.ContainsKey(actorId))
                return mStaticActors[actorId];
            else
                return null;
        }

    }
    
}

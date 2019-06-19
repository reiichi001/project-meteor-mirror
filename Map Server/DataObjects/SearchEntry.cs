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
using System.Text;

namespace Meteor.Map.dataobjects
{
    class SearchEntry
    {
        public ushort preferredClass;
        public ushort langauges;
        public ushort location;
        public ushort grandCompany;
        public ushort status;
        public ushort currentClass;
        public string name;
        public ushort[] classes = new ushort[2 * 20];
        public ushort[] jobs = new ushort[8];

        public void WriteSearchEntry(BinaryWriter writer)
        {
            writer.Write((UInt16)preferredClass);
            writer.Write((UInt16)langauges);
            writer.Write((UInt16)location);
            writer.Write((UInt16)grandCompany);
            writer.Write((UInt16)status);
            writer.Write((UInt16)currentClass);

            writer.Write(Encoding.ASCII.GetBytes(name), 0, Encoding.ASCII.GetByteCount(name) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(name));

            for (int i = 0; i < classes.Length; i++)
                writer.Write((UInt16)classes[i]);
            for (int i = 0; i < jobs.Length; i++)
                writer.Write((UInt16)jobs[i]);
        }
    }
}

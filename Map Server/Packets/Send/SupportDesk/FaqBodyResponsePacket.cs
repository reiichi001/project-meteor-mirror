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

using System.IO;
using System.Text;

using Meteor.Common;

namespace Meteor.Map.packets.send.supportdesk
{
    class FaqBodyResponsePacket
    {
        public const ushort OPCODE = 0x01D1;
        public const uint PACKET_SIZE = 0x587;

        public static SubPacket BuildPacket(uint playerActorID, string faqsBodyText)
        {            
            byte[] data = new byte[PACKET_SIZE - 0x20];
            int maxBodySize = data.Length - 0x04;

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                        binWriter.Seek(4, SeekOrigin.Begin);
                        binWriter.Write(Encoding.ASCII.GetBytes(faqsBodyText), 0, Encoding.ASCII.GetByteCount(faqsBodyText) >= maxBodySize ? maxBodySize : Encoding.ASCII.GetByteCount(faqsBodyText));                    
                }
            }

            return new SubPacket(OPCODE, playerActorID, data);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.dataobjects
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

        public void writeSearchEntry(BinaryWriter writer)
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

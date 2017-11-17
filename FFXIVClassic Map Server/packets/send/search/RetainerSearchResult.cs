using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.search
{
    class RetainerSearchResult
    {
        public uint itemId;
        public uint marketWard;
        public uint gilCostPerItem;
        public uint quantity;
        public byte numStack;
        public byte quality;
        public string sellerRetainerName;
        public byte[] materiaType = new byte[5];
        public byte[] materiaGrade = new byte[5];
    }
}

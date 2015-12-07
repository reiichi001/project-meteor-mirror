using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.dataobjects
{
    class RecruitmentDetails
    {
        public string recruiterName;
        public string comment;

        public uint purposeId;
        public uint locationId;
        public uint subTaskId;
        public uint timeSinceStart;

        public uint[] discipleId = new uint[4];
        public uint[] classjobId = new uint[4];
        public byte[] minLvl = new byte[4];
        public byte[] maxLvl = new byte[4];
        public byte[] num = new byte[4];

    }
}

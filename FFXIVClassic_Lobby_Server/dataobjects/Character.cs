using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.dataobjects;

namespace FFXIVClassic_Lobby_Server
{
    class Character
    {
        public uint id;
        public ushort slot;
        public ushort serverId;
        public string name;
        public ushort state;
        public string charaInfo;
        public bool isLegacy;
        public bool doRename;
        public uint currentZoneId;

        public byte guardian = 0;
        public byte birthMonth = 0;
        public byte birthDay = 0;
        public uint currentClass = 3;
        public uint currentJob = 0;
        public byte initialTown = 0;
        public byte tribe = 0;

        public uint currentLevel = 1;

        public static CharaInfo EncodedToCharacter(String charaInfo)
        {
            charaInfo.Replace("+", "-");
            charaInfo.Replace("/", "_");
            byte[] data = System.Convert.FromBase64String(charaInfo);

            Console.WriteLine("------------Base64 printout------------------");
            Console.WriteLine(Utils.ByteArrayToHex(data));
            Console.WriteLine("------------Base64 printout------------------");

            CharaInfo chara = new CharaInfo();

            return chara;
        }
    }
}

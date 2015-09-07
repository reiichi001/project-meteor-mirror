using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Lobby_Server.common;

namespace FFXIVClassic_Lobby_Server
{
    class Character
    {
        public string		name = "Test Test";
        public string       world = "Test World";

        public uint	        id = 0;

        public uint         tribe = 0;
        public uint         size = 0;
        public uint         voice = 0;
        public uint         skinColor = 0;

        public uint         hairStyle = 0;
        public uint         hairColor = 0;
        public uint         eyeColor = 0;

        public uint         faceType = 0;
        public uint         faceBrow = 0;
        public uint         faceEye = 0;
        public uint         faceIris = 0;
        public uint         faceNose = 0;
        public uint         faceMouth = 0;
        public uint         faceJaw = 0;
        public uint         faceCheek = 0;
        public uint         faceOption1 = 0;
        public uint         faceOption2 = 0;

        public uint         guardian = 0;
        public uint         birthMonth = 0;
        public uint         birthDay = 0;
        public uint         allegiance = 0;

        public uint         weapon1 = 0;
        public uint         weapon2 = 0;

        public uint         headGear = 0;
        public uint         bodyGear = 0;
        public uint         legsGear = 0;
        public uint         handsGear = 0;
        public uint         feetGear = 0;
        public uint         waistGear = 0;
        public uint         rightEarGear = 0;
        public uint         leftEarGear = 0;
        public uint         rightFingerGear = 0;
        public uint         leftFingerGear = 0;
        
        public byte[] toBytes()
        {
            byte[] bytes = new byte[0x120];
            return bytes;
        }

        public static String characterToEncoded(Character chara)
        {
            String charaInfo = System.Convert.ToBase64String(chara.toBytes());
            charaInfo.Replace("+", "-");
            charaInfo.Replace("/", "_");
            return charaInfo;
        }

        public static Character EncodedToCharacter(String charaInfo)
        {
            charaInfo.Replace("+", "-");
            charaInfo.Replace("/", "_");
            byte[] data = System.Convert.FromBase64String(charaInfo);

            Console.WriteLine("------------Base64 printout------------------");
            Console.WriteLine(Utils.ByteArrayToHex(data));
            Console.WriteLine("------------Base64 printout------------------");

                Character chara = new Character();
            return chara;
        }
    }
}

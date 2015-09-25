using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Lobby_Server.common
{
    static class Utils
    {
        private static readonly uint[] _lookup32 = CreateLookup32();

        private static uint[] CreateLookup32()
        {
            var result = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                string s = i.ToString("X2");
                result[i] = ((uint)s[0]) + ((uint)s[1] << 16);
            }
            return result;
        }

        public static string ByteArrayToHex(byte[] bytes)
        {
            var lookup32 = _lookup32;
            var result = new char[(bytes.Length * 3) + ((bytes.Length/16) < 1 ? 1 : (bytes.Length/16)*3) + bytes.Length+60];
            int numNewLines = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                var val = lookup32[bytes[i]];
                result[(3 * i) + (17 * numNewLines) + 0] = (char)val;
                result[(3 * i) + (17 * numNewLines) + 1] = (char)(val >> 16);
                result[(3 * i) + (17 * numNewLines) + 2] = ' ';

                result[(numNewLines * (3*16+17)) + (3 * 16) + (i % 16)] = (char)bytes[i] >= 32 && (char)bytes[i] <= 126 ? (char)bytes[i] : '.';
                
                if (i != bytes.Length - 1 && bytes.Length > 16 && i != 0 && (i+1) % 16 == 0)
                {
                    result[(numNewLines * (3*16+17)) + (3 * 16) + (16)] = '\n';
                    numNewLines++;
                }
               
            }
            return new string(result);
        }

        public static UInt32 UnixTimeStampUTC()
        {
            UInt32 unixTimeStamp;
            DateTime currentTime = DateTime.Now;
            DateTime zuluTime = currentTime.ToUniversalTime();
            DateTime unixEpoch = new DateTime(1970, 1, 1);
            unixTimeStamp = (UInt32)(zuluTime.Subtract(unixEpoch)).TotalSeconds;
            return unixTimeStamp;
        }

    }
}

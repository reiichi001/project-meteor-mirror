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

            return 0x55555555;
            //return unixTimeStamp;
        }

        public static uint MurmurHash2(string key, uint seed)
        {
	        // 'm' and 'r' are mixing constants generated offline.
	        // They're not really 'magic', they just happen to work well.

            byte[] data = Encoding.ASCII.GetBytes(key);
	        const uint m = 0x5bd1e995;
	        const int r = 24;
            int len = key.Length;
            int dataIndex = len - 4;

	        // Initialize the hash to a 'random' value

	        uint h = seed ^ (uint)len;

	        // Mix 4 bytes at a time into the hash
	        

	        while (len >= 4)
	        {
		        h *= m;

                uint k = (uint)BitConverter.ToInt32(data, dataIndex);
		        k =     ((k >> 24) & 0xff) | // move byte 3 to byte 0
			            ((k << 8) & 0xff0000) | // move byte 1 to byte 2
			            ((k >> 8) & 0xff00) | // move byte 2 to byte 1
			            ((k << 24) & 0xff000000); // byte 0 to byte 3

		        k *= m;
		        k ^= k >> r;
		        k *= m;

		        h ^= k;

		        dataIndex -= 4;
		        len -= 4;
	        }

	        // Handle the last few bytes of the input array

	        switch (len)
	        {
                case 3: 
                    h ^= (uint)data[2] << 16; goto case 2;
                case 2: 
                    h ^= (uint)data[0] << 8; goto case 1;
	            case 1: 
                    h ^= data[1];
		            h *= m;
                    break;
	        };

	        // Do a few final mixes of the hash to ensure the last few
	        // bytes are well-incorporated.

	        h ^= h >> 13;
	        h *= m;
	        h ^= h >> 15;

	        return h;
        }

    }
}

using System;
using System.Text;

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
            if (bytes == null) return "<null>";
            int bytesLength = bytes.Length;
            var bytesPerLine = 16;
            char[] HexChars = "0123456789ABCDEF".ToCharArray();

            int firstHexColumn =
                  8                   // 8 characters for the address
                + 3;                  // 3 spaces

            int firstCharColumn = firstHexColumn
                + bytesPerLine * 3       // - 2 digit for the hexadecimal value and 1 space
                + (bytesPerLine - 1) / 8 // - 1 extra space every 8 characters from the 9th
                + 2;                  // 2 spaces 

            int lineLength = firstCharColumn
                + bytesPerLine           // - characters to show the ascii value
                + Environment.NewLine.Length; // Carriage return and line feed (should normally be 2)

            char[] line = (new String(' ', lineLength - Environment.NewLine.Length) + Environment.NewLine).ToCharArray();
            int expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
            StringBuilder result = new StringBuilder(expectedLines * lineLength);

            for (int i = 0; i < bytesLength; i += bytesPerLine)
            {
                line[0] = HexChars[(i >> 28) & 0xF];
                line[1] = HexChars[(i >> 24) & 0xF];
                line[2] = HexChars[(i >> 20) & 0xF];
                line[3] = HexChars[(i >> 16) & 0xF];
                line[4] = HexChars[(i >> 12) & 0xF];
                line[5] = HexChars[(i >> 8) & 0xF];
                line[6] = HexChars[(i >> 4) & 0xF];
                line[7] = HexChars[(i >> 0) & 0xF];

                int hexColumn = firstHexColumn;
                int charColumn = firstCharColumn;

                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (j > 0 && (j & 7) == 0) hexColumn++;
                    if (i + j >= bytesLength)
                    {
                        line[hexColumn] = ' ';
                        line[hexColumn + 1] = ' ';
                        line[charColumn] = ' ';
                    }
                    else
                    {
                        byte b = bytes[i + j];
                        line[hexColumn] = HexChars[(b >> 4) & 0xF];
                        line[hexColumn + 1] = HexChars[b & 0xF];
                        line[charColumn] = (b < 32 ? '.' : (char)b);
                    }
                    hexColumn += 3;
                    charColumn++;
                }
                result.Append(line);
            }
            return Environment.NewLine + result.ToString();
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

        public static UInt64 MilisUnixTimeStampUTC()
        {
            UInt64 unixTimeStamp;
            DateTime currentTime = DateTime.Now;
            DateTime zuluTime = currentTime.ToUniversalTime();
            DateTime unixEpoch = new DateTime(1970, 1, 1);
            unixTimeStamp = (UInt64)(zuluTime.Subtract(unixEpoch)).TotalMilliseconds;

            return unixTimeStamp;
        }

        public static ulong swapEndian(ulong input)
        {
            return ((0x00000000000000FF) & (input >> 56) |
                    (0x000000000000FF00) & (input >> 40) |
                    (0x0000000000FF0000) & (input >> 24) |
                    (0x00000000FF000000) & (input >> 8) |
                    (0x000000FF00000000) & (input << 8) |
                    (0x0000FF0000000000) & (input << 24) |
                    (0x00FF000000000000) & (input << 40) |
                    (0xFF00000000000000) & (input << 56));
        }

        public static uint swapEndian(uint input)
        {
            return ((input >> 24) & 0xff) |
                    ((input << 8) & 0xff0000) |
                    ((input >> 8) & 0xff00) |
                    ((input << 24) & 0xff000000);
        }

        public static int swapEndian(int input)
        {
            uint inputAsUint = (uint)input;

            input = (int)
                    (((inputAsUint >> 24) & 0xff) |
                    ((inputAsUint << 8) & 0xff0000) |
                    ((inputAsUint >> 8) & 0xff00) |
                    ((inputAsUint << 24) & 0xff000000));

            return input;
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
                k = ((k >> 24) & 0xff) | // move byte 3 to byte 0
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
                    h ^= (uint)data[0] << 16; goto case 2;
                case 2:
                    h ^= (uint)data[len - 2] << 8; goto case 1;
                case 1:
                    h ^= data[len - 1];
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

        public static byte[] ConvertBoolArrayToBinaryStream(bool[] array)
        {
            byte[] data = new byte[(array.Length / 8) + (array.Length % 8 != 0 ? 1 : 0)];

            int dataCounter = 0;
            for (int i = 0; i < array.Length; i += 8)
            {
                for (int bitCount = 0; bitCount < 8; bitCount++)
                {
                    if (i + bitCount >= array.Length)
                        break;
                    data[dataCounter] = (byte)(((array[i + bitCount] ? 1 : 0) << 7 - bitCount) | data[dataCounter]);
                }
                dataCounter++;
            }

            return data;
        }
    }
}

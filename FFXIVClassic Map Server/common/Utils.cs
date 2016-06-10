using System;
using System.IO;
using System.Text;

namespace FFXIVClassic_Map_Server.common
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

        public static string ByteArrayToHex(byte[] bytes, int offset = 0, int bytesPerLine = 16)
        {
            if (bytes == null)
            {
                return String.Empty;
            }

            char[] hexChars = "0123456789ABCDEF".ToCharArray();

            // 00000000   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00  ................
            // 00000010   00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00  ................
            int offsetBlock = 8 + 3;
            int byteBlock = offsetBlock + (bytesPerLine * 3) + ((bytesPerLine - 1) / 8) + 2;
            int lineLength = byteBlock + bytesPerLine + Environment.NewLine.Length;

            char[] line = (new String(' ', lineLength - Environment.NewLine.Length) + Environment.NewLine).ToCharArray();
            int numLines = (bytes.Length + bytesPerLine - 1) / bytesPerLine;

            StringBuilder sb = new StringBuilder(numLines * lineLength);

            for (int i = offset; i < bytes.Length; i += bytesPerLine)
            {
                line[0] = hexChars[(i >> 28) & 0xF];
                line[1] = hexChars[(i >> 24) & 0xF];
                line[2] = hexChars[(i >> 20) & 0xF];
                line[3] = hexChars[(i >> 16) & 0xF];
                line[4] = hexChars[(i >> 12) & 0xF];
                line[5] = hexChars[(i >> 8) & 0xF];
                line[6] = hexChars[(i >> 4) & 0xF];
                line[7] = hexChars[(i >> 0) & 0xF];

                int hexColumn = offsetBlock;
                int charColumn = byteBlock;

                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (j > 0 && (j & 7) == 0)
                    {
                        hexColumn++;
                    }

                    if (i + j >= bytes.Length)
                    {
                        line[hexColumn] = ' ';
                        line[hexColumn + 1] = ' ';
                        line[charColumn] = ' ';
                    }
                    else
                    {
                        byte by = bytes[i + j];
                        line[hexColumn] = hexChars[(by >> 4) & 0xF];
                        line[hexColumn + 1] = hexChars[by & 0xF];
                        line[charColumn] = (by < 32 ? '.' : (char)by);
                    }

                    hexColumn += 3;
                    charColumn++;
                }

                sb.Append(line);
            }

            return sb.ToString();
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

        public static string FFXIVLoginStringDecodeBinary(string path)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            byte[] data = File.ReadAllBytes(path);
            int offset = 0x5405a;
            //int offset = 0x5425d;
            //int offset = 0x53ea0;
            while (true)
            {
                string result = "";
                uint key = (uint)data[offset + 0] << 8 | data[offset + 1];
                uint key2 = data[offset + 2];
                key = RotateRight(key, 1) & 0xFFFF;
                key -= 0x22AF;
                key &= 0xFFFF;
                key2 = key2 ^ key;
                key = RotateRight(key, 1) & 0xFFFF;
                key -= 0x22AF;
                key &= 0xFFFF;
                uint finalKey = key;
                key = data[offset + 3];
                uint count = (key2 & 0xFF) << 8;
                key = key ^ finalKey;
                key &= 0xFF;
                count |= key;

                int count2 = 0;
                while (count != 0)
                {
                    uint encrypted = data[offset + 4 + count2];
                    finalKey = RotateRight(finalKey, 1) & 0xFFFF;
                    finalKey -= 0x22AF;
                    finalKey &= 0xFFFF;
                    encrypted = encrypted ^ (finalKey & 0xFF);

                    result += (char)encrypted;
                    count--;
                    count2++;
                }

                offset += 4 + count2;

                Log.Debug(result);
            }
        }

        public static string FFXIVLoginStringDecode(byte[] data)
        {
            string result = "";
            uint key = (uint)data[0] << 8 | data[1];
            uint key2 = data[2];
            key = RotateRight(key, 1) & 0xFFFF;
            key -= 0x22AF;
            key2 = key2 ^ key;
            key = RotateRight(key, 1) & 0xFFFF;
            key -= 0x22AF;
            uint finalKey = key;
            key = data[3];
            uint count = (key2 & 0xFF) << 8;
            key = key ^ finalKey;
            key &= 0xFF;
            count |= key;

            int count2 = 0;
            while (count != 0)
            {
                uint encrypted = data[4 + count2];
                finalKey = RotateRight(finalKey, 1) & 0xFFFF;
                finalKey -= 0x22AF;
                encrypted = encrypted ^ (finalKey & 0xFF);
                result += (char)encrypted;
                count--;
                count2++;
            }

            return result;
        }

        public static byte[] FFXIVLoginStringEncode(uint key, string text)
        {
            key = key & 0xFFFF;

            uint count = 0;
            byte[] asciiBytes = Encoding.ASCII.GetBytes(text);
            byte[] result = new byte[4 + text.Length];
            for (count = 0; count < text.Length; count++)
            {
                result[result.Length - count - 1] = (byte)(asciiBytes[asciiBytes.Length - count - 1] ^ (key & 0xFF));
                key += 0x22AF;
                key &= 0xFFFF;
                key = RotateLeft(key, 1) & 0xFFFF;
            }

            count = count ^ key;
            result[3] = (byte)(count & 0xFF);

            key += 0x22AF & 0xFFFF;
            key = RotateLeft(key, 1) & 0xFFFF;

            result[2] = (byte)(key & 0xFF);

            key += 0x22AF & 0xFFFF;
            key = RotateLeft(key, 1) & 0xFFFF;


            result[1] = (byte)(key & 0xFF);
            result[0] = (byte)((key >> 8) & 0xFF);

            return result;
        }

        public static uint RotateLeft(uint value, int bits)
        {
            return (value << bits) | (value >> (16 - bits));
        }

        public static uint RotateRight(uint value, int bits)
        {
            return (value >> bits) | (value << (16 - bits));
        }

    }
}

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

using System;
using System.IO;
using System.Text;

namespace Launcher_Editor
{
    //ffxivboot.exe:
    //Offset
    //0x9663FC: Patch Server Port
    //0x966404: Patch Server URL

    //0x9663FC + 0x400000: Port Offset to search
    //0x966404 + 0x400000: URL Offset to search

    class Program
    {
        const string ORIGINAL_PATCH_PORT_STRING = "54996";
        const string ORIGINAL_PATCH_URL_STRING = "ver01.ffxiv.com";
        const string ORIGINAL_PATCH_LOGIN_STRING = "http://account.square-enix.com/account/content/ffxivlogin";

        static void Main(string[] args)
        {
            byte[] exeDataBoot;
            byte[] exeDataLogin;

            string patchPortString;
            string patchUrlString;
            string loginUrlString;
 
            string lobbyUrlString = "lobby01.ffxiv.com";

            Console.WriteLine("---------------------");
            Console.WriteLine("FFXIV 1.0 EXE Patcher");
            Console.WriteLine("By Ioncannon");
            Console.WriteLine("Version 1.0");
            Console.WriteLine("---------------------");

            Console.WriteLine("Please enter the full path to your FINAL FANTASY XIV folder. It should have ffxivgame.exe inside it.");
            string path = Console.ReadLine();

            if (!File.Exists(path + "\\ffxivboot.exe"))
            {
                Console.WriteLine("Missing ffxivboot.exe, aborting");
                Console.ReadKey();
                return;
            }
            if (!File.Exists(path + "\\ffxivgame.exe"))
            {
                Console.WriteLine("Missing ffxivgame.exe, aborting");
                Console.ReadKey();
                return;
            }
            if (!File.Exists(path + "\\ffxivlogin.exe"))
            {
                Console.WriteLine("Missing ffxivlogin.exe, aborting");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("EXEs found!");

            Console.WriteLine("Please enter the url to the patch webpage (do not include \"http://\", max 32 characters).");
            patchUrlString = Console.ReadLine();
            Console.WriteLine("Please enter the port to the patch webpage (usually 80).");
            patchPortString = Console.ReadLine();

            try
            {
                int.Parse(patchPortString);
            }
            catch (FormatException e)
            {
                Console.WriteLine("Not a number, aborting");
                Console.ReadKey();
                return;
            }
            catch (OverflowException e)
            {
                Console.WriteLine("Not a number, aborting");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Please enter the url to the login webpage (max 56 characters, please include \"http://\").");
            loginUrlString = Console.ReadLine();

            if (loginUrlString.Length > 0x56)
            {
                Console.WriteLine("URL too long, aborting");
                Console.ReadKey();
                return;
            }

            long patchPortStringOffset = 0;
            long patchUrlStringOffset = 0;
            long lobbyUrlStringOffset = 0;
            long freeSpaceOffset = 0;

            long loginUrlOffset = 0;
            long freeSpaceInLoginOffset = 0;

            Console.WriteLine("Patching started!");
            exeDataBoot = File.ReadAllBytes(path + "\\ffxivboot.exe");
            exeDataLogin = File.ReadAllBytes(path + "\\ffxivlogin.exe");

            Console.WriteLine("---Editing FFXIVBOOT.EXE---");

            patchPortStringOffset = PrintSearch(exeDataBoot, ORIGINAL_PATCH_PORT_STRING);
            patchUrlStringOffset = PrintSearch(exeDataBoot, ORIGINAL_PATCH_URL_STRING);            
            freeSpaceOffset = PrintFreeSpaceSearch(exeDataBoot);

            if (patchPortStringOffset == -1 || patchUrlStringOffset == -1 || freeSpaceOffset == -1)
            {
                Console.WriteLine("There was an error finding the address locations...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Writing \"{0}\" and updating offset to 0x{1:X}.", patchPortString, freeSpaceOffset);
            WriteNewString(exeDataBoot, patchPortStringOffset, patchPortString, freeSpaceOffset);
            Console.WriteLine("Writing \"{0}\" and updating offset to 0x{1:X}.", patchUrlString, freeSpaceOffset + 0x20);
            WriteNewString(exeDataBoot, patchUrlStringOffset, patchUrlString, freeSpaceOffset + 0x20);

            Console.WriteLine("---Editing FFXIVLOGIN.EXE---");
            loginUrlOffset = PrintEncodedSearch(exeDataLogin, 0x739, ORIGINAL_PATCH_LOGIN_STRING);
            freeSpaceInLoginOffset = PrintFreeSpaceSearch(exeDataLogin);

            if (loginUrlOffset == -1 || freeSpaceInLoginOffset == -1)
            {
                Console.WriteLine("There was an error finding the address locations...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Writing encoded \"{0}\" and updating offset to 0x{1:X}.", loginUrlString, freeSpaceInLoginOffset);
            WriteNewStringEncoded(exeDataLogin, loginUrlOffset, 0x739, loginUrlString, freeSpaceInLoginOffset);

            File.WriteAllBytes("C:\\Users\\Filip\\Desktop\\ffxivboot.exe", exeDataBoot);
            File.WriteAllBytes("C:\\Users\\Filip\\Desktop\\ffxivlogin.exe", exeDataLogin);

            Console.WriteLine("Done! New .EXEs created in the same folder as this application. Make sure to backup your originals!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

        }

        public static void WriteNewString(byte[] exeData, long offsetLocation, string newString, long freeSpaceLocation)
        {
            using (MemoryStream memStream = new MemoryStream(exeData))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memStream))
                {
                    binaryWriter.BaseStream.Seek(offsetLocation, SeekOrigin.Begin);
                    binaryWriter.Write((uint)freeSpaceLocation + 0x400000);
                    binaryWriter.BaseStream.Seek(freeSpaceLocation, SeekOrigin.Begin);
                    binaryWriter.Write(Encoding.ASCII.GetBytes(newString), 0, Encoding.ASCII.GetByteCount(newString) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(newString));
                }
            }
        }

        public static void WriteNewStringEncoded(byte[] exeData, long offsetLocation, uint key, string newString, long freeSpaceLocation)
        {
            byte[] encodedString = FFXIVLoginStringEncode(key, newString);
            using (MemoryStream memStream = new MemoryStream(exeData))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memStream))
                {
                    //binaryWriter.BaseStream.Seek(offsetLocation, SeekOrigin.Begin);
                    //binaryWriter.Write((uint)freeSpaceLocation + 0x400000);
                    binaryWriter.BaseStream.Seek(offsetLocation, SeekOrigin.Begin);
                    binaryWriter.Write(encodedString);
                }
            }
        }

        public static long PrintSearch(byte[] exeData, string searchString)
        {
            Console.Write("Searching for string \"{0}\"...", searchString);
            long offset = SearchForStringOffset(exeData, searchString);

            if (offset != -1)
                Console.WriteLine(" FOUND @ 0x{0:X}!", offset);
            else
            {
                Console.WriteLine(" ERROR, could not find string.");                
            }

            return offset;
        }

        public static long PrintEncodedSearch(byte[] exeData, uint key, string searchString)
        {
            Console.Write("Searching for encoded string \"{0}\"...", searchString);
            long offset = SearchForEncodedStringOffset(exeData, key, searchString);

            if (offset != -1)
                Console.WriteLine(" FOUND @ 0x{0:X}!", offset);
            else
            {
                Console.WriteLine(" ERROR, could not find string.");
            }

            return offset;
        }

        public static long PrintFreeSpaceSearch(byte[] exeData)
        {
            Console.Write("Searching for free space...");
            long freeSpaceOffset = SearchForFreeSpace(exeData);
            if (freeSpaceOffset != -1)
                Console.WriteLine(" FOUND @ 0x{0:X}!", freeSpaceOffset);
            else
            {
                Console.WriteLine(" ERROR, could not find free space.");
            }

            return freeSpaceOffset;
        }

        public static bool EditOffset(long offset, uint value)
        {
            return true;
        }

        public static long SearchForFreeSpace(byte[] exeData)
        {
            using (MemoryStream memStream = new MemoryStream(exeData))
            {
                using (BinaryReader binReader = new BinaryReader(memStream))
                {
                    //Find the .data section header
                    long textSectionOffset = -1;
                    int strCheckoffset = 0;
                    while (binReader.BaseStream.Position + 4 < binReader.BaseStream.Length)
                    {
                        if (binReader.ReadByte() == ".text"[strCheckoffset])
                        {
                            if (strCheckoffset == 0)
                                textSectionOffset = binReader.BaseStream.Position - 1;

                            strCheckoffset++;
                            if (strCheckoffset == Encoding.ASCII.GetByteCount(".data"))
                                break;
                        }
                        else
                        {
                            strCheckoffset = 0;
                            textSectionOffset = -1;
                        }
                    }

                    //Read in the position and size
                    binReader.BaseStream.Seek(textSectionOffset, SeekOrigin.Begin);
                    binReader.ReadUInt64();
                    uint virtualSize = binReader.ReadUInt32();
                    uint address = binReader.ReadUInt32();
                    uint sizeOfRawData = binReader.ReadUInt32();

                    if (sizeOfRawData - virtualSize < 0x50)
                        return -1;

                    //Find a spot
                    binReader.BaseStream.Seek(address + sizeOfRawData, SeekOrigin.Begin);
                    while (binReader.BaseStream.Position >= address + virtualSize)
                    {                        
                        binReader.BaseStream.Seek(-0x50, SeekOrigin.Current);
                        long newPosition = binReader.BaseStream.Position;

                        bool foundNotZero = false;
                        for (int i = 0; i < 0x50; i++)
                        {
                            if (binReader.ReadByte() != 0)
                            {
                                foundNotZero = true;
                                break;
                            }
                        }

                        if (!foundNotZero)                        
                            return newPosition;                        
                        else
                            binReader.BaseStream.Seek(newPosition, SeekOrigin.Begin);
                    }
                }
            }

            return -1;
        }

        public static long SearchForStringOffset(byte[] exeData, string testString)
        {
            testString += "\0";

            using (MemoryStream memStream = new MemoryStream(exeData))
            {
                using (BinaryReader binReader = new BinaryReader(memStream))
                {
                    long strOffset = -1;
                    int strCheckoffset = 0;
                    while (binReader.BaseStream.Position + 4 < binReader.BaseStream.Length)
                    {
                        if (binReader.ReadByte() == testString[strCheckoffset])
                        {
                            if (strCheckoffset == 0)
                                strOffset = binReader.BaseStream.Position-1;

                            strCheckoffset++;
                            if (strCheckoffset == Encoding.ASCII.GetByteCount(testString))
                                break;
                        }
                        else
                        {
                            strCheckoffset = 0;
                            strOffset = -1;
                        }
                    }                    

                    if (strOffset != -1)
                    {
                        strOffset += 0x400000;

                        binReader.BaseStream.Seek(0, SeekOrigin.Begin);

                        while (binReader.BaseStream.Position + 4 < binReader.BaseStream.Length)
                        {
                            if (binReader.ReadUInt32() == strOffset)
                                return binReader.BaseStream.Position - 0x4;
                        }

                        return -1;
                    }
                    else
                        return -1;
                }
            }
        }

        public static long SearchForEncodedStringOffset(byte[] exeData, uint testKey, string testString)
        {
            byte[] encoded = FFXIVLoginStringEncode(testKey, testString);

            using (MemoryStream memStream = new MemoryStream(exeData))
            {
                using (BinaryReader binReader = new BinaryReader(memStream))
                {
                    long strOffset = -1;
                    int strCheckoffset = 0;
                    while (binReader.BaseStream.Position + 4 < binReader.BaseStream.Length)
                    {
                        if (binReader.ReadByte() == encoded[strCheckoffset])
                        {
                            if (strCheckoffset == 0)
                                strOffset = binReader.BaseStream.Position - 1;

                            strCheckoffset++;
                            if (strCheckoffset == encoded.Length)
                                break;
                        }
                        else
                        {
                            strCheckoffset = 0;
                            strOffset = -1;
                        }
                    }

                    return strOffset;
                }
            }
        }

        public static string FFXIVLoginStringDecodeBinary(string path)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            byte[] data = File.ReadAllBytes(path);
            //int offset = 0x5405a;
            //int offset = 0x5425d;
            int offset = 0x53ea0;
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

                return result;
                //offset += 4 + count2;
            }
        }

        public static string FFXIVLoginStringDecode(byte[] data)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            while (true)
            {
                string result = "";
                uint key = (uint)data[0] << 8 | data[1];
                uint key2 = data[2];
                key = RotateRight(key, 1) & 0xFFFF;
                key -= 0x22AF;
                key &= 0xFFFF;
                key2 = key2 ^ key;
                key = RotateRight(key, 1) & 0xFFFF;
                key -= 0x22AF;
                key &= 0xFFFF;
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
                    finalKey &= 0xFFFF;
                    encrypted = encrypted ^ (finalKey & 0xFF);

                    result += (char)encrypted;
                    count--;
                    count2++;
                }

                return result;
                //offset += 4 + count2;
            }
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
                key = RotateLeft(key, 1);
                key &= 0xFFFF;
            }

            count = count ^ key;
            result[3] = (byte)(count & 0xFF);

            key += 0x22AF;
            key &= 0xFFFF;
            key = RotateLeft(key, 1); 
            key &= 0xFFFF;

            result[2] = (byte)(key & 0xFF);

            key += 0x22AF;
            key &= 0xFFFF;
            key = RotateLeft(key, 1);
            key &= 0xFFFF;

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

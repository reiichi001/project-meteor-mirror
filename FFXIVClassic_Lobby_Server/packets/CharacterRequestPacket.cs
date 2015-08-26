using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Lobby_Server.packets
{
    class CharacterRequestPacket
    {
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct CharacterRequest
        {
            public uint sequence;
            public uint unknown;
            public uint characterId;
            public uint unknown2;
            public byte slot;
            public byte command;
            public ushort worldId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public String characterName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x190)]
            public String characterInfoEncoded;
        }

        public static unsafe CharacterRequest toStruct(byte[] bytes)
        {
            fixed (byte* pdata = &bytes[0])
            {
                return (CharacterRequest)Marshal.PtrToStructure(new IntPtr(pdata), typeof(CharacterRequest));
            }
        }
    }
}

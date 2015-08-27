using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Lobby_Server.packets
{
    class PacketStructs
    {
        [StructLayout(LayoutKind.Explicit)]
        public unsafe struct SessionPacket
        {
            [FieldOffset(0)]
            public uint sequence;            
            [FieldOffset(0x50)]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public String version;
            [FieldOffset(0x70)]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public String session;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct CharacterRequestPacket
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

        public static unsafe CharacterRequestPacket toCharacterRequestStruct(byte[] bytes)
        {
            fixed (byte* pdata = &bytes[0])
            {
                return (CharacterRequestPacket)Marshal.PtrToStructure(new IntPtr(pdata), typeof(CharacterRequestPacket));
            }
        }

        public static unsafe SessionPacket toSessionStruct(byte[] bytes)
        {
            fixed (byte* pdata = &bytes[0])
            {
                return (SessionPacket)Marshal.PtrToStructure(new IntPtr(pdata), typeof(SessionPacket));
            }
        }
    }
}

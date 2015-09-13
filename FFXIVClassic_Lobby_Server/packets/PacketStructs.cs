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
            public UInt64 sequence;
            [FieldOffset(0x10)]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public String session;
            [FieldOffset(0x50)]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public String version;            
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct CharacterRequestPacket
        {
            public UInt64 sequence;
            public uint characterId;
            public uint personType;
            public byte slot;
            public byte command;
            public ushort worldId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public String characterName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x190)]
            public String characterInfoEncoded;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct SelectCharRequestPacket
        {
            public UInt64 sequence;
            public uint characterId;
            public uint unknownId;
            public UInt64 ticket;            
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

        public static unsafe SelectCharRequestPacket toSelectCharRequestStruct(byte[] bytes)
        {
            fixed (byte* pdata = &bytes[0])
            {
                return (SelectCharRequestPacket)Marshal.PtrToStructure(new IntPtr(pdata), typeof(SelectCharRequestPacket));
            }
        }

        public static byte[] StructureToByteArray(object obj)
        {
            int len = Marshal.SizeOf(obj);

            byte[] arr = new byte[len];

            IntPtr ptr = Marshal.AllocHGlobal(len);

            Marshal.StructureToPtr(obj, ptr, true);

            Marshal.Copy(ptr, arr, 0, len);

            Marshal.FreeHGlobal(ptr);

            return arr;
        }
    }
}

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

        //Response Packets
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct ReserveCharaResponse
        {
            public UInt64 sequence;
            public uint errorCode;
            public uint statusCode;
            public uint errorId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x2BB)]
            public String errorMessage;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct MakeCharaResponse
        {
            public UInt64 sequence;
            public uint errorCode;
            public uint statusCode;
            public uint errorId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x2BB)]
            public String errorMessage;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct DeleteCharaResponse
        {
            public UInt64 sequence;
            public uint errorCode;
            public uint statusCode;
            public uint errorId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x2BB)]
            public String errorMessage;
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

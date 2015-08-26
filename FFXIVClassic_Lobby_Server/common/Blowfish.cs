using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace FFXIVClassic_Lobby_Server.common
{
  
    public class Blowfish
    {
        [DllImport("../../../Debug/Blowfish.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short initializeBlowfish(byte[] key, short keySize, uint[] P, uint[,] S);
        [DllImport("../../../Debug/Blowfish.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void blowfish_encipher(ref int xl, ref int xr, uint[] P);
        [DllImport("../../../Debug/Blowfish.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void blowfish_decipher(ref int xl, ref int xr, uint[] P);

        private uint[] P = new uint[16+2];
        private uint[,] S = new uint[4,256];
        
        public Blowfish(byte[] key)
        {
            InitBlowfish(key);
        }

        private int InitBlowfish(byte[] key)
        {
            return initializeBlowfish(key, (short)key.Length, P, S);
        }

        public void Encipher(byte[] data, int offset, int length)
        {
            if ((length - offset) % 8 != 0)
                throw new ArgumentException("Needs to be a multiple of 8");

            for (int i = offset; i < offset+length; i+=8)
            {
                int xl = (data[i + 0]) | (data[i + 1] << 8) | (data[i + 2] << 16) | (data[i + 3] << 24);
                int xr = (data[i + 4]) | (data[i + 5] << 8) | (data[i + 6] << 16) | (data[i + 7] << 24);
                blowfish_encipher(ref xl, ref xr, P);
                data[i + 0] = (byte)(xl >> 0);
                data[i + 1] = (byte)(xl >> 8);
                data[i + 2] = (byte)(xl >> 16);
                data[i + 3] = (byte)(xl >> 24);
                data[i + 4] = (byte)(xr >> 0);
                data[i + 5] = (byte)(xr >> 8);
                data[i + 6] = (byte)(xr >> 16);
                data[i + 7] = (byte)(xr >> 24);
            }
        }

        public void Decipher(byte[] data, int offset, int length)
        {
            if ((length - offset) % 8 != 0)
                throw new ArgumentException("Needs to be a multiple of 8");

            for (int i = offset; i < offset + length; i += 8)
            {
                int xl = (data[i + 0]) | (data[i + 1] << 8) | (data[i + 2] << 16) | (data[i + 3] << 24);
                int xr = (data[i + 4]) | (data[i + 5] << 8) | (data[i + 6] << 16) | (data[i + 7] << 24);
                blowfish_decipher(ref xl, ref xr, P);
                data[i + 0] = (byte)(xl >> 0);
                data[i + 1] = (byte)(xl >> 8);
                data[i + 2] = (byte)(xl >> 16);
                data[i + 3] = (byte)(xl >> 24);
                data[i + 4] = (byte)(xr >> 0);
                data[i + 5] = (byte)(xr >> 8);
                data[i + 6] = (byte)(xr >> 16);
                data[i + 7] = (byte)(xr >> 24);
            }
        }
    }
    
}
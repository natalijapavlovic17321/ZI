using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _17321_Zadatak1_ZI.BlowFish2;
using System.Security.Cryptography;

namespace _17321_Zadatak1_ZI.MD5
{
    class Md5
    {
        private byte[] text;
        private int[] r;
        private uint[] k;
        private uint h1;
        private uint h2;
        private uint h3;
        private uint h0;

        #region Constructors
        public Md5()
        {
            k = new uint[64];
            set();
        }
        public Md5(string text)
        {
            k = new uint[64];
            this.text = Encoding.ASCII.GetBytes(text);
            set();
        }
        public void set()
        {
            r = new int[64]{ 7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,
            5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,
            4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,
            6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21
            };

            for (int i = 0; i < 64; i++)
                k[i] = Convert.ToUInt32(Math.Floor((Math.Abs(Math.Sin(i + 1)))* Math.Pow(2,32)));

            h0 = 0x67452301;
            h1 = 0xEFCDAB89;
            h2 = 0x98BADCFE;
            h3 = 0x10325476;
        }
        #endregion Constructors

        #region HashAlgorithm
        public string GetHash(string text)
        {
            this.text = Encoding.ASCII.GetBytes(text);
            uint a0 = h0;   // A
            uint b0 = h1;   // B
            uint c0 = h2;   // C
            uint d0 = h3;   // D

            var newLength = (56 - ((this.text.Length + 1) % 64)) % 64; // length= append "0" bits until message length in bits ≡ 448 (mod 512)
            byte[] newText= new byte[this.text.Length + 1 + newLength + 8];
            Array.Copy(this.text, newText, this.text.Length);
            newText[this.text.Length] = 0x80; // append "1" bit to message

            byte[] length = BitConverter.GetBytes(this.text.Length * 8); // little endian reči   
            Array.Copy(length, 0, newText, newText.Length - 8, 4); // add length in bits

            for (int i = 0; i < newText.Length / 64; ++i) //po 512 bitova
            {
                uint[] w = new uint[16]; //niz od po 16 little endian reči   
                for (int j = 0; j < 16; ++j)
                     w[j] = BitConverter.ToUInt32(newText, (i * 64) + (j * 4));

                uint A = a0, B = b0, C = c0, D = d0, f = 0, g = 0;
                // Glavna petlja:
                for (uint y = 0; y < 64; ++y)
                {
                    if (y>=0 && y <= 15)
                    {
                        f = (B & C) | (~B & D);
                        g = y;
                    }
                    else if (y >= 16 && y <= 31)
                    {
                        f = (D & B) | (~D & C);
                        g = ((5 * y) + 1) % 16;
                    }
                    else if (y >= 32 && y <= 47)
                    {
                        f = B ^ C ^ D;
                        g = ((3 * y) + 5) % 16;
                    }
                    else if (y >= 48)
                    {
                        f = C ^ (B | ~D);
                        g = (7 * y) % 16;
                    }
                    var temp = D;
                    D = C;
                    C = B;
                    B = B + leftRotate((A + f + k[y] + w[g]), r[y]);
                    A = temp;
                }
                a0 += A;
                b0 += B;
                c0 += C;
                d0 += D;
            }
            return (ByteToString(a0) + ByteToString(b0) + ByteToString(c0) + ByteToString(d0));
        }
        #endregion HashAlgorithm

        #region Functions

        public uint leftRotate(uint x, int c)
        {
            return (x << c) | (x >> (32 - c));
        }
        private static string ByteToString(uint x)
        {
            return String.Join("", BitConverter.GetBytes(x).Select(y => y.ToString("x2")));
            //return UInt32.ToString()
        }

        #endregion Functions
    }
}

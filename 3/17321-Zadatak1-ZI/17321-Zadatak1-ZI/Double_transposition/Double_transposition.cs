using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Double_trasposition2
{
    public class Double_trasposition
    {
        #region Variables
        private byte[] InitVector;
        private bool IVSet;
        public byte[] IV
        {
            get { return InitVector; }
            set
            {
                if (value.Length == 8)
                {
                    InitVector = value;
                    IVSet = true;
                }
                else
                {
                    throw new Exception("Invalid IV size.");
                }
            }
        }
        private int length;
        public int Length
        {
            get { return length; }
            set { length = value; }
        }
        private int m;
        public int M
        {
            get { return m; }
            set { m = value; }
        }
        private string permN;
        public string PermN
        {
            get { return permN; }
            set { permN = value; }
        }
        private string permM;
        public string PermM
        {
            get { return permM; }
            set { permM = value; }
        }
        private int n;
        public int N
        {
            get { return n; }
            set { n = value; }
        }
        #endregion Variables

        #region Constructors
        public Double_trasposition() { }
        public Double_trasposition(string text)
        {    
            // inicijalizacija matrice, M, N
            this.length = text.Length;
            double root= Math.Sqrt(length);
            this.m = this.n = (int)root;
            if(root%1!=0)   //root nije ceo broj
            {
                this.n++;   //potrebna jos jedna vrsta
                // if (root < System.Convert.ToInt32(root))    //drugi nacin
                //   this.m++;
                if (this.n * this.m < this.length)   //potrebna jos jedna kolona
                    this.m++;
            }
        }
        public Double_trasposition(string text, string iv)
        {
            // inicijalizacija matrice, M, N
            this.length = text.Length;
            double root = Math.Sqrt(length);
            this.m = this.n = (int)root;
            if (root % 1 != 0)   //root nije ceo broj
            {
                this.n++;   //potrebna jos jedna vrsta
                // if (root < System.Convert.ToInt32(root))    //drugi nacin
                //   this.m++;
                if (this.n * this.m < this.length)   //potrebna jos jedna kolona
                    this.m++;
            }
            IV = HexToByte(iv);
        }
        #endregion Constructors

        #region Init 
        public void InitShuffle()
        {
            //inicijalizacija pemrM i permN kao 1,2,3,4....
            this.permM = this.permN = "";
            for (int i = 0; i < this.m - 1; i++)
                this.permM += i.ToString() + ",";
            this.permM += (this.m - 1).ToString();
            for (int i = 0; i < this.n - 1; i++)
                this.permN += i.ToString() + ",";
            this.permN += (this.n - 1).ToString();
        }
        public byte[] SetIV()
        {
            InitVector = new byte[8];
            RNGCryptoServiceProvider randomSource = new RNGCryptoServiceProvider();
            randomSource.GetBytes(InitVector);
            IVSet = true;
            //this.IV = InitVector;
            return InitVector;
        }
        public string[] Shuffle(string key)
        {
            //shuffle permM i permN da bi se dobile nasumicne vrednosti kljuceva
            string[] keys = key.Split(',');
            Random random = new Random();
            for (int i = keys.Length - 1; i > 0; i--)
            {
                int next = random.Next(i + 1);
                string pom = keys[i];
                keys[i] = keys[next];
                keys[next] = pom;
            }
            return keys;
        }
        #endregion Init

        #region Code/Decode
        public string[] Code(string text)
        {
            //pripremanje kljuceva i texta
            this.InitShuffle();
            string[] keyM = this.Shuffle(this.permM);
            string[] keyN = this.Shuffle(this.permN);
            //text = text.ToLower();
            //.Trim(new Char[] { ' ', '.' , '?' , '!'});
            
            char[,] matrix= new char[this.m, this.n];
            for (int i = 0; i < this.m; i++)
            {
                for (int j = 0; j < this.n; j++)
                {
                    if (i * n + j < text.Length)
                        matrix[i, j] = text[i * n + j];
                }
            }
            //mesanje kolona i vrsta
            char[,] transpositionM = new char[this.m, this.n];
            for (int i = 0; i < keyM.Length; i++)
            {
                int k = System.Convert.ToInt32(keyM[i]);
                for (int j = 0; j < this.n; j++)
                {
                    transpositionM[i, j] = matrix[k, j];
                }
            }

            char[,] transpositionN = new char[this.m, this.n];
            for (int i = 0; i < keyN.Length; i++)
            {
                int k = System.Convert.ToInt32(keyN[i]);
                for (int j = 0; j < this.m; j++)
                {
                    transpositionN[j, i] = transpositionM[j, k];
                }
            }

            string outputText = "";
            for (int i = 0; i < this.m; i++)
            {
                for (int j = 0; j < this.n; j++)
                {
                    outputText += transpositionN[i, j];
                }
            }
            // postavljanje permM i permN na shufflovane vrednosti kljuceva
            this.permN = this.permM = "";
            for (int i = 0; i < this.m - 1; i++)
                this.permM += keyM[i] + ",";
            this.permM += keyM[this.m - 1];
            for (int i = 0; i < this.n - 1; i++)
                permN += keyN[i] + ",";
            this.permN += keyN[this.n - 1];
            //vracanje kodiranog teksta i kljuceva
            string[] output=new string[3];
            output[0] = outputText; 
            output[1] = this.permM; 
            output[2] = this.permN;
            return output;
        }
        public string Decode(string text, string kM, string kN)
        {   //pripremanje kljuceva
            string[] keyM = kM.Split(',');
            string[] keyN = kN.Split(',');

            char[,] matrix = new char[this.m, this.n];
            for (int i = 0; i < this.m; i++)
            {
                for (int j = 0; j < this.n; j++)
                {
                     if (i * n + j < text.Length)
                        matrix[i, j] = text[i * n + j];
                }
            }
            //dekodiranje prvo vrsta pa kolona
            char[,] transpositionN = new char[this.m, this.n];
            for (int i = 0; i < keyN.Length; i++)
            {
                int k = System.Convert.ToInt32(keyN[i]);
                for (int j = 0; j < this.m; j++)
                {
                    transpositionN[j, k] = matrix[j, i];
                }
            }

           
            char[,] transpositionM = new char[this.m, this.n];
            for (int i = 0; i < keyM.Length; i++)
            {
                int k = System.Convert.ToInt32(keyM[i]);
                for (int j = 0; j < this.n; j++)
                {
                    transpositionM[k, j] = transpositionN[i, j];
                }
            }

            string output = "";
            for (int i = 0; i < this.m; i++)
            {
                for (int j = 0; j < this.n; j++)
                {
                    output += transpositionM[i, j];
                }
            }
            return output;
        }
        public string[] CodeOFB(string text)
        {
            this.IV=SetIV();
            return null;
        }
        public string DecodeOFB(string text, string keyN, string keyM)
        {
            if (!IVSet)
            {
                throw new Exception("IV not set.");
            }
            return null;
        }
        #endregion Code/Decode

        #region Functions
        private void XorBlock(ref byte[] block, byte[] iv)
        {
            for (int i = 0; i < block.Length; i++)
            {
                block[i] ^= iv[i];
            }
        }
        #endregion Functions

        #region Conversions
        public byte[] HexToByte(string hex)
        {
            byte[] b = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length - 1; i += 2)
            {
                byte x = Hex(hex[i]);
                byte y = Hex(hex[i + 1]);
                b[i / 2] = (byte)(x * 16 + y);
            }
            return b;
        }
        private byte Hex(char x)
        {
            if (x <= '9' && x >= '0')
            { return (byte)(x - '0'); }
            else if (x <= 'z' && x >= 'a')
            { return (byte)(x - 'a' + 10); }
            else if (x <= 'Z' && x >= 'A')
            { return (byte)(x - 'A' + 10); }
            return 0;
        }
        public string ByteToHex(byte[] bytes)
        {
            StringBuilder str = new StringBuilder();
            foreach (byte b in bytes)
                str.Append(b.ToString("x2"));
            return str.ToString();
        }
        #endregion Conversions
    }
}
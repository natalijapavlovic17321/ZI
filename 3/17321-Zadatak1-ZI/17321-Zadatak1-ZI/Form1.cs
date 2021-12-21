using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Double_trasposition2;
using System.IO;
using _17321_Zadatak1_ZI.BlowFish2;
using _17321_Zadatak1_ZI.MD5;
using System.Security.Cryptography;

namespace _17321_Zadatak1_ZI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void fileSystemWatcher1_Created(object sender, FileSystemEventArgs e)
        {
            if (checkBox1.Checked)
            {
                {
                    if (!IsFileReady(e.FullPath))
                    { }
                    else
                    {
                        string destination = textBox3.Text;
                        SelectAlgorithm(e.FullPath, e.Name, destination);
                    }
                }
            }
        }
        public void SelectAlgorithm(string source, string name, string destination)
        {
            if (radioButton1.Checked == true)
            {
                CodeDT(source, name, destination);
            }
            else if (radioButton2.Checked == true)
            {
                CodeBF(source, name, destination);
            }
        }
        public void SelectAlgorithmDecode(string source, string name, string destination)
        {
            if (radioButton1.Checked == true)
            {
                DecodeDT(source, name, destination);
            }
            else if (radioButton2.Checked == true)
            {
                DecodeBF(source, name, destination);
            }
        }
        public static bool IsFileReady(string filename)
        { //Solved
          //Exception The process cannot access the file,because it is being used by another process
            try
            {
                using (FileStream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                    return inputStream.Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private void CodeBF(string source, string name, string destination)
        {
            string file = File.ReadAllText(source);
            byte[] hashValue=null;
            SHA256 mySHA256 = SHA256.Create();
            try
            {
                FileStream fileStream = File.Open(source, FileMode.Open, FileAccess.Read, FileShare.None);
                fileStream.Position = 0;
                hashValue = mySHA256.ComputeHash(fileStream);
                fileStream.Close();
            }
            catch (IOException e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
            if (mySHA256.HashSize > 448)
                System.Windows.Forms.MessageBox.Show("Maksimalna duzina kljuca je 56 bajta");

            BlowFish bf = new BlowFish(hashValue);
            string text, mode="" ;
            if (checkBox2.Checked == true) //biranje moda
            {
                text = bf.CodeOFB(file);
                if(bf.IVSet)
                   mode = bf.ByteToHex(bf.IV); //vrednost IV
            }
            else text = bf.Code(file);

            Md5 MD5 = new Md5();
            string hashCode = MD5.GetHash(file); //hash

            string write = hashCode + "\n" + text; //upisuje u fajl 
                                                   //prva linija hash

            string dest = destination + @"\" + name;
            File.WriteAllText(dest, write);
            string key;
            if (mode=="")
                key = source  + " " + bf.ByteToHex(hashValue) + " " + dest + "\n";
            else key = source + " " + bf.ByteToHex(hashValue) + " " + mode + " " + dest + "\n";
            string pathKey = @"C:\Users\Natalija\Desktop\17321-Zadatak1-ZI\KeysBF.txt";
            File.AppendAllText(pathKey, key);
        }
        private void DecodeBF(string source, string name, string destination)
        {
            try
            {
                string hash1 = File.ReadLines(source).First(); // hash
                string textDecode = File.ReadAllText(source).Remove(0, hash1.Length + 1);

                string pathKey = @"C:\Users\Natalija\Desktop\17321-Zadatak1-ZI\KeysBF.txt";
                string key = null,mode=null;
                foreach (string line in File.ReadLines(pathKey))
                {

                    if (line.EndsWith(source))
                    {
                        string[] keys = line.Split(" ");
                        key = keys[1];
                        if (checkBox2.Checked && !keys[2].Contains(@":\")) // u fajlu se posle kljuca nalazi IV
                            mode = keys[2];
                        else // u fajlu se posle kljuca nalazi putanja destination
                        {
                            if(checkBox2.Checked)
                                MessageBox.Show("Encrypted file doesnt use OFB mode. Please turn it off.");
                            else MessageBox.Show("Encrypted file use OFB mode. Please turn it on.");
                            return;
                        }
                    }
                }

                BlowFish bf = new BlowFish(key); 
                string text;
                if (checkBox2.Checked)
                {
                    bf.IV = bf.HexToByte(mode);
                    text = bf.DecodeOFB(textDecode);
                }
                else text = bf.Decode(textDecode);

                Md5 MD5 = new Md5();
                string hash2 = MD5.GetHash(text); //hash dekodiranog teksta

                if (hash1 != hash2)
                    MessageBox.Show("Different hash codes");
                else
                {
                    string dest = destination + @"\" + name;
                    File.WriteAllText(dest, text);
                    MessageBox.Show("Same hash codes");
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("Try different encryption/decryption algorythm. \n" +
                    "Error: " + e + "\n");
            }
        }
        private void CodeDT(string source, string name, string destination)
        {
            string file = null;
            file = File.ReadAllText(source);
            string[] text = new string[3];
            string mode = "";
            Double_trasposition dt = new Double_trasposition();
            if (checkBox2.Checked == true) //biranje moda
            {
                text = dt.CodeOFB(file); //kodirani tekst
                if (dt.IVSet)
                    mode = dt.ByteToHex(dt.IV); //vrednost IV
            }
            else text = dt.Code(file);

            Md5 MD5 = new Md5();
            string hashCode=MD5.GetHash(file); //hash

            string write = hashCode + "\n" + text[0]; //upisuje u fajl 
                                                   //prva linija hash

            string dest = destination + @"\" + name;
            File.WriteAllText(dest, write);
            string key;
            if (mode == "")
                key = source + " " + text[1] + " " + text[2] +" "+ dest + "\n";
            else key = source + " " + text[1] + " " + text[2] + " " + mode + " " + dest + "\n";
            string pathKey = @"C:\Users\Natalija\Desktop\17321-Zadatak1-ZI\KeysDT.txt";
            File.AppendAllText(pathKey, key);
        }
        private void DecodeDT(string source, string name, string destination)
        {
            try
            {
                string hash1 = File.ReadLines(source).First(); // hash
                string textDecode = File.ReadAllText(source).Remove(0, hash1.Length + 1);

                string text, keyM = "", keyN = "",mode="";
                Double_trasposition dt = new Double_trasposition();

                string pathKey = @"C:\Users\Natalija\Desktop\17321-Zadatak1-ZI\KeysDT.txt";
                foreach (string line in File.ReadLines(pathKey))
                {

                    if (line.EndsWith(source))
                    {
                        string[] keys = line.Split(" ");
                        keyM = keys[1];
                        keyN = keys[2];
                        if (checkBox2.Checked && !keys[3].Contains(@":\")) // u fajlu se posle kljuca nalazi IV
                            mode = keys[3];
                        else // u fajlu se posle kljuca nalazi putanja destination
                        {
                            if (checkBox2.Checked)
                                MessageBox.Show("Encrypted file doesnt use OFB mode. Please turn it off.");
                            else MessageBox.Show("Encrypted file use OFB mode. Please turn it on.");
                            return;
                        }
                    }
                }

                if (checkBox2.Checked) //biranje moda
                {
                    dt.IV = dt.HexToByte(mode);
                    text = dt.DecodeOFB(textDecode, keyM, keyN);
                }
                else text = dt.Decode(textDecode, keyM, keyN);
                MessageBox.Show(text);
                Md5 MD5 = new Md5();
                string hash2 = MD5.GetHash(text); //hash dekodiranog teksta

                if (hash1 != hash2)
                    MessageBox.Show("Different hash codes");
                else
                {
                    string dest = destination + @"\" + name;
                    File.WriteAllText(dest, text);
                    MessageBox.Show("Same hash codes");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Try different encryption/decryption algorythm OR Try different mode.\n" +
                    "Error: " + e + "\n" );
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    textBox3.Text = fbd.SelectedPath;
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                button1.Enabled = false;
                textBox1.Enabled = false;
                button2.Enabled = false;
                textBox2.Enabled = false;
                button3.Enabled = false;
                textBox3.Enabled = false;
                button4.Enabled = false;
                textBox4.Enabled = false;
            }
            else
            {
                button1.Enabled = true;
                textBox1.Enabled = true;
                button2.Enabled = true;
                textBox2.Enabled = true;
                button3.Enabled = true;
                textBox3.Enabled = true;
                button4.Enabled = true;
                textBox4.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (new FileInfo(ofd.FileName).Length == 0) //nema sta da dekodira
                    {
                        return;
                    }
                    textBox2.Text = ofd.FileName;
                    SelectAlgorithmDecode(ofd.FileName, Path.GetFileName(ofd.FileName), textBox4.Text);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (new FileInfo(ofd.FileName).Length == 0) //nema sta da kodira
                    {
                        return;
                    }
                    textBox1.Text = ofd.FileName;
                    deleteFromKeys(ofd.FileName); //ukoliko se kodira opet sta je kodirano/ postoji vrednost kljuceva za taj fajl
                                                  //obrisace se prethodne vrednosti
                                                  //Code(ofd.FileName, Path.GetFileName(ofd.FileName), textBox3.Text);

                    SelectAlgorithm(ofd.FileName, Path.GetFileName(ofd.FileName), textBox3.Text);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    textBox4.Text = fbd.SelectedPath;
                }
            }
        }
        private void deleteFromKeys(string FullPath)
        {
            string pathKeys;
            if (radioButton1.Checked)
                pathKeys = @"C:\Users\Natalija\Desktop\17321-Zadatak1-ZI\KeysDT.txt";
            else pathKeys = @"C:\Users\Natalija\Desktop\17321-Zadatak1-ZI\KeysBF.txt";
            var tempFile = Path.GetTempFileName();
            var linesToKeep = File.ReadLines(pathKeys).Where(l => !l.StartsWith(FullPath));
            File.WriteAllLines(tempFile, linesToKeep);
            File.Delete(pathKeys);
            File.Move(tempFile, pathKeys);
        }
      private void fileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {
            if (checkBox1.Checked)
            {
                {
                    if (!IsFileReady(e.FullPath))
                    { }
                    else
                    {
                        deleteFromKeys(e.FullPath);
                        string destination = textBox3.Text;
                        SelectAlgorithm(e.FullPath, e.Name, destination);
                    }
                }
            }
        }
    }
}

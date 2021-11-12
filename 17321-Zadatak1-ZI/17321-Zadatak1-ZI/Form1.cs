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
                    string destination = textBox3.Text;
                    Code(e.FullPath, e.Name, destination);
                }
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
        private void Code(string source, string name, string destination)
        {
            string file = null;
            while (!IsFileReady(source))
            { }
            file = File.ReadAllText(source);
            string[] text = new string[3];
            Double_trasposition dt = new Double_trasposition(file);
            text = dt.Code(file);

            deleteFromKeys(source); //ukoliko se kodira opet sta je kodirano/ postoji vrednost kljuceva za taj fajl
                                    //obrisace se prethodne vrednosti
            string dest = destination + @"\" + name;
            File.WriteAllText(dest, text[0]);
            string key = source + " " + text[1] + " " + text[2] +" "+ dest + "\n";
            string pathKey = @"C:\Users\Natalija\Desktop\17321-Zadatak1-ZI\Keys.txt";
            File.AppendAllText(pathKey, key);
        }
        private void Decode(string source, string name, string destination)
        {
            string text,keyM="",keyN="";
            string file = File.ReadAllText(source);
            Double_trasposition dt = new Double_trasposition(file);
            
            string pathKey = @"C:\Users\Natalija\Desktop\17321-Zadatak1-ZI\Keys.txt";
            foreach (string line in File.ReadLines(pathKey))
            {

                if (line.EndsWith(source))
                {
                    string[] keys = line.Split(" ");
                    keyM = keys[1];
                    keyN = keys[2];
                }
            }
            text = dt.Decode(file,keyM,keyN);
            string dest = destination + @"\" + name;
            File.WriteAllText(dest, text);
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
                    Decode(ofd.FileName, Path.GetFileName(ofd.FileName), textBox4.Text);
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
                    Code(ofd.FileName, Path.GetFileName(ofd.FileName), textBox3.Text);
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
            string pathKeys = @"C:\Users\Natalija\Desktop\17321-Zadatak1-ZI\Keys.txt";
            var tempFile = Path.GetTempFileName();
            var linesToKeep = File.ReadLines(pathKeys).Where(l => !l.StartsWith(FullPath));
            if (linesToKeep != null)
            {
                File.WriteAllLines(tempFile, linesToKeep);
                File.Delete(pathKeys);
                File.Move(tempFile, pathKeys);
            }
        }
        private void fileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {
            if (checkBox1.Checked)
            {
                {
                    deleteFromKeys(e.FullPath);
                    string destination = textBox3.Text;
                    Code(e.FullPath, e.Name, destination);
                }
            }
        }
    }
}

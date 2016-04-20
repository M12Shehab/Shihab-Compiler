using ShihabCompiler.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ShihabCompiler
{
    public partial class Form1 : Form
    {

        string path;
        string[] SubFolders;
        int countFiles;
 int MaxCsharp, MaxCPlus, MaxJava;
            int MinCsharp, MinCPlus, MinJava;
           
        void LoadWait()
        {
            double i = 0;
            foreach (string file in SubFolders)
            {

                // MessageBox.Show(folderName,"This is folder");
                listBoxFiles.Items.Add(file);
               
                lblSystem.Text = "Loading( " + ((i/ SubFolders.Count()) * 100).ToString() + " %)....";
                lblCountFiles.Text = listBoxFiles.Items.Count.ToString() + " Programs, " + (listBoxFiles.Items.Count * 3).ToString() + " Files";
                lblCountFiles.Refresh();
                i++;
            }
            countFiles = (int)i;
            lblSystem.Text = "Ready...";
        }

        void SaveData(string ProgramName,string ProgramLanguage,ClassComplexityAttr attr,double halsted,int CC,int LOC, int SW)
        {
            ProgramLanguage = ProgramLanguage.ToLower();
            int total = attr.FlowChart.NumCases + attr.FlowChart.NumIF + attr.FlowChart.NumLoops + attr.FlowChart.NumRecursion;
            total += attr.NumOfFunctions + attr.NumOfExternalLibANDFun + attr.Varibles.Globel + attr.Varibles.Local+ (int)(Math.Log10( attr.NumOfOpreations+attr.FunctionCall)/Math.Log10(2));
            
           // int CC = 0;
            int Halsted = Convert.ToInt32(halsted);

            switch (ProgramLanguage)
            {
                case "c#":
                case "C#":
                    {
                        dataSet1.CSharp.AddCSharpRow(CC, Halsted, total, ProgramName,LOC,SW);
                        if (MaxCsharp < txtInput.Lines.Count())
                        {
                            MaxCsharp = txtInput.Lines.Count();
                        }

                        if (MinCsharp > txtInput.Lines.Count())
                        {
                            MinCsharp = txtInput.Lines.Count();
                        }
                    } break;
                case "c++":
                case "C++": { 
                    dataSet1.CPlus.AddCPlusRow(CC, Halsted, total, ProgramName,LOC,SW);
                    if (MaxCPlus < txtInput.Lines.Count())
                    {
                        MaxCPlus = txtInput.Lines.Count();
                    }
                    if (MinCPlus > txtInput.Lines.Count())
                    {
                        MinCPlus = txtInput.Lines.Count();
                    }

                    } break;
                case "java":
                case "Java":
                    {
                        dataSet1.Java.AddJavaRow(CC, Halsted, total, ProgramName,LOC,SW);
                        if (MaxJava < txtInput.Lines.Count())
                        {
                            MaxJava = txtInput.Lines.Count();
                        }

                        if (MinJava > txtInput.Lines.Count())
                        {
                            MinJava = txtInput.Lines.Count();
                        }
                    } break;
            }
 
        }

        void Compile()
        {
            MaxCsharp = MaxCPlus = MaxJava = int.MinValue;
            MinCsharp = MinCPlus = MinJava = int.MaxValue;

            for (int i = 0; i < listBoxFiles.Items.Count; i++)
            {
                string folderName = "";
                string[] files = Directory.GetFiles(listBoxFiles.Items[i].ToString(), "*.txt", SearchOption.AllDirectories);

                foreach (string file in files)
                {

                    //
                    folderName = new DirectoryInfo(file).Parent.ToString();
                    string FileName = Path.GetFileNameWithoutExtension(file);
                    if (FileName.Equals("C++") || FileName.Equals("c++") || FileName.Equals("C#") || FileName.Equals("c#"))
                    {
                        //This condition becasue this version does not support C# or C++ language
                        //we will try to add this on next version
                        continue;
                    }

                    txtReport.AppendText("Program : ");
                    txtReport.SelectionColor = Color.BlueViolet;

                    txtReport.AppendText(folderName);
                    txtReport.SelectionColor = Color.Black;

                    txtReport.AppendText(" , Language : ");
                    txtReport.SelectionColor = Color.DarkGreen;
                    txtReport.AppendText(FileName + "\n");

                    StreamReader readfile = new StreamReader(file);
                    string code = readfile.ReadToEnd();
                    readfile.Close();

                    txtInput.Text = code;
                    lblProgramInfo.Text = "Program : " + folderName + " , Language : " + FileName + " , " + txtInput.Lines.Count() + " Lines";
                    lblProgramInfo.Refresh();
                    Thread.Sleep(1);
                    ClassCompiler c = new ClassCompiler(txtInput.Text);
                    c.Compile();
                    Halestead clsHalsterd = new Halestead();
                    ClassCycloma2 clsCC = new ClassCycloma2(txtInput.Text);
                    ClassShaoWang ShaoWang = new ClassShaoWang(txtInput.Text);
                    int SW = ShaoWang.GetShaoWangComplexity();
                    txtReport.SelectionColor = Color.Black;
                    SaveData(folderName, FileName, c.GetReport(), clsHalsterd.calc_halested(txtInput.Text), clsCC.GetNumIfAndSwitch(), txtInput.Lines.Count(),SW);
                    txtReport.AppendText(c.PrintData1() + "\n====================\n\n");
                }
            }//End Read Files

            dataSet1.StaticalCPlus.AddStaticalCPlusRow(MaxCPlus, MinCPlus);
            dataSet1.StaticalCsharp.AddStaticalCsharpRow(MaxCsharp, MinCsharp);
            dataSet1.StaticalJava.AddStaticalJavaRow(MaxJava, MinJava);

            dataSet1.Statical.AddStaticalRow(MaxCPlus, MinCPlus, "C++");
            dataSet1.Statical.AddStaticalRow(MaxCsharp, MinCsharp, "C#");
            dataSet1.Statical.AddStaticalRow(MaxJava, MinJava, "Java");
        }


        public Form1()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnload_Click(object sender, EventArgs e)
        {
            try
            {
                folderBrowserDialog1.ShowDialog();
                path = folderBrowserDialog1.SelectedPath;
                SubFolders = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);

                listBoxFiles.Items.Clear();
                LoadWait();
                countFiles = 0;
            }
            catch (Exception ex)
            { }
        }

        private void toolStripButtonRun_Click(object sender, EventArgs e)
        {
            //oper_var cls = new oper_var(txtInput.Text);
            //MessageBox.Show("G "+ cls.G+"\nL "+cls.L);
            //Complextiy.Varibles.Local = ;
            Compile();
           // ClassCycloma2 c = new ClassCycloma2(txtInput.Text);
            //MessageBox.Show(c.GetNumIfAndSwitch().ToString(),"CC");
           // ClassShaoWang x = new ClassShaoWang(txtInput.Text);
            //x.GetShaoWangComplexity();
            //MessageBox.Show(x.fff.ToString(),"count of functions");
          //  ClassCompiler c = new ClassCompiler(txtInput.Text);
            //c.Compile();
            //txtReport.SelectionColor = Color.Black;
            //c.PrintData();
            //txtReport.AppendText(c.PrintData1() + "\n====================\n\n");
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            lblProgramInfo.Text = "Program : X , Language : L , " + txtInput.Lines.Count() + " Lines";
            lblProgramInfo.Refresh();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void saveResultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataSet1.Statical.WriteXml("Main Staitscal("+DateTime.Now.ToString("ss")+").xml");
            dataSet1.Java.WriteXml("Java Staitscal(" + DateTime.Now.ToString("ss") + ").xml");
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This Code is written by Mohammed Shehab, Mohammed Alndoliy and Wegdan Abdullkader");
        }
    }
}

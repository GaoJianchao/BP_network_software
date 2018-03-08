using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET.Utility;
using MLApp;

namespace fhfx
{
    public partial class Form2 : Form
    {
        private string[,] Mtable;//训练集条件属性
        private string[] Dici;
        private int I;//数据表行数，从1开始，训练集
        private int Attr;//数据表列数-1，从0开始，训练集
        private string[,] testData;//测试集
        private int I2;//测试集表行数
        private int Attr2;//测试集列数；
        private double[,] center1;//聚类中心 
        private int[] culser;//类标签
        public Form2()
        {
            InitializeComponent();
            showRecentFile();
            showRecentFile_test();
            config();
            date();
            timer1.Enabled = true;
        }
        //日期时间
        public void date()
        {
            DateTime time = DateTime.Today;
            label6.Text = time.Year.ToString()
                + "年" + time.Month.ToString() +
                "月" + time.Day.ToString() + "日" + " " +
                System.Globalization.CultureInfo.GetCultureInfo("zh-CN").DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek);
        }
        //定时器
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
            string tm = DateTime.Now.ToLongTimeString();
            label8.Text =tm;
        }
        //运行工作
        private void work(object sender, EventArgs e)
        {
            if (this.dataGridView1.DataSource != null && this.dataGridView2.DataSource != null)
                button6.Enabled = 调试ToolStripMenuItem.Enabled= true;
            else
                button6.Enabled = 调试ToolStripMenuItem.Enabled = true;
        }
        //打开数据文件
        public void OpenFile(string myText)
        {
            string str;
            int i, UI;

            string[] predata = new string[138];
            Dici = new string[138];
            FileStream aFile = new FileStream(myText, FileMode.Open);
            StreamReader sr = new StreamReader(aFile);
            str = sr.ReadLine();
            i = 0;
            while (str != null)
            {
                predata[i] = str;
                str = sr.ReadLine();
                i++;//predata储存还未处理过的信息
            }
            sr.Close();//以上为文件操作
            I = i;
            UI = I;//date存储数据，有逗号  I为对象个数
            string[] sp = predata[0].Split(new char[] { ',', ';' });
            Console.WriteLine(sp.Length);
            if (sp[sp.Length - 1] == "")
            {
                Attr = (predata[0].Split(new char[] { ',', ';' },
                    StringSplitOptions.None)).Length - 1;
                Console.WriteLine(Attr);
            }
            else
            {
                Attr = (predata[0].Split(new char[] { ',', ';' }, StringSplitOptions.None)).Length ;
            }
            Mtable = new string[I, Attr];
            for (i = 0; i < I; i++)
            {
                string[] CArray = predata[i].Split(new char[] { ',', ' ' });
                for (int j = 0; j < Attr; j++)
                {
                    Mtable[i, j] = CArray[j];//pradate数据转入date 每一个字符串的位置储存一个取值
                }
               // Dici[i] = CArray[Attr].Replace(";", "");//决策属性取值  去掉最后的分号
            }
            //
            //导入至dataview
            showDataTable();
            //  
            //更新表的信息
            string[] name = myText.Split('\\');
            label1.Text = name[name.Length - 1];
            label3.Text = Convert.ToString(I);
            label5.Text = Convert.ToString(Attr);
        }
        //显示数据表
        private void showDataTable()
        {
            dataGridView1.DataSource = null;
            dataGridView1.Refresh();
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn();//添加列的名字
            for (int num = 0; num < Attr; num++)
            {
                dc = new DataColumn((num + 1).ToString());
                dt.Columns.Add(dc);
            }
            //dc = new DataColumn("决策");
            //dt.Columns.Add(dc);
            int ii, jj;
            for (ii = 0; ii < I; ii++)
            {
                DataRow drr = dt.NewRow();
                for (jj = 0; jj < Attr; jj++)
                {
                    drr[jj] = Mtable[ii, jj];
                }
               // drr[jj] = Dici[ii];
                dt.Rows.Add(drr); //把每一行都加入datatable中，好像不能直接给数组
            }
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.DataSource = dt;
        }
        //显示最近打开文件路径
        private void showRecentFile()
        {
            FileStream showRecent = new FileStream(Directory.GetCurrentDirectory() + "\\filePath\\recentPath.txt", FileMode.Open);
            StreamReader sr = new StreamReader(showRecent);
            string str;
            str = sr.ReadLine();
            int i;
            for (i = 0; i < 6 && str != null; i++)
            {
                string[] str2 = str.Split('\\');
                switch (i)
                {
                    case 0: radioButton1.Visible = true; radioButton1.Text = str2[str2.Length - 1];
                        toolStripMenuItem1.Visible = true; toolStripMenuItem1.Text = "1 " + str;
                        break;
                    case 1: radioButton2.Visible = true; radioButton2.Text = str2[str2.Length - 1];
                        toolStripMenuItem2.Visible = true; toolStripMenuItem2.Text = "2 " + str;
                        break;
                    case 2: radioButton3.Visible = true; radioButton3.Text = str2[str2.Length - 1];
                        toolStripMenuItem3.Visible = true; toolStripMenuItem3.Text = "3 " + str;
                        break;
                    case 3: toolStripMenuItem3.Visible = true; toolStripMenuItem4.Text = "4 " + str;
                        break;
                    case 4: toolStripMenuItem5.Visible = true; toolStripMenuItem5.Text = "5 " + str;
                        break;
                    case 5: toolStripMenuItem6.Visible = true; toolStripMenuItem6.Text = "6 " + str;
                        break;
                    default: break;
                }
                str = sr.ReadLine();
            }
            sr.Close();
        }
        //更新最近文件信息
        private void updateTrianFile(string name)
        {
            FileStream recent = new FileStream(Directory.GetCurrentDirectory() + "\\filePath\\recentPath.txt", FileMode.Open);
            FileInfo fileInfo = new FileInfo(Directory.GetCurrentDirectory() + "\\filePath\\recentPath.txt");
            StreamReader sr = new StreamReader(recent);
            int i, j;
            string str;
            str = sr.ReadLine();
            string[] fileName = new string[6];
            string[] newName = new string[6];
            for (i = 0; str != null; i++)
            {
                fileName[i] = str;
                str = sr.ReadLine();
            }
            sr.Close();
            newName[0] = name;
            for (i = 0, j = 1; j < 6; i++)
            {
                if (!newName[0].Equals(fileName[i]))
                {
                    newName[j] = fileName[i];
                    j++;
                }
            }
            StreamWriter sw = fileInfo.CreateText();
            for (i = 0; i < 6 && newName[i] != null; i++)
            {
                sw.WriteLine(newName[i]);
            }
            sw.Close();
        }
        //导入文件，并修改最近文件路径
        private void open_train_file()
        {
            OpenFileDialog openfile = new OpenFileDialog();
            DialogResult result = openfile.ShowDialog();
            openfile.Filter = "文本文件(*txt)|*.txt";
            if (result == DialogResult.OK)
            {
                OpenFile(openfile.FileName);
                //更新最近文件信息
                updateTrianFile(openfile.FileName);
            }
            showRecentFile();
        }
        //导入文件，并修改最近文件路径入口1
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            open_train_file();
        }
        //导出结果
        private void export_result()
        {
            int i, j;
            SaveFileDialog Savefile = new SaveFileDialog();
            Savefile.Filter = "文本文件(*txt)|*.txt";
            Savefile.FilterIndex = 2;
            Savefile.RestoreDirectory = true;
            DialogResult result = Savefile.ShowDialog();
            if (result == DialogResult.OK)
            {
                FileStream aFile = new FileStream(Savefile.FileName, FileMode.Create);
                StreamWriter sw = new StreamWriter(aFile);
                DataTable dt = new DataTable();
                dt = (DataTable)this.dataGridView2.DataSource;
                DataRow drr = dt.NewRow();
                for (j = 0; j < dt.Columns.Count; j++)
                {
                    sw.Write(dt.Columns[j].ToString());
                    if (j < (dt.Columns.Count - 1))
                        sw.Write("\t");
                }
                sw.WriteLine();
                for (i = 0; i < dt.Rows.Count; i++)
                {
                    for (j = 0; j < dt.Columns.Count; j++)
                    {
                        sw.Write(dt.Rows[i][j]);
                        if (j < (dt.Columns.Count - 1))
                            sw.Write("\t");
                    }
                    sw.WriteLine();
                }
                sw.Close();
            }
        }
        //导出数据表入口1
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            export_result();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == false && radioButton2.Checked == false && radioButton3.Checked == false && radioButton4.Checked == false && radioButton5.Checked == false && radioButton5.Checked == false)
                MessageBox.Show("请选择要导入的文件！", "消息提示");
            else
            {
                if (radioButton1.Checked == true)
                    select_open(1, radioButton1.Text);
                if (radioButton2.Checked == true)
                    select_open(2, radioButton2.Text);
                if (radioButton3.Checked == true)
                    select_open(3, radioButton3.Text);
                if (radioButton4.Checked == true)
                    select_openTest(4, radioButton4.Text);
                if (radioButton5.Checked == true)
                    select_openTest(5, radioButton5.Text);
                if (radioButton6.Checked == true)
                    select_openTest(6, radioButton6.Text);
            }
        }
        //选择打开最近文件
        public void select_open(int pos, string filename)
        {
            FileStream recent = new FileStream(Directory.GetCurrentDirectory() + "\\filePath\\recentPath.txt", FileMode.Open);
            StreamReader sr = new StreamReader(recent);
            string str;
            str = sr.ReadLine();
            int i;
            for (i = 0; i < 3 && str != null; i++)
            {
                string[] str2 = str.Split('\\');
                if (filename.Equals(str2[str2.Length - 1]))
                {
                    OpenFile(str);
                    break;
                }
                str = sr.ReadLine();
            }
            sr.Close();
            updateTrianFile(str);
        }
        public void select_openTest(int pos, string filename)
        {
            FileStream recent = new FileStream(Directory.GetCurrentDirectory() + "\\filePath\\testPath.txt", FileMode.Open);
            StreamReader sr = new StreamReader(recent);
            string str;
            str = sr.ReadLine();
            int i;
            for (i = 0; i < 3 && str != null; i++)
            {
                string[] str2 = str.Split('\\');
                if (filename.Equals(str2[str2.Length - 1]))
                {
                    OpenFile_test(str);
                    break;
                }
                str = sr.ReadLine();
            }
            sr.Close();
            updateTestFile(str);
        }
        //二维string转换double
        public double[,] convert_two(string[,] Mat)
        {
            double[,] cvt = new double[Mat.GetLength(0), Mat.GetLength(1)];
            for (int i = 0; i < Mat.GetLength(0); i++)
                for (int j = 0; j < Mat.GetLength(1); j++)
                    cvt[i, j] = Convert.ToDouble(Mat[i, j]);
            return cvt;
        }
        //一维string转换double
        public double[] convert_one(string[] Mat)
        {
            double[] cvt = new double[Mat.Length];
            for (int i = 0; i < Mat.Length; i++)
                cvt[i] = Convert.ToDouble(Mat[i]);
            return cvt;
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
        //二维聚类
        public int[] kmeans_two(double[,] a, int k)
        {
            if (k < 1)
            {
                Console.WriteLine("ERROR!");
                return new int[] { 1 };
            }
            culser = new int[a.GetLength(0)];//返回的各个样本所属类的标签；
            int count1 = 0;
            //随机初始化聚类中心
            center1 = new double[k, a.GetLength(1)];
            /*double [][]center1={{0.9975,0.1087,0.9975,0.0834,0.9875,0.2},
                    {0.9725,0.3696,0.9725,0.3334,0.9625,0.5},
                    {0.925,0.6739,0.925,0.6667,0.925,0.7},
                    {0.89,0.9131,0.89,0.9167,0.7,0.9}
    };*/
            for (int i = 0; i < k; i++)
            {
                /*Random random = new Random(Guid.NewGuid().GetHashCode());
                int num = random.Next(0, a.GetLength(0));*/
                //System.arraycopy(a[num],0,center1[i],0,a[num].length);
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    //center1[i, j] = a[num, j];
                    center1[0,j] = a[2,j];
                    center1[1,j] = a[45,j];
                    center1[2,j] = a[46,j];
                    /*center1[3,j] = a[22,j];
                    center1[4,j] = a[52,j];
                    center1[5,j] = a[112,j];*/
                }
            }

            bool f = false;
            do
            {
                double[,] distence = new double[a.GetLength(0), k];//样本到聚类中心的距离
                f = false;
                for (int i = 0; i < a.GetLength(0); i++)
                {                    //计算每个样本到四个聚类中心的距离
                    for (int j = 0; j < k; j++)
                    {
                        double[] a_row = new double[a.GetLength(1)];
                        double[] c_row = new double[a.GetLength(1)];
                        for (int p = 0; p < a.GetLength(1); p++)
                        {
                            a_row[p] = a[i, p];
                            c_row[p] = center1[j, p];
                        }
                        distence[i, j] = means(a_row, c_row);
                    }
                }
                double[] rdis = new double[a.GetLength(0)];             //存放每个样本到所属类中心的距离         
                for (int i = 0; i < a.GetLength(0); i++)
                {                     //找出每个样本所属类
                    double min = distence[i, 0];
                    for (int j = 1; j < k; j++)
                    {
                        if (distence[i, j] < min)
                        {
                            min = distence[i, j];
                            rdis[i] = distence[i, j];
                            culser[i] = j;
                        }
                    }
                }
                //重新计算每个类的每个属性的均值
                double[,] reculser = new double[k, a.GetLength(1)];
                for (int i = 0; i < k; i++)
                {
                    for (int j = 0; j < a.GetLength(1); j++)
                    {
                        int count = 0;
                        double s = 0;
                        for (int p = 0; p < a.GetLength(0); p++)
                        {
                            if (culser[p] == i)
                            {
                                s += a[p, j];
                                count++;
                            }
                        }
                        if (count != 0)
                            reculser[i, j] = (s / count);
                    }
                }
                //判断两次聚类中心是否相等，若不相等则继续循环
                /*for(int i=0;i<k;i++){
                    if(!(java.util.Arrays.equals(reculser[i],center1[i]))){
                        //System.arraycopy(reculser[i],0,center1[i],0,center1[i].length);		
                        f=true;
                        break;
                    }
                }*/
                for (int i = 0; i < k; i++)
                {
                    for (int j = 0; j < a.GetLength(1); j++)
                    {
                        if (reculser[i, j] != center1[i, j])
                        {
                            f = true;
                            break;
                        }
                    }
                    if (f)
                        break;
                }
                if (f)
                {
                    for (int i = 0; i < k; i++)
                    {
                        for (int j = 0; j < a.GetLength(1); j++)
                            center1[i, j] = reculser[i, j];
                    }
                }
                count1++;
            } while (f);
            for (int i = 0; i < center1.GetLength(0); i++)
            {
                this.richTextBox1.Text += "第" + (i+1)+ "类：";
                for (int j = 0; j < center1.GetLength(1); j++)
                    this.richTextBox1.Text += (decimal.Round(decimal.Parse(Convert.ToString(center1[i,j])),2)) + " ";
                this.richTextBox1.Text += "\n";
            }
            bool[] b = new bool[a.GetLength(0)];
            int[] culser2 = new int[k];
            Console.WriteLine("count1=" + count1);
            for (int i = 0; i < a.GetLength(0); i++)
            {
                if (!b[i])
                {
                    double count3 = culser[i];
                    for (int j = i; j < a.GetLength(0); j++)
                    {
                        if (count3 == culser[j])
                        {
                            culser2[(int)count3]++;
                            b[i] = b[j] = true;
                        }
                    }
                }
            }
            for (int i = 0; i < k; i++)
            {
                Console.Write(i + ":" + culser2[i] + " ");
            }
            Console.WriteLine();
            return culser;
        }
        //计算两个样本之间的距离
        public static double means(double[] a, double[] b)
        {
            double sum = 0;
            for (int i = 0; i < a.Length; i++)
            {
                sum += (a[i] - b[i]) * (a[i] - b[i]);
            }
            sum = Math.Sqrt(sum);
            return sum;
        }
        //一维离散
        public int[] kmeans_one(double[] a, int k)
        {
            if (k < 1)
            {
                Console.WriteLine("ERROR!");
                return new int[] { 1 };
            }
            int[] culser = new int[a.Length];//返回的各个样本所属类的标签；
            int count1 = 0;
            //随机初始化聚类中心
            double[] center1 = new double[k];
            for (int i = 0; i < k; i++)
            {
                Random random = new Random(Guid.NewGuid().GetHashCode());
                int num = random.Next(0, a.Length);
                Console.Write(num + " ");
                center1[i] = a[num];
            }
            Console.WriteLine();
            bool f = false;
            do
            {
                double[,] distence = new double[a.Length, k];//样本到聚类中心的距离
                f = false;
                for (int i = 0; i < a.Length; i++)
                {                    //计算每个样本到四个聚类中心的距离
                    for (int j = 0; j < k; j++)
                    {
                        distence[i, j] = Math.Abs(a[i] - center1[j]);
                    }
                }
                double[] rdis = new double[a.GetLength(0)];             //存放每个样本到所属类中心的距离         
                //int label=0;
                for (int i = 0; i < a.Length; i++)
                {                     //找出每个样本所属类
                    double min = distence[i, 0];
                    for (int j = 1; j < k; j++)
                    {
                        if (distence[i, j] < min)
                        {
                            min = distence[i, j];
                            rdis[i] = distence[i, j];
                            culser[i] = j;
                        }
                    }
                }
                //重新计算每个类的每个属性的均值
                double[] reculser = new double[k];
                this.richTextBox2.Text += "*********************************************\n";
                this.richTextBox2.Text += "  离散第" + (count1+1) + "次：\n";
                for (int i = 0; i < k; i++)
                {
                    double min=-1, max=-1;
                    int count = 0;
                    double s = 0;
                    for (int p = 0; p < a.Length; p++)
                    {
                        if (culser[p] == i)
                        {
                            s += a[p];
                            count++;
                            if (min == -1)
                                min = a[p];
                            else
                            {
                                if (a[p] < min)
                                    min = a[p];//某类最小值
                            }
                            if (max < a[p])
                                max = a[p];//某类最大值
                        }
                    }
                    if (count != 0)
                        reculser[i] = (s / count);
                    Console.Write(i + ":" + count + " ");
                    this.richTextBox2.Text+= "    第" + (i+1) + "类：" + "个数："+count + "\t"+"属性值区间："+min+"---"+max+"\n";
                }
                Console.WriteLine();
                //判断两次聚类中心是否相等，若不相等则继续循环
                for (int i = 0; i < k; i++)
                {
                    if (reculser[i] != center1[i])
                    {
                        f = true;
                        break;
                    }
                }
                if (f)
                {
                    for (int i = 0; i < k; i++)
                    {
                        center1[i] = reculser[i];
                    }
                }
                count1++;
            } while (f);
            this.richTextBox2.Text = "离散总次数：" + count1 + "\n"+this.richTextBox2.Text;
            Console.WriteLine("count1=" + count1);
            for (int i = 0; i < k; i++)
            {
                double min = -1, max = -1;
                int count = 0;
                double s = 0;
                for (int p = 0; p < a.Length; p++)
                {
                    if (culser[p] == i)
                    {
                        s += a[p];
                        count++;
                        if (min == -1)
                            min = a[p];
                        else
                        {
                            if (a[p] < min)
                                min = a[p];//某类最小值
                        }
                        if (max < a[p])
                            max = a[p];//某类最大值
                    }
                }
                this.richTextBox1.Text += "第" + (i + 1) + "类：" + "样本个数：" + count + "个\n" + "离散数字：" + i + "\t" + "属性值区间：" + min + " -- " + max + "\n";
            }
            return culser;
        }
        //导出
        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog Savefile = new SaveFileDialog();
            Savefile.Filter = "文本文件(*txt)|*.txt";
            Savefile.FilterIndex = 2;
            Savefile.RestoreDirectory = true;
            DialogResult result = Savefile.ShowDialog();
            if (result == DialogResult.OK)
            {
                FileStream aFile = new FileStream(Savefile.FileName, FileMode.Create);
                StreamWriter sw = new StreamWriter(aFile);
                sw.WriteLine("数据表名：" + label1.Text);
                sw.WriteLine("保存日期:" + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
                sw.WriteLine("############################################\n");
                sw.WriteLine(this.groupBox4.Text+"：");
                string[] text= this.richTextBox1.Text.Split('\n');
                for (int i = 0; i < text.Length; i++)
                    sw.WriteLine(text[i]);
                sw.WriteLine("---------------------------------------------\n");
                sw.WriteLine(this.groupBox5.Text + "：");
                string[] text2= this.richTextBox2.Text.Split('\n');
                for (int i = 0; i < text2.Length; i++)
                    sw.WriteLine(text2[i]);
                sw.Close();
            }
        }
         //清空
        private void button4_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Text = this.richTextBox2.Text = "";
            button3.Enabled = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int[] a = kmeans_two(convert_two(Mtable), 3);
            for (int i = 0; i < a.Length; i++)
                Console.Write(a[i] + " ");

        }

        //预测结果显示
        private void showDataTable_test_result(double[,]t_result)
        {
            dataGridView2.DataSource = null;
            dataGridView2.Refresh();
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn();//添加列的名字
            int cnum;
            if (Mtable.GetLength(1) == testData.GetLength(1))
                cnum = t_result.GetLength(1) - 3;
            else
                cnum = t_result.GetLength(1) - 1;
            for (int num = 0; num < cnum; num++)
            {
                dc = new DataColumn((num + 1).ToString());
                dt.Columns.Add(dc);
            }
            if (Mtable.GetLength(1) == testData.GetLength(1))
            {
                dc = new DataColumn("实际值");
                dt.Columns.Add(dc);
            }
            dc = new DataColumn("预测值");
            dt.Columns.Add(dc);
            if (Mtable.GetLength(1) == testData.GetLength(1))
            {
                dc = new DataColumn("相对误差");
                dt.Columns.Add(dc);
            }
            int ii, jj;
            for (ii = 0; ii < t_result.GetLength(0); ii++)
            {
                DataRow drr = dt.NewRow();
                for (jj = 0; jj < t_result.GetLength(1); jj++)
                {
                    drr[jj] = t_result[ii, jj];
                }
                // drr[jj] = Dici[ii];
                dt.Rows.Add(drr); //把每一行都加入datatable中，好像不能直接给数组
            }
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView2.DataSource = dt;
        }
        private void button6_Click(object sender, EventArgs e)
        {
            button3.Enabled = true;
            toolStripButton2.Enabled = true;
            MLAppClass matlab = new MLAppClass();
            matlab.Execute(@"clear");
            int y_num = Convert.ToInt32(this.label18.Text);
            double vt= Convert.ToDouble(this.label20.Text);
            double DL= Convert.ToDouble(this.label22.Text);
            double DC= Convert.ToDouble(this.label24.Text);
            double WC= Convert.ToDouble(this.label26.Text);
            double [] yn = { y_num };
            double [] ynIm = { 0 };
            double[] s = { vt };
            double[] dl = { DL };
            double[] dc = { DC };
            double[] wc = { WC };
            double[] sIm = {0};
            double[] dlIm= {0};
            double[] dcIm= {0};
            double[] wcIm= {0};
            double[,] dataIm = new double[Mtable.GetLength(0), Mtable.GetLength(1)];
            double[,] dataIm2 = new double[testData.GetLength(0), testData.GetLength(1)];
            matlab.PutFullMatrix("m_data", "base", convert_two(Mtable), dataIm);//第一个m_data表示存储的数据，第二个参数表示工作空间，第三个参数data表示输入数据的实部，第四个参数表示输入数据的虚部
            matlab.PutFullMatrix("yn", "base", yn, ynIm);
            matlab.PutFullMatrix("t_data", "base", convert_two(testData), dataIm2);
            matlab.PutFullMatrix("s", "base", s, sIm);
            matlab.PutFullMatrix("dl", "base", dl, dlIm);
            matlab.PutFullMatrix("dc", "base", dc, dcIm);
            matlab.PutFullMatrix("wc", "base", wc, wcIm);

            string path_project = Directory.GetCurrentDirectory();
            string path4_matlab = "cd('" + path_project + "')";
            matlab.Execute(path4_matlab);
            int changeColumn;
            if (Mtable.GetLength(1) == testData.GetLength(1))
            {
                matlab.Execute(@"[XDE3,inputWeights,inb,outw,outbias]= net_make(m_data, yn, t_data,s,dl,dc,wc);");//测试结果有比较
                changeColumn = testData.GetLength(1) + 2;
            }
            else
            {
                matlab.Execute(@"[XDE3,inputWeights,inb,outw,outbias]= net_make_little(m_data, yn, t_data,s,dl,dc,wc);");//测试结果无比较
                changeColumn = testData.GetLength(1) + 1;
            }
            matlab.Execute(@"XDE=XDE3");
            matlab.Execute(@"inputw=inputWeights");
            // System.Array prresult1= new double[1,43];
            // System.Array piresult1 = new double[1, 43];
            //输出原数据+测试值+相对误差数据表
            System.Array prresult = new double[testData.GetLength(0), changeColumn];
            System.Array piresult = new double[testData.GetLength(0), changeColumn];
            matlab.GetFullMatrix("XDE", "base", ref prresult, ref piresult);
            double[,] result1 = new double[testData.GetLength(0), changeColumn];
            this.richTextBox2.Text+="预测值：\n";
            double sum = 0;
            double ave = 0;
            for (int i = 0; i < testData.GetLength(0); i++)
            {
                for (int j = 0; j < changeColumn; j++)
                {
                    result1[i, j] = Convert.ToDouble(prresult.GetValue(i, j));
                    if (j >= testData.GetLength(1))
                        result1[i, j] = Convert.ToDouble(decimal.Round(decimal.Parse(Convert.ToString(result1[i, j])), 4));
                    if (j == testData.GetLength(1))
                    {
                        if(i%3!=0)
                            this.richTextBox2.Text += "第" + (i + 1) + "个："+result1[i,j]+"\t";
                        else
                            this.richTextBox2.Text += "\n第" + (i + 1) + "个：" + result1[i, j] + "\t";
                    }
                    if (j == (testData.GetLength(1) + 1))
                    {
                        sum += result1[i, j];
                    }
                    Console.Write(result1[i, j] + " ");
                }
                Console.WriteLine();
            }
            if (changeColumn ==(testData.GetLength(1) + 2))
            {
                ave = sum / testData.GetLength(0);
                this.richTextBox2.Text = "平均预测误差："+ave+"\n"+this.richTextBox2.Text;
            }
            this.richTextBox2.Text = "预测样本个数：" + testData.GetLength(0) + "\n" + this.richTextBox2.Text;
            showDataTable_test_result(result1);
            //隐含层输出开始
            this.richTextBox1.Text += "所有隐含层神经元的权重和阈值：\n";
            this.richTextBox1.Text += "***********************************\n";
            //输出隐含层权值
            System.Array iw=new double[y_num, (Mtable.GetLength(1)-1)];
            System.Array iwIm = new double[y_num, (Mtable.GetLength(1) - 1)];
            matlab.GetFullMatrix("inputw", "base", ref iw, ref iwIm);
            double[,] result_iw = new double[y_num, (Mtable.GetLength(1) - 1)];
            Console.WriteLine("iw#####################");
            this.richTextBox1.Text+="  神经元的输入权重：\n";
            for (int i = 0; i < y_num; i++)
            {
                this.richTextBox1.Text += "  第" + (i+1) + "个：";
                for (int j = 0; j < (Mtable.GetLength(1) - 1); j++)
                {
                    result_iw[i, j] = Convert.ToDouble(iw.GetValue(i, j));
                    this.richTextBox1.Text += (decimal.Round(decimal.Parse(Convert.ToString(result_iw[i,j])), 4)) + "    ";
                    Console.Write(result_iw[i, j] + " ");
                }
                this.richTextBox1.Text += "\n";
                Console.WriteLine();
            }
            //输出隐含层阈值
            this.richTextBox1.Text += "***********************************\n";
            this.richTextBox1.Text += "  神经元的阈值：\n";
            System.Array ib = new double[1,y_num];
            System.Array ibIm= new double[1,y_num];
            matlab.GetFullMatrix("inb", "base", ref ib, ref ibIm);
            double[,] result_ib= new double[1,y_num];
            Console.WriteLine("ib####################");
            for (int j = 0; j < y_num; j++)
            {
                result_ib[0, j] = Convert.ToDouble(ib.GetValue(0, j));
                if (j % 5 != 0)
                    this.richTextBox1.Text += "阈值" + (j+1) + "：" + (decimal.Round(decimal.Parse(Convert.ToString(result_ib[0, j])), 4)) + "    ";
                else
                {
                    this.richTextBox1.Text += "\n"+"    阈值" + (j+1) + "：" + (decimal.Round(decimal.Parse(Convert.ToString(result_ib[0, j])), 4))+"    ";
                }
                Console.Write(result_ib[0, j] + " ");
            }
            Console.WriteLine();
            //输出层输出开始
            this.richTextBox1.Text += "\n###################################\n";
            this.richTextBox1.Text += "输出层的权重和阈值：\n";
            this.richTextBox1.Text += "***********************************\n";
            //输出输出层权值
            this.richTextBox1.Text += "  输出层权重：\n";
            this.richTextBox1.Text += "  ";
            System.Array ow= new double[1, y_num];
            System.Array owIm=new double[1, y_num];
            matlab.GetFullMatrix("outw", "base", ref ow, ref owIm);
            double[,] result_ow= new double[1, y_num];
            Console.WriteLine("ow#############################");
            for (int j = 0; j < y_num; j++)
            {
                result_ow[0, j] = Convert.ToDouble(ow.GetValue(0, j));
                if (j % 5 != 0)
                    this.richTextBox1.Text += "权重" + (j+1) + "：" + (decimal.Round(decimal.Parse(Convert.ToString(result_ow[0, j])), 4)) + "    ";
                else
                    this.richTextBox1.Text += "\n"+"    权重" + (j+1) + "：" + (decimal.Round(decimal.Parse(Convert.ToString(result_ow[0, j])), 4)) + "    ";
                Console.Write(result_ow[0, j] + " ");
            }
            Console.WriteLine();
            //输出输出层阈值
            this.richTextBox1.Text += "\n***********************************\n";
            System.Array ob= new double[1, 1];
            System.Array obIm= new double[1, 1];
            matlab.GetFullMatrix("outbias", "base", ref ob, ref obIm);
            double result_ob = Convert.ToDouble(ob.GetValue(0,0));
            this.richTextBox1.Text += "  神经元的阈值：" + (decimal.Round(decimal.Parse(Convert.ToString(result_ob)), 4)) + "\n";
            Console.WriteLine("ob=" + result_ob);

            matlab.MinimizeCommandWindow();//最小化matlab command窗口
            matlab.Quit();//关闭Matlab服务器
            MessageBox.Show("Predict Completed!", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        //打开测试集文件
        public void OpenFile_test(string myText)
        {
            string str;
            int i, UI;
            string[] predata = new string[138];
           // Dici = new string[138];
            FileStream aFile = new FileStream(myText, FileMode.Open);
            StreamReader sr = new StreamReader(aFile);
            str = sr.ReadLine();
            i = 0;
            while (str != null)
            {
                predata[i] = str;
                str = sr.ReadLine();
                i++;//predata储存还未处理过的信息
            }
            sr.Close();//以上为文件操作
            I2= i;
            UI = I;//date存储数据，有逗号  I为对象个数
            string[] sp = predata[0].Split(new char[] { ',', ';' });
            Console.WriteLine(sp.Length);
            if (sp[sp.Length - 1] == "")
            {
                Attr2= (predata[0].Split(new char[] { ',', ';' },
                    StringSplitOptions.None)).Length - 1;
                //Console.WriteLine(Attr2);
            }
            else
            {
                Attr2= (predata[0].Split(new char[] { ',', ';' }, StringSplitOptions.None)).Length;
            }
            testData = new string[I2, Attr2];
            for (i = 0; i < I2; i++)
            {
                string[] CArray = predata[i].Split(new char[] { ',', ' ' });
                for (int j = 0; j < Attr2; j++)
                {
                    testData[i, j] = CArray[j];//pradate数据转入date 每一个字符串的位置储存一个取值
                }
                // Dici[i] = CArray[Attr].Replace(";", "");//决策属性取值  去掉最后的分号
            }
            //
            //导入至dataview
            showDataTable_test();
            //  
            //更新测试集表的信息
            string[] name = myText.Split('\\');
            label12.Text = name[name.Length - 1];
            label14.Text = Convert.ToString(I2);
            label16.Text = Convert.ToString(Attr2);
        }
        //显示测试集
        private void showDataTable_test()
        {
            dataGridView2.DataSource = null;
            dataGridView2.Refresh();
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn();//添加列的名字
            for (int num = 0; num < Attr2; num++)
            {
                dc = new DataColumn((num + 1).ToString());
                dt.Columns.Add(dc);
            }
            //dc = new DataColumn("决策");
            //dt.Columns.Add(dc);
            int ii, jj;
            for (ii = 0; ii < I2; ii++)
            {
                DataRow drr = dt.NewRow();
                for (jj = 0; jj < Attr2; jj++)
                {
                    drr[jj] = testData[ii, jj];
                }
                // drr[jj] = Dici[ii];
                dt.Rows.Add(drr); //把每一行都加入datatable中，好像不能直接给数组
            }
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView2.DataSource = dt;
        }

        private void showRecentFile_test()
        {
            FileStream showRecent = new FileStream(Directory.GetCurrentDirectory() + "\\filePath\\testPath.txt", FileMode.Open);
            StreamReader sr = new StreamReader(showRecent);
            string str;
            str = sr.ReadLine();
            int i;
            for (i = 0; i < 6 && str != null; i++)
            {
                string[] str2 = str.Split('\\');
                switch (i)
                {
                    case 0: radioButton4.Visible = true; radioButton4.Text = str2[str2.Length - 1];
                            toolStripMenuItem7.Visible = true; toolStripMenuItem7.Text = "1 " + str;
                        break;
                    case 1: radioButton5.Visible = true; radioButton5.Text = str2[str2.Length - 1];
                            toolStripMenuItem8.Visible = true; toolStripMenuItem8.Text = "2 " + str;
                        break;
                    case 2: radioButton6.Visible = true; radioButton6.Text = str2[str2.Length - 1];
                            toolStripMenuItem9.Visible = true; toolStripMenuItem9.Text = "3 " + str;
                        break;
                    case 3: toolStripMenuItem10.Visible = true; toolStripMenuItem10.Text = "4 " + str;
                        break;
                    case 4: toolStripMenuItem11.Visible = true; toolStripMenuItem11.Text = "5 " + str;
                        break;
                    case 5: toolStripMenuItem12.Visible = true; toolStripMenuItem12.Text = "6 " + str;
                        break;
                    default: break;
                }
                str = sr.ReadLine();
            }
            sr.Close();
        }
        //更新测试集最近文件信息
        private void updateTestFile(string name)
        {
            FileStream recent = new FileStream(Directory.GetCurrentDirectory() + "\\filePath\\testPath.txt", FileMode.Open);
            FileInfo fileInfo = new FileInfo(Directory.GetCurrentDirectory() + "\\filePath\\testPath.txt");
            StreamReader sr = new StreamReader(recent);
            int i, j;
            string str;
            str = sr.ReadLine();
            string[] fileName = new string[6];
            string[] newName = new string[6];
            for (i = 0; str != null; i++)
            {
                fileName[i] = str;
                str = sr.ReadLine();
            }
            sr.Close();
            newName[0] = name;
            for (i = 0, j = 1; j < 6; i++)
            {
                if (!newName[0].Equals(fileName[i]))
                {
                    newName[j] = fileName[i];
                    j++;
                }
            }
            StreamWriter sw = fileInfo.CreateText();
            for (i = 0; i < 6 && newName[i] != null; i++)
            {
                sw.WriteLine(newName[i]);
            }
            sw.Close();
        }
        //打开测试集文件
        private void open_test_file()
        {
            OpenFileDialog openfile = new OpenFileDialog();
            DialogResult result = openfile.ShowDialog();
            openfile.Filter = "文本文件(*txt)|*.txt";
            if (result == DialogResult.OK)
            {
                OpenFile_test(openfile.FileName);
                //更新测试集最近文件信息
                updateTestFile(openfile.FileName);
            }
            showRecentFile_test();
        }
        //打开测试集文件入口1
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            open_test_file();
        }
        //默认设置
        public void config()
        {
            this.label18.Text = "15";
            this.label20.Text = "0.05";
            this.label22.Text = "0.9";
            this.label24.Text = "8000";
            this.label26.Text = "1E-3";
        }
        //还原默认设置
        private void button5_Click_1(object sender, EventArgs e)
        {
            config();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text == "" && this.textBox2.Text == "")
                MessageBox.Show("请填写信息", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                int place1,place2;
                if (this.textBox1.Text == "")
                    place1 = 0;
                else
                    place1 = Convert.ToInt32(this.textBox1.Text);
                if (this.textBox2.Text == "")
                    place2 = 0;
                else
                    place2 = Convert.ToInt32(this.textBox2.Text);
                if ((place1 < 0) || (place2< 0))
                    MessageBox.Show("禁止为负值", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {
                    if (this.textBox1.Text != "")
                        this.label18.Text = this.textBox1.Text;
                    if (this.textBox2.Text != "")
                        this.label24.Text = this.textBox2.Text;
                    MessageBox.Show("参数设置成功", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Form3 f3 = new Form3();
            f3.Owner = this;
            f3.ShowDialog();
        }

        private void toolStripMenuItem15_Click(object sender, EventArgs e)
        {
            Close();
        }
        //导入文件，并修改最近文件路径入口2
        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            open_train_file();
        }
        //打开测试集文件入口2
        private void toolStripMenuItem14_Click(object sender, EventArgs e)
        {
            open_test_file();
        }
        //导出数据表入口2
        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            export_result();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = radioButton2.Checked = radioButton3.Checked = radioButton4.Checked = radioButton5.Checked = radioButton6.Checked = false;
        }
        //菜单打开训练集文件，并更新最近文件
        private void menuOpenTrain(object sender, EventArgs e)
        {
            ToolStripMenuItem tsm = (ToolStripMenuItem)sender;
            FileStream recent = new FileStream(Directory.GetCurrentDirectory() + "\\filePath\\recentPath.txt", FileMode.Open);
            StreamReader sr = new StreamReader(recent);
            string str;
            str = sr.ReadLine();
            string []str2 = tsm.Text.Split(' ');
            int i;
            for (i = 0; i < 6 && str != null; i++)
            {
                if (str2[1].Equals(str))
                {
                    OpenFile(str);
                    break;
                }
                str = sr.ReadLine();
            }
            sr.Close();
            updateTrianFile(str2[1]);
        }
        //菜单打开测试集文件，并更新最近文件
        private void menuOpenTest(object sender, EventArgs e)
        {
            ToolStripMenuItem tsm = (ToolStripMenuItem)sender;
            FileStream recent = new FileStream(Directory.GetCurrentDirectory() + "\\filePath\\testPath.txt", FileMode.Open);
            StreamReader sr = new StreamReader(recent);
            string str;
            str = sr.ReadLine();
            string[] str2 = tsm.Text.Split(' ');
            int i;
            for (i = 0; i < 6 && str != null; i++)
            {
                if (str2[1].Equals(str))
                {
                    OpenFile_test(str);
                    break;
                }
                str = sr.ReadLine();
            }
            sr.Close();
            updateTestFile(str2[1]);
        }

        private void toolStripMenuItem18_Click(object sender, EventArgs e)
        {
            MessageBox.Show("\t由于本系统操作文档还未出版,\n要想对系统进行熟练操作，请尽快与开发人员(高建超)进行联系!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void toolStripMenuItem19_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4();
            f4.Show();
        }

    }
}

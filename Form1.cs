using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace fhfx
{
    public partial class 登陆 : Form
    {
        public static string msg="";
        public static bool isContinue=true;
        public 登陆()
        {
            InitializeComponent();
        }
        private int waitNum = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            if (msg.Equals(""))
                msg = "正在进行初始化，请稍后......";
            this.label3.Text = msg;
            Application.DoEvents();//处理UI线程中排队的事件,没有这个,父进程可能会死掉
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isContinue && waitNum < 90)
            {
                Application.DoEvents();
                Random random = new Random(Guid.NewGuid().GetHashCode());//随机种子
                int num = random.Next(0,10);
                waitNum=waitNum+num;
                this.label4.Text = "初始化："+waitNum + "%";
            }
            else
            {
                Application.DoEvents();
                if (!isContinue)
                    this.DialogResult = DialogResult.OK;
                Thread thread = new Thread(new ThreadStart(delegate { Application.Run(new Form2()); }));//使用新线程进行委托
                thread.TrySetApartmentState(ApartmentState.STA);//设置线程属性为STA模式,没有这个会报错
                thread.Start();
                this.Dispose(true);
            }
        }

    }
}

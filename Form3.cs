using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace fhfx
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text == "" && this.textBox2.Text == "" && this.textBox3.Text == "")
                MessageBox.Show("请输入设置信息", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                Form2 f2 = new Form2();
                f2 = (Form2)this.Owner;
                if (this.textBox1.Text != "")
                {
                    f2.label20.Text = this.textBox1.Text;
                    this.label2.Text = "设置成功";
                    this.label2.ForeColor = Color.Green;
                }
                if (this.textBox2.Text != "")
                {
                    f2.label22.Text = this.textBox2.Text;
                    this.label5.Text = "设置成功";
                    this.label5.ForeColor = Color.Green;
                }
                if (this.textBox3.Text != "")
                {
                    f2.label26.Text = this.textBox3.Text;
                    this.label6.Text = "设置成功";
                    this.label6.ForeColor = Color.Green;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = textBox2.Text = textBox3.Text = "";
            label2.Text = label5.Text = label6.Text = "未设置";
            label2.ForeColor = label5.ForeColor = label6.ForeColor = Color.Red;
        }
    }
}

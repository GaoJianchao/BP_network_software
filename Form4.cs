﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace fhfx
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
            this.label11.Text = System.Environment.MachineName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

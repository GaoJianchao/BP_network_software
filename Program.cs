﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace fhfx
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool isRuned;
            System.Threading.Mutex mutex = new System.Threading.Mutex(true, "OnlyRunOneInstance", out isRuned);
            if (isRuned)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new 登陆());
                mutex.ReleaseMutex();
            }
            else
                MessageBox.Show("程序已启动", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

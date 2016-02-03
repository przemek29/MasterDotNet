using System;
using System.Windows.Forms;
using AutonomicznySystemNawigacyjny;

namespace Autonomiczny_Sytem_Nawigacyjny
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Control_weighing_By_Roman_07032017_0
{
    static class Program
    {

    
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
    static class zamis
    {
        public static string start { get; set; }
        public static string do_zamis { get; set; }
        public static string remainder { get; set; }
        public static string part_name { get; set; }
    }
}

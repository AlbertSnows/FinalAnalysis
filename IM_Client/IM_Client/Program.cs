using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IM_Client
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

            DataBase db = new DataBase();
            MainViewForm mvf = new MainViewForm(db);
            Controller c = new Controller(db, mvf);
            mvf.LinkInputHandler(c);
            mvf.ShowLogonScreen();
            
            Application.Run(mvf);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Control_weighing_By_Roman_07032017_0
{
    public class option
    {

        public string load_config_seting(string txt)
        {
            string r = "";
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                r = ConfigurationManager.AppSettings[txt];

                return r;
            }
            catch (ConfigurationErrorsException err)
            {
                return r;
            }

        }


        #region SetPortNameValues
        public void set_value(object obj)
        {
            if (obj is ComboBox)
            {
                load_config_seting(((ComboBox)obj).Name);
                ((ComboBox)obj).Items.Add(load_config_seting(((ComboBox)obj).Name));
            }
            if (obj is TextBox)
            {
                ((TextBox)obj).Text = load_config_seting(((TextBox)obj).Name);
            }
        }
        #endregion

        #region SetIPadress
        public void SetIPadress(object obj)
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ((ComboBox)obj).Items.Add(ip.ToString());
                }
            }
        }
        #endregion

        public void lod_option(Telerik.WinControls.UI.RadGridView recept_show)
        {

            String fullPath = System.Windows.Forms.Application.StartupPath.ToString() + "\\options\\show_data_grid.xml";
            recept_show.LoadLayout(fullPath);
        }


        #region змінити колір обєкту 
        public void set_color_red(object obj)
        {
            ((Button)obj).BackColor = Color.Red;
        }
        #endregion
    }
}

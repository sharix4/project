using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Control_weighing_By_Roman_07032017_0
{
    class show_all_error
    {
        public void bed_ip_or_port(string mesge)
        {
            if (mesge == "An invalid IP address was specified.")
            {
                const string message =
                 @"ІP адрес не виявлений або  не вказаний 
             1.Спробуйте налаштувати мережу правельно
             2.Заповніть поле в ручну";
                const string caption = "ІP адрес не виявлений";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (mesge == "Input string was not in a correct format.")
            {
                const string message =
             @"Порт вказано не правелно спробуйте ще раз";
                const string caption = "Порт не правильний";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        public void show_exeption(string mesge)
        {
                const string caption = "Помилка Помилка ";
                MessageBox.Show(mesge, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);      
        }
    }
}

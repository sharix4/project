using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Control_weighing_By_Roman_07032017_0
{
    class redWeight
    {
        TextBox real_wght;

        public redWeight(object weight)
        {
            real_wght = (TextBox)weight;  
        }

        public decimal red_weight()
        {
            try
            {
                string amount;
                decimal n;
                decimal number = decimal.MinValue;
                if (real_wght.IsDisposed == false)
                {
                    amount = real_wght.Text;

                    int idex_dot = amount.IndexOf(".");

                    if (idex_dot == -1)
                    {
                        number = decimal.MinValue;
                    }

                    amount = amount.Replace('.', ',');

                    bool isNumeric = decimal.TryParse(amount, out n);

                    if (isNumeric == true)
                    {
                        number = Convert.ToDecimal(amount);
                    }
                    else
                    {
                        number = decimal.MinValue;
                    }
                    return number;
                }
                return 0;
            }
            catch (Exception exp)
            {
                return -1;
            }
        }
       

    }
}

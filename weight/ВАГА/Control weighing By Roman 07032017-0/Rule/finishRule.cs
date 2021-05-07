using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Control_weighing_By_Roman_07032017_0
{
    class finishRule
    {
        private static EventWaitHandle stoper;
        private volatile bool _shouldStop;
        redWeight dataRead = null;
        Button tara = null;
        object ned_weight = null;

        public finishRule(EventWaitHandle ewh, object weight, object taraButon, object ned_weightBox)
        {
            dataRead = new redWeight(weight);
            stoper = ewh;
            tara = (Button)taraButon;
            ned_weight = ned_weightBox;
        }
        public void DoWork()
        {
            try
            {
                decimal number;
                while (!_shouldStop)
                {

                    Thread.Sleep(10);
                    number = dataRead.red_weight();
                    if ((number < 0) && (tara.BackColor == Color.Green))
                    {
                        Thread.Sleep(500);
                        number = dataRead.red_weight();
                        if ((number < 0) && (tara.BackColor == Color.Green))
                        {                    
                            set_color(ned_weight, Color.White);
                            _shouldStop = true;
                        }
                    }
                }

                stoper.Set();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }


        delegate void UpdateReceivebuton(object obj, Color clor_name);

        private void set_color(object obj, Color clor_name)
        {

            if (_shouldStop != true)
            {
                if (obj is Button)
                {
                    if (((Button)obj).InvokeRequired == true)
                    {
                        UpdateReceivebuton rdd = new UpdateReceivebuton(set_color);
                        ((Button)obj).Invoke(rdd, new object[] { obj, clor_name });
                    }
                    else
                    {
                        ((Button)obj).BackColor = clor_name;
                    }
                }

                if (obj is TextBox)
                {
                    if (((TextBox)obj).InvokeRequired == true)
                    {
                        UpdateReceivebuton rdd = new UpdateReceivebuton(set_color);
                        ((TextBox)obj).Invoke(rdd, new object[] { obj, clor_name });
                    }
                    else
                    {
                        ((TextBox)obj).BackColor = clor_name;
                    }
                }
            }
        }


        public void RequestStop()
        {
            _shouldStop = true;
            Thread.Sleep(10);
        }
    }
}

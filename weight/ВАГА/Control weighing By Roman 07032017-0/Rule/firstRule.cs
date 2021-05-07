using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Control_weighing_By_Roman_07032017_0
{
    class firstRule
    {
        private volatile bool _shouldStop;
        private static EventWaitHandle stoper;
        Button tara = null;
        redWeight dataRead = null;
    

        public firstRule(EventWaitHandle ewh,object taraButon,object weight)
        {
            tara = (Button)taraButon;
            dataRead = new redWeight(weight);
            stoper = ewh;
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
                    if ((number == 0) && (tara.BackColor == Color.Green))
                    {
                        Thread.Sleep(1000);
                        number = dataRead.red_weight();
                        if ((number == 0) && (tara.BackColor == Color.Green))
                        {
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

        public void RequestStop()
        {
            _shouldStop = true;
            Thread.Sleep(10);
        }
    }
}

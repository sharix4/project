using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace Control_weighing_By_Roman_07032017_0
{
    class comPortWorker
    {
        static System.IO.Ports.SerialPort myPort = null;

        TextBox showWeight = null;
        Button taraControl = null;


        public comPortWorker(SerialPort inputPortObj,object tara,object textbox)
        {
            showWeight= (TextBox)textbox;
            taraControl = (Button)tara;
            myPort = inputPortObj;      
        }

        private volatile bool _shouldStop;

        public void DoWork()
        {       
            try
            {
                string tString = string.Empty;


                Control_weighing_By_Roman_07032017_0.Controler.info_weight answer = new Control_weighing_By_Roman_07032017_0.Controler.info_weight();

                if (myPort.IsOpen == false)
                    myPort.Open();

                myPort.DiscardInBuffer();
                myPort.DiscardOutBuffer();



                int i = 0;
                while (!_shouldStop)
                {
                    if (myPort.IsOpen != true)
                    {
                        _shouldStop = true;
                    }

                    if (myPort.BytesToRead > 0)
                    {
                        byte[] buffer = new byte[1];

                        int bytesRead = myPort.Read(buffer, 0, buffer.Length);
          
                    //i++;
                    //if (i == 10)
                    //{
                    //    i = 0;
                    //}
                    tString += System.Text.Encoding.ASCII.GetString(new[] { buffer[0] });

                        if (buffer[0] == '>')
                        {
                            if (tString.Length == 10)
                            {
                                showComPortReadData(parcer(tString), taraControl, showWeight);
                                //d(parcer(tString));

                                tString = string.Empty;

                            }
                            else if (tString.Length < 10)
                            {
                                tString = string.Empty;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
               MessageBox.Show(e.Message);
            }
        }

        private Controler.info_weight parcer(string value)
        {
            try
            {
                Controler.info_weight answer = new Controler.info_weight();
                string TEST = value[2].ToString() + value[3].ToString() + value[4].ToString() + "." + value[5].ToString() + value[6].ToString() + value[7].ToString();
                answer.index_weight = TEST;
                answer.comand = Convert.ToByte(value[8]);
                return answer;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                return null;
            }

        }

        public void RequestStop()
        {
            _shouldStop = true;
            if (myPort != null)
            {
                myPort.Dispose();
                myPort = null;
            }
        }

        public void showComPortReadData(Control_weighing_By_Roman_07032017_0.Controler.info_weight param,Button tara,TextBox weight)
        {
            if (_shouldStop != true)
            {
                var bits = new BitArray(new byte[] { param.comand });
                if (bits[4] == true)
                {
                    set_color(tara, Color.Green);
                }
                else
                {
                    set_color(tara, Color.White);
                }

                if (bits[2] == false)
                {
                    update_weight(weight, param.index_weight.Replace(".", " "));
                }
                else
                {
                    update_weight(weight, param.index_weight);
                }
            }
        }


        delegate void UpdateReceivebuton(object obj, Color clor_name);

        private void set_color(object obj, Color clor_name)
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

        delegate void UpdateWght(object obj,string data);

        private void update_weight(object obj, string data)
        {
     
                if (((TextBox)obj).InvokeRequired == true)
                {
                    UpdateWght rdd = new UpdateWght(update_weight);
                    ((TextBox)obj).Invoke(rdd, new object[] { obj, data });
                }
                else
                {
                    ((TextBox)obj).Text = data;
                }
        }
    }
}

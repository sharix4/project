using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Telerik.WinControls.UI;

namespace Control_weighing_By_Roman_07032017_0
{
    class secondRule
    {


        private volatile bool _shouldStop;
        private static EventWaitHandle stoper;
        object grid =null;
        DataTable table_w = null;
        redWeight dataRead = null;
        Button tara = null;
        object ned_weight = null;
        DateTime start ;
        object text = null;

        public secondRule(EventWaitHandle ewh, object receptGrid, DataTable table, object weight, object taraButon,object ned_weightBox,DateTime StartTime,object name)
        {
            grid = receptGrid;
            stoper = ewh;
            table_w = table;
            dataRead = new redWeight(weight);
            tara = (Button)taraButon;
            ned_weight = ned_weightBox;
            start = StartTime;
            text = name;
        }


        decimal weight_sawe = 0;

        public void DoWork()
        {
            try
            {
                bool wait = true;

                if (_shouldStop == true)
                {
                    wait = false;
                }

                set_color(ned_weight, Color.White);



                decimal number = decimal.MinValue;

                for (int iz = ((RadGridView)grid).Rows.Count - 1; iz >= 0; iz--)
                {
                    if (_shouldStop == true)
                    {
                        wait = false;
                        break; 
                    }


                
                        set_select(iz, (RadGridView)grid);

                        name_set(((RadGridView)grid).Rows[iz].Cells[1].Value.ToString(), text);

                        ned_weight_set(Convert.ToDecimal(((RadGridView)grid).Rows[iz].Cells[4].Value), ned_weight);
                        decimal value_compare = Convert.ToDecimal(((RadGridView)grid).Rows[iz].Cells[4].Value);
                        if (_shouldStop == true)
                        {
                        }

                        DataRow row = table_w.NewRow();

                        DateTime start = DateTime.Now;
    

                        row["date_work"] = DateTime.Now;
                        row["name"] = ((RadGridView)grid).Rows[iz].Cells[1].Value;
                        row["ID_name"] = ((RadGridView)grid).Rows[iz].Cells[0].Value;
    
                    if (_shouldStop == true)
                    {
                        wait = false;
                    }
                    else
                    {
                        wait = true;
                    }

                    while (wait)
                    {

                        if (_shouldStop == true)
                        {
                            wait = false;
                        }

                        Thread.Sleep(10);
                        number = dataRead.red_weight();


                        if ((number == value_compare) && (tara.BackColor == Color.Green))
                        {
                            Thread.Sleep(1700);

                            number = dataRead.red_weight();
                            if ((number == value_compare) && (tara.BackColor == Color.Green))
                            {
                                Thread.Sleep(0);

                                row["amount_real"] = number - weight_sawe;
                                weight_sawe = number;

                                DateTime stop_time = DateTime.Now;

                                stop_time = DateTime.Now;

                                row["amount"] = number;

                                row["time_weighting"] = subDateTime(start, stop_time);

                                Thread.Sleep(0);

                                table_w.Rows.Add(row.ItemArray);


                                wait = false;

                            }
                        }
                    }

                }

                set_color(ned_weight, Color.LightSkyBlue);
                stoper.Set();
            }
            catch(Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
      
        }

        delegate void UpdateReceiveDisplayDelegate1(int message, object recept_show);
        private void set_select(int value, object recept_show)
        {
            if (_shouldStop != true)
            {
                if (((RadGridView)recept_show).InvokeRequired == true)
                {
                    UpdateReceiveDisplayDelegate1 rdd = new UpdateReceiveDisplayDelegate1(set_select);
                    ((RadGridView)recept_show).Invoke(rdd, new object[] { value, recept_show });
                }
                else
                {
                    ((RadGridView)recept_show).Rows[value].IsSelected = true;
                    ((RadGridView)recept_show).Rows[value].IsCurrent = true;
                }
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

        delegate void UpdateReceiveDisplayDelegate(string message,object box);
        private void name_set(string value, object box)
        {
            if (_shouldStop != true)
            {
                if (((TextBox)box).InvokeRequired == true)
                {
                    UpdateReceiveDisplayDelegate rdd = new UpdateReceiveDisplayDelegate(name_set);
                    ((TextBox)box).Invoke(rdd, new object[] { value, box });
                }
                else
                {
                    ((TextBox)box).Text = value;
                }
            }
        }

        delegate void UpdateReceiveDisplayDelegate_D(decimal message,object indecator);
        private void ned_weight_set(decimal value, object indecator)
        {
            if (_shouldStop != true)
            {
                if (((TextBox)indecator).InvokeRequired == true)
                {
                    UpdateReceiveDisplayDelegate_D rdd = new UpdateReceiveDisplayDelegate_D(ned_weight_set);
                    ((TextBox)indecator).Invoke(rdd, new object[] { value, indecator });
                }
                else
                {
                    ((TextBox)indecator).Text = value.ToString();
                }
            }

        }

        public TimeSpan subDateTime(DateTime start, DateTime stop)
        {
            TimeSpan saveTime = stop.Subtract(start);

            if (saveTime.Days > 0)
            {
                saveTime = new TimeSpan(0, 22, 99, 99);
            }
            return saveTime;
        }

        public void RequestStop()
        {
            _shouldStop = true;
            Thread.Sleep(19);
        }
    }
}

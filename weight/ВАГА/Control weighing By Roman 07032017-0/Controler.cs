using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Collections;
using System.Threading;
using Telerik.WinControls.UI;
using System.Text.RegularExpressions;

namespace Control_weighing_By_Roman_07032017_0
{
         
    public partial class Controler : Form
    {
         static System.IO.Ports.SerialPort myPort =null;

        DataSet recept = null;
        option lod_all_data = new option();



        string Com_name = string.Empty;
        int sped_com = 0;
        //private string tString = string.Empty;
        private static EventWaitHandle ewh;

        //public static bool stop_comand = false;

        Thread com_port_thred = null;
        Thread main_control_thred = null;
        Thread twoo_thred = null;
        Thread ferst_thred = null;
        Thread three_thred = null;

        public Controler(DataSet value, string Com_name, int sped_com)
        {


            this.recept = value.Copy();
            this.Com_name = Com_name;
            this.sped_com = sped_com;

            value.Clear();

            InitializeComponent();

            start_amount.Text = recept.Tables["send_info_from_weight"].Rows[0]["amount"].ToString();

            this.recept_show.CellFormatting += new CellFormattingEventHandler(radGridView1_CellFormatting);

            myPort = new System.IO.Ports.SerialPort(Com_name);
            myPort.BaudRate = sped_com;

            analizator_weight += work;
        }

        comPortWorker workerObject = null;

        private void Controler_Load(object sender, EventArgs e)
        {
            lod_all_data.lod_option(recept_show);

            recept_show.DataSource = recept.Tables["Calc_result"].Copy();

            this.recept_show.AutoSizeRows = true;
            workerObject = new comPortWorker(myPort, tara, real_wght);

            com_port_thred = new Thread(workerObject.DoWork);
            com_port_thred.IsBackground = true;
            com_port_thred.Start();

            Thread.Sleep(0);
            if (analizator_weight != null)
                analizator_weight(null, EventArgs.Empty);
        }


        #region збільшив Шрифт
        void radGridView1_CellFormatting(object sender, CellFormattingEventArgs e)
        {

            if (e.CellElement.ColumnInfo is GridViewDataColumn)
            {
                Font f = new Font(new FontFamily("Microsoft Sans Serif"), 30f);
                e.CellElement.Font = f;
            }
        }
        #endregion

        public event EventHandler analizator_weight;

        void work(object sender, EventArgs e)
        {
            Thread.Sleep(2);
            if (this.IsDisposed != true)
            {
                main_control_thred = new Thread(main_controler);
                main_control_thred.IsBackground = true;
                main_control_thred.Start();
            }
        }


        DataTable table_w = null;


        firstRule workerFirst = null;
        secondRule workerTwoo = null;
        finishRule workerfinish = null;
        bool stoper = false;

        private void main_controler()
        {
            try
            {
                zamis.do_zamis = do_amount.Text;
                zamis.start = start_amount.Text;
                zamis.remainder = ned_amount.Text;

                zamis.part_name = recept.Tables["send_info_from_weight"].Rows[0]["part_name"].ToString();

                DataSet insert_data = new DataSet();

                DataTable_create table = new DataTable_create();

                table_w = table.weighting();

                DataTable table_how_much = table.how_much();
                DataRow row = table_how_much.NewRow();


                DataTable data = (DataTable)recept_show.DataSource;

                mesage("1. Головий потік Стартанув");


                ewh = new EventWaitHandle(false, EventResetMode.AutoReset);

                workerFirst = new firstRule(ewh, tara, real_wght);

                ferst_thred = new Thread(workerFirst.DoWork);
                ferst_thred.IsBackground = true;

                ferst_thred.Start();

                ewh.WaitOne();


                mesage("Головний потік дочекався виконання 1 правила ");

                DateTime start = DateTime.Now;

                row["name_rec"] = recept.Tables["send_info_from_weight"].Rows[0]["name_rec"];
                row["ID_rec"] = recept.Tables["send_info_from_weight"].Rows[0]["ID_rec"];
                row["part_name"] = recept.Tables["send_info_from_weight"].Rows[0]["part_name"];
                row["ID_part"] = recept.Tables["send_info_from_weight"].Rows[0]["ID_part"];
                row["note"] = recept.Tables["send_info_from_weight"].Rows[0]["note"];
                row["date_work"] = DateTime.Now;


                ewh.Reset();
                if (stoper != true)
                {
                    return;
                }
                workerTwoo = new secondRule(ewh, recept_show, table_w, real_wght, tara, ned_weight, start, name);
                twoo_thred = new Thread(workerTwoo.DoWork);
                twoo_thred.IsBackground = true;
                twoo_thred.Start();

             
                ewh.WaitOne();

                insert_data.Tables.Add(table_w);

                ewh.Reset();

                mesage("Головний потік дочекався виконання 2 правила ");

                if (stoper != true)
                {
                    return;
                }

                workerfinish = new finishRule(ewh, real_wght, tara, ned_weight);
                three_thred = new Thread(workerfinish.DoWork);
                three_thred.IsBackground = true;
                three_thred.Start();


                ewh.WaitOne();

                DateTime stop_time = DateTime.Now;

                object sumObject;

                sumObject = table_w.Compute("Sum(amount_real)", "");

                row["amount"] = Convert.ToDecimal(sumObject);
                row["time_work"] = subDateTime(start, stop_time);


                table_how_much.Rows.Add(row.ItemArray);
                insert_data.Tables.Add(table_how_much);


                bool save_answer = save_data(insert_data);

                if (save_answer == true)
                {
                    if (stoper != true)
                    {
                        return;
                    }
                    if (stoper != true)
                    {


                        insert_data.Clear();
                        table_w.Clear();
                        table_how_much.Clear();


                        increment_data(do_amount, 1);

                        decrement_data(ned_amount);

                        zamis.do_zamis = do_amount.Text;
                        zamis.start = start_amount.Text;
                        zamis.remainder = ned_amount.Text;

                        zamis.part_name = recept.Tables["send_info_from_weight"].Rows[0]["part_name"].ToString();

                        if (Convert.ToInt32(ned_amount.Text) != 0)
                        {
                            if (Convert.ToInt32(ned_amount.Text) < 3)
                            {
                                set_color(ned_amount, Color.PaleVioletRed);
                            }


                            ewh.Reset();

                            mesage("Головний потік дочекався виконання 3 правила ");

                            if (stoper != true)
                            {

                                if (analizator_weight != null)
                                    analizator_weight(null, EventArgs.Empty);
                                mesage("Головний потік викликав  знову себе ");
                            }

                        }
                        else
                        {
                            DataTable null_table = (DataTable)recept_show.DataSource;

                            null_table.Clear();

                            recept_show.DataSource = null_table;

                            end_work();
                        }
                    }
                }
                else
                {
                    mesage("Дані не записалися в базу");
                    end_work();
                }

            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }



        #region Всі виводи на екран
        ////-----------------------------------ВИВІД НА Екран -----------------------------//

        delegate void UpdateReceivebuton(object obj, Color clor_name);

        private void set_color(object obj, Color clor_name)
        {
            if (stoper != true)
            {
                if (obj is Button)
                {
                    if (((Button)obj).InvokeRequired == true)
                    {
                        UpdateReceivebuton rdd = new UpdateReceivebuton(set_color);
                        Invoke(rdd, new object[] { obj, clor_name });
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
                        Invoke(rdd, new object[] { obj, clor_name });
                    }
                    else
                    {
                        ((TextBox)obj).BackColor = clor_name;
                    }
                }
            }
        }


        delegate void increment(object obj, int value_increment);

        private void increment_data(object obj, int value_increment)
        {
            if (stoper != true)
            {
                if (obj is TextBox)
                {
                    if (((TextBox)obj).InvokeRequired == true)
                    {
                        increment rdd = new increment(increment_data);
                        Invoke(rdd, new object[] { obj, value_increment });
                    }
                    else
                    {
                        ((TextBox)obj).Text = (Convert.ToInt32(((TextBox)obj).Text) + value_increment).ToString();
                    }
                }
            }
        }

        delegate void decrement(object obj);

        private void decrement_data(object obj)
        {
            if (stoper != true)
            {
                if (obj is TextBox)
                {
                    if (((TextBox)obj).InvokeRequired == true)
                    {
                        decrement rdd = new decrement(decrement_data);
                        Invoke(rdd, new object[] { obj });
                    }
                    else
                    {

                        ((TextBox)obj).Text = Convert.ToString(Convert.ToInt32(start_amount.Text) - Convert.ToInt32(do_amount.Text));
                    }
                }
            }
        }

      

        delegate void Updatemesage(string message);
        public void mesage(string message)
        {
            if (stoper != true)
            {
                if (show_server_log.InvokeRequired == true)
                {
                    Updatemesage rdd = new Updatemesage(mesage);
                    Invoke(rdd, new object[] { message });

                }
                else
                {
                    show_server_log.Items.Add((show_server_log.Items.Count + 1).ToString() + "." + message);
                    show_server_log.SelectedIndex = show_server_log.Items.Count - 1;
                }
            }
        }
        #endregion

        public bool save_data(DataSet value)
        {
            try
            {
                DB object_in_select = new DB();
                DataTable answer = object_in_select.select_new("Записати дані в базу", value.Tables["how_much"]);

                if (answer != null)
                {
                    DataTable insert_table = value.Tables["weighting"].Copy();
                    insert_table.Clear();

                    int id = Convert.ToInt32(answer.Rows[0]["ID"].ToString());

                    foreach (DataRow rows in value.Tables["weighting"].Rows)
                    {
                        rows["ID_calc"] = id;
                        insert_table.Rows.Add(rows.ItemArray);

                        DataTable answer_weight = object_in_select.select_new("рецепт в базу", insert_table);

                        if (answer_weight == null)
                        {
                            mesage("помилка запису");
                            return false;

                        }

                        insert_table.Clear();
                    }
                    return true;

                }
                return false;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                return false;
            }
        }


        public class info_weight
        {
            public string index_weight;

            public byte comand;
        }

        internal void end_work()
        {
            stoper = true;
            analizator_weight -= work;

            Thread.Sleep(10);

            if (workerfinish!=null)
            workerfinish.RequestStop();

            if (workerTwoo != null)
                workerTwoo.RequestStop();
            Thread.Sleep(10);

            Thread.Sleep(10);
            if (workerFirst != null)
            workerFirst.RequestStop();

            Thread.Sleep(10);
            if (workerObject != null)
            workerObject.RequestStop();
            Thread.Sleep(10);
            if (myPort==null)
            {
            myPort.Dispose();
            myPort = null;
            }
            Thread.Sleep(110);
          Invoke(new EventHandler(delegate{this.Close();}));    
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

        private void Controler_FormClosing(object sender, FormClosingEventArgs e)
        {
            //end_work();
        }
    } 
}

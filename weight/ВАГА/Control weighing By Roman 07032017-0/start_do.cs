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
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace Control_weighing_By_Roman_07032017_0
{
    public partial class Form1 : Form
    {
        show_all_error mesage = new show_all_error();
   
        Controler form2 = null;

        public System.Timers.Timer aTimer;
        static System.IO.Ports.SerialPort myPort = null;

        int server_port = 0;
        IPAddress ipaddress = null;
        static bool bloc = false;


        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            set_all_option();
            SetTimer();
        }

        #region запускаю таймер 
        public void SetTimer()
        {    
            aTimer = new System.Timers.Timer(10000);     
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }
        #endregion


        #region  Стартові налаштування
        public void set_all_option()
        {
            option set_data = new option();

            set_data.set_value(port);
            set_data.set_value(cboPort);
            set_data.set_value(sped);
            set_data.set_value(ip);
            set_data.set_value(bd_string);

            set_data.set_color_red(bd_status);
            set_data.set_color_red(com_test);
            set_data.set_color_red(bd_status);


            if (ip.Items.Count >= 0)
            {
                ip.SelectedIndex = 0;
            }
            if (cboPort.Items.Count > 0)
            {
                cboPort.SelectedIndex = 0;
            }
        }
        #endregion

        #region  Перевірка зєднання  Перевірка бази Перевірка  ваги

        public void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (status_server.BackColor == Color.Green && com_test.BackColor == Color.Green && bd_status.BackColor == Color.Green)
            {
                aTimer.Enabled = false;
            }
            else
            {
                if (status_server.BackColor != Color.Green)
                {
                    start_server();
                }
                if (bd_status.BackColor != Color.Green)
                {
                    start_test_bd();
                }
                if (com_test.BackColor != Color.Green)
                {
                    start_test_com();
                }
                   
            }

        }

        public bool running = true;
        public TcpListener server;

        public X509Certificate2 cert = new X509Certificate2("server.pfx", "instant");

        public void start_server()
        {
            try
            {
                if (bloc == false)
                {
                    bloc = true;

                    ip.Invoke(new EventHandler(delegate { ipaddress = IPAddress.Parse(ip.Text.ToString()); }));
                    ip.Invoke(new EventHandler(delegate { server_port = Convert.ToInt32(port.Text); }));


                    if (ipaddress != null && server_port != 0)
                    {
                      
                        status_server.BackColor = Color.Green;
                    }
                    else
                    {
                        status_server.BackColor = Color.Red;

                    }

                    server = new TcpListener(ipaddress, server_port);
                    server.Start();
                    Listen();
                }
            }
            catch (Exception e)
            {
                mesage.bed_ip_or_port(e.Message);
            }
        }

        void Listen()  // Listen to incoming connections.
        {
            while (running)
            {
                Thread.Sleep(5);
                TcpClient tcpClient = server.AcceptTcpClient();  // Accept incoming connection.
                control client = new control(this, tcpClient);     // Handle in another thread.
            }
        }

        public void start_test_bd()
        {
            try
            {
                DB object_in_select = new DB();
                DataTable answer = object_in_select.select_new("Перевірити зєднання", null);
                if (answer != null)
                {
                    bd_status.BackColor = Color.Green;
                }
            }
            catch
            {
                bd_status.BackColor = Color.Red;
            }
        }

        public void start_test_com()
        {
            try
            {
                if (myPort == null)
                {
                    string com_name = string.Empty;
                    int spead = 0;

                    cboPort.Invoke(new EventHandler(delegate { com_name = cboPort.Text; }));

                    myPort = new System.IO.Ports.SerialPort(com_name);

                    sped.Invoke(new EventHandler(delegate { spead = Convert.ToInt32(sped.Text); }));
                    myPort.BaudRate = Convert.ToInt32(spead);
                }



                if (myPort.IsOpen == false)
                    myPort.Open();


                myPort.DiscardInBuffer();
                myPort.DiscardOutBuffer();
                Thread.Sleep(90);

                while (myPort.IsOpen)
                {
                    if (myPort.BytesToRead > 0)
                    {
                        byte[] inbyte = new byte[10];

                        myPort.Read(inbyte, 0, 10);

                        if (inbyte.Length > 0)
                        {
                            com_test.Invoke(new EventHandler(delegate
                            {
                                com_test.BackColor = Color.Green;
                            }));

                        }
                    }
                    else
                    {
                        myPort.DiscardInBuffer();
                        myPort.DiscardOutBuffer();
                        myPort.Close();
                        myPort.Dispose();

                    }
                    myPort.DiscardInBuffer();
                    myPort.DiscardOutBuffer();
                    myPort.Dispose();
                }
                
            }
            catch (Exception e)
            {
                com_test.BackColor = Color.Red;

            }
        }

        #endregion


        delegate void UpdateReceiveDisplayDelegate(string message);
        public void ShowReceiveMessage(string message)
        {
            if (show_server_log.InvokeRequired == true)
            {
                UpdateReceiveDisplayDelegate rdd = new UpdateReceiveDisplayDelegate(ShowReceiveMessage);
                Invoke(rdd, new object[] { message });
            }
            else
            {
                show_server_log.Items.Add((show_server_log.Items.Count + 1).ToString() + ". " + message);
            }
        }

        static bool controler_work = false;
  
        public DataSet synchronization(DataSet value)
        {
            try
            {
                DB object_in_select = new DB();
                DataSet synchronization_data = new DataSet();
                if (value.Tables["how_much"].Rows.Count == 1 || value.Tables["how_much"].Rows.Count == 0)
                {
                    DataTable how_much = object_in_select.select_new("get_how_much", value.Tables["how_much"]);
                how_much.TableName = "how_much";
                synchronization_data.Tables.Add(how_much);
                }

                if (value.Tables["weighting"].Rows.Count == 1 || value.Tables["weighting"].Rows.Count == 0)
                {
                    DataTable weighting = object_in_select.select_new("get_weighting", value.Tables["weighting"]);
                    weighting.TableName = "weighting";
                    synchronization_data.Tables.Add(weighting);
                }
                synchronization_data.DataSetName = "Синхронізація завершена";
                return synchronization_data;
            }
            catch
            {
                return null;
            }
        }


        internal object do_work(string p, DataSet value)
        {
            DataSet answer_value = new DataSet();

            switch (p)
            {
                case "синхронізація":

                    try
                    {
                        return synchronization(value);
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.Message);
                        return null;
                    }

                case "Рецепт":
                    string mesage_from_answer = string.Empty;

                    if (status_server.BackColor != Color.Green)
                    {
                        mesage_from_answer = "Помилка";
                    }
                    if (com_test.BackColor != Color.Green)
                    {
                        mesage_from_answer = "Помилка";
                    }
                    if (bd_status.BackColor != Color.Green)
                    {
                        mesage_from_answer = "Помилка";
                    }
                    if (value == null)
                    {
                        mesage_from_answer = "Помилка";
                    }

                    if (mesage_from_answer == string.Empty)
                    {
                        if (controler_work == true)
                        {
                            mesage_from_answer = "Рецепт в роботі";
                        }
                        else
                        {

                            controler_work = open_do_form(value);
                            if (controler_work == true)
                            {
                                mesage_from_answer = "Отримав Рецепт";
                            }
                            else
                            {
                                mesage_from_answer = "Помилка";
                            }
                        }
                    }
                    answer_value.DataSetName = mesage_from_answer;
                    return answer_value;

                case "Заміна рецепту":

                    
                    controler_work = close_do_form();
                    Thread.Sleep(100);

                    controler_work = open_do_form(value);

                    mesage_from_answer = "Отримав Рецепт";

                    answer_value.DataSetName = mesage_from_answer;
                    return answer_value;


                case "Зважування":
                    try
                    {
                        DataTable_create table = new DataTable_create();
                        DataTable real_data = table.number();
                        DataRow row = real_data.NewRow();

                        row["start"] = zamis.start;
                        row["do_zamis"] = zamis.do_zamis;
                        row["remainder"] = zamis.remainder;
                        row["part_name"] = zamis.part_name;

                        real_data.Rows.Add(row.ItemArray);

                        answer_value.Tables.Add(real_data);

                        mesage_from_answer = "дані зважування";

                        answer_value.DataSetName = mesage_from_answer;
                        return answer_value;

                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.Message);
                        return null;
                    }
                default:
                    return null;
            }
        }

        public bool open_do_form(DataSet value)
        {
            try
            {
                if (InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        if (form2 != null)
                        {
                            Thread.Sleep(40);
                            if (form2.IsDisposed)
                            {
                                form2 = new Controler(value, cboPort.Text, Convert.ToInt32(sped.Text));

                                form2.Show();

                                controler_work = true;
                            }
                        }
                        else
                        {
                            form2 = new Controler(value, cboPort.Text, Convert.ToInt32(sped.Text));

                            form2.Show();

                            controler_work = true;
                        }
                    }));

                }
                return true;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                return false;
            }
        }

        public bool close_do_form()
        {
            try
            {

                //form2.Close();
                if (form2.IsDisposed==false)
                {
                    //form2.Invoke(new MethodInvoker(delegate { form2.Close(); }));
                    form2.end_work();
                    Thread.Sleep(4000);
                }

                return false;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                return true;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            running = false;
            System.Windows.Forms.Application.Exit();
        }
    }
}

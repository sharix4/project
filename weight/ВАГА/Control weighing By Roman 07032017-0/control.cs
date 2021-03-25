using serilizet;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Control_weighing_By_Roman_07032017_0
{
    class control
    {

        Form1 prog;

        exchange server_side;

        public SslStream ssl;    


        public TcpClient client;
        public NetworkStream netStream;
        public BinaryReader br;
        public BinaryWriter bw;

        public control(Form1 p, TcpClient c)
        {
            prog = p;
         
            client = c;

            server_side = new exchange(c);

            // Handle client in another thread.
            (new Thread(new ThreadStart(SetupConn))).Start();
        }

        #region start
        void SetupConn()  // Setup connection and login or register.
        {
            try
            {
                netStream = client.GetStream();

                ssl = new SslStream(netStream, false);
                ssl.AuthenticateAsServer(prog.cert, false, SslProtocols.Tls, true);

     

                br = new BinaryReader(ssl, Encoding.UTF8);
                bw = new BinaryWriter(ssl, Encoding.UTF8);


                bw.Write("Я сервер ваги Привіт");
                bw.Flush();


                string hello = br.ReadString();
                if (hello == "Привіт я Клієнт")
                {
                    Receiver();
                }

                CloseConn();

            }
            catch (Exception ep)
            {
               CloseConn();
            }
        }
        #endregion

        #region end
        void CloseConn() // Close connection.
        {
            try
            {
                br.Close();
                bw.Close();
                ssl.Close();
                netStream.Close();
                client.Close();
            }
            catch { }
        }
        #endregion

        void Receiver()  
        {

            values_for_the_base from_server_value;
            values_for_the_base send_strim = new values_for_the_base();

            try
            {
                while (client.Client.Connected)  
                {
                    string type = string.Empty;

                    string read_or_not = br.ReadString();

                    if (read_or_not == "Я_без_грузу")
                    {
                        type = br.ReadString();
                        var a = prog.do_work(type.ToString(),null);
                        send_strim.dataset = (DataSet)a;
                        server_side.send(send_strim);

                  
                    }
                    else
                    {

                        from_server_value = server_side.read();
                        var a = prog.do_work(from_server_value.dataset.DataSetName.ToString(), from_server_value.dataset);
                        send_strim.dataset = (DataSet)a;
                        server_side.send(send_strim);
                    }

                }
            }
            catch (Exception ep)
            {
                CloseConn();
            }
        }//end resiver
  
    }
    public class SimpleAES
    {
        // Change these keys
        private byte[] Key = { 13, 21, 11, 154, 64, 24, 15, 45, 14, 18, 127, 16, 237, 2, 222, 219, 221, 214, 17, 145, 111, 83, 6, 129, 224, 126, 1, 2, 3, 4, 53, 29 };
        private byte[] Vector = { 46, 164, 91, 131, 123, 113, 1, 2, 3, 4, 51, 12, 49, 132, 18, 16 };


        private ICryptoTransform EncryptorTransform, DecryptorTransform;

        public SimpleAES()
        {
            //This is our encryption method
            RijndaelManaged rm = new RijndaelManaged();

            //Create an encryptor and a decryptor using our encryption method, key, and vector.
            EncryptorTransform = rm.CreateEncryptor(this.Key, this.Vector);
            DecryptorTransform = rm.CreateDecryptor(this.Key, this.Vector);

        }

        /// Generates an encryption key.
        static public byte[] GenerateEncryptionKey()
        {
            //Generate a Key.
            RijndaelManaged rm = new RijndaelManaged();
            rm.GenerateKey();
            return rm.Key;
        }

        /// Generates a unique encryption vector
        static public byte[] GenerateEncryptionVector()
        {
            //Generate a Vector
            RijndaelManaged rm = new RijndaelManaged();
            rm.GenerateIV();
            return rm.IV;
        }

        /// Encrypt some text and return an encrypted byte array.
        public byte[] Encrypt(Byte[] bytes)
        {

            //Used to stream the data in and out of the CryptoStream.
            MemoryStream memoryStream = new MemoryStream();

            /*
             * We will have to write the unencrypted bytes to the stream,
             * then read the encrypted result back from the stream.
             */
            #region Write the decrypted value to the encryption stream
            CryptoStream cs = new CryptoStream(memoryStream, EncryptorTransform, CryptoStreamMode.Write);
            cs.Write(bytes, 0, bytes.Length);
            cs.FlushFinalBlock();
            #endregion

            #region Read encrypted value back out of the stream
            memoryStream.Position = 0;
            byte[] encrypted = new byte[memoryStream.Length];
            memoryStream.Read(encrypted, 0, encrypted.Length);
            #endregion

            //Clean up.
            cs.Close();
            memoryStream.Close();

            return encrypted;
        }

        /// Decryption when working with byte arrays.    
        public byte[] Decrypt(byte[] EncryptedValue)
        {


            #region Write the encrypted value to the decryption stream
            MemoryStream encryptedStream = new MemoryStream();
            CryptoStream decryptStream = new CryptoStream(encryptedStream, DecryptorTransform, CryptoStreamMode.Write);
            decryptStream.Write(EncryptedValue, 0, EncryptedValue.Length);
            decryptStream.FlushFinalBlock();
            #endregion

            #region Read the decrypted value from the stream.
            encryptedStream.Position = 0;
            Byte[] decryptedBytes = new Byte[encryptedStream.Length];
            encryptedStream.Read(decryptedBytes, 0, decryptedBytes.Length);
            encryptedStream.Close();
            #endregion

            return decryptedBytes;
        }

    }
    class exchange
    {
        private TcpClient my_client;

        SimpleAES my_AES = new SimpleAES();

        public exchange(TcpClient client)
        {
            this.my_client = client;
        }

        public values_for_the_base read()
        {

            values_for_the_base p = null;
            try
            {
                NetworkStream stream = this.my_client.GetStream();
                byte[] bsize = new byte[4];
                if (stream.Read(bsize, 0, 4) != 4)
                {
                    throw new IndexOutOfRangeException();
                }
                int size = BitConverter.ToInt32(bsize, 0);

                byte[] bytes = new byte[size];

                int total_read = 0;
                var bytes_read = 0;
                //this.my_client.ReceiveTimeout = 2000;
                do
                {
                    try
                    {
                        bytes_read = stream.Read(bytes, total_read, size - total_read);
                        total_read += bytes_read;
                    }
                    catch (IOException ex)
                    {
                        // if the ReceiveTimeout is reached an IOException will be raised...
                        // with an InnerException of type SocketException and ErrorCode 10060
                        var socketExept = ex.InnerException as SocketException;
                        if (socketExept == null || socketExept.ErrorCode != 10060)
                            // if it's not the "expected" exception, let's not hide the error
                            throw ex;
                        // if it is the receive timeout, then reading ended
                        bytes_read = 0;
                    }
                } while (bytes_read > 0 && total_read != size);


                byte[] decrypted = my_AES.Decrypt(bytes);

                MemoryStream memstrm = new MemoryStream();
                memstrm.Write(decrypted, 0, decrypted.Length);
                memstrm.Position = 0;

                BinaryFormatter formatter = new BinaryFormatter();
                p = (values_for_the_base)formatter.Deserialize(memstrm);

                stream.Flush();
                memstrm.Close();

            }
            catch (Exception air)
            {
                System.Console.WriteLine("===========Exception=============");
                System.Console.WriteLine(air.ToString());
                System.Console.WriteLine("=================================");
                FileStream file3 = new FileStream("exchange.txt", FileMode.Append);
                StreamWriter writer = new StreamWriter(file3);
                writer.WriteLine("\n\n*****************************************************");
                writer.WriteLine("DataTime:-->" + DateTime.Now.ToString());
                writer.WriteLine("function:-->read()");
                writer.WriteLine("===error====");
                writer.WriteLine(air.ToString());
                writer.WriteLine("===error====");
                writer.WriteLine("*****************************************************\n\n");
                writer.Close();

            }
            return p;
        }

        public bool send(values_for_the_base value)
        {
            try
            {

                #region serialize
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream memstrm = new MemoryStream();
                formatter.Serialize(memstrm, value);
                #endregion

                byte[] data = my_AES.Encrypt(memstrm.GetBuffer());

                NetworkStream stream = this.my_client.GetStream();
                int memsize = (int)data.Length;
                byte[] size = BitConverter.GetBytes(memsize);
                stream.Write(size, 0, 4);
                stream.Write(data, 0, (int)memsize);
                stream.Flush();

                memstrm.Close();
            }
            catch (Exception air)
            {
                System.Console.WriteLine("===========Exception=============");
                System.Console.WriteLine(air.ToString());
                System.Console.WriteLine("=================================");
                FileStream file3 = new FileStream("exchange.txt", FileMode.Append);
                StreamWriter writer = new StreamWriter(file3);
                writer.WriteLine("\n\n*****************************************************");
                writer.WriteLine("DataTime:-->" + DateTime.Now.ToString());
                writer.WriteLine("function:-->send()");
                writer.WriteLine("===error====");
                writer.WriteLine(air.ToString());
                writer.WriteLine("===error====");
                writer.WriteLine("*****************************************************\n\n");
                writer.Close();


                return false;
            }
            return true;
        }

    }
}
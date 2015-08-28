using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Net.Mail;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;


namespace MikroTikFinal
{
    class NetworkTool
    {
        private Ping pingSender = new Ping();

        public string getPing(string ip)
        {
            try
            {

                PingReply reply = pingSender.Send(ip, 500);
                if (reply.Status == IPStatus.Success)
                {
                    return "ping ok";
                }

                else
                {
                    return "no ping";
                }              
            }
            catch (Exception ee)
            {
                return "no ping";
               //return "IP adresa nije validna";
            }
        }
                     
    }

    class Telnet
    {
        private const int PORTNUM = 23;
        private const int TIMEOUTMS = 200;
        private TcpClient tcpClient = new TcpClient();
        private NetworkStream networkStream;

        enum Verbs
        {
            WILL = 251,
            WONT = 252,
            DO = 253,
            DONT = 254,
            IAC = 255
        }
        enum Options
        {
            SGA = 3
        }


        public string setTelnetConnection(string ip) 
        {
            try
            {
                this.tcpClient.Connect(ip, PORTNUM);
                this.networkStream = this.tcpClient.GetStream();
                if (this.tcpClient.Connected)
                {
                    return "Connected.";
                }
                else
                {
                    return "Not connected.";
                }               
            }
            catch (Exception e)
            {
                return "Problem with connection.";
            }           
        }

        public bool isConnected()
        {
            if (tcpClient.Connected)
            {
                return true;

            }
            else
            { 
                return false;
            }
        }

        public void resetTelnetConnection()
        {
            tcpClient.Close();
        }

        public bool loginTelnetConnection(string username, string password) 
        {
            return true;
        }

        // ova funkcija obavlja parsiranje stringa za citanje i upisivanje u telnet komunikaciju
        void ParseTelnet(StringBuilder sb)
        {
            while (tcpClient.Available > 0)
            {
                int input = tcpClient.GetStream().ReadByte();   // ucitavanje prvog podatka
                switch (input)
                {
                    case -1:
                        break;
                    case (int)Verbs.IAC:
                        // interpret as command
                        int inputverb = tcpClient.GetStream().ReadByte();
                        if (inputverb == -1) break;
                        switch (inputverb)
                        {
                            case (int)Verbs.IAC:
                                //literal IAC = 255 escaped, so append char 255 to string
                                sb.Append(inputverb);
                                break;
                            case (int)Verbs.DO:
                            case (int)Verbs.DONT:
                            case (int)Verbs.WILL:
                            case (int)Verbs.WONT:
                                // reply to all commands with "WONT", unless it is SGA (suppres go ahead)
                                int inputoption = tcpClient.GetStream().ReadByte();
                                if (inputoption == -1) break;
                                tcpClient.GetStream().WriteByte((byte)Verbs.IAC);
                                if (inputoption == (int)Options.SGA)
                                    tcpClient.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WILL : (byte)Verbs.DO);
                                else
                                    tcpClient.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WONT : (byte)Verbs.DONT);
                                tcpClient.GetStream().WriteByte((byte)inputoption);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        sb.Append((char)input);
                        break;
                }
            }
        }

        // ova funkcija edituje procitani string 
        public string Read()
        {
            if (!tcpClient.Connected) return null;
            StringBuilder sb = new StringBuilder();   // kreira objekat za izgradnju stringa
            do
            {
                ParseTelnet(sb);
                Thread.Sleep(TIMEOUTMS);
            } while (tcpClient.Available > 0);
            return sb.ToString();
        }

        // ispis stringa pracen sa escape character "\n"
        public void WriteLine(string cmd)
        {
            Write(cmd + "\n");
        }

        // ova funkcija edituje string za slanje komande
        public void Write(string cmd)
        {
            if (!tcpClient.Connected) return;
            byte[] buf = System.Text.ASCIIEncoding.ASCII.GetBytes(cmd.Replace("\0xFF", "\0xFF\0xFF"));
            networkStream.Write(buf, 0, buf.Length);
            networkStream.Flush();
        }

        // funkcija za logovanje
        public string Login(string Username, string Password)
        {
            string s = Read();
            if (!s.TrimEnd().EndsWith(":"))   //ako poslednji karakter nije :
            {
                /* Zbog stabilnosti aplikacije*/
                //throw new Exception("Failed to connect : no login prompt");
            }
            WriteLine(Username + "\r");

            s += Read();

            //if (!s.TrimEnd().EndsWith(":"))
                /* Zbog stabilnosti aplikacije*/
                //throw new Exception("Failed to connect : no password prompt");
            WriteLine(Password + "\r");

            Thread.Sleep(TIMEOUTMS);
            s += Read();
            return s;
        }        

        // funkcija za zatvaranje konekcije
        public void closeConnection() 
        {
            networkStream.Close();
            tcpClient.Close();
        }
    }

    class MikroTikApi 
    {
        private const int APINUM = 8728;
        private const int TIMEOUTMS = 200;
        private TcpClient tcpClient = new TcpClient();
        private NetworkStream networkStream;

        public string setMikroTikApiConnection(string ip)
        {
            try
            {
                this.tcpClient.Connect(ip, APINUM);
                this.networkStream = this.tcpClient.GetStream();
                if (this.tcpClient.Connected)
                {
                    return "Connected.";
                }
                else
                {
                    return "Not connected.";
                }
            }
            catch (Exception e)
            {
                return "Problem with connection.";
            }

        }

        // logovanje preko api porta
        public bool Login(string username, string password)
        {
            Send("/login", true);
            string hash = Read()[0].Split(new string[] { "ret=" }, StringSplitOptions.None)[1];
            Send("/login");
            Send("=name=" + username);
            Send("=response=00" + EncodePassword(password, hash), true);
            if (Read()[0] == "!done")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // slanje rijeci (pretpostavljam) preko api porta
        public void Send(string co)
        {
            byte[] bajty = Encoding.ASCII.GetBytes(co.ToCharArray());
            byte[] velikost = EncodeLength(bajty.Length);

            networkStream.Write(velikost, 0, velikost.Length);
            networkStream.Write(bajty, 0, bajty.Length);
        }

        // slanje rijeci (pretpostavljam) preko api porta koje ima zavrsni karakter
        public void Send(string co, bool endsentence)
        {
            byte[] bajty = Encoding.ASCII.GetBytes(co);
            byte[] velikost = EncodeLength(bajty.Length);
            networkStream.Write(velikost, 0, velikost.Length);
            networkStream.Write(bajty, 0, bajty.Length);
            networkStream.WriteByte(0);
        }

        // funkcija koja realizuje citanje povratne informacije
        public List<string> Read()
        {
            List<string> output = new List<string>();
            string o = "";
            byte[] tmp = new byte[4];
            long count;
            while (true)
            {
                tmp[3] = (byte)networkStream.ReadByte();
                //if(tmp[3] == 220) tmp[3] = (byte)connection.ReadByte(); it sometimes happend to me that 
                //mikrotik send 220 as some kind of "bonus" between words, this fixed things, not sure about it though
                if (tmp[3] == 0)
                {
                    output.Add(o);
                    if (o.Substring(0, 5) == "!done")
                    {
                        break;
                    }
                    else
                    {
                        o = "";
                        continue;
                    }
                }
                else
                {
                    if (tmp[3] < 0x80)
                    {
                        count = tmp[3];
                    }
                    else
                    {
                        if (tmp[3] < 0xC0)
                        {
                            int tmpi = BitConverter.ToInt32(new byte[] { (byte)networkStream.ReadByte(), tmp[3], 0, 0 }, 0);
                            count = tmpi ^ 0x8000;
                        }
                        else
                        {
                            if (tmp[3] < 0xE0)
                            {
                                tmp[2] = (byte)networkStream.ReadByte();
                                int tmpi = BitConverter.ToInt32(new byte[] { (byte)networkStream.ReadByte(), tmp[2], tmp[3], 0 }, 0);
                                count = tmpi ^ 0xC00000;
                            }
                            else
                            {
                                if (tmp[3] < 0xF0)
                                {
                                    tmp[2] = (byte)networkStream.ReadByte();
                                    tmp[1] = (byte)networkStream.ReadByte();
                                    int tmpi = BitConverter.ToInt32(new byte[] { (byte)networkStream.ReadByte(), tmp[1], tmp[2], tmp[3] }, 0);
                                    count = tmpi ^ 0xE0000000;
                                }
                                else
                                {
                                    if (tmp[3] == 0xF0)
                                    {
                                        tmp[3] = (byte)networkStream.ReadByte();
                                        tmp[2] = (byte)networkStream.ReadByte();
                                        tmp[1] = (byte)networkStream.ReadByte();
                                        tmp[0] = (byte)networkStream.ReadByte();
                                        count = BitConverter.ToInt32(tmp, 0);
                                    }
                                    else
                                    {
                                        //Error in packet reception, unknown length
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                for (int i = 0; i < count; i++)
                {
                    o += (Char)networkStream.ReadByte();
                }
            }
            return output;
        }

        // Ova funkcija obavlja encoding duzine komande (ovo je bitno)
        byte[] EncodeLength(int delka)
        {
            if (delka < 0x80)
            {
                byte[] tmp = BitConverter.GetBytes(delka);
                return new byte[1] { tmp[0] };
            }
            if (delka < 0x4000)
            {
                byte[] tmp = BitConverter.GetBytes(delka | 0x8000);
                return new byte[2] { tmp[1], tmp[0] };
            }
            if (delka < 0x200000)
            {
                byte[] tmp = BitConverter.GetBytes(delka | 0xC00000);
                return new byte[3] { tmp[2], tmp[1], tmp[0] };
            }
            if (delka < 0x10000000)
            {
                byte[] tmp = BitConverter.GetBytes(delka | 0xE0000000);
                return new byte[4] { tmp[3], tmp[2], tmp[1], tmp[0] };
            }
            else
            {
                byte[] tmp = BitConverter.GetBytes(delka);
                return new byte[5] { 0xF0, tmp[3], tmp[2], tmp[1], tmp[0] };
            }
        }

        // ova funkcija obavlja kodovanje sifre pomocu MD5 protokola
        public string EncodePassword(string Password, string hash)
        {
            byte[] hash_byte = new byte[hash.Length / 2];
            for (int i = 0; i <= hash.Length - 2; i += 2)
            {
                hash_byte[i / 2] = Byte.Parse(hash.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
            }
            byte[] heslo = new byte[1 + Password.Length + hash_byte.Length];
            heslo[0] = 0;
            Encoding.ASCII.GetBytes(Password.ToCharArray()).CopyTo(heslo, 1);
            hash_byte.CopyTo(heslo, 1 + Password.Length);

            Byte[] hotovo;
            System.Security.Cryptography.MD5 md5;

            md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            hotovo = md5.ComputeHash(heslo);

            //Convert encoded bytes back to a 'readable' string
            string navrat = "";
            foreach (byte h in hotovo)
            {
                navrat += h.ToString("x2");
            }
            return navrat;
        }

    }

    class Mail 
    {
        const string SENDER = "aleksandar@zona.ba";
        const string RECEIVER = "noviextra@zona.ba";
        const string SMTP_HOST = "smtp.zona.ba";
        const int SMTP_PORT = 587; 
        string NewLine = "\r\n";
        MailMessage mail = new MailMessage(SENDER, RECEIVER);
        MailMessage mail1 = new MailMessage();
        //SmtpClient smtpClient = new SmtpClient();
        SmtpClient smtpClient = new SmtpClient(SMTP_HOST, SMTP_PORT);
               
        public Mail() 
        {
            //smtpClient.Credentials = new NetworkCredential("aleksandar", "enciklopedija");
            //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(Temp);
            smtpClient.EnableSsl = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            //client1.UseDefaultCredentials = false;
            //smtpClient.Port = 25;
            //smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            //smtpClient.UseDefaultCredentials = false;
            ////smtpClient.EnableSsl = true;
            //smtpClient.Host = "smtp.zona.ba";
            ////smtpClient.Host = "91.191.19.3";
            ////smtpClient.Port = 587;
        }

        public void setMailReceiver(string email, string password)
        {

            smtpClient.Credentials = new NetworkCredential(email, password);
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(Temp);
            smtpClient.EnableSsl = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            MailAddress mailAddress = new MailAddress(email + "@zona.ba");
            //MailAddress mailAddressBackup = new MailAddress(email_receiver + "@zona.ba");
        }

        public void setMailSender(string email, string password)
        {
            smtpClient.Credentials = new NetworkCredential(email, password);
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(Temp);
            smtpClient.EnableSsl = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            MailAddress mailAddress = new MailAddress(email + "@zona.ba");
            mail.From = mailAddress;
            mail.To.Add(email + "@zona.ba");
        }

        public bool Temp(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //debugConsole.Text = certificate.GetName();
            //debugConsole.Text += "\r\n" + certificate.GetPublicKeyString();
            //debugConsole.Text += "\r\n" + certificate.GetSerialNumberString();
            //debugConsole.Text += "\r\n" + certificate.Subject;
            //debugConsole.Text += "\r\n" + certificate.Issuer;
            return true;
        }
        
        public void sendMail(string sender, string subject, string body) 
        {
            if (sender != "")
            {
                MailAddress mailAddress = new MailAddress(sender);
                mail.From = mailAddress;
                smtpClient.Send(mail);
            }                      
        }

        public string sendMail()
        {
            try
            {
                smtpClient.Send(mail);
                return "OK";
            }
            catch (Exception eee)
            {
                return "e-mail adresa nije ispravna\r\nUnesite ispravan username i password.";
            }
            
        }

        public void addSubject(string subject)
        {
            mail.Subject = subject;
        }

        public void addLine(string line)
        {
            mail.Body += NewLine + line;
        }
    }
}

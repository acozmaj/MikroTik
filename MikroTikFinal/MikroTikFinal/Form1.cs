using System;
using System.Collections.Generic;
using System.ComponentModel;
//using System.Data;
//using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;


namespace MikroTikFinal
{
    public partial class Form1 : Form
    {
        enum State
        {
            Ready,
            Connect,
            Login,
            Associate,
            DoScript,
            SendMail,
            Go
        };

        Thread infoThread = null;
        public delegate void SetTextCallback(string text);
        public delegate string GetTextCallback();
        public delegate void SetButtonCallback();
        public delegate void sendShutdown();


        private const string USERNAME = "admin";
        private const UInt32 MAX_MANAGER = 150000;
        private const string PASSWORD = "";
        private const string NewLine = "\r\n";

        NetworkTool network = new NetworkTool();
        Telnet telnet = new Telnet();
        MikroTikApi mikrotik = new MikroTikApi();
        Mail mail = new Mail();
        Mail mail_partner = new Mail();
        StreamReader streamReader; 
        StreamReader streamReaderSxt;
        HashCode hashCode = new HashCode();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            this.timer.Interval = 1507;
            this.timer.Enabled = true;
            this.timer.Start();
            this.buttonThread.Enabled = false;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            labelPingStatus.Text = network.getPing(textIpAddress.Text);
            switch (labelPingStatus.Text) 
            { 
                case "ping ok":
                labelPingStatus.ForeColor = System.Drawing.Color.Green;
                buttonThread.Enabled = true;
                break;
                case "no ping":
                labelPingStatus.ForeColor = System.Drawing.Color.Red;
                buttonThread.Enabled = false;
                break;
                default:
                labelPingStatus.ForeColor = System.Drawing.Color.Black;
                buttonThread.Enabled = false;
                break;
            }
        }

        private string getSsid(string raw) 
        {
            int position1 = raw.LastIndexOf('!');
            int position2 = raw.IndexOf('=', 6) + 1;
            return raw.Substring(position2, position1 - position2);
        }
        
        // prikupljanje informacija o MikroTiku
        public bool getMikroTikInfo()
        { 
            string raw = "";
            // parsiranje podataka o ssid za mail
            mikrotik.Send("/interface/wireless/print");
            mikrotik.Send("=.proplist=ssid", true);
            foreach (string h in mikrotik.Read())
            {
                raw += h;
            }
            mail.addLine("SSID na koji je korisnik asociran: " + getSsid(raw));
            raw = "";

            // parsiranje podataka o serijskom broju za mail
            mikrotik.Send("/system/routerboard/print");
            mikrotik.Send("=.proplist=model", true);
            foreach (string h in mikrotik.Read())
            {
                raw += h;
            }
            mail.addLine("Model uredjaja uredjaja: RB" + getSsid(raw));
            raw = "";

            // parsiranje podataka o wireless MAC adresi za mail
            mikrotik.Send("/interface/wireless/print");
            mikrotik.Send("=.proplist=mac-address", true);
            foreach (string h in mikrotik.Read())
            {
                raw += h;
            }
            mail.addLine("Mac adresa uredjaja: " + getSsid(raw));
            raw = "";

            // parsiranje podataka o modelu ruterboard-a
            mikrotik.Send("/system/routerboard/print");
            mikrotik.Send("=.proplist=serial-number", true);
            foreach (string h in mikrotik.Read())
            {
                raw += h;
            }
            mail.addLine("Serijski broj uredjaja: " + getSsid(raw));
            raw = "";

            // provjera signala
            mikrotik.Send("/interface/wireless/registration-table/print");
            mikrotik.Send("=.proplist=signal-strength,tx-signal-strength", true);
            foreach (string h in mikrotik.Read())
            {
                raw += h;
            }

            //parsiranje primljenog signala
            if (raw.Length > 15)
            {
                int rxSignal = (int)raw.IndexOf('=', 15);
                int txSignal = (int)raw.LastIndexOf('=');
                int rxValue;
                int txValue;
                if ((rxSignal != -1) && (txSignal != -1))
                {
                    rxValue = Convert.ToInt16(raw.Substring(rxSignal + 2, 2));
                    txValue = Convert.ToInt16(raw.Substring(txSignal + 2, 2));
                    if (rxValue > 75 || txValue > 75)
                    {
                        writeToConsole("Signal je losiji od -75dB. Skripta zaustavljena.");
                    }
                    else if (rxValue < 70 && txValue > 75)
                    {
                        writeToConsole("Signal nije dobar (los RX). Skripta zaustavljena.");
                    }
                    else if (rxValue > 75 && txValue < 70)
                    {
                        writeToConsole("Signal nije dobar (los TX). Skripta zaustavljena.");
                    }
                    else
                    {
                        writeToConsole("Signal je OK: -" + Convert.ToString(rxValue) + "/-" + Convert.ToString(txValue));
                        mail.addLine("Nivo signala glasi: " + "-" + Convert.ToString(rxValue) + "/-" + Convert.ToString(txValue));                                       
                        return true;
                    }                
                }
            return false;   
            }
            else 
            {
                writeToConsole("Uredjaj nije asociran." + NewLine + "Potrebno povezivanje po wireless-u." + NewLine + "Aplikacija se gasi za 5 sekundi.");
                return false;
            }
        }

        // Slanje skripte koja u Resource folderu
        public void sendScriptToMicrotikProtected()
        { 
            
        }

        // Slanje skripte MikroTik-u
        public void sendScriptToMikroTik()
        {
            string raw = "";
            string temp = "";
            try
            {
                /* Ovo radi ispravno*/
                //streamReader = new StreamReader("script.txt");
                //streamReaderSxt = new StreamReader("script_sxt.txt");
                /* Novo dodato */
                streamReader = new StreamReader(new MemoryStream(Encoding.ASCII.GetBytes(Properties.Resources.script)));
                streamReaderSxt = new StreamReader(new MemoryStream(Encoding.ASCII.GetBytes(Properties.Resources.script_sxt)));

                raw = streamReaderSxt.ReadLine();
                while (raw != null)
                {
                    telnet.WriteLine(raw + "\r");
                    raw = streamReaderSxt.ReadLine();
                    Thread.Sleep(200);
                }

                raw = streamReader.ReadLine();
                raw = raw.Replace("$1", readTextUsername());
                raw = raw.Replace("$2", readTextPassword());
                mail.addLine("Korisnicki nalog: " + readTextUsername());
                telnet.WriteLine(raw + "\r");
                Thread.Sleep(200);

                raw = streamReader.ReadLine();
                raw = raw.Replace("edited from outside", readTextUserData());
                mail.addLine("Korisnicko ime i prezime: " + readTextUserData());
                mail.addSubject("Novi extra paket - " + readIdManager() + " " + readTextUserData());
                while (raw != null)
                {
                    telnet.WriteLine(raw + "\r");
                    raw = streamReader.ReadLine();
                    temp = raw;
                    if (temp != null)
                    {
                        if (temp.IndexOf('#') != -1)
                        {
                            int count;
                            count = hashCode.hash.IndexOf('0');
                            raw = temp.Replace("#", hashCode.hash.Substring(count + 1, 12));

                        }
                    }
                    Thread.Sleep(200);
                }
                writeToConsole("Skripta je zavrsena.");             
            }
            catch
            {
                writeToConsole("Could not open file.");
            }
        }

        // Slanje mail-a
        public void sendMailToOffice()
        {
            NetworkTool pingGoogle = new NetworkTool();
            int counter = 10;
            string message;
            while (counter != 0)
            {
                string temp = pingGoogle.getPing("8.8.8.8");
                switch (temp)
                {
                    case "ping ok":
                        mail.setMailSender(textMailAddress.Text, textPartnerPassword.Text);
                        message = mail.sendMail();
                        switch (temp)
                        { 
                            case "OK":
                            writeToConsole(message);
                            break;
                            default:
                            writeToConsole(message);
                            break;
                        }
                       
                        counter = 1;
                        break;
                    case "no ping":
                        if (counter == 1)
                        {
                            writeToConsole("Mail nije poslat.");
                            writeToConsole("Problem sa internet konekcijom.");
                        }
                        break;
                    default:
                        if (counter == 1)
                            writeToConsole("Mail nije poslat. Problem sa internet konekcijom.");
                        break;

                }
                Thread.Sleep(1500);
                counter--;
            }
            
        }

        public void startConnection() 
        {          
            string temp;
            if (telnet.setTelnetConnection(readIpAddress()) != "Connected.")
                writeToConsole("Problem sa konekcijom.\nPokusajte ponovo.");
            else
            {
                writeToConsole("Konekcija je OK.\r\nSacekajte...");
                temp = telnet.Login(USERNAME + "+ct", PASSWORD);
                Thread.Sleep(5000);
                telnet.WriteLine("/ip service set api disabled=no" + "\r");
                // ukljucivanje api porta
                mikrotik.setMikroTikApiConnection(readIpAddress());
                if (mikrotik.Login(USERNAME, PASSWORD))
                {
                    writeToConsole("Aplikacija uspjesno logovana na MikroTik.");
                    if (getMikroTikInfo())
                    {
                        writeToConsole("Skripta se izvrsava...");
                        sendScriptToMikroTik();
                        writeToConsole("Salje se mail...");
                        sendMailToOffice();
                        writeToConsole("Kraj.");
                    }
                    else 
                    {
                        //writeToConsole(Environment.CurrentDirectory);
                        //writeToConsole(Environment.MachineName);
                        //writeToConsole(Convert.ToString(Environment.OSVersion));
                        //writeToConsole(Convert.ToString(Environment.ProcessorCount));
                        //writeToConsole(Environment.UserName);
                        //writeToConsole(Convert.ToString(Environment.Version));
                        //writeToConsole(Environment.StackTrace);
                        //writeToConsole(Environment.CommandLine);
                        Thread.Sleep(5000);                         
                        Environment.Exit(Environment.ExitCode);
                    }

                }
                else 
                {
                    writeToConsole("Problem sa logovanjem aplikacije.\r\nResetovati ruter na fabricka podesavanja.");
                }
            }
            telnet.closeConnection();
        }   

        private void buttonStart_Click(object sender, EventArgs e)
        {        
            string raw = "";
            string temp = "";
            if (textMailAddress.Text == "" || textUsername.Text == "" || textPassword.Text == "" || textUserData.Text == "")
            {
                if (textUserData.Text == "")
                    MessageBox.Show("Unesite korisnicko prezime i ime.");
                else if (textUsername.Text == "")
                    MessageBox.Show("Unesite korisnicki nalog.");
                else if (textPassword.Text == "")
                    MessageBox.Show("Unesite korisnicku sifru.");
                else if (textMailAddress.Text == "")
                    MessageBox.Show("Unesite e-mail adresu.");
            }
            else
            {
                textBox1.Text = telnet.setTelnetConnection(textIpAddress.Text);

                if (textBox1.Text != "Connected.")
                    MessageBox.Show("Problem sa konekcijom.\nPokusajte ponovo.");
                else
                {
                    //buttonStart.Enabled = false;
                    textBox1.Text += telnet.Login(USERNAME + "+ct", PASSWORD);
                    Thread.Sleep(1000);
                    // logovanje zavrseno
                    telnet.WriteLine("/ip service set api disabled=no" + "\r");
                    // aktiviranje api porta
                    // telnet.WriteLine("/system reset-configuration " + "\ry");
                    // telnet.WriteLine("y" + "\r");
                    

                    textBox1.Text += mikrotik.setMikroTikApiConnection(textIpAddress.Text);
                    if (mikrotik.Login(USERNAME, PASSWORD))
                    {
                        // parsiranje podataka o ssid za mail
                        mikrotik.Send("/interface/wireless/print");
                        mikrotik.Send("=.proplist=ssid", true);
                        foreach (string h in mikrotik.Read())
                        {
                            raw += h;
                        }
                        mail.addLine("SSID na koji je korisnik asociran: " + getSsid(raw));
                        raw = "";

                        // parsiranje podataka o serijskom broju za mail
                        mikrotik.Send("/system/routerboard/print");
                        mikrotik.Send("=.proplist=model", true);
                        foreach (string h in mikrotik.Read())
                        {
                            raw += h;
                        }
                        mail.addLine("Model uredjaja uredjaja: RB" + getSsid(raw));
                        raw = "";

                        // parsiranje podataka o modelu ruterboard-a
                        mikrotik.Send("/system/routerboard/print");
                        mikrotik.Send("=.proplist=serial-number", true);
                        foreach (string h in mikrotik.Read())
                        {
                            raw += h;
                        }
                        mail.addLine("Serijski broj uredjaja: " + getSsid(raw));
                        raw = "";

                        // provjera signala
                        mikrotik.Send("/interface/wireless/registration-table/print");
                        mikrotik.Send("=.proplist=signal-strength,tx-signal-strength", true);
                        foreach (string h in mikrotik.Read())
                        {
                            raw += h;
                        }
                        
                        // parsiranje primljenog signala
                        if (raw.Length > 15)
                        {
                            int rxSignal = (int)raw.IndexOf('=', 15);
                            int txSignal = (int)raw.LastIndexOf('=');
                            int rxValue;
                            int txValue;
                            if ((rxSignal != -1) && (txSignal != -1))
                            {
                                rxValue = Convert.ToInt16(raw.Substring(rxSignal + 2, 2));
                                txValue = Convert.ToInt16(raw.Substring(txSignal + 2, 2));
                                if (rxValue > 75 || txValue > 75)
                                {
                                    textBox1.Text += "Signal je losiji od -75dB. Skripta zaustavljena.";
                                }
                                else if (rxValue < 70 && txValue > 75)
                                {
                                    textBox1.Text += "Signal nije dobar (los RX). Skripta zaustavljena.";
                                }
                                else if (rxValue > 75 && txValue < 70)
                                {
                                    textBox1.Text += "Signal nije dobar (los TX). Skripta zaustavljena.";
                                }
                                else
                                {
                                    if (Math.Abs(rxValue - txValue) > 8)
                                    {
                                        textBox1.Text += "Signal je previse nesimetrican. Skripta zaustavljena.";
                                    }
                                    else
                                    {
                                        textBox1.Text += "Signal je OK: ";
                                        textBox1.Text += "-" + Convert.ToString(rxValue) + "/-" + Convert.ToString(txValue);
                                        mail.addLine("Nivo signala glasi: " + "-" + Convert.ToString(rxValue) + "/-" + Convert.ToString(txValue));
                                        // upis skripte
                                        try
                                        {
                                            streamReader = new StreamReader("script.txt");

                                            raw = streamReader.ReadLine();
                                            raw = raw.Replace("$1", textUsername.Text);
                                            raw = raw.Replace("$2", textPassword.Text);
                                            mail.addLine("Korisnicki nalog: " + textUsername.Text);
                                            telnet.WriteLine(raw + "\r");
                                            Thread.Sleep(200);

                                            raw = streamReader.ReadLine();
                                            raw = raw.Replace("edited from outside", textUserData.Text);
                                            mail.addLine("Korisnicko ime i prezime: " + textUserData.Text);
                                            mail.addSubject("Novi extra paket. Korisnik: " + textUserData.Text);
                                            while (raw != null)
                                            {
                                                telnet.WriteLine(raw + "\r");
                                                raw = streamReader.ReadLine();
                                                temp = raw;
                                                if (temp != null)
                                                {
                                                    if (temp.IndexOf('#') != -1)
                                                    {
                                                        int count;
                                                        count = hashCode.hash.IndexOf('0');
                                                        raw = temp.Replace("#", hashCode.hash.Substring(count + 1, 12));

                                                    }
                                                }
                                                Thread.Sleep(200);
                                            }
                                            // kodovanje  
                                            textBox1.Text += "\r\nSkripta je zavrsena.";
                                            mail.sendMail(textMailAddress.Text + labelMailAddress.Text, "Naziv", "sadrzaj");
                                        }
                                        catch
                                        {
                                            textBox1.Text += "Could not open file.";
                                        }

                                    }
                                }

                            }
                        }
                        else
                        {
                            textBox1.Text += "Uredjaj nije asociran" + NewLine + "Uredjaj asocirati i restartovati aplikaciju";
                        }
                    }
                    
                    telnet.WriteLine("/ip service set api disabled=yes" + "\r");
                }
            }
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            Thread reset = new Thread(resetDevice);
            buttonReset.Enabled = false;
            reset.Start();
        }

        private void resetDevice()
        {
            // kreiranje nove konekcije za reset
            Telnet resetMikroTik = new Telnet();
            string temp = resetMikroTik.setTelnetConnection(textIpAddress.Text);
            if (temp != "Connected.")
            {
                MessageBox.Show("Problem sa konekcijom.\nPokusajte ponovo.");
            }
            else
            {   
                disableButton();
                resetMikroTik.Login("backup" + "+ct\r\n", "M29u8wHz6c4r\r\n");
                Thread.Sleep(2000);
                writeToConsole("Mikrotik se resetuje.");
                resetMikroTik.Login("admin" + "+ct\r\n", "\r\n");
                Thread.Sleep(2000);
                resetMikroTik.WriteLine("/system reset-configuration" + "\ry");
                Thread.Sleep(2000);
                resetMikroTik.WriteLine("/system reset-configuration" + "\ry");
                writeToConsole("Pokrenite aplikaciju ponovo.");
            }
        }
       
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
        }

        private void buttonThread_Click(object sender, EventArgs e)
        {
            int index = 0;
            UInt32 id = 0;
            try
            {
                id = Convert.ToUInt32(textIdManager.Text);
                if (id > MAX_MANAGER)
                {
                    MessageBox.Show("Unijeli ste prevelik broj.", "Zoned");
                    id = 0;
                }
                textIdManager.Text = Convert.ToString(id);
            }
            catch (Exception ee)
            {
                MessageBox.Show("Unesite validan ID broj.", "Zoned");
                id = 0;
            }


            //if (textMailAddress.Text == "" || textUsername.Text == "" || textPassword.Text == "" || textUserData.Text == "" || textPartnerPassword.Text == "" || id == 0)
            if (textMailAddress.Text.IndexOf('@') > index)
                MessageBox.Show("e-mail adresa nije ispravna", "Zoned");
            else if (textUserData.Text == "")
                MessageBox.Show("Unesite korisnicko prezime i ime.", "Zoned");
            else if (textUsername.Text == "")
                MessageBox.Show("Unesite korisnicki nalog.", "Zoned");
            else if (textPassword.Text == "")
                MessageBox.Show("Unesite korisnicku sifru.", "Zoned");
            else if (textMailAddress.Text == "")
                MessageBox.Show("Unesite e-mail adresu.", "Zoned");
            else if (textPartnerPassword.Text == "")
                MessageBox.Show("Unesite e-mail sifru.", "Zoned");
            else
            {
                buttonThread.Enabled = false;
                var infoThread = new Thread(startConnection);
                infoThread.IsBackground = true;
                infoThread.Start();
            }

        }

        // funkcija za upis u textBox polje infoText
        private void writeToConsole(string text)
        {
            if (textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(writeToConsole);
                Invoke(d, new object[] { text });
            }
            else
            {
                textBox1.Text += text + NewLine;
            }
        }

        // funkcija za enable button-a
        private void enableButton()
        {
            if (buttonThread.InvokeRequired)
            {
                SetButtonCallback d = new SetButtonCallback(enableButton);
                Invoke(d);
            }
            else
            {
                buttonThread.Enabled = true;
            }
        }

        // funkcija za disable button-a
        private void disableButton()
        {
            if (buttonThread.InvokeRequired)
            {
                SetButtonCallback d = new SetButtonCallback(disableButton);
                Invoke(d);
            }
            else
            {
                buttonThread.Enabled = false;
            }
        }
      
        // gasenje aplikacije
        private void shutDown()
        {}

        // funkcija za ocitavanje textBox polja textIpAddress
        private string readIpAddress()
        {
            if (textIpAddress.InvokeRequired)
            {
                GetTextCallback d = new GetTextCallback(readIpAddress);
                string temp = (string)Invoke(d);
                return temp;
            }
            else
            {
                return textIpAddress.Text;
            }
        }

        // funkcija za ocitavanje textBox polja textIdManager
        private string readIdManager()
        {
            if (textIdManager.InvokeRequired)
            {
                GetTextCallback d = new GetTextCallback(readIdManager);
                string temp = (string)Invoke(d);
                return temp;
            }
            else
            {
                return textIdManager.Text;
            }
        }

        // funkcija za ocitavanje textBox polja textUsername
        private string readTextUsername()
        {
            if (textUsername.InvokeRequired)
            {
                GetTextCallback d = new GetTextCallback(readTextUsername);
                string temp = (string)Invoke(d);
                return temp;
            }
            else
            {
                return textUsername.Text;
            }
        }

        // funkcija za ocitavanje textBox polja textPassword
        private string readTextPassword()
        {
            if (textPassword.InvokeRequired)
            {
                GetTextCallback d = new GetTextCallback(readTextPassword);
                string temp = (string)Invoke(d);
                return temp;
            }
            else
            {
                return textPassword.Text;
            }
        }

        // funkcija za ocitavanje textBox polja textUserData
        private string readTextUserData()
        {
            if (textUserData.InvokeRequired)
            {
                GetTextCallback d = new GetTextCallback(readTextUserData);
                string temp = (string)Invoke(d);
                return temp;
            }
            else
            {
                return textUserData.Text;
            }
        }


        // nebitno
        private void textIpAddress_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void axWindowsMediaPlayer1_Enter(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void textIdManager_TextChanged(object sender, EventArgs e)
        {
           
        }



    }
}

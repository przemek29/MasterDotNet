using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AHRS;

namespace AutonomicznySystemNawigacyjny
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }


        public string rx_str = " ";
        public int licznik = 0;
        public double[] gyro1 = new double[3];
        public double[] gyro2 = new double[3];
        public double[] akcel1 = new double[3];
        public double[] akcel2 = new double[3];
        public double[] magnet = new double[3];
        public int licznikWykresu = 0;
        public int licznikRaspberry = 0;

        MahonyAHRS MahonyFilter= new MahonyAHRS(0.002f); // ZMIENIC ARGUMENT
        MadgwickAHRS MadgwickFilter = new MadgwickAHRS(0.002f);




        private void button1_Click(object sender, EventArgs e)
        {

            
            if (serialPort1.IsOpen == false)
            {
                try
                {
                    button1.Text = "Rozłącz";
                    serialPort1.BaudRate = 115200;
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.Open();
                    panel1.BackColor = Color.Green;
                    label5.Text = "Połączony";
                    notifyIcon1.ShowBalloonTip(1000, "KOMUNIKAT", "Połączono z portem COM", ToolTipIcon.Info); //to należy dodać :)
                    
                }
                catch (UnauthorizedAccessException)
                {

                    notifyIcon1.ShowBalloonTip(1000, "KOMUNIKAT", "Nie masz uprawnień na otwarcie portu COM, być może jeden z twoich programów go wykorzystuje", ToolTipIcon.Error);
                    MessageBox.Show("NIE MA DOSTĘPU DO PORTU COM!", "UWAGA", MessageBoxButtons.OK, MessageBoxIcon.Warning); //to należy dodać :)



                }



            }
            else
            {

                button1.Text = "Połącz";
                serialPort1.Close();
                panel1.BackColor = Color.Red;
                label5.Text = "Rozłączony";
                notifyIcon1.ShowBalloonTip(1000, "KOMUNIKAT", "Rozłączono z portem COM", ToolTipIcon.Info);

            }


        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {

            licznik++;
            if (licznik == 1) //jeśli pierwszy raz uruchomiona aplikacja wyczyść odebrany łańcuch
            {
                rx_str = serialPort1.ReadTo("\r\n");
                rx_str = " ";
               


            }
            else
            {
                rx_str = serialPort1.ReadTo("\r\n"); // przekazanie odebranego łańcucha do zmiennej rx_str
                this.Invoke(new EventHandler(rx_parse)); // instalacja zdarzenia parsującego odebrany łańcuch
            }



        }

        // utworzenie zdarzenia  parsującego łańcuch odbierany przez rs232

        private void rx_parse(object sender, EventArgs e)
        {


            string[] dane = new string[16];
            dane = rx_str.Split(',');

                  
            
            textBox1.Text = dane[1];
            textBox2.Text = dane[2];
            textBox3.Text = dane[3];

            textBox4.Text = dane[4];
            textBox5.Text = dane[5];
            textBox6.Text = dane[6];

            textBox7.Text = dane[13];
            textBox8.Text = dane[14];
            textBox9.Text = dane[15];

            textBox10.Text = dane[7];
            textBox11.Text = dane[8];
            textBox12.Text = dane[9];

            textBox13.Text = dane[10];
            textBox14.Text = dane[11];
            textBox15.Text = dane[12];

            textBox16.Text = dane[0];

            gyro1[0] = -Convert.ToDouble(textBox2.Text.Replace('.', ','));
            gyro1[1] = -Convert.ToDouble(textBox1.Text.Replace('.', ','));
            gyro1[2] = Convert.ToDouble(textBox3.Text.Replace('.', ','));

            gyro2[0] = Convert.ToDouble(textBox10.Text.Replace('.', ','));
            gyro2[1] = -Convert.ToDouble(textBox11.Text.Replace('.', ','));
            gyro2[2] = Convert.ToDouble(textBox12.Text.Replace('.', ','));

            akcel1[0] = Convert.ToDouble(textBox5.Text.Replace('.', ','));
            akcel1[1] = -Convert.ToDouble(textBox4.Text.Replace('.', ','));
            akcel1[2] = Convert.ToDouble(textBox6.Text.Replace('.', ','));

            akcel2[0] = Convert.ToDouble(textBox13.Text.Replace('.', ','));
            akcel2[1] = -Convert.ToDouble(textBox14.Text.Replace('.', ','));
            akcel2[2] = -Convert.ToDouble(textBox15.Text.Replace('.', ','));

            magnet[0] = Convert.ToDouble(textBox7.Text.Replace('.', ','));
            magnet[1] = -Convert.ToDouble(textBox8.Text.Replace('.', ','));
            magnet[2] = Convert.ToDouble(textBox9.Text.Replace('.', ','));

            
            rysujWykresy();

            scrollWykres();
            
            zrobNiewidoczne();

            sprawdzZyroskopy();

            sprawdzAkcelerometry();

            sprawdzMagnetometry();

            

            
            if (licznikWykresu == 500)
            {
                czyscZyro("Oś X - L3G4200D");
                czyscZyro("Oś Y - L3G4200D");
                czyscZyro("Oś Z - L3G4200D");

                czyscZyro("Oś X - MPU6050");
                czyscZyro("Oś Y - MPU6050");
                czyscZyro("Oś Z - MPU6050");

                czyscAkcel("Oś X - ADXL345");
                czyscAkcel("Oś Y - ADXL345");
                czyscAkcel("Oś Z - ADXL345");

                czyscAkcel("Oś X - MPU6050");
                czyscAkcel("Oś Y - MPU6050");
                czyscAkcel("Oś Z - MPU6050");

                czyscMagnet("Oś X - HMC5883L");
                czyscMagnet("Oś Y - HMC5883L");
                czyscMagnet("Oś Z - HMC5883L");
                
                licznikWykresu = 0;
            }

            licznikWykresu++;
        }

        private void sprawdzZyroskopy()
        {
            if (checkGyro1X.Checked)
            {
                wykres1.Series["Oś X - L3G4200D"].Enabled = true;
            }

            if (!checkGyro1X.Checked)
            {
                wykres1.Series["Oś X - L3G4200D"].Enabled = false;
            }

            if (checkGyro1Y.Checked)
            {
                wykres1.Series["Oś Y - L3G4200D"].Enabled = true;
            }

            if (!checkGyro1Y.Checked)
            {
                wykres1.Series["Oś Y - L3G4200D"].Enabled = false;
            }

            if (checkGyro1Z.Checked)
            {
                wykres1.Series["Oś Z - L3G4200D"].Enabled = true;
            }

            if (!checkGyro1Z.Checked)
            {
                wykres1.Series["Oś Z - L3G4200D"].Enabled = false;
            }
            //*****************************************************
            if (checkGyro2X.Checked)
            {
                wykres1.Series["Oś X - MPU6050"].Enabled = true;
            }

            if (!checkGyro2X.Checked)
            {
                wykres1.Series["Oś X - MPU6050"].Enabled = false;
            }

            if (checkGyro2Y.Checked)
            {
                wykres1.Series["Oś Y - MPU6050"].Enabled = true;
            }

            if (!checkGyro2Y.Checked)
            {
                wykres1.Series["Oś Y - MPU6050"].Enabled = false;
            }

            if (checkGyro2Z.Checked)
            {
                wykres1.Series["Oś Z - MPU6050"].Enabled = true;
            }

            if (!checkGyro2Z.Checked)
            {
                wykres1.Series["Oś Z - MPU6050"].Enabled = false;
            }
        }

        private void sprawdzAkcelerometry()
        {
            if (checkAkcel1X.Checked)
            {
                wykres2.Series["Oś X - ADXL345"].Enabled = true;
            }

            if (!checkAkcel1X.Checked)
            {
                wykres2.Series["Oś X - ADXL345"].Enabled = false;
            }

            if (checkAkcel1Y.Checked)
            {
                wykres2.Series["Oś Y - ADXL345"].Enabled = true;
            }

            if (!checkAkcel1Y.Checked)
            {
                wykres2.Series["Oś Y - ADXL345"].Enabled = false;
            }

            if (checkAkcel1Z.Checked)
            {
                wykres2.Series["Oś Z - ADXL345"].Enabled = true;
            }

            if (!checkAkcel1Z.Checked)
            {
                wykres2.Series["Oś Z - ADXL345"].Enabled = false;
            }

            if (checkAkcel2X.Checked)
            {
                wykres2.Series["Oś X - MPU6050"].Enabled = true;
            }

            if (!checkAkcel2X.Checked)
            {
                wykres2.Series["Oś X - MPU6050"].Enabled = false;
            }

            if (checkAkcel2Y.Checked)
            {
                wykres2.Series["Oś Y - MPU6050"].Enabled = true;
            }

            if (!checkAkcel2Y.Checked)
            {
                wykres2.Series["Oś Y - MPU6050"].Enabled = false;
            }

            if (checkAkcel2Z.Checked)
            {
                wykres2.Series["Oś Z - MPU6050"].Enabled = true;
            }

            if (!checkAkcel2Z.Checked)
            {
                wykres2.Series["Oś Z - MPU6050"].Enabled = false;
            }

        }

        private void sprawdzMagnetometry()
        {
            if(checkMagnet1X.Checked)
            {
                wykres3.Series["Oś X - HMC5883L"].Enabled = true;
            }

            if (!checkMagnet1X.Checked)
            {
                wykres3.Series["Oś X - HMC5883L"].Enabled = false;
            }

            if (checkMagnet1Y.Checked)
            {
                wykres3.Series["Oś Y - HMC5883L"].Enabled = true;
            }

            if (!checkMagnet1Y.Checked)
            {
                wykres3.Series["Oś Y - HMC5883L"].Enabled = false;
            }

            if (checkMagnet1Z.Checked)
            {
                wykres3.Series["Oś Z - HMC5883L"].Enabled = true;
            }

            if (!checkMagnet1Z.Checked)
            {
                wykres3.Series["Oś Z - HMC5883L"].Enabled = false;
            }
        }

        private void scrollWykres()
        {
            //scrollowanie wykresu żyrskopów
            /*
            wykres1.ChartAreas["ChartArea1"].AxisY.ScaleView.Zoomable = true;
            wykres1.ChartAreas["ChartArea1"].AxisY2.ScaleView.Size.Equals(0.1);
            wykres1.ChartAreas["ChartArea1"].CursorY.AutoScroll = true;
            wykres1.ChartAreas["ChartArea1"].CursorY.IsUserSelectionEnabled = true;
            */

            // Set automatic zooming
            wykres1.ChartAreas["ChartArea1"].AxisX.ScaleView.Zoomable = true;
            wykres1.ChartAreas["ChartArea1"].AxisY.ScaleView.Zoomable = true;

            // Set automatic scrolling 
            wykres1.ChartAreas["ChartArea1"].CursorX.AutoScroll = true;
            wykres1.ChartAreas["ChartArea1"].CursorY.AutoScroll = true;

            // Allow user selection for Zoom
            wykres1.ChartAreas["ChartArea1"].CursorX.IsUserSelectionEnabled = true;
            wykres1.ChartAreas["ChartArea1"].CursorY.IsUserSelectionEnabled = true;
            //*****************************

            //scrollowanie wykresu akcelerometrów
            wykres2.ChartAreas["ChartArea1"].AxisY.ScaleView.Zoomable = true;
            wykres2.ChartAreas["ChartArea1"].AxisY2.ScaleView.Size.Equals(0.1);
            wykres2.ChartAreas["ChartArea1"].CursorY.AutoScroll = true;
            wykres2.ChartAreas["ChartArea1"].CursorY.IsUserSelectionEnabled = true;
            //*****************************

            //scrollowanie wykresu magnetometru
            wykres3.ChartAreas["ChartArea1"].AxisY.ScaleView.Zoomable = true;
            wykres3.ChartAreas["ChartArea1"].AxisY2.ScaleView.Size.Equals(magnet);
            wykres3.ChartAreas["ChartArea1"].CursorY.AutoScroll = true;
            wykres3.ChartAreas["ChartArea1"].CursorY.IsUserSelectionEnabled = true;
            //*****************************
        }

        private void zrobNiewidoczne()
        {
            //ukryj wykresy z żyroskopów
            wykres1.Series["Oś X - L3G4200D"].Enabled = false;
            wykres1.Series["Oś Y - L3G4200D"].Enabled = false;
            wykres1.Series["Oś Z - L3G4200D"].Enabled = false;

            wykres1.Series["Oś X - MPU6050"].Enabled = false;
            wykres1.Series["Oś Y - MPU6050"].Enabled = false;
            wykres1.Series["Oś Z - MPU6050"].Enabled = false;
            //*****************************

            //ukryj wykresy z akcelerometrów
            wykres2.Series["Oś X - ADXL345"].Enabled = false;
            wykres2.Series["Oś Y - ADXL345"].Enabled = false;
            wykres2.Series["Oś Z - ADXL345"].Enabled = false;

            wykres2.Series["Oś X - MPU6050"].Enabled = false;
            wykres2.Series["Oś Y - MPU6050"].Enabled = false;
            wykres2.Series["Oś Z - MPU6050"].Enabled = false;
            //*****************************
            
            //ukryj wykresy z magnetometru
            wykres3.Series["Oś X - HMC5883L"].Enabled = false;
            wykres3.Series["Oś Y - HMC5883L"].Enabled = false;
            wykres3.Series["Oś Z - HMC5883L"].Enabled = false;
            //*****************************
        }
        private void rysujWykresy()
        {
            //rysuj wykresy z żyroskopów
            rysujZyro("Oś X - L3G4200D", gyro1[0]);
            rysujZyro("Oś Y - L3G4200D", gyro1[1]);
            rysujZyro("Oś Z - L3G4200D", gyro1[2]);

            rysujZyro("Oś X - MPU6050", gyro2[0]);
            rysujZyro("Oś Y - MPU6050", gyro2[1]);
            rysujZyro("Oś Z - MPU6050", gyro2[2]);
            //*****************************

            //rysuj wykresy z akcelerometrow
            rysujAkcel("Oś X - ADXL345", akcel1[0]);
            rysujAkcel("Oś Y - ADXL345", akcel1[1]);
            rysujAkcel("Oś Z - ADXL345", akcel1[2]);

            rysujAkcel("Oś X - MPU6050", akcel2[0]);
            rysujAkcel("Oś Y - MPU6050", akcel2[1]);
            rysujAkcel("Oś Z - MPU6050", akcel2[2]);
            //*****************************

            //rysuj wykresy z magnetometru
            rysujMagnet("Oś X - HMC5883L", magnet[0]);
            rysujMagnet("Oś Y - HMC5883L", magnet[1]);
            rysujMagnet("Oś Z - HMC5883L", magnet[2]);
        }

        private void rysujZyro(string nazwaSerii, double wartoscSerii)
       {
            wykres1.Series[nazwaSerii].Points.AddY(wartoscSerii);
        }

        private void rysujAkcel(string nazwaSerii, double wartoscSerii)
        {
           wykres2.Series[nazwaSerii].Points.AddY(wartoscSerii);
        }

        private void rysujMagnet(string nazwaSerii, double wartoscSerii)
        {
           wykres3.Series[nazwaSerii].Points.AddY(wartoscSerii);
        }

        private void czyscZyro(string nazwaSerii)
        {
            wykres1.Series[nazwaSerii].Points.Clear();
        }

        private void czyscAkcel(string nazwaSerii)
        {
            wykres2.Series[nazwaSerii].Points.Clear();
        }

        private void czyscMagnet(string nazwaSerii)
        {
            wykres3.Series[nazwaSerii].Points.Clear();
        }




        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            this.Hide();

        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        

        private void Form1_Load(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == false)
            {
                //*-- USTAWIENIE POCZĄTKOWYCH STANÓW KONTROLEK I ETYKIER **--//

                panel1.BackColor = Color.Red; //
                panel2.BackColor = SystemColors.HotTrack;
                label5.Text = "Rozłączony";
                label6.Text = "Nieaktywny";


                //*-- WSZYTANIE DOSTĘPNEJ LISTY PORTÓW COM DO COMBOBOXA **--/                                           

                
                string[] port = System.IO.Ports.SerialPort.GetPortNames();
                foreach (string item in port)
                {
                    comboBox1.Items.Add(item);
                }
                comboBox1.Text = port[0]; //pierwszym elementem wyswietlanym w comboboxie będzie pierwszy port






            }
        }

        private void pokażToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();

        }


    }
}

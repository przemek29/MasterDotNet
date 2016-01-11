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
using Autonomiczny_System_Nawigacyjny;

namespace AutonomicznySystemNawigacyjny
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }
        public long millisecondTime { get; set; }
        public int rzad = 10;
        public int n;
        public double czestotliwosc = 90;
        public bool kalibracjaUkonczona = false;

        public string rx_str = " ";
        public int licznik = 0;
        public double[] gyro1 = new double[3];
        public double[] gyro1Kalibracja = new double[3];

        public double[] gyro2 = new double[3];
        public double[] gyro2Kalibracja = new double[3];

        public double[] akcel1 = new double[3];
        public double[] akcel1Kalibracja = new double[3];

        public double[] akcel2 = new double[3];
        public double[] akcel2Kalibracja = new double[3];

        public double[] magnet = new double[3];
        public double[] magnetKalibracja = new double[3] { 0, 0, 0 };

        public double[] GainGyro1 = new double[3] { 1.0, 1.0, 1.0 };
        public double[] GainGyro2 = new double[3] { 1.0, 1.0, 1.0 };

        public double[] katyKalibrowane = new double[6];
        public double[] katyCalkowane = new double[6];


        public long licznikWykresu = 0;
        public int licznikRaspberry = 0;

        private readonly KalibracjaMagnetometru _kalibracjaMagnetometru = new KalibracjaMagnetometru(3000, 3000, 3000);
        private readonly KalibracjaBiasGyro _kalibracjaBiasGyro1 = new KalibracjaBiasGyro();
        private readonly KalibracjaBiasGyro _kalibracjaBiasGyro2 = new KalibracjaBiasGyro();
        private readonly UzyskajKatyZAkcelerometru _katyAkcel1 = new UzyskajKatyZAkcelerometru();
        private readonly UzyskajKatyZAkcelerometru _katyAkcel2 = new UzyskajKatyZAkcelerometru();
        private readonly MetodaTrapezow trapez = new MetodaTrapezow();

        public double[] paczka = new double[9];

        MahonyAHRS MahonyFilter= new MahonyAHRS(0.002f); // ZMIENIC ARGUMENT
        MadgwickAHRS MadgwickFilter = new MadgwickAHRS(0.002f);
        KalmanFilter kalman = new KalmanFilter(0.001, 0.003, 0.03);

        Stopwatch stopWatch = new Stopwatch();
        

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

        
        private void MySerialPortOnDataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs serialDataReceivedEventArgs)
        {
            throw new NotImplementedException();
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



            if (licznikRaspberry % rzad == 0)
            {
                stopWatch.Restart();
            }

            string[] dane = new string[16];
            dane = rx_str.Split(',');
            
            

            konwertujDane(dane);
            wypiszDane();
            
            kalibracjaZyroskopow();
            KatyZAkcelerometru();

            KatyZZyroskopu();


            licznikWykresu++;
            if (licznikRaspberry % rzad == 0)
            {
                stopWatch.Stop();
                millisecondTime= stopWatch.ElapsedMilliseconds;
               
                czestotliwosc = Math.Round(rzad * ((1 / (double)millisecondTime) * 1000));
                tCzestotliwosc.Text = Convert.ToString(czestotliwosc);
            }
        }
        
        private void KatyZZyroskopu()
        {
            for (int i = 0; i < 3; i++)
            {
                katyKalibrowane[i] = gyro1Kalibracja[i];
            }

            for (int i = 3; i < 6; i++)
            {
                katyKalibrowane[i] = gyro2Kalibracja[i - 3];
            }

            if (kalibracjaUkonczona == true)
            {
                katyCalkowane = trapez.calkuj(katyKalibrowane, 1 / czestotliwosc);

            }

            katyCalkowane = zaokraglacz(katyCalkowane);    

            tRollGyro1.Text = Convert.ToString(katyCalkowane[0]);
            tPitchGyro1.Text = Convert.ToString(katyCalkowane[1]);
            tYawGyro1.Text = Convert.ToString(katyCalkowane[2]);

            tRollGyro2.Text = Convert.ToString(katyCalkowane[3]);
            tPitchGyro2.Text = Convert.ToString(katyCalkowane[4]);
            tYawGyro2.Text = Convert.ToString(katyCalkowane[5]);

        }

        public double[] zaokraglacz(double[] wejscie)
        {
            double[] wyjscie = new double[6];

            for (int i =0; i< 6; i++)
            {
                wyjscie[i] =  Math.Round(wejscie[i], 4);
            }

            return wyjscie;
        }

        private void KatyZAkcelerometru()
        {
            _katyAkcel1.PrzeliczKaty(akcel1[0], akcel1[1], akcel1[2]);
            _katyAkcel2.PrzeliczKaty(akcel2[0], akcel2[1], akcel2[2]);

            tRollAkcel1.Text = Convert.ToString(_katyAkcel1.roll);
            tRollAkcel2.Text = Convert.ToString(_katyAkcel2.roll);

            tPitchAkcel1.Text = Convert.ToString(_katyAkcel1.pitch);
            tPitchAkcel2.Text = Convert.ToString(_katyAkcel2.pitch);

        }
        private void kalibracjaZyroskopow()
        {
            gyro1Kalibracja[0] = Math.Round(gyro1[0] * GainGyro1[0] - _kalibracjaBiasGyro1.SredniaX, 4);
            gyro1Kalibracja[1] = Math.Round(gyro1[1] * GainGyro1[1] - _kalibracjaBiasGyro1.SredniaY, 4);
            gyro1Kalibracja[2] = Math.Round(gyro1[2] * GainGyro1[2] - _kalibracjaBiasGyro1.SredniaZ, 4);

            tGyro1XKalib.Text = Convert.ToString(gyro1Kalibracja[0]);
            tGyro1YKalib.Text = Convert.ToString(gyro1Kalibracja[1]);
            tGyro1ZKalib.Text = Convert.ToString(gyro1Kalibracja[2]);

            gyro2Kalibracja[0] = Math.Round(gyro2[0] * GainGyro2[0] - _kalibracjaBiasGyro2.SredniaX, 4);
            gyro2Kalibracja[1] = Math.Round(gyro2[1] * GainGyro2[1] - _kalibracjaBiasGyro2.SredniaY, 4);
            gyro2Kalibracja[2] = Math.Round(gyro2[2] * GainGyro2[2] - _kalibracjaBiasGyro2.SredniaZ, 4);

            tGyro2XKalib.Text = Convert.ToString(gyro2Kalibracja[0]);
            tGyro2YKalib.Text = Convert.ToString(gyro2Kalibracja[1]);
            tGyro2ZKalib.Text = Convert.ToString(gyro2Kalibracja[2]);

            magnetKalibracja[0] = Math.Round(magnet[0] * _kalibracjaMagnetometru.GainX + _kalibracjaMagnetometru.OffsetX, 0);
            magnetKalibracja[1] = Math.Round(magnet[1] * _kalibracjaMagnetometru.GainY + _kalibracjaMagnetometru.OffsetY, 0);
            magnetKalibracja[2] = Math.Round(magnet[2] * _kalibracjaMagnetometru.GainZ + _kalibracjaMagnetometru.OffsetZ, 0);

            tMagnet1XKalib.Text = Convert.ToString(magnetKalibracja[0]);
            tMagnet1YKalib.Text = Convert.ToString(magnetKalibracja[1]);
            tMagnet1ZKalib.Text = Convert.ToString(magnetKalibracja[2]);
        }


        private void wypiszDane()
        {
            
            textBox16.Text = Convert.ToString(licznikRaspberry);

            textBox1.Text = Convert.ToString(gyro1[0]);
            textBox2.Text = Convert.ToString(gyro1[1]);
            textBox3.Text = Convert.ToString(gyro1[2]);

            textBox4.Text = Convert.ToString(akcel1[0]);
            textBox5.Text = Convert.ToString(akcel1[1]);
            textBox6.Text = Convert.ToString(akcel1[2]);

            textBox10.Text = Convert.ToString(gyro2[0]);
            textBox11.Text = Convert.ToString(gyro2[1]);
            textBox12.Text = Convert.ToString(gyro2[2]);

            textBox13.Text = Convert.ToString(akcel2[0]);
            textBox14.Text = Convert.ToString(akcel2[1]);
            textBox15.Text = Convert.ToString(akcel2[2]);

            textBox7.Text = Convert.ToString(magnet[0]);
            textBox8.Text = Convert.ToString(magnet[1]);
            textBox9.Text = Convert.ToString(magnet[2]);

        }

        private void konwertujDane(string[] dane)
        {
            
            gyro1[0] = -Convert.ToDouble(dane[2].Replace('.', ','));
            gyro1[1] = -Convert.ToDouble(dane[1].Replace('.', ','));
            gyro1[2] = Convert.ToDouble(dane[3].Replace('.', ','));

            gyro2[0] = Convert.ToDouble(dane[7].Replace('.', ','));
            gyro2[1] = -Convert.ToDouble(dane[8].Replace('.', ','));
            gyro2[2] = Convert.ToDouble(dane[9].Replace('.', ','));

            akcel1[0] = Convert.ToDouble(dane[5].Replace('.', ','));
            akcel1[1] = -Convert.ToDouble(dane[4].Replace('.', ','));
            akcel1[2] = Convert.ToDouble(dane[6].Replace('.', ','));

            akcel2[0] = Convert.ToDouble(dane[10].Replace('.', ','));
            akcel2[1] = -Convert.ToDouble(dane[11].Replace('.', ','));
            akcel2[2] = -Convert.ToDouble(dane[12].Replace('.', ','));

            magnet[0] = Convert.ToDouble(dane[13].Replace('.', ','));
            magnet[1] = -Convert.ToDouble(dane[14].Replace('.', ','));
            magnet[2] = Convert.ToDouble(dane[15].Replace('.', ','));

            licznikRaspberry = (int)Convert.ToInt64(dane[0]);
        }

        private void InicjalizujWykresy()
        {
            rysujWykresy();
            sprawdzZyroskopy();
            sprawdzAkcelerometry();
            sprawdzMagnetometry();
        }

        private void czyscWszystko()
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
            // Ustawianie automatycznego zoomowania żyroskopów
            wykres1.ChartAreas["ChartArea1"].AxisX.ScaleView.Zoomable = true;
            wykres1.ChartAreas["ChartArea1"].AxisY.ScaleView.Zoomable = true;

            // Ustawianie automatycznego scrollowania żyroskopów
            wykres1.ChartAreas["ChartArea1"].CursorX.AutoScroll = true;
            wykres1.ChartAreas["ChartArea1"].CursorY.AutoScroll = true;

            // Zezwolenie na wykonanie zadania żyroskopów
            wykres1.ChartAreas["ChartArea1"].CursorX.IsUserSelectionEnabled = true;
            wykres1.ChartAreas["ChartArea1"].CursorY.IsUserSelectionEnabled = true;
            //*****************************

            // Ustawianie automatycznego zoomowania akcelerometrów
            wykres2.ChartAreas["ChartArea1"].AxisX.ScaleView.Zoomable = true;
            wykres2.ChartAreas["ChartArea1"].AxisY.ScaleView.Zoomable = true;

            // Ustawianie automatycznego scrollowania akcelerometrów
            wykres2.ChartAreas["ChartArea1"].CursorX.AutoScroll = true;
            wykres2.ChartAreas["ChartArea1"].CursorY.AutoScroll = true;

            // Zezwolenie na wykonanie zadania akcelerometrów
            wykres2.ChartAreas["ChartArea1"].CursorX.IsUserSelectionEnabled = true;
            wykres2.ChartAreas["ChartArea1"].CursorY.IsUserSelectionEnabled = true;
            //*****************************

            // Ustawianie automatycznego zoomowania magnetometru
            wykres3.ChartAreas["ChartArea1"].AxisX.ScaleView.Zoomable = true;
            wykres3.ChartAreas["ChartArea1"].AxisY.ScaleView.Zoomable = true;

            // Ustawianie automatycznego scrollowania magnetometru
            wykres3.ChartAreas["ChartArea1"].CursorX.AutoScroll = true;
            wykres3.ChartAreas["ChartArea1"].CursorY.AutoScroll = true;

            // Zezwolenie na wykonanie zadania magnetometru
            wykres3.ChartAreas["ChartArea1"].CursorX.IsUserSelectionEnabled = true;
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

        private void rysujNowy(string nazwa, double wartosc)
        {
            chart1.Series[nazwa].Points.AddY(wartosc);
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


                scrollWykres();
                zrobNiewidoczne();



            }
        }

        private void pokażToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();

        }



        private void button6_Click(object sender, EventArgs e)
        {
            KalibrujMagnetometr();
               
     
        }

        private void KalibrujMagnetometr()
        {
            _kalibracjaMagnetometru.czysc();

            for (int i = 0; i < 150000; i++)
            {
                _kalibracjaMagnetometru.AddValues(magnet[0], magnet[1], magnet[2]);

                
                progressBar3.Value = i;
            }

            tMinMagnetX.Text = Convert.ToString(_kalibracjaMagnetometru.MinX);
            tMinMagnetY.Text = Convert.ToString(_kalibracjaMagnetometru.MinY);
            tMinMagnetZ.Text = Convert.ToString(_kalibracjaMagnetometru.MinZ);

            tMaxMagnetX.Text = Convert.ToString(_kalibracjaMagnetometru.MaxX);
            tMaxMagnetY.Text = Convert.ToString(_kalibracjaMagnetometru.MaxY);
            tMaxMagnetZ.Text = Convert.ToString(_kalibracjaMagnetometru.MaxZ);

            tOffsetMagnetX.Text = Convert.ToString(_kalibracjaMagnetometru.OffsetX);
            tOffsetMagnetY.Text = Convert.ToString(_kalibracjaMagnetometru.OffsetY);
            tOffsetMagnetZ.Text = Convert.ToString(_kalibracjaMagnetometru.OffsetZ);

            tGainMagnetX.Text = Convert.ToString(_kalibracjaMagnetometru.GainX);
            tGainMagnetY.Text = Convert.ToString(_kalibracjaMagnetometru.GainY);
            tGainMagnetZ.Text = Convert.ToString(_kalibracjaMagnetometru.GainZ);

            t3DMagnet.Text = Convert.ToString(_kalibracjaMagnetometru.Wektor3D);


            //****************


            ;


        }

        private void button2_Click(object sender, EventArgs e)
        {
            _kalibracjaBiasGyro1.zeruj();
            _kalibracjaBiasGyro2.zeruj();
            kalibracjaUkonczona = false;

            for (int i = 1; i < 1500; i++)
            {
                _kalibracjaBiasGyro1.kalibruj(gyro1[0], gyro1[1], gyro1[2], i );
                _kalibracjaBiasGyro2.kalibruj(gyro2[0], gyro2[1], gyro2[2], i);

                
                progressBar1.Value = i;
            }

            tBiasGyro1X.Text = Convert.ToString(_kalibracjaBiasGyro1.SredniaX);
            tBiasGyro1Y.Text = Convert.ToString(_kalibracjaBiasGyro1.SredniaY);
            tBiasGyro1Z.Text = Convert.ToString(_kalibracjaBiasGyro1.SredniaZ);

            tBiasGyro2X.Text = Convert.ToString(_kalibracjaBiasGyro2.SredniaX);
            tBiasGyro2Y.Text = Convert.ToString(_kalibracjaBiasGyro2.SredniaY);
            tBiasGyro2Z.Text = Convert.ToString(_kalibracjaBiasGyro2.SredniaZ);

            kalibracjaUkonczona = true;
        }

       
    }
}

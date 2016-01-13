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
        public bool kalibracjaKoniecGyro = false;
        public bool kalibracjaKoniecAkcel = false;

        public string rx_str = " ";
        public int licznik = 0;
        public double[] gyro1 = new double[3];
        public double[] gyro1Kalibracja = new double[3] { 0, 0, 0 };

        public double[] gyro2 = new double[3];
        public double[] gyro2Kalibracja = new double[3] { 0, 0, 0 };

        public double[] akcel1 = new double[3];
        public double[] akcel1Kalibracja = new double[3] { 0, 0, 0 };

        public double[] akcel2 = new double[3];
        public double[] akcel2Kalibracja = new double[3];

        public double[] magnet = new double[3];
        public double[] magnetKalibracja = new double[3] { 0, 0, 0 };

        public double[] GainGyro1 = new double[3] { 1.0, 1.0, 1.0 };
        public double[] GainGyro2 = new double[3] { 1.0, 1.0, 1.0 };

        public double[] GainAkcel1 = new double[3] { 1.0, 1.0, 1.0 };
        public double[] GainAkcel2 = new double[3] { 1.0, 1.0, 1.0 };

        public double[] predkosciKatoweKalibrowane = new double[6];
        public double[] przyspieszeniaKalibrowane = new double[6];
        public double[] katyCalkowane = new double[6];

        public double[] katyAkcel = new double[4];

        public double[] mahony1Q = new double[4];
        public double[] mahony2Q = new double[4];

        public double[] madgwick1Q = new double[4];
        public double[] madgwick2Q = new double[4];

        public double[] buforGyro1X = new double[11];
        public double[] buforGyro1Y = new double[11];
        public double[] buforGyro1Z = new double[11];

        public double[] buforGyro2X = new double[11];
        public double[] buforGyro2Y = new double[11];
        public double[] buforGyro2Z = new double[11];

        public double rollMah1;
        public double rollMah2;
        public double pitchMah1;
        public double pitchMah2;
        public double yawMah1;
        public double yawMah2;

        public double rollMadg1;
        public double rollMadg2;
        public double pitchMadg1;
        public double pitchMadg2;
        public double yawMadg1;
        public double yawMadg2;

        public double[] surowaPredkosc = new double [6];

        public double deklinacja;
        public double kursDeklinacja;

        public int[] licznikWspolczynnikowGyro = new int[6] { 1,1,1,1,1,1};
        public int[] licznikWspolczynnikowAkcel = new int[6] { 1, 1, 1, 1, 1, 1 };
        public long licznikWykresu = 0;
        public int licznikRaspberry = 0;

        private readonly KalibracjaMagnetometru _kalibracjaMagnetometru = new KalibracjaMagnetometru(3000, 3000, 3000);
        private readonly KalibracjaBias _kalibracjaBiasGyro1 = new KalibracjaBias();
        private readonly KalibracjaBias _kalibracjaBiasGyro2 = new KalibracjaBias();
        private readonly KalibracjaBias _kalibracjaBiasAkcel1 = new KalibracjaBias();
        private readonly KalibracjaBias _kalibracjaBiasAkcel2 = new KalibracjaBias();

        private readonly UzyskajKatyZAkcelerometru _katyAkcel1 = new UzyskajKatyZAkcelerometru();
        private readonly UzyskajKatyZAkcelerometru _katyAkcel2 = new UzyskajKatyZAkcelerometru();

        private readonly FiltrKomplementarny _komplementarny = new FiltrKomplementarny();

        private readonly MetodaTrapezow _trapezKaty = new MetodaTrapezow();
        private readonly OdczytKursu _odczytKursu = new OdczytKursu();
        private readonly MetodaTrapezow _trapezyPredkosci = new MetodaTrapezow();

        private readonly MahonyRozbudowany _mahonyZestaw1 = new MahonyRozbudowany();
        private readonly MahonyRozbudowany _mahonyZestaw2 = new MahonyRozbudowany();

        private readonly FiltrMadgwicka _madgwickZestaw1 = new FiltrMadgwicka();
        private readonly FiltrMadgwicka _madgwickZestaw2 = new FiltrMadgwicka();

        private readonly KalibracjaWspolczynnikow _kalWspolGyro1X = new KalibracjaWspolczynnikow();
        private readonly KalibracjaWspolczynnikow _kalWspolGyro1Y = new KalibracjaWspolczynnikow();
        private readonly KalibracjaWspolczynnikow _kalWspolGyro1Z = new KalibracjaWspolczynnikow();

        private readonly KalibracjaWspolczynnikow _kalWspolGyro2X = new KalibracjaWspolczynnikow();
        private readonly KalibracjaWspolczynnikow _kalWspolGyro2Y = new KalibracjaWspolczynnikow();
        private readonly KalibracjaWspolczynnikow _kalWspolGyro2Z = new KalibracjaWspolczynnikow();

        private readonly KalibracjaWspolczynnikow _kalWspolAkcel1X = new KalibracjaWspolczynnikow();
        private readonly KalibracjaWspolczynnikow _kalWspolAkcel1Y = new KalibracjaWspolczynnikow();
        private readonly KalibracjaWspolczynnikow _kalWspolAkcel1Z = new KalibracjaWspolczynnikow();

        private readonly KalibracjaWspolczynnikow _kalWspolAkcel2X = new KalibracjaWspolczynnikow();
        private readonly KalibracjaWspolczynnikow _kalWspolAkcel2Y = new KalibracjaWspolczynnikow();
        private readonly KalibracjaWspolczynnikow _kalWspolAkcel2Z = new KalibracjaWspolczynnikow();

        //test
        private readonly MetodaTrapezow _predkoscLiniowa1 = new MetodaTrapezow();
        private readonly MetodaTrapezow _droga1 = new MetodaTrapezow();

        //test

        public double[] paczka = new double[9];

        MahonyAHRS MahonyFilter = new MahonyAHRS(0.002f); // ZMIENIC ARGUMENT
        MadgwickAHRS MadgwickFilter1 = new MadgwickAHRS(0.011);
        MadgwickAHRS MadgwickFilter2 = new MadgwickAHRS(0.011);
        KalmanFilter kalman1 = new KalmanFilter(0.1, 0.03, 0.03, 100);
        KalmanFilter kalman2 = new KalmanFilter(0.1, 0.03, 0.03, 100);
        KalmanFilter kalman3 = new KalmanFilter(0.1, 0.03, 0.03, 100);
        KalmanFilter kalman4 = new KalmanFilter(0.1, 0.03, 0.03, 100);

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

            if (radioButton1.Checked)
                KalibrujMagnetometr();
            
            kalibracjaZyroskopow();
            kalibracjaMagnetometru();
            kalibracjaAkcelerometrów();

            KatyZAkcelerometru();
            KatyZZyroskopu();
            ObliczKursy();
            PrzepiszSuroweKaty();

            KontrolujGyro();

            //test
            if (kalibracjaKoniecAkcel == true)
            {

                kwaternionyMahony();
                katyMahony();

                kwaternionyMadgwick();
                katyMadgwick();


              


                double kalRoll1 = Math.Round(kalman1.update(katyAkcel[0],gyro1Kalibracja[0],1/czestotliwosc));
                double kalPitch1 = Math.Round(kalman2.update(katyAkcel[1], gyro1Kalibracja[1], 1 / czestotliwosc));

                double kalRoll2 = Math.Round(kalman3.update(katyAkcel[2], gyro2Kalibracja[0], 1 / czestotliwosc));
                double kalPitch2 = Math.Round(kalman4.update(katyAkcel[3], gyro2Kalibracja[1], 1 / czestotliwosc));

                t1.Text = Convert.ToString(kalRoll1);
                t2.Text = Convert.ToString(kalPitch1);

                t4.Text= Convert.ToString(kalRoll2);
                t5.Text = Convert.ToString(kalPitch2);

                var predkosc = _predkoscLiniowa1.calkuj(akcel1Kalibracja, 1 / czestotliwosc);


                //var droga = _droga1.calkuj(predkosc, 1 / czestotliwosc);
                var droga1 = 0.5 * 9 * Math.Round(akcel1Kalibracja[0]) * (1 / czestotliwosc)*(1 / czestotliwosc) + predkosc[0] * (1 / czestotliwosc);
                var droga2 = 0.5 * 9 *Math.Round(akcel1Kalibracja[1]) * (1 / czestotliwosc)* (1 / czestotliwosc) + predkosc[1] * (1 / czestotliwosc);
                //droga[0] = kalman1.update(droga[0], 0.1, czestotliwosc);
                //droga[1] = kalman2.update(droga[1], 0.1, czestotliwosc);

                tVxAkcel1.Text = Convert.ToString(predkosc[0]);
                tVyAkcel1.Text = Convert.ToString(predkosc[1]);


                tSxAkcel1.Text = Convert.ToString(droga1);
                tSyAkcel1.Text = Convert.ToString(droga2);

                if (licznikRaspberry % 7 == 0)
                {
                    Wykres.Series["Czysty kat"].Points.AddY(katyAkcel[1]);
                    Wykres.Series["Filtr komplementarny"].Points.AddY(Math.Round(_komplementarny.oblicz(katyAkcel[1], gyro1Kalibracja[1], akcel1Kalibracja[1], 1 / czestotliwosc)));
                    Wykres.Series["Filtr Kalmana"].Points.AddY(kalPitch1);
                    Wykres.Series["Filtr Mahony"].Points.AddY(pitchMah1);
                    Wykres.Series["Filtr Madgwicka"].Points.AddY(pitchMadg1);
                }
                if (licznikRaspberry % 500 == 0)
                {
                    Wykres.Series["Czysty kat"].Points.Clear();
                    Wykres.Series["Filtr komplementarny"].Points.Clear();
                    Wykres.Series["Filtr Kalmana"].Points.Clear();
                    Wykres.Series["Filtr Mahony"].Points.Clear();
                    Wykres.Series["Filtr Madgwicka"].Points.Clear();
                }


            }//test

            helperKomplementarny();
            
            helperCzestotliwosci();
            licznikWykresu++;
            
        }

        private double[] EulerToQuaternion(double heading, double attitude, double bank)
        {

            var C1 = Math.Cos(heading);
            var C2 = Math.Cos(attitude);
            var C3 = Math.Cos(bank);

            var S1 = Math.Sin(heading);
            var S2 = Math.Sin(attitude);
            var S3 = Math.Sin(bank);

            double[] quaternion = new double[4];

            quaternion[0] = Math.Sqrt(1.0 + C1 * C2 + C1 * C3 - S1 * S2 * S3 + C2 * C3) / 2;
            quaternion[1] = (C2 * S3 + C1 * S3 + S1 * S2 * C3) / (4.0 * quaternion[0]);
            quaternion[2] = (S1 * C2 + S1 * C3 + C1 * S2 * S3) / (4.0 * quaternion[0]);
            quaternion[3] = (-S1 * S3 + C1 * S2 * C3 + S2) / (4.0 * quaternion[0]);

            return quaternion;
        }

        private void katyMadgwick()
        {
            var Z1q0 = madgwick1Q[0];
            var Z1q1 = madgwick1Q[1];
            var Z1q2 = madgwick1Q[2];
            var Z1q3 = madgwick1Q[3];

            var Z2q0 = madgwick2Q[0];
            var Z2q1 = madgwick2Q[1];
            var Z2q2 = madgwick2Q[2];
            var Z2q3 = madgwick2Q[3];

             rollMadg1= Math.Round(Math.Atan2(2 * (Z1q0 * Z1q1 + Z1q2 * Z1q3), (1 - 2 * (Z1q1 * Z1q1 + Z1q2 * Z1q2))) * 180 / Math.PI);
             rollMadg2= Math.Round(Math.Atan2(2 * (Z2q0 * Z2q1 + Z2q2 * Z2q3), (1 - 2 * (Z2q1 * Z2q1 + Z2q2 * Z2q2))) * 180 / Math.PI);

            tMadgwickObrotX1.Text = Convert.ToString(rollMadg1);
            tMadgwickObrotX2.Text = Convert.ToString(rollMadg2);

            pitchMadg1 = Math.Round(-Math.Asin(2 * (Z1q0 * Z1q2 - Z1q3 * Z1q1)) * 180.0 / Math.PI);
            pitchMadg2 = Math.Round(-Math.Asin(2 * (Z2q0 * Z2q2 - Z2q3 * Z2q1)) * 180.0 / Math.PI);

            tMadgwickObrotY1.Text = Convert.ToString(pitchMadg1);
            tMadgwickObrotY2.Text = Convert.ToString(pitchMadg2);

            yawMadg1 = Math.Round(1.034 * Math.Atan2(2 * (Z1q0 * Z1q3 + Z1q1 * Z1q2), (1 - 2 * (Z1q2 * Z1q2 + Z1q3 * Z1q3))) * 180 / Math.PI);
            yawMadg2 = Math.Round(1.066 * Math.Atan2(2 * (Z2q0 * Z2q3 + Z2q1 * Z2q2), (1 - 2 * (Z2q2 * Z2q2 + Z2q3 * Z2q3))) * 180 / Math.PI);

            tMadgwickObrotZ1.Text = Convert.ToString(yawMadg1);
            tMadgwickObrotZ2.Text = Convert.ToString(yawMadg2);
        }

        private void katyMahony()
        {
            var Z1q0 = mahony1Q[0];
            var Z1q1 = mahony1Q[1];
            var Z1q2 = mahony1Q[2];
            var Z1q3 = mahony1Q[3];

            var Z2q0 = mahony2Q[0];
            var Z2q1 = mahony2Q[1];
            var Z2q2 = mahony2Q[2];
            var Z2q3 = mahony2Q[3];

            rollMah1 = Math.Round(Math.Atan2(2 * (Z1q0 * Z1q1 + Z1q2 * Z1q3), (1 - 2 * (Z1q1 * Z1q1 + Z1q2 * Z1q2))) * 180 / Math.PI);
            rollMah2 = Math.Round(Math.Atan2(2 * (Z2q0 * Z2q1 + Z2q2 * Z2q3), (1 - 2 * (Z2q1 * Z2q1 + Z2q2 * Z2q2))) * 180 / Math.PI);

            tMahonyObrotX1.Text = Convert.ToString(rollMah1);
            tMahonyObrotX2.Text = Convert.ToString(rollMah2);

            pitchMah1 = Math.Round(-Math.Asin(2 * (Z1q0 * Z1q2 - Z1q3 * Z1q1)) * 180.0 / Math.PI);
            pitchMah2 = Math.Round(-Math.Asin(2 * (Z2q0 * Z2q2 - Z2q3 * Z2q1)) * 180.0 / Math.PI);

            tMahonyObrotY1.Text = Convert.ToString(pitchMah1);
            tMahonyObrotY2.Text = Convert.ToString(pitchMah2);

            yawMah1 = Math.Round(1.034 * Math.Atan2(2 * (Z1q0 * Z1q3 + Z1q1 * Z1q2), (1 - 2 * (Z1q2 * Z1q2 + Z1q3 * Z1q3))) * 180 / Math.PI);
            yawMah2 = Math.Round(1.066 * Math.Atan2(2 * (Z2q0 * Z2q3 + Z2q1 * Z2q2), (1 - 2 * (Z2q2 * Z2q2 + Z2q3 * Z2q3))) * 180 / Math.PI);

            tMahonyObrotZ1.Text = Convert.ToString(yawMah1);
            tMahonyObrotZ2.Text = Convert.ToString(yawMah2);
        }

        private void KontrolujGyro()
        {
            if (Math.Abs(katyCalkowane[0] - katyAkcel[0]) > 3)
                _trapezKaty.Calka[0] = katyAkcel[0];
        }

        private void kwaternionyMadgwick()
        {


            //_madgwickZestaw1.MadgwickAHRSupdateIMU(mahony1Q, czestotliwosc, akcel1Kalibracja[0], akcel1Kalibracja[1], akcel1Kalibracja[2], gyro1Kalibracja[0], gyro1Kalibracja[1], gyro1Kalibracja[2]);
            //_madgwickZestaw2.MadgwickAHRSupdateIMU(mahony2Q, czestotliwosc, akcel2Kalibracja[0], akcel2Kalibracja[1], akcel2Kalibracja[2], gyro2Kalibracja[0], gyro2Kalibracja[1], gyro2Kalibracja[2]);

            //_madgwickZestaw1.MadgwickAHRSupdate(mahony1Q, czestotliwosc, akcel1Kalibracja[0], akcel1Kalibracja[1], akcel1Kalibracja[2], gyro1Kalibracja[0], gyro1Kalibracja[1], gyro1Kalibracja[2], magnet[0], magnet[1], magnet[2]);
            //_madgwickZestaw2.MadgwickAHRSupdate(mahony2Q, czestotliwosc, akcel2Kalibracja[0], akcel2Kalibracja[1], akcel2Kalibracja[2], gyro2Kalibracja[0], gyro2Kalibracja[1], gyro2Kalibracja[2], magnet[0], magnet[1], magnet[2]);

            MadgwickFilter1.Update(gyro1Kalibracja[0], gyro1Kalibracja[1], gyro1Kalibracja[2], akcel1Kalibracja[0], akcel1Kalibracja[1], akcel1Kalibracja[2]);
            MadgwickFilter2.Update(gyro2Kalibracja[0], gyro2Kalibracja[1], gyro2Kalibracja[2], akcel2Kalibracja[0], akcel2Kalibracja[1], akcel2Kalibracja[2]);
            madgwick1Q = MadgwickFilter1.Quaternion;
            madgwick2Q = MadgwickFilter2.Quaternion;

            //madgwick1Q = _madgwickZestaw1.quaternion;
            //madgwick2Q = _madgwickZestaw2.quaternion;

            tMadgwick1Q0.Text = Convert.ToString(madgwick1Q[0]);
            tMadgwick1Q1.Text = Convert.ToString(madgwick1Q[1]);
            tMadgwick1Q2.Text = Convert.ToString(madgwick1Q[2]);
            tMadgwick1Q3.Text = Convert.ToString(madgwick1Q[3]);

            tMadgwick2Q0.Text = Convert.ToString(madgwick2Q[0]);
            tMadgwick2Q1.Text = Convert.ToString(madgwick2Q[1]);
            tMadgwick2Q2.Text = Convert.ToString(madgwick2Q[2]);
            tMadgwick2Q3.Text = Convert.ToString(madgwick2Q[3]);
        }

        private void kwaternionyMahony()
        {
            _mahonyZestaw1.MahonyAHRSupdateIMU(gyro1Kalibracja[0], gyro1Kalibracja[1], gyro1Kalibracja[2], akcel1Kalibracja[0], akcel1Kalibracja[1], akcel1Kalibracja[2], czestotliwosc);
            _mahonyZestaw2.MahonyAHRSupdateIMU(gyro2Kalibracja[0], gyro2Kalibracja[1], gyro2Kalibracja[2], akcel2Kalibracja[0], akcel2Kalibracja[1], akcel2Kalibracja[2], czestotliwosc);


            mahony1Q[0] = _mahonyZestaw1.q0;
            mahony1Q[1] = _mahonyZestaw1.q1;
            mahony1Q[2] = _mahonyZestaw1.q2;
            mahony1Q[3] = _mahonyZestaw1.q3;

            mahony2Q[0] = _mahonyZestaw2.q0;
            mahony2Q[1] = _mahonyZestaw2.q1;
            mahony2Q[2] = _mahonyZestaw2.q2;
            mahony2Q[3] = _mahonyZestaw2.q3;

            tMahony1Q0.Text = Convert.ToString(mahony1Q[0]);
            tMahony1Q1.Text = Convert.ToString(mahony1Q[1]);
            tMahony1Q2.Text = Convert.ToString(mahony1Q[2]);
            tMahony1Q3.Text = Convert.ToString(mahony1Q[3]);

            tMahony2Q0.Text = Convert.ToString(mahony2Q[0]);
            tMahony2Q1.Text = Convert.ToString(mahony2Q[1]);
            tMahony2Q2.Text = Convert.ToString(mahony2Q[2]);
            tMahony2Q3.Text = Convert.ToString(mahony2Q[3]);
        }

        private void PrzepiszSuroweKaty()
        {
            tSurowyRoll1.Text = Convert.ToString(Math.Round(katyAkcel[0]));
            tSurowyRoll2.Text = Convert.ToString(Math.Round(katyAkcel[2]));

            tSurowyPitch1.Text = Convert.ToString(Math.Round(katyAkcel[1]));
            tSurowyPitch2.Text = Convert.ToString(Math.Round(katyAkcel[3]));
        }

        private void helperKomplementarny()
        {
            var roll1 = Math.Round(_komplementarny.oblicz(katyAkcel[0], gyro1Kalibracja[0], akcel1Kalibracja[0], 1 / czestotliwosc));
            var pitch1 = Math.Round(_komplementarny.oblicz(katyAkcel[1], gyro1Kalibracja[1], akcel1Kalibracja[1], 1 / czestotliwosc));

            var roll2 = Math.Round(_komplementarny.oblicz(katyAkcel[2], gyro2Kalibracja[0], akcel2Kalibracja[0], 1 / czestotliwosc));
            var pitch2 = Math.Round(_komplementarny.oblicz(katyAkcel[3], gyro2Kalibracja[1], akcel2Kalibracja[1], 1 / czestotliwosc));


            tRollKomplementarny1.Text = Convert.ToString(roll1);
            tPitchKomplementarny1.Text = Convert.ToString(pitch1);

            tRollKomplementarny2.Text = Convert.ToString(roll2);
            tPitchKomplementarny2.Text = Convert.ToString(pitch2);
        }

        private void helperCzestotliwosci()
        {
            if (licznikRaspberry % rzad == 0)
            {
                stopWatch.Stop();
                millisecondTime = stopWatch.ElapsedMilliseconds;

                czestotliwosc = Math.Round(rzad * ((1 / (double)millisecondTime) * 1000));
                tCzestotliwosc.Text = Convert.ToString(czestotliwosc);
            }

        }

        private void ObliczKursy()
        {
            var kurs = _odczytKursu.czytajKurs(magnet[1], magnet[0]);
            var kursGeograficzny = kurs + deklinacja;
            var kursFuzja = _odczytKursu.czytajKurs(magnet[0], magnet[1], magnet[2], _katyAkcel1.roll, _katyAkcel1.roll);


            tKursMagnetyczny.Text = Convert.ToString(kurs);
            tKursGeograficzny.Text = Convert.ToString(kursGeograficzny);
            tKursFuzja.Text = Convert.ToString(kursFuzja);
        }

        private void KatyZZyroskopu()
        {
            for (int i = 0; i < 3; i++)
            {
                predkosciKatoweKalibrowane[i] = gyro1Kalibracja[i];
            }

            for (int i = 3; i < 6; i++)
            {
                predkosciKatoweKalibrowane[i] = gyro2Kalibracja[i - 3];
            }

            if (kalibracjaKoniecGyro == true)
            {
                katyCalkowane = _trapezKaty.calkuj(predkosciKatoweKalibrowane, 1 / czestotliwosc) ;

            }

            katyCalkowane = rad2degGyro(katyCalkowane);

            tRollGyro1.Text = Convert.ToString(katyCalkowane[0]);
            tPitchGyro1.Text = Convert.ToString(katyCalkowane[1]);
            tYawGyro1.Text = Convert.ToString(katyCalkowane[2]);

            tRollGyro2.Text = Convert.ToString(katyCalkowane[3]);
            tPitchGyro2.Text = Convert.ToString(katyCalkowane[4]);
            tYawGyro2.Text = Convert.ToString(katyCalkowane[5]);

        }

        public double[] rad2degGyro(double[] wejscie)
        {
            double[] wyjscie = new double[6];

            for (int i = 0; i < 6; i++)
            {
                wyjscie[i] = Math.Round(wejscie[i] * 180.0 / Math.PI, 4);
            }

            return wyjscie;
        }

        private void KatyZAkcelerometru()
        {

            for (int i = 0; i < 3; i++)
            {
                przyspieszeniaKalibrowane[i] = akcel1Kalibracja[i];
            }

            for (int i = 3; i < 6; i++)
            {
                przyspieszeniaKalibrowane[i] = akcel2Kalibracja[i - 3];
            }


            _katyAkcel1.PrzeliczKaty(akcel1Kalibracja[0], akcel1Kalibracja[1], akcel1Kalibracja[2]);
            _katyAkcel2.PrzeliczKaty(akcel2Kalibracja[0], akcel2Kalibracja[1], akcel2Kalibracja[2]);

            katyAkcel[0] = Math.Round( GainAkcel1[0] * _katyAkcel1.roll,2);
            katyAkcel[2] = Math.Round(GainAkcel2[0] * _katyAkcel2.roll);
            katyAkcel[1] = Math.Round(GainAkcel1[1] * _katyAkcel1.pitch);
            katyAkcel[3] = Math.Round(GainAkcel2[1] * _katyAkcel2.pitch);

            tRollAkcel1.Text = Convert.ToString(katyAkcel[0]);
            tRollAkcel2.Text = Convert.ToString(katyAkcel[2]);

            tPitchAkcel1.Text = Convert.ToString(katyAkcel[1]);
            tPitchAkcel2.Text = Convert.ToString(katyAkcel[3]);

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

            
        }

        private void kalibracjaAkcelerometrów()
        {
            akcel1Kalibracja[0] = Math.Round(akcel1[0]  - _kalibracjaBiasAkcel1.SredniaX, 4);
            akcel1Kalibracja[1] = Math.Round(akcel1[1]  - _kalibracjaBiasAkcel1.SredniaY, 4);
            akcel1Kalibracja[2] = Math.Round(akcel1[2] * (1/_kalibracjaBiasAkcel1.SredniaZ), 4);

            tAkcel1XKalib.Text = Convert.ToString(akcel1Kalibracja[0]);
            tAkcel1YKalib.Text = Convert.ToString(akcel1Kalibracja[1]);
            tAkcel1ZKalib.Text = Convert.ToString(akcel1Kalibracja[2]);

            akcel2Kalibracja[0] = Math.Round(akcel2[0]  - _kalibracjaBiasAkcel2.SredniaX, 4);
            akcel2Kalibracja[1] = Math.Round(akcel2[1]  - _kalibracjaBiasAkcel2.SredniaY, 4);
            akcel2Kalibracja[2] = Math.Round(akcel2[2] * (1/ _kalibracjaBiasAkcel1.SredniaZ), 4);

            tAkcel2XKalib.Text = Convert.ToString(akcel2Kalibracja[0]);
            tAkcel2YKalib.Text = Convert.ToString(akcel2Kalibracja[1]);
            tAkcel2ZKalib.Text = Convert.ToString(akcel2Kalibracja[2]);
        }

        private void kalibracjaMagnetometru ()
        {
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
            //sprawdzZyroskopy();
           // sprawdzAkcelerometry();
            //sprawdzMagnetometry();
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

        //private void sprawdzZyroskopy()
        //{
        //    if (checkGyro1X.Checked)
        //    {
        //        wykres1.Series["Oś X - L3G4200D"].Enabled = true;
        //    }

        //    if (!checkGyro1X.Checked)
        //    {
        //        wykres1.Series["Oś X - L3G4200D"].Enabled = false;
        //    }

        //    if (checkGyro1Y.Checked)
        //    {
        //        wykres1.Series["Oś Y - L3G4200D"].Enabled = true;
        //    }

        //    if (!checkGyro1Y.Checked)
        //    {
        //        wykres1.Series["Oś Y - L3G4200D"].Enabled = false;
        //    }

        //    if (checkGyro1Z.Checked)
        //    {
        //        wykres1.Series["Oś Z - L3G4200D"].Enabled = true;
        //    }

        //    if (!checkGyro1Z.Checked)
        //    {
        //        wykres1.Series["Oś Z - L3G4200D"].Enabled = false;
        //    }
        //    //*****************************************************
        //    if (checkGyro2X.Checked)
        //    {
        //        wykres1.Series["Oś X - MPU6050"].Enabled = true;
        //    }

        //    if (!checkGyro2X.Checked)
        //    {
        //        wykres1.Series["Oś X - MPU6050"].Enabled = false;
        //    }

        //    if (checkGyro2Y.Checked)
        //    {
        //        wykres1.Series["Oś Y - MPU6050"].Enabled = true;
        //    }

        //    if (!checkGyro2Y.Checked)
        //    {
        //        wykres1.Series["Oś Y - MPU6050"].Enabled = false;
        //    }

        //    if (checkGyro2Z.Checked)
        //    {
        //        wykres1.Series["Oś Z - MPU6050"].Enabled = true;
        //    }

        //    if (!checkGyro2Z.Checked)
        //    {
        //        wykres1.Series["Oś Z - MPU6050"].Enabled = false;
        //    }
        //}

        //private void sprawdzAkcelerometry()
        //{
        //    if (checkAkcel1X.Checked)
        //    {
        //        wykres2.Series["Oś X - ADXL345"].Enabled = true;
        //    }

        //    if (!checkAkcel1X.Checked)
        //    {
        //        wykres2.Series["Oś X - ADXL345"].Enabled = false;
        //    }

        //    if (checkAkcel1Y.Checked)
        //    {
        //        wykres2.Series["Oś Y - ADXL345"].Enabled = true;
        //    }

        //    if (!checkAkcel1Y.Checked)
        //    {
        //        wykres2.Series["Oś Y - ADXL345"].Enabled = false;
        //    }

        //    if (checkAkcel1Z.Checked)
        //    {
        //        wykres2.Series["Oś Z - ADXL345"].Enabled = true;
        //    }

        //    if (!checkAkcel1Z.Checked)
        //    {
        //        wykres2.Series["Oś Z - ADXL345"].Enabled = false;
        //    }

        //    if (checkAkcel2X.Checked)
        //    {
        //        wykres2.Series["Oś X - MPU6050"].Enabled = true;
        //    }

        //    if (!checkAkcel2X.Checked)
        //    {
        //        wykres2.Series["Oś X - MPU6050"].Enabled = false;
        //    }

        //    if (checkAkcel2Y.Checked)
        //    {
        //        wykres2.Series["Oś Y - MPU6050"].Enabled = true;
        //    }

        //    if (!checkAkcel2Y.Checked)
        //    {
        //        wykres2.Series["Oś Y - MPU6050"].Enabled = false;
        //    }

        //    if (checkAkcel2Z.Checked)
        //    {
        //        wykres2.Series["Oś Z - MPU6050"].Enabled = true;
        //    }

        //    if (!checkAkcel2Z.Checked)
        //    {
        //        wykres2.Series["Oś Z - MPU6050"].Enabled = false;
        //    }

        //}

        //private void sprawdzMagnetometry()
        //{
        //    if (checkMagnet1X.Checked)
        //    {
        //        wykres3.Series["Oś X - HMC5883L"].Enabled = true;
        //    }

        //    if (!checkMagnet1X.Checked)
        //    {
        //        wykres3.Series["Oś X - HMC5883L"].Enabled = false;
        //    }

        //    if (checkMagnet1Y.Checked)
        //    {
        //        wykres3.Series["Oś Y - HMC5883L"].Enabled = true;
        //    }

        //    if (!checkMagnet1Y.Checked)
        //    {
        //        wykres3.Series["Oś Y - HMC5883L"].Enabled = false;
        //    }

        //    if (checkMagnet1Z.Checked)
        //    {
        //        wykres3.Series["Oś Z - HMC5883L"].Enabled = true;
        //    }

        //    if (!checkMagnet1Z.Checked)
        //    {
        //        wykres3.Series["Oś Z - HMC5883L"].Enabled = false;
        //    }
        //}

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

        private void KalibrujMagnetometr()
        {
           

            for (int i = 0; i < 150000; i++)
            {
                _kalibracjaMagnetometru.AddValues(magnet[0], magnet[1], magnet[2]);

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
            kalibracjaKoniecGyro = false;

            for (int i = 1; i < 1500; i++)
            {
                _kalibracjaBiasGyro1.kalibruj(gyro1[0], gyro1[1], gyro1[2], i);
                _kalibracjaBiasGyro2.kalibruj(gyro2[0], gyro2[1], gyro2[2], i);


                progressBar1.Value = i;
            }

            tBiasGyro1X.Text = Convert.ToString(_kalibracjaBiasGyro1.SredniaX);
            tBiasGyro1Y.Text = Convert.ToString(_kalibracjaBiasGyro1.SredniaY);
            tBiasGyro1Z.Text = Convert.ToString(_kalibracjaBiasGyro1.SredniaZ);

            tBiasGyro2X.Text = Convert.ToString(_kalibracjaBiasGyro2.SredniaX);
            tBiasGyro2Y.Text = Convert.ToString(_kalibracjaBiasGyro2.SredniaY);
            tBiasGyro2Z.Text = Convert.ToString(_kalibracjaBiasGyro2.SredniaZ);

            kalibracjaKoniecGyro = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            _kalibracjaMagnetometru.czysc();
            tGainMagnetX.Clear();
            tGainMagnetY.Clear();
            tGainMagnetZ.Clear();

            tOffsetMagnetX.Clear();
            tOffsetMagnetY.Clear();
            tOffsetMagnetZ.Clear();

            t3DMagnet.Clear();

            tMaxMagnetX.Clear();
            tMaxMagnetY.Clear();
            tMaxMagnetZ.Clear();

            tMinMagnetX.Clear();
            tMinMagnetY.Clear();
            tMinMagnetZ.Clear();

        }

        private void button7_Click(object sender, EventArgs e)
        {
            deklinacja = Convert.ToDouble(tDeklinacja.Text);   
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _kalibracjaBiasAkcel1.zeruj();
            _kalibracjaBiasAkcel2.zeruj();

            kalibracjaKoniecAkcel = false;

            for (var i = 1; i < 1500; i++)
            {
                _kalibracjaBiasAkcel1.kalibruj(i, akcel1[0], akcel1[1], akcel1[2]);
                _kalibracjaBiasAkcel2.kalibruj(i, akcel2[0], akcel2[1], akcel2[2]);

                progressBar2.Value = i;
            }

            tBiasAkcel1X.Text = Convert.ToString(_kalibracjaBiasAkcel1.SredniaX);
            tBiasAkcel1Y.Text = Convert.ToString(_kalibracjaBiasAkcel1.SredniaY);
            tBiasAkcel1Z.Text = Convert.ToString(Math.Round(1/_kalibracjaBiasAkcel1.SredniaZ,2));

            tBiasAkcel2X.Text = Convert.ToString(_kalibracjaBiasAkcel2.SredniaX);
            tBiasAkcel2Y.Text = Convert.ToString(_kalibracjaBiasAkcel2.SredniaY);
            tBiasAkcel2Z.Text = Convert.ToString(Math.Round(1/_kalibracjaBiasAkcel2.SredniaZ,2));

            kalibracjaKoniecAkcel = true;

        }

        private void bAxisX1_Click(object sender, EventArgs e)
        {
            _kalWspolGyro1X.kalibruj(katyCalkowane[0], licznikWspolczynnikowGyro[0]);

            bAxisX1.Text = Convert.ToString(licznikWspolczynnikowGyro[0]);

            if (licznikWspolczynnikowGyro[0] == 10)
            {
                if (_kalWspolGyro1X.Srednia > 0)
                     GainGyro1[0] = 90.0 /_kalWspolGyro1X.Srednia ;

                if (_kalWspolGyro1X.Srednia < 0)
                    GainGyro1[0] = -90.0 / _kalWspolGyro1X.Srednia;

                tGainGyro1X.Text = Convert.ToString(GainGyro1[0]);
                licznikWspolczynnikowGyro[0] = 0;
                _kalWspolGyro1X.zeruj();
            }
            licznikWspolczynnikowGyro[0]++;
        }

        private void bAxisY1_Click(object sender, EventArgs e)
        {
            _kalWspolGyro1Y.kalibruj(katyCalkowane[1], licznikWspolczynnikowGyro[1]);

            bAxisY1.Text = Convert.ToString(licznikWspolczynnikowGyro[1]);

            if (licznikWspolczynnikowGyro[1] == 10)
            {
                if (_kalWspolGyro1Y.Srednia > 0)
                    GainGyro1[1] = 90.0 / _kalWspolGyro1Y.Srednia;

                if (_kalWspolGyro1Y.Srednia < 0)
                    GainGyro1[1] = -90.0 / _kalWspolGyro1Y.Srednia;

                tGainGyro1Y.Text = Convert.ToString(GainGyro1[1]);
                licznikWspolczynnikowGyro[1] = 0;
                _kalWspolGyro1Y.zeruj();
            }
            licznikWspolczynnikowGyro[1]++;
        }

        private void bAxisZ1_Click(object sender, EventArgs e)
        {
            _kalWspolGyro1Z.kalibruj(katyCalkowane[2], licznikWspolczynnikowGyro[2]);

            bAxisZ1.Text = Convert.ToString(licznikWspolczynnikowGyro[2]);

            if (licznikWspolczynnikowGyro[2] == 10)
            {
                if (_kalWspolGyro1Z.Srednia > 0)
                    GainGyro1[2] = 90.0 / _kalWspolGyro1Z.Srednia;

                if (_kalWspolGyro1Z.Srednia < 0)
                    GainGyro1[2] = -90.0 / _kalWspolGyro1Z.Srednia;

                tGainGyro1Z.Text = Convert.ToString(GainGyro1[2]);
                licznikWspolczynnikowGyro[2] = 0;
                _kalWspolGyro1Z.zeruj();
            }
            licznikWspolczynnikowGyro[2]++;
        }

        private void bAxisX2_Click(object sender, EventArgs e)
        {
            _kalWspolGyro2X.kalibruj(katyCalkowane[3], licznikWspolczynnikowGyro[3]);

            bAxisX2.Text = Convert.ToString(licznikWspolczynnikowGyro[3]);

            if (licznikWspolczynnikowGyro[3] == 10)
            {
                if (_kalWspolGyro2X.Srednia > 0)
                    GainGyro2[0] = 90.0 / _kalWspolGyro2X.Srednia;

                if (_kalWspolGyro2X.Srednia < 0)
                    GainGyro2[0] = -90.0 / _kalWspolGyro2X.Srednia;

                tGainGyro2X.Text = Convert.ToString(GainGyro2[0]);
                licznikWspolczynnikowGyro[3] = 0;
                _kalWspolGyro2X.zeruj();
            }
            licznikWspolczynnikowGyro[3]++;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            _kalWspolGyro2Y.kalibruj(katyCalkowane[4], licznikWspolczynnikowGyro[4]);

            bAxisY2.Text = Convert.ToString(licznikWspolczynnikowGyro[4]);

            if (licznikWspolczynnikowGyro[4] == 10)
            {
                if (_kalWspolGyro2Y.Srednia > 0)
                    GainGyro2[1] = 90.0 / _kalWspolGyro2Y.Srednia;

                if (_kalWspolGyro2Y.Srednia < 0)
                    GainGyro2[1] = -90.0 / _kalWspolGyro2Y.Srednia;

                tGainGyro2Y.Text = Convert.ToString(GainGyro2[1]);
                licznikWspolczynnikowGyro[4] = 0;
                _kalWspolGyro2Y.zeruj();
            }
            licznikWspolczynnikowGyro[4]++;

        }

        private void bAxisZ2_Click(object sender, EventArgs e)
        {
            _kalWspolGyro2Z.kalibruj(katyCalkowane[5], licznikWspolczynnikowGyro[5]);

            bAxisZ2.Text = Convert.ToString(licznikWspolczynnikowGyro[5]);

            if (licznikWspolczynnikowGyro[5] == 10)
            {
                if (_kalWspolGyro2Z.Srednia > 0)
                    GainGyro2[2] = 90.0 / _kalWspolGyro2Z.Srednia;

                if (_kalWspolGyro2Z.Srednia < 0)
                    GainGyro2[2] = -90.0 / _kalWspolGyro2Z.Srednia;

                tGainGyro2Z.Text = Convert.ToString(GainGyro2[2]);
                licznikWspolczynnikowGyro[5] = 0;
                _kalWspolGyro2Z.zeruj();
            }
            licznikWspolczynnikowGyro[5]++;
        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            Wykres.Series.Clear();
        }

        private void button13_Click(object sender, EventArgs e)
        {

            _trapezKaty.zerujWszystkie();
           

        }

        private void bWspX1_Click(object sender, EventArgs e)
        {
            _kalWspolAkcel1X.kalibruj(katyAkcel[0], licznikWspolczynnikowAkcel[0]);

            bWspX1.Text = Convert.ToString(licznikWspolczynnikowAkcel[0]);

            if (licznikWspolczynnikowAkcel[0] == 10)
            {
                if (_kalWspolAkcel1X.Srednia > 0)
                    GainAkcel1[0] = Math.Round (90 / _kalWspolAkcel1X.Srednia,2);

                if (_kalWspolAkcel1X.Srednia < 0)
                    GainAkcel1[0] = Math.Round(- 90 / _kalWspolAkcel1X.Srednia,2);
                
                tGainAkcel1X.Text = Convert.ToString(GainAkcel1[0]);
                licznikWspolczynnikowAkcel[0] = 0;
                _kalWspolAkcel1X.zeruj();
            }
            licznikWspolczynnikowAkcel[0]++;
        }

        private void bWspY1_Click(object sender, EventArgs e)
        {
            _kalWspolAkcel1Y.kalibruj(katyAkcel[1], licznikWspolczynnikowAkcel[1]);

            bWspY1.Text = Convert.ToString(licznikWspolczynnikowAkcel[1]);

            if (licznikWspolczynnikowAkcel[1] == 10)
            {
                if (_kalWspolAkcel1Y.Srednia > 0)
                    GainAkcel1[1] = Math.Round(90 / _kalWspolAkcel1Y.Srednia, 2);

                if (_kalWspolAkcel1Y.Srednia < 0)
                    GainAkcel1[1] = Math.Round(-90 / _kalWspolAkcel1Y.Srednia, 2);

                tGainAkcel1Y.Text = Convert.ToString(GainAkcel1[1]);
                licznikWspolczynnikowAkcel[1] = 0;
                _kalWspolAkcel1Y.zeruj();
            }
            licznikWspolczynnikowAkcel[1]++;

        }

        private void bWspX2_Click(object sender, EventArgs e)
        {
            _kalWspolAkcel2X.kalibruj(katyAkcel[2], licznikWspolczynnikowAkcel[2]);

            bWspX2.Text = Convert.ToString(licznikWspolczynnikowAkcel[2]);

            if (licznikWspolczynnikowAkcel[2] == 10)
            {
                if (_kalWspolAkcel2X.Srednia > 0)
                    GainAkcel2[0] = Math.Round(90 / _kalWspolAkcel2X.Srednia, 2);

                if (_kalWspolAkcel2X.Srednia < 0)
                    GainAkcel2[0] = Math.Round(-90 / _kalWspolAkcel2X.Srednia, 2);

                tGainAkcel2X.Text = Convert.ToString(GainAkcel2[0]);
                licznikWspolczynnikowAkcel[2] = 0;
                _kalWspolAkcel2X.zeruj();
            }
            licznikWspolczynnikowAkcel[2]++;
        }

        private void bWspY2_Click(object sender, EventArgs e)
        {
            _kalWspolAkcel2Y.kalibruj(katyAkcel[3], licznikWspolczynnikowAkcel[3]);

            bWspY2.Text = Convert.ToString(licznikWspolczynnikowAkcel[3]);

            if (licznikWspolczynnikowAkcel[3] == 10)
            {
                if (_kalWspolAkcel2Y.Srednia > 0)
                    GainAkcel2[1] = Math.Round(90 / _kalWspolAkcel2Y.Srednia, 2);

                if (_kalWspolAkcel2Y.Srednia < 0)
                    GainAkcel2[1] = Math.Round(-90 / _kalWspolAkcel2Y.Srednia, 2);

                tGainAkcel2Y.Text = Convert.ToString(GainAkcel2[1]);
                licznikWspolczynnikowAkcel[3] = 0;
                _kalWspolAkcel2Y.zeruj();
            }
            licznikWspolczynnikowAkcel[3]++;
        }
    }
}

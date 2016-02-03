using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Filtering;
using Sensors;
using Obliczanie;
    

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

        public string rx_str = " ";
        public int licznik = 0;

        Stopwatch stopWatch = new Stopwatch();

        Konwerter konwerter = new Konwerter();

        Giroskop L3G4200D = new Giroskop();
        Giroskop MPU6050_Giro = new Giroskop();

        Akcelerometr ADXL345 = new Akcelerometr();
        Akcelerometr MPU6050_Akcel = new Akcelerometr();

        Magnetometr HMC5883L = new Magnetometr(3000, 3000, 3000);

        Kurs _kurs = new Kurs();


        private void button1_Click(object sender, EventArgs e)
        {


            if (serialPort1.IsOpen == false)
            {
                try
                {
                    button1.Text = "Rozłącz";
                    serialPort1.BaudRate = 115200;
                    serialPort1.PortName = "COM7";//comboBox1.Text;
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
                try
                {
                    rx_str = serialPort1.ReadTo("\r\n"); // przekazanie odebranego łańcucha do zmiennej rx_str
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);


                }

                this.Invoke(new EventHandler(rx_parse)); // instalacja zdarzenia parsującego odebrany łańcuch
            }

        }

        private void rx_parse(object sender, EventArgs e)
        {
            if (konwerter.licznik % rzad == 0)
            {
                stopWatch.Restart();
            }

            string[] dane = new string[16];
            dane = rx_str.Split(',');

            /* KONWERSJA DANYCH*/
            konwerter.konwertujDane(dane);//ok
            wypiszDane();//ok
            /*--------------------------------*/
                    
            
            /*ODBIÓR I KALIBRACJA MAGNETOMETR*/
            if (radioButton1.Checked)
            {
                HMC5883L.dodajPomiar(konwerter.hmc5883l);//ok
                HMC5883L.kalibruj();//ok
            }
            HMC5883L.aktualizacja();//ok

            wypiszKalibracjaMagnetometru();//ok
            /*---------------------------------*/
            
            
            /*ODBIÓR I KALIBRACJA ŻYROSKOPY*/
            L3G4200D.dodajPomiar(konwerter.l3g4200d);//ok
            MPU6050_Giro.dodajPomiar(konwerter.mpu6050_G);//ok

            kalibrujBiasGyro(1500);//ok

            L3G4200D.aktualizacja();//ok
            MPU6050_Giro.aktualizacja();//ok

            wypiszGyroPoKalibracji();//ok

            if ((L3G4200D.koniecKalibracji == true) && (MPU6050_Giro.koniecKalibracji == true))
            {
                L3G4200D.obliczKaty(czestotliwosc);//ok
                MPU6050_Giro.obliczKaty(czestotliwosc);//ok
            }
            wypiszKatyGiro();//ok
            /*---------------------------*/
            

            /*ODBIÓR I KALIBRACJA AKCELEROMETRY*/
            ADXL345.dodajPomiar(konwerter.adxl345);//ok
            MPU6050_Akcel.dodajPomiar(konwerter.mpu6050_A);//ok

            kalibrujBiasAkcel(1500);//ok

            ADXL345.aktualizacja();//ok
            MPU6050_Akcel.aktualizacja();//ok

            wypiszAkcelPoKalibracji();//ok

            if ((ADXL345.koniecKalibracji == true) && (MPU6050_Akcel.koniecKalibracji == true))
            {
                ADXL345.obliczKaty();//ok
                MPU6050_Akcel.obliczKaty();//ok
            }
            wypiszKatyAccel();//ok
                              /*------------------------------------*/

            /*OBLICZANIE KURSÓW*/
            _kurs.obliczPlaskiKurs(HMC5883L.bKal[0], HMC5883L.bKal[1]);
            _kurs.obliczDynamicznyKurs(HMC5883L.bKal[0], HMC5883L.bKal[1], HMC5883L.bKal[2], ADXL345.euler[0], ADXL345.euler[1]);

            try
            {
                _kurs.uwzglednijDeklinacje(Convert.ToDouble(tDeklinacja.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                tDeklinacja.Text = "0";
            }

            wypiszKursy();

            /*-----------------*/



            //ObliczKursy();
            //PrzepiszSuroweKaty();

            //KontrolujGyro();

            //test



            helperCzestotliwosci();
            //licznikWykresu++;

        }

        private void kalibrujBiasAkcel(int iloscProbek)//ok
        {
            progressBar2.Maximum = iloscProbek;

            if ((ADXL345.startKalibracji == true) && (MPU6050_Akcel.startKalibracji == true) &&
                (ADXL345.licznikKalibracji < iloscProbek) && (MPU6050_Akcel.licznikKalibracji < iloscProbek))
            {
                ADXL345.kalibruj(ADXL345.accel[0], ADXL345.accel[1], ADXL345.accel[2], ADXL345.licznikKalibracji);
                MPU6050_Akcel.kalibruj(MPU6050_Akcel.accel[0], MPU6050_Akcel.accel[1], MPU6050_Akcel.accel[2], MPU6050_Akcel.licznikKalibracji);

                progressBar2.Value = ADXL345.licznikKalibracji;
                ADXL345.licznikKalibracji++;
                MPU6050_Akcel.licznikKalibracji++;
            }

            if (ADXL345.licznikKalibracji == iloscProbek)
            {
                ADXL345.koniecKalibracji = true;
                MPU6050_Akcel.koniecKalibracji = true;

                ADXL345.licznikKalibracji = 0;
                ADXL345.startKalibracji = false;

                MPU6050_Akcel.licznikKalibracji = 0;
                MPU6050_Akcel.startKalibracji = false;

                wypiszBiasAkcel();
            }
        }

        private void kalibrujBiasGyro(int iloscProbek)//ok
        {
            progressBar1.Maximum = iloscProbek;

            if ((L3G4200D.startKalibracji == true) && (MPU6050_Giro.startKalibracji == true) &&
                (L3G4200D.licznikKalibracji < iloscProbek) && (MPU6050_Giro.licznikKalibracji < iloscProbek))
            {
                L3G4200D.kalibruj(L3G4200D.omega[0], L3G4200D.omega[1], L3G4200D.omega[2], L3G4200D.licznikKalibracji);
                MPU6050_Giro.kalibruj(MPU6050_Giro.omega[0], MPU6050_Giro.omega[1], MPU6050_Giro.omega[2], MPU6050_Giro.licznikKalibracji);


                progressBar1.Value = L3G4200D.licznikKalibracji;
                L3G4200D.licznikKalibracji++;
                MPU6050_Giro.licznikKalibracji++;
            }

            if (L3G4200D.licznikKalibracji == iloscProbek)
            {
                L3G4200D.koniecKalibracji = true;
                MPU6050_Giro.koniecKalibracji = true;

                L3G4200D.licznikKalibracji = 0;
                L3G4200D.startKalibracji = false;

                MPU6050_Giro.licznikKalibracji = 0;
                MPU6050_Giro.startKalibracji = false;
                wypiszBiasGyro();
            }
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

        /* private void katyMadgwick()
        {
            var Z1q0 = madgwick1Q[0];
            var Z1q1 = madgwick1Q[1];
            var Z1q2 = madgwick1Q[2];
            var Z1q3 = madgwick1Q[3];

            var Z2q0 = madgwick2Q[0];
            var Z2q1 = madgwick2Q[1];
            var Z2q2 = madgwick2Q[2];
            var Z2q3 = madgwick2Q[3];

            rollMadg1 = Math.Round(Math.Atan2(2 * (Z1q0 * Z1q1 + Z1q2 * Z1q3), (1 - 2 * (Z1q1 * Z1q1 + Z1q2 * Z1q2))) * 180 / Math.PI);
            rollMadg2 = Math.Round(Math.Atan2(2 * (Z2q0 * Z2q1 + Z2q2 * Z2q3), (1 - 2 * (Z2q1 * Z2q1 + Z2q2 * Z2q2))) * 180 / Math.PI);

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
        }*/

        /* private void katyMahony()
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
        }*/

        /* private void KontrolujGyro()
        {
            if (Math.Abs(katyCalkowane[0] - katyAkcel[0]) > 3)
                _trapezKaty.Calka[0] = katyAkcel[0];
        }*/

        /* private void kwaternionyMadgwick()
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
        }*/

        /*  private void kwaternionyMahony()
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
        }*/
     
        /*private void helperKomplementarny()
        {
            var roll1 = Math.Round(_komplementarny.oblicz(katyAkcel[0], gyro1Kalibracja[0], akcel1Kalibracja[0], 1 / czestotliwosc));
            var pitch1 = Math.Round(_komplementarny.oblicz(katyAkcel[1], gyro1Kalibracja[1], akcel1Kalibracja[1], 1 / czestotliwosc));

            var roll2 = Math.Round(_komplementarny.oblicz(katyAkcel[2], gyro2Kalibracja[0], akcel2Kalibracja[0], 1 / czestotliwosc));
            var pitch2 = Math.Round(_komplementarny.oblicz(katyAkcel[3], gyro2Kalibracja[1], akcel2Kalibracja[1], 1 / czestotliwosc));


            tRollKomplementarny1.Text = Convert.ToString(roll1);
            tPitchKomplementarny1.Text = Convert.ToString(pitch1);

            tRollKomplementarny2.Text = Convert.ToString(roll2);
            tPitchKomplementarny2.Text = Convert.ToString(pitch2);
        }*/

        private void helperCzestotliwosci()//ok
        {
            if (konwerter.licznik % rzad == 0)
            {
                stopWatch.Stop();
                millisecondTime = stopWatch.ElapsedMilliseconds;

                czestotliwosc = Math.Round(rzad * ((1 / (double)millisecondTime) * 1000));
                tCzestotliwosc.Text = Convert.ToString(czestotliwosc);
            }
        }

        private void wypiszKursy()
        {
            tKursMagnetyczny.Text = Convert.ToString(_kurs.kursPlaski);
            tKursGeograficzny.Text = Convert.ToString(_kurs.kursDeklinacja);
            tKursFuzja.Text = Convert.ToString(_kurs.kursDynamiczny);
        }
        

        private void wypiszKatyGiro()//ok
        {
            tRollGyro1.Text = Convert.ToString(L3G4200D.euler[0]);
            tPitchGyro1.Text = Convert.ToString(L3G4200D.euler[1]);
            tYawGyro1.Text = Convert.ToString(L3G4200D.euler[2]);

            tRollGyro2.Text = Convert.ToString(MPU6050_Giro.euler[0]);
            tPitchGyro2.Text = Convert.ToString(MPU6050_Giro.euler[1]);
            tYawGyro2.Text = Convert.ToString(MPU6050_Giro.euler[2]);
        }

        private void wypiszKatyAccel()//ok
        {
            tRollAkcel1.Text = Convert.ToString(ADXL345.euler[0]);
            tRollAkcel2.Text = Convert.ToString(MPU6050_Akcel.euler[0]);

            tPitchAkcel1.Text = Convert.ToString(ADXL345.euler[1]);
            tPitchAkcel2.Text = Convert.ToString(MPU6050_Akcel.euler[1]);
        }

        private void wypiszGyroPoKalibracji()//ok
        {
            tGyro1XKalib.Text = Convert.ToString(L3G4200D.omegaKal[0]);
            tGyro1YKalib.Text = Convert.ToString(L3G4200D.omegaKal[1]);
            tGyro1ZKalib.Text = Convert.ToString(L3G4200D.omegaKal[2]);

            tGyro2XKalib.Text = Convert.ToString(MPU6050_Giro.omegaKal[0]);
            tGyro2YKalib.Text = Convert.ToString(MPU6050_Giro.omegaKal[1]);
            tGyro2ZKalib.Text = Convert.ToString(MPU6050_Giro.omegaKal[2]);
        }

        private void wypiszAkcelPoKalibracji()//ok
        {
            tAkcel1XKalib.Text = Convert.ToString(ADXL345.akcelKal[0]);
            tAkcel1YKalib.Text = Convert.ToString(ADXL345.akcelKal[1]);
            tAkcel1ZKalib.Text = Convert.ToString(ADXL345.akcelKal[2]);

            tAkcel2XKalib.Text = Convert.ToString(MPU6050_Akcel.akcelKal[0]);
            tAkcel2YKalib.Text = Convert.ToString(MPU6050_Akcel.akcelKal[1]);
            tAkcel2ZKalib.Text = Convert.ToString(MPU6050_Akcel.akcelKal[2]);
        }

        private void wypiszKalibracjaMagnetometru()//ok
        {
            tMinMagnetX.Text = Convert.ToString(HMC5883L.MinX);
            tMinMagnetY.Text = Convert.ToString(HMC5883L.MinY);
            tMinMagnetZ.Text = Convert.ToString(HMC5883L.MinZ);

            tMaxMagnetX.Text = Convert.ToString(HMC5883L.MaxX);
            tMaxMagnetY.Text = Convert.ToString(HMC5883L.MaxY);
            tMaxMagnetZ.Text = Convert.ToString(HMC5883L.MaxZ);

            tOffsetMagnetX.Text = Convert.ToString(HMC5883L.OffsetX);
            tOffsetMagnetY.Text = Convert.ToString(HMC5883L.OffsetY);
            tOffsetMagnetZ.Text = Convert.ToString(HMC5883L.OffsetZ);

            tGainMagnetX.Text = Convert.ToString(HMC5883L.GainX);
            tGainMagnetY.Text = Convert.ToString(HMC5883L.GainY);
            tGainMagnetZ.Text = Convert.ToString(HMC5883L.GainZ);

            t3DMagnet.Text = Convert.ToString(HMC5883L.Wektor3D);

            tMagnet1XKalib.Text = Convert.ToString(HMC5883L.bKal[0]);
            tMagnet1YKalib.Text = Convert.ToString(HMC5883L.bKal[1]);
            tMagnet1ZKalib.Text = Convert.ToString(HMC5883L.bKal[2]);
        }

        private void wypiszDane()//ok
        {
            textBox16.Text = Convert.ToString(konwerter.licznik);

            textBox1.Text = Convert.ToString(konwerter.l3g4200d[0]);
            textBox2.Text = Convert.ToString(konwerter.l3g4200d[1]);
            textBox3.Text = Convert.ToString(konwerter.l3g4200d[2]);

            textBox4.Text = Convert.ToString(konwerter.adxl345[0]);
            textBox5.Text = Convert.ToString(konwerter.adxl345[1]);
            textBox6.Text = Convert.ToString(konwerter.adxl345[2]);

            textBox10.Text = Convert.ToString(konwerter.mpu6050_G[0]);
            textBox11.Text = Convert.ToString(konwerter.mpu6050_G[1]);
            textBox12.Text = Convert.ToString(konwerter.mpu6050_G[2]);

            textBox13.Text = Convert.ToString(konwerter.mpu6050_A[0]);
            textBox14.Text = Convert.ToString(konwerter.mpu6050_A[1]);
            textBox15.Text = Convert.ToString(konwerter.mpu6050_A[2]);

            textBox7.Text = Convert.ToString(konwerter.hmc5883l[0]);
            textBox8.Text = Convert.ToString(konwerter.hmc5883l[1]);
            textBox9.Text = Convert.ToString(konwerter.hmc5883l[2]);
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

        private void button4_Click(object sender, EventArgs e)//ok
        {
            this.Hide();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)//ok
        {
            this.Show();
        }

        private void button5_Click(object sender, EventArgs e)//ok
        {
            this.Close();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)//ok
        {
            this.Hide();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)//ok
        {
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)//ok
        {
            if (serialPort1.IsOpen == false)
            {
                string[] port = System.IO.Ports.SerialPort.GetPortNames();
                foreach (string item in port)
                {
                    comboBox1.Items.Add(item);
                }
                comboBox1.Text = port[0]; //pierwszym elementem wyswietlanym w comboboxie będzie pierwszy port
            }
        }

        private void pokażToolStripMenuItem_Click(object sender, EventArgs e)//ok
        {
            this.Show();
        }

        private void wypiszBiasGyro()//ok
        {
            tBiasGyro1X.Text = Convert.ToString(L3G4200D.offset[0]);
            tBiasGyro1Y.Text = Convert.ToString(L3G4200D.offset[1]);
            tBiasGyro1Z.Text = Convert.ToString(L3G4200D.offset[2]);

            tBiasGyro2X.Text = Convert.ToString(MPU6050_Giro.offset[0]);
            tBiasGyro2Y.Text = Convert.ToString(MPU6050_Giro.offset[1]);
            tBiasGyro2Z.Text = Convert.ToString(MPU6050_Giro.offset[2]);
        }

        private void button2_Click(object sender, EventArgs e)//ok
        {
            L3G4200D.zeruj();
            MPU6050_Giro.zeruj();

            L3G4200D.zerujKaty();
            MPU6050_Giro.zerujKaty();

            L3G4200D.koniecKalibracji = false;
            MPU6050_Giro.koniecKalibracji = false;

            L3G4200D.startKalibracji = true;
            MPU6050_Giro.startKalibracji = true;
        }

        private void button6_Click(object sender, EventArgs e)//ok
        {
            HMC5883L.czysc();
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
            //deklinacja = Convert.ToDouble(tDeklinacja.Text);
        }
private void button3_Click(object sender, EventArgs e)//ok
        {
            ADXL345.zeruj();
            MPU6050_Akcel.zeruj();

            ADXL345.koniecKalibracji = false;
            MPU6050_Akcel.koniecKalibracji = false;

            ADXL345.startKalibracji = true;
            MPU6050_Akcel.startKalibracji = true;
        }

        public void wypiszBiasAkcel()//ok
        {
            tBiasAkcel1X.Text = Convert.ToString(ADXL345.offset[0]);
            tBiasAkcel1Y.Text = Convert.ToString(ADXL345.offset[1]);
            tBiasAkcel1Z.Text = Convert.ToString(ADXL345.offset[2]);

            tBiasAkcel2X.Text = Convert.ToString(MPU6050_Akcel.offset[0]);
            tBiasAkcel2Y.Text = Convert.ToString(MPU6050_Akcel.offset[1]);
            tBiasAkcel2Z.Text = Convert.ToString(MPU6050_Akcel.offset[2]);
        }

        private void bAxisX1_Click(object sender, EventArgs e)//ok
        {
            L3G4200D.okreslWspolczynniki(0, L3G4200D.euler[0]);

            bAxisX1.Text = Convert.ToString(L3G4200D.licznikWspolczynnikowGyro[0]);

            tGainGyro1X.Text = Convert.ToString(L3G4200D.gain[0]);

        }

        private void bAxisY1_Click(object sender, EventArgs e)//ok
        {
            L3G4200D.okreslWspolczynniki(1, L3G4200D.euler[1]);

            bAxisY1.Text = Convert.ToString(L3G4200D.licznikWspolczynnikowGyro[1]);

            tGainGyro1Y.Text = Convert.ToString(L3G4200D.gain[1]);
        }

        private void bAxisZ1_Click(object sender, EventArgs e)//ok
        {
            L3G4200D.okreslWspolczynniki(2, L3G4200D.euler[2]);

            bAxisZ1.Text = Convert.ToString(L3G4200D.licznikWspolczynnikowGyro[2]);

            tGainGyro1Z.Text = Convert.ToString(L3G4200D.gain[2]);
        }

        private void bAxisX2_Click(object sender, EventArgs e)//ok
        {
            MPU6050_Giro.okreslWspolczynniki(0, MPU6050_Giro.euler[0]);

            bAxisX2.Text = Convert.ToString(MPU6050_Giro.licznikWspolczynnikowGyro[0]);

            tGainGyro2X.Text = Convert.ToString(MPU6050_Giro.gain[0]);
        }

        private void button12_Click(object sender, EventArgs e)//ok
        {
            MPU6050_Giro.okreslWspolczynniki(1, MPU6050_Giro.euler[1]);

            bAxisY2.Text = Convert.ToString(MPU6050_Giro.licznikWspolczynnikowGyro[1]);

            tGainGyro2Y.Text = Convert.ToString(MPU6050_Giro.gain[1]);
        }

        private void bAxisZ2_Click(object sender, EventArgs e)//ok
        {
            MPU6050_Giro.okreslWspolczynniki(2, MPU6050_Giro.euler[2]);

            bAxisZ2.Text = Convert.ToString(MPU6050_Giro.licznikWspolczynnikowGyro[2]);

            tGainGyro2Z.Text = Convert.ToString(MPU6050_Giro.gain[2]);
        }

        private void button13_Click(object sender, EventArgs e)//ok
        {
            L3G4200D.zerujKaty();
            MPU6050_Giro.zerujKaty();
        }

        private void bWspX1_Click(object sender, EventArgs e)//ok
        {
            ADXL345.okreslWspolczynniki(0, ADXL345.euler[0]);

            bWspX1.Text = Convert.ToString(ADXL345.licznikWspolczynnikowAkcel[0]);

            tGainAkcel1X.Text = Convert.ToString(ADXL345.gain[0]);
        }

        private void bWspY1_Click(object sender, EventArgs e)//ok
        {
            ADXL345.okreslWspolczynniki(1, ADXL345.euler[1]);

            bWspY1.Text = Convert.ToString(ADXL345.licznikWspolczynnikowAkcel[1]);

            tGainAkcel1Y.Text = Convert.ToString(ADXL345.gain[1]);
        }

        private void bWspX2_Click(object sender, EventArgs e)//ok
        {
            MPU6050_Akcel.okreslWspolczynniki(0, MPU6050_Akcel.euler[0]);

            bWspX2.Text = Convert.ToString(MPU6050_Akcel.licznikWspolczynnikowAkcel[0]);

            tGainAkcel2X.Text = Convert.ToString(MPU6050_Akcel.gain[0]);
        }

        private void bWspY2_Click(object sender, EventArgs e)//ok
        {
            MPU6050_Akcel.okreslWspolczynniki(1, MPU6050_Akcel.euler[1]);

            bWspY2.Text = Convert.ToString(MPU6050_Akcel.licznikWspolczynnikowAkcel[1]);

            tGainAkcel2Y.Text = Convert.ToString(MPU6050_Akcel.gain[1]);
        }

        private void bZerujSkalGiro_Click(object sender, EventArgs e)//ok
        {
            for (var i = 0; i < 3; i++)
            {
                L3G4200D.gain[i] = 1;
                MPU6050_Giro.gain[i] = 1;

                L3G4200D.licznikWspolczynnikowGyro[i] = 1;
                MPU6050_Giro.licznikWspolczynnikowGyro[i] = 1;
            }

            tGainGyro1X.Text = Convert.ToString(L3G4200D.gain[0]);
            tGainGyro1Y.Text = Convert.ToString(L3G4200D.gain[1]);
            tGainGyro1Z.Text = Convert.ToString(L3G4200D.gain[2]);

            tGainGyro2X.Text = Convert.ToString(MPU6050_Giro.gain[0]);
            tGainGyro2Y.Text = Convert.ToString(MPU6050_Giro.gain[1]);
            tGainGyro2Z.Text = Convert.ToString(MPU6050_Giro.gain[2]);

            bAxisX1.Text = Convert.ToString(L3G4200D.licznikWspolczynnikowGyro[0]);
            bAxisY1.Text = Convert.ToString(L3G4200D.licznikWspolczynnikowGyro[1]);
            bAxisZ1.Text = Convert.ToString(L3G4200D.licznikWspolczynnikowGyro[2]);

            bAxisX2.Text = Convert.ToString(MPU6050_Giro.licznikWspolczynnikowGyro[0]);
            bAxisY2.Text = Convert.ToString(MPU6050_Giro.licznikWspolczynnikowGyro[1]);
            bAxisZ2.Text = Convert.ToString(MPU6050_Giro.licznikWspolczynnikowGyro[2]);
        }

        private void bZerujSkalAkcel_Click(object sender, EventArgs e)//ok
        {
            for (var i = 0; i < 3; i++)
            {
                ADXL345.gain[i] = 1;
                MPU6050_Akcel.gain[i] = 1;
            }

            for (var i = 0; i < 2; i++)
            {
                ADXL345.licznikWspolczynnikowAkcel[i] = 1;
                MPU6050_Akcel.licznikWspolczynnikowAkcel[i] = 1;
            }

            bWspX1.Text = Convert.ToString(ADXL345.gain[0]);
            bWspY1.Text = Convert.ToString(ADXL345.gain[1]);

            bWspX2.Text = Convert.ToString(MPU6050_Akcel.gain[0]);
            bWspY2.Text = Convert.ToString(MPU6050_Akcel.gain[1]);

            tGainAkcel1X.Text = Convert.ToString(ADXL345.licznikWspolczynnikowAkcel[0]);
            tGainAkcel1Y.Text = Convert.ToString(ADXL345.licznikWspolczynnikowAkcel[1]);

            tGainAkcel2X.Text = Convert.ToString(MPU6050_Akcel.licznikWspolczynnikowAkcel[0]);
            tGainAkcel2Y.Text = Convert.ToString(MPU6050_Akcel.licznikWspolczynnikowAkcel[0]);
        }
    }
}

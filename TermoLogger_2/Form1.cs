using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;

namespace TermoLogger_2
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


        public int debug_val1 = 0;
        public int debug_val2 = 0;



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

            
            /**WYŚWIETLANIE TEMPERATURY I NUMERÓW SERYJNYCH**/

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

            var zmienna = Convert.ToDouble(textBox1.Text.Replace('.', ','));


            rysujSerie("L3G4200D", gyro1[0]);

            rysujSerie("Gyro-MPU6050", gyro2[0]);

            if (licznikWykresu == 100)
            {
                czyscSerie("L3G4200D");
                czyscSerie("Gyro-MPU6050");
                licznikWykresu = 0;
            }

            licznikWykresu++;
        }

        

        private void rysujSerie(string nazwaSerii, double wartoscSerii)
        {
            wykres1.Series[nazwaSerii].Points.AddY(wartoscSerii);
            

            /*wykres1.Series.Add(nazwaSerii);
            wykres1.Series[nazwaSerii].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            wykres1.Series[nazwaSerii].Points.AddY(wartoscSerii);
            wykres1.Series[nazwaSerii].ChartArea = "ChartArea1";*/
        }

        private void czyscSerie(string nazwaSerii)
        {
            wykres1.Series[nazwaSerii].Points.Clear();
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

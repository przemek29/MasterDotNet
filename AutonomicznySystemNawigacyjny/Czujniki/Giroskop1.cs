using System;
using Kalibracje;
using Helper;
using Calkowanie;

namespace Sensors
{
    public class Giroskop1 : KalibracjaBias
    {
        public double[] omega { get; private set; } = new double[3];

        public double[] omegaKal { get; private set; } = new double[3];

        public double[] gain { get; set; } = { 1.0, 1.0, 1.0 };

        public double[] offset { get; set; } = new double[3];

        public double[] euler { get; private set; } = new double[3];

        public int[] licznikWspolczynnikowGyro { get; set; } = { 1, 1, 1 };

        public int licznikKalibracji { get; set; }

        private readonly MetodaSimpsona _calkaSimpsona = new MetodaSimpsona(4);
        private readonly Rad2Deg _helper = new Rad2Deg();
        private readonly KalibracjaWspolczynnikow _kalibrujWspolczynniki = new KalibracjaWspolczynnikow();

        public void dodajPomiar(double[] predkosciKatowe)
        {
            this.omega = predkosciKatowe;
            
        }

        public void aktualizacja()
        {
            offset[0] = SredniaX;
            offset[1] = SredniaY;
            offset[2] = SredniaZ;

            omegaKal[0] = Math.Round(omega[0] - SredniaX, 4);
            omegaKal[1] = Math.Round(omega[1] - SredniaY, 4);
            omegaKal[2] = Math.Round(omega[2] - SredniaZ, 4);
        }

        public void okreslWspolczynniki(int wyborOsi, double zmienna)
        {
            _kalibrujWspolczynniki.kalibruj(wyborOsi, zmienna, licznikWspolczynnikowGyro[wyborOsi]);

            if (licznikWspolczynnikowGyro[wyborOsi] == 10)
            {
                if (_kalibrujWspolczynniki.Srednia[wyborOsi] > 0)
                {
                    gain[wyborOsi] = 90.0 / _kalibrujWspolczynniki.Srednia[wyborOsi];
                    gain[wyborOsi] = Math.Round(gain[wyborOsi], 2);
                }

                if (_kalibrujWspolczynniki.Srednia[wyborOsi] < 0)
                {
                    gain[wyborOsi] = -90.0 / _kalibrujWspolczynniki.Srednia[wyborOsi];
                    gain[wyborOsi] = Math.Round(gain[wyborOsi], 2);
                }

                licznikWspolczynnikowGyro[wyborOsi] = 0;
                _kalibrujWspolczynniki.zeruj();
            }
            licznikWspolczynnikowGyro[wyborOsi]++;
        }

        public void obliczKaty(double[] predkoscKatowa)
        {

            euler = _calkaSimpsona.oblicz(predkoscKatowa);

            euler = _helper.rad2degGyro(euler);

            for (var i = 0; i < 3; i++)
            {
                euler[i] = Math.Round(gain[i] * euler[i]);
            }
        }

        public void zerujWspolczynniki()
        {
            for (var i = 0; i < 3; i++)
            {
                licznikWspolczynnikowGyro[i] = 1;
            }
        }

        public void zerujKaty()
        {
            _calkaSimpsona.zerujWszystkie();
        }
               
    }
}

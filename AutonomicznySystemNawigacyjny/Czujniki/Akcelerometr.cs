using System;
using Kalibracje;
using Obliczanie;

namespace Sensors
{
    public class Akcelerometr : KalibracjaBias
    {
        public double[] accel { get; private set; } = new double[3];

        public double[] akcelKal { get; private set; } = new double[3];

        public double[] gain { get; set; } = { 1.0, 1.0 };

        public double[] offset { get; set; } = new double[3];

        public double[] euler { get; private set; } = new double[2];

        public int[] licznikWspolczynnikowAkcel { get; set; } = { 1, 1 };

        public int licznikKalibracji { get; set; }

        private readonly TrygonometriaKaty _trygonometria = new TrygonometriaKaty();
        private readonly KalibracjaWspolczynnikow _kalibrujWspolczynniki = new KalibracjaWspolczynnikow();

        public void dodajPomiar(double[] przyspieszenia)
        {
            this.accel = przyspieszenia;
        }

        public void aktualizacja()
        {
            offset[0] = SredniaX;
            offset[1] = SredniaY;
            offset[2] = 1 - SredniaZ;

            akcelKal[0] = Math.Round(accel[0]  - offset[0], 4);
            akcelKal[1] = Math.Round(accel[1]  - offset[1], 4);
            akcelKal[2] = Math.Round(accel[2]  + offset[2], 4);

        }

        public void obliczKaty()
        {
            _trygonometria.PrzeliczKaty(akcelKal[0], akcelKal[1], akcelKal[2]);

            euler[0] = Math.Round(gain[0] * _trygonometria.roll);
            euler[1] = Math.Round(gain[1] * _trygonometria.pitch);
        }

              public void okreslWspolczynniki(int wyborOsi, double zmienna)
        {
            _kalibrujWspolczynniki.kalibruj(wyborOsi, zmienna, licznikWspolczynnikowAkcel[wyborOsi]);

            if (licznikWspolczynnikowAkcel[wyborOsi] == 10)
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

                licznikWspolczynnikowAkcel[wyborOsi] = 0;
                _kalibrujWspolczynniki.zeruj();
            }
            licznikWspolczynnikowAkcel[wyborOsi]++;
        }
    }
}

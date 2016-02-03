using System;

namespace Kalibracje
{
    public class KalibracjaWspolczynnikow
    {
        public double[] Suma { get; set; } = { 0.0, 0.0, 0.0 };

        public double[] Wektor { get; set; } = { 0.0, 0.0, 0.0 };

        public double[] Srednia { get; set; } = { 0.0, 0.0, 0.0 };

        public int[] Licznik { get; set; } = { 1, 1, 1 };

        public void kalibruj(int os, double wartosc, int nrKalibracji)
        {
            this.Licznik[os] = nrKalibracji;
            this.Wektor[os] = wartosc;

            Suma[os] += Wektor[os];
            Srednia[os] = Math.Round(Suma[os] / Licznik[os], 4);
        }

        public void zeruj()
        {
            for (var i = 0; i < 3; i++)
            {
                Wektor[i] = 0;

                Srednia[i] = 0;

                Suma[i] = 0;
            }
        }
    }
}

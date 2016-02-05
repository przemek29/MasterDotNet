using System;

namespace Calkowanie
{
    public class MetodaSimpsona
    {
        private double[] Fa { get; set; } = { 0, 0, 0 };
        private double[] Fb { get; set; } = { 0, 0, 0 };
        private double[] Fab { get; set; } = { 0, 0, 0 };

        private double[] wynik { get; set; } = { 0, 0, 0 };

        public double[] Calka { get; set; } = { 0, 0, 0 };

        private int licznik { get; set; } = 1;
        private int Precyzja { get; set; }

        public MetodaSimpsona(int precyzja)
        {
            this.Precyzja = precyzja;
        }

        public double[] oblicz(double[] wartosc)
        {
            if (licznik == 1)
                Fa = wartosc;
            if (licznik == 5)
                Fab = wartosc;
            if (licznik == 9)
            {
                Fb = wartosc;
                simpson();
                licznik = 0;
            }
            licznik++;
            return Calka;
        }

        private void simpson()
        {
            for (var i = 0; i < 3; i++)
            {
                wynik[i] = (8/54) * (Fa[i] + 4 * Fab[i] + 4 * Fb[i]);
                Calka[i] += Math.Round(wynik[i],Precyzja);
            }
        }

        internal void zerujWszystkie()
        {
            for (var i = 0; i < 3; i++)
            {
                Calka[i] = 0;
            }
        }
    }
}


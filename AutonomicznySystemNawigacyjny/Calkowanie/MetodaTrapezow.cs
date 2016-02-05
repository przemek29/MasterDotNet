using System;
namespace Calkowanie
{
    public class MetodaTrapezow
    {
        public double czas { get; set; }

        public double[] Poprzedni { get; set; } = { 0, 0, 0 };

        public double[] Calka { get; set; } = { 0, 0, 0 };

        public double[] calka { get; set; } = { 0, 0, 0 };

        public int Precyzja { get; set; }

        public MetodaTrapezow(int precyzja)
        {
            this.Precyzja = precyzja;
        }

        public double[] calkuj(double[] aktualny, double czas)
        {
            for (int i = 0; i < aktualny.Length; i++)
            {
                calka[i] = 0.5 * (aktualny[i] + Poprzedni[i]) * czas;
                Calka[i] += Math.Round(calka[i], Precyzja);
            }
            Poprzedni = aktualny;
            return Calka;
        }

        public void zerujWszystkie()
        {
            for (var i = 0; i < 3; i++)
            {
                Calka[i] = 0;
            }
        }

    }
}

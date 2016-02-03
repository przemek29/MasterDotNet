
namespace Calkowanie
{
    class MetodaSimpsona
    {
        public double calculate(double xp, double xk, int n, double[] paczka)
        {
            double dx, calka, s, x;

            dx = (xk - xp) / (double)n;

            calka = 0;
            s = 0;
            for (int i = 1; i < n; i++)
            {
                x = xp + i * dx;
                s += paczka[(int)(x - dx / 2)];
                calka += paczka[(int)x];
            }
            s += paczka[(int)(xk - dx / 2)];
            calka = (dx / 6) * (paczka[0] + paczka[paczka.Length - 1] + 2 * calka + 4 * s);

            return calka;
        }
    }
}

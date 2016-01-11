using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autonomiczny_System_Nawigacyjny
{
    class MetodaSimpsona
    {
        

        /// <summary>
        /// Oblicza calke metoda Simpsona w przedziale od xp do xk z dokladnoscia n dla funkcji fun
        /// </summary>
        /// <param name="xp">poczatek przedzialu calkowania</param>
        /// <param name="xk">koniec przedzialu calkowania</param>
        /// <param name="n">dokladnosc calkowania</param>
        /// <param name="func">funkcja calkowana</param>
        /// <returns>przyblizona wartosc calki</returns>
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

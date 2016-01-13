using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autonomiczny_System_Nawigacyjny
{
    public class MetodaTrapezow
    {
        public double czas { get; set; }
        public double[] Poprzedni { get; set; } = { 0, 0, 0, 0, 0, 0 };
        public double[] Calka { get; set; } = { 0, 0, 0, 0, 0, 0 };
        public double[] calka { get; set; } = { 0, 0, 0, 0, 0, 0 };

        public MetodaTrapezow()
        {
            
        }

        public double[] calkuj (double[] aktualny,  double czas)
        {
            for (int i = 0; i < aktualny.Length; i++)
            {
                calka[i] = 0.5 * (aktualny[i] + Poprzedni[i]) * czas;
                Calka[i] += calka[i];
            }
            Poprzedni = aktualny;
            return Calka;
        }

        public void zeruj(int parametr)
        {
            Calka[parametr] = 0;
        }
        public void zerujWszystkie()
        {
            for (var i = 0; i < 6; i++)
            {
                Calka[i] = 0;
            }
        }

    }
}

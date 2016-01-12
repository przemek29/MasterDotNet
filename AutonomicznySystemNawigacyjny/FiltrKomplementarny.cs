using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autonomiczny_System_Nawigacyjny
{
    public class FiltrKomplementarny
    {
        public double wynik { get; set; }

        public double oblicz(double kat, double predkoscKatowa, double przyspieszenieKatowe, double czas)
        {
            var angle = 0.98 * (kat + predkoscKatowa * czas) + 0.02 * przyspieszenieKatowe;
            wynik = angle;

            return angle;
        }

    }
}

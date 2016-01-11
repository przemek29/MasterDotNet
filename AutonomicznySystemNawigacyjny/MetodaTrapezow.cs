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
        public double[] Poprzedni { get; set; } = {0,0,0,0,0,0 };
        public double[] Calka { get; set; } = { 0, 0, 0, 0, 0, 0 };
        public double[] calka { get; set; } = { 0, 0, 0, 0, 0, 0 };

        public MetodaTrapezow()
        {
            
        }

        public double[] calkuj (double[] aktualniy,  double czas)
        {
            for (int i = 0; i < aktualniy.Length; i++)
            {
                calka[i] = 0.5 * (aktualniy[i] + Poprzedni[i]) * czas;
                Calka[i] += calka[i];
            }
            Poprzedni = aktualniy;
            return Calka;
        }
    }
}

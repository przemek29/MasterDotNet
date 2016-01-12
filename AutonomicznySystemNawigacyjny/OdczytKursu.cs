using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autonomiczny_System_Nawigacyjny
{
    

    public class OdczytKursu
    {
        public double kurs { get; set; }

        public double czytajKurs(double X, double Y)
        {
            kurs = Math.Atan2(Y, X);

            if (kurs < 0)
                kurs += 2 * Math.PI;

            if (kurs > 0)
                kurs -= 2 * Math.PI;

            kurs = rad2deg(kurs);
            return Math.Round(kurs, 2);
        }

        public double czytajKurs(double X, double Y, double Z, double roll, double pitch)
        {
            var Xh = X * Math.Cos(roll) + Z * Math.Sin(roll);
            var Yh = X * Math.Cos(pitch) * Math.Sin(roll) + Y * Math.Cos(pitch) - Z * Math.Sin(pitch) * Math.Cos(roll);

            kurs = Math.Atan2(Yh, Xh);

            if (kurs < 0)
                kurs += 2 * Math.PI;

            if (kurs > 0)
                kurs -= 2 * Math.PI;

            kurs = rad2deg(kurs);
            return Math.Round(kurs, 2);
        }
        public double rad2deg(double rad)
        {
            var deg = rad * 180 / Math.PI;
            return deg;
        }
    }
}

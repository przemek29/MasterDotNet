using System;

namespace Obliczanie
{   
    public class Kurs
    {
        private double kursRadiany { get; set; }

        public double kursPlaski { get; set; }

        public double kursDynamiczny{ get; set; }

        public double kursDeklinacja { get; set; }

        public void obliczPlaskiKurs(double X, double Y)
        {
            kursRadiany = Math.Atan2(Y, X);

            if (kursRadiany < 0)
                kursRadiany += 2 * Math.PI;

            if (kursRadiany > 0)
                kursRadiany -= 2 * Math.PI;

            kursPlaski = Math.Round(rad2deg(kursRadiany),2);
        }

        public void obliczDynamicznyKurs(double X, double Y, double Z, double roll, double pitch)
        {
            var Xh = X * Math.Cos(roll) + Z * Math.Sin(roll);
            var Yh = X * Math.Cos(pitch) * Math.Sin(roll) + Y * Math.Cos(pitch) - Z * Math.Sin(pitch) * Math.Cos(roll);

            kursRadiany = Math.Atan2(Yh, Xh);

            if (kursRadiany < 0)
                kursRadiany += 2 * Math.PI;

            if (kursRadiany > 0)
                kursRadiany -= 2 * Math.PI;

            kursDynamiczny = Math.Round(rad2deg(kursRadiany),2);
        }

        public void uwzglednijDeklinacje(double deklinacja)
        {
            kursDeklinacja = kursPlaski + deklinacja;
        }

        public double rad2deg(double rad)
        {
            var deg = rad * 180 / Math.PI;
            return deg;
        }
    }
}

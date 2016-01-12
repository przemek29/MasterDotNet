using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autonomiczny_System_Nawigacyjny
{
    public class KalibracjaBias
    {
        public double SumaX { get; set; }
        public double SumaY { get; set; }
        public double SumaZ { get; set; }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public double SredniaX
        {
            get
            {
                return Math.Round(SumaX / licznik, 4);
            }

            set {; }
        }

        public double SredniaY
        {
            get
            {
                return Math.Round(SumaY / licznik, 4);
            }

            set {; }
        }

        public double SredniaZ
        {
            get
            {
                return Math.Round(SumaZ / licznik, 4);
            }

            set {; }
        }


        
        public int licznik { get; set; }

        public KalibracjaBias()
        {
            
            SumaX = 0;
            SumaY = 0;
            SumaZ = 0;
        }

        public void kalibruj(double X, double Y, double Z, int licznik)
        {
            this.licznik = licznik;
            this.X = X;
            this.Y = Y;
            this.Z = Z;

            SumaX += X;
            SumaY += Y;
            SumaZ += Z;
        }

        public void kalibruj(int licznik, double X, double Y, double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.licznik = licznik;

            SumaX += X;
            SumaY += Y;
            SumaZ += Z;
        }

        public void zeruj()
        {
            X = 0;
            Y = 0;
            Z = 0; 
            SredniaX = 0;
            SredniaY = 0;
            SredniaZ = 0;
            SumaX = 0;
            SumaY = 0;
            SumaZ = 0;
        }

    }
}

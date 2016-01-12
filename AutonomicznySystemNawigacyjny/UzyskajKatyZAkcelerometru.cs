using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autonomiczny_System_Nawigacyjny
{
    public class UzyskajKatyZAkcelerometru
    {
        public double roll
        {
            get
            {
                //return Math.Round((Math.Atan2(-AccY , AccZ) * 180.0)/ Math.PI,2); //GŁUPOTA
                return Math.Round(Math.Atan2(AccY, Math.Sqrt(AccX*AccX + AccZ*AccZ))*180/Math.PI,2);
            }

            set {; }
        }
        public double pitch
        {
            get
            {
                return Math.Round(Math.Atan2(AccX, Math.Sqrt(AccY * AccY + AccZ * AccZ)) * 180.0 / Math.PI, 2);
            }

            set {; }
        }

        public double AccX { get; set; }
        public double AccY { get; set; }
        public double AccZ { get; set; }

        public void PrzeliczKaty(double AccX, double AccY, double AccZ)
        {
            this.AccX = AccX;
            this.AccY = AccY;
            this.AccZ = AccZ;
        }
    }
}

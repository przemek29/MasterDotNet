using System;
using Kalibracje;

namespace Sensors
{
    public class Magnetometr : KalibracjaMagnetometru
    {
        public Magnetometr(double vendoMax_X, double vendorMax_Y, double vendorMax_Z) : base(3000, 3000, 3000)
        {
        }

        public double[] b { get; private set; } = new double[3];
        public double[] bKal { get; private set; } = new double[3];

        public void dodajPomiar(double[] b)
        {
            this.b = b;
        }

        public void kalibruj()
        {
            AddValues(b[0], b[1], b[2]);
        }

        public void aktualizacja()
        {
            bKal[0] = Math.Round(b[0] * GainX + OffsetX, 0);
            bKal[1] = Math.Round(b[1] * GainY + OffsetY, 0);
            bKal[2] = Math.Round(b[2] * GainZ + OffsetZ, 0);
        }
    }
}

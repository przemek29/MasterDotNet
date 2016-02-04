using Sensors;

namespace Obliczanie
{
    public class OrientacjaKatowa
    {
        public double  rollADXL345 { get; private set; }
        public double pitchADXL345 { get; private set; }

        public double rollMPU6050 { get; private set; }
        public double pitchMPU6050 { get; private set; }


        public void oblicz(Akcelerometr A1, Akcelerometr A2)
        {
            this.rollADXL345 = A1.euler[0];
            this.pitchADXL345 = A1.euler[1];

            this.rollMPU6050 = A2.euler[0];
            this.pitchMPU6050 = A2.euler[1];
        }

    }
}

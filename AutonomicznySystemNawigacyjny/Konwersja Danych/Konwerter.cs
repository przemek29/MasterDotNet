using System;

namespace Sensors
{
    public class Konwerter
    {
        public double[] l3g4200d { get; private set; } = new double[3];

        public double[] mpu6050_G { get; private set; } = new double[3];

        public double[] adxl345 { get; private set; } = new double[3];

        public double[] mpu6050_A { get; private set; } = new double[3];

        public double[] hmc5883l { get; private set; } = new double[3];

        public int licznik { get; private set; }
        
        public void konwertujDane(string[] ramkaRaspberry)
        {

            l3g4200d[0] = -Convert.ToDouble(ramkaRaspberry[2].Replace('.', ','));
            l3g4200d[1] = -Convert.ToDouble(ramkaRaspberry[1].Replace('.', ','));
            l3g4200d[2] = Convert.ToDouble(ramkaRaspberry[3].Replace('.', ','));

            mpu6050_G[0] = Convert.ToDouble(ramkaRaspberry[7].Replace('.', ','));
            mpu6050_G[1] = -Convert.ToDouble(ramkaRaspberry[8].Replace('.', ','));
            mpu6050_G[2] = Convert.ToDouble(ramkaRaspberry[9].Replace('.', ','));

            adxl345[0] = Convert.ToDouble(ramkaRaspberry[5].Replace('.', ','));
            adxl345[1] = -Convert.ToDouble(ramkaRaspberry[4].Replace('.', ','));
            adxl345[2] = Convert.ToDouble(ramkaRaspberry[6].Replace('.', ','));

            mpu6050_A[0] = Convert.ToDouble(ramkaRaspberry[10].Replace('.', ','));
            mpu6050_A[1] = -Convert.ToDouble(ramkaRaspberry[11].Replace('.', ','));
            mpu6050_A[2] = -Convert.ToDouble(ramkaRaspberry[12].Replace('.', ','));

            hmc5883l[0] = Convert.ToDouble(ramkaRaspberry[13].Replace('.', ','));
            hmc5883l[1] = -Convert.ToDouble(ramkaRaspberry[14].Replace('.', ','));
            hmc5883l[2] = Convert.ToDouble(ramkaRaspberry[15].Replace('.', ','));

            licznik = (int)Convert.ToInt64(ramkaRaspberry[0]);


        }
    }
}

using System;

namespace Filtracje
{
    public class Mahony
    {

        public double twoKp { get; set; } = 5;                                                                                  // 2 * proportional gain (Kp)
        public double twoKi { get; set; } = 0;                                                                            // 2 * integral gain (Ki)
        public double sampleFreq { get; set; }
        public double q0 { get; set; } = 1.0;
        public double q1 { get; set; } = 0.0;
        public double q2 { get; set; } = 0.0;
        public double q3 { get; set; } = 0.0;                                      // quaternion of sensor frame relative to auxiliary frame
        public double integralFBx = 0.0, integralFBy = 0.0, integralFBz = 0.0;     // integral error terms scaled by Ki

        public void MahonyAHRSupdate(double gx, double gy, double gz, double ax, double ay, double az, double mx, double my, double mz, double sampleFreq)
        {
            this.sampleFreq = sampleFreq;
            double recipNorm;
            double q0q0, q0q1, q0q2, q0q3, q1q1, q1q2, q1q3, q2q2, q2q3, q3q3;
            double hx, hy, bx, bz;
            double halfvx, halfvy, halfvz, halfwx, halfwy, halfwz;
            double halfex, halfey, halfez;
            double qa, qb, qc;

            // Use IMU algorithm if magnetometer measurement invalid (avoids NaN in magnetometer normalisation)
            if ((mx == 0.0) && (my == 0.0) && (mz == 0.0))
            {
                MahonyAHRSupdateIMU(gx, gy, gz, ax, ay, az, sampleFreq);
                return;
            }

            // Compute feedback only if accelerometer measurement valid (avoids NaN in accelerometer normalisation)
            if (!((ax == 0.0) && (ay == 0.0) && (az == 0.0)))
            {

                // Normalise accelerometer measurement
                recipNorm = 1 / Math.Sqrt(ax * ax + ay * ay + az * az);
                ax *= recipNorm;
                ay *= recipNorm;
                az *= recipNorm;

                // Normalise magnetometer measurement
                recipNorm = 1 / Math.Sqrt(mx * mx + my * my + mz * mz);
                mx *= recipNorm;
                my *= recipNorm;
                mz *= recipNorm;

                // Auxiliary variables to avoid repeated arithmetic
                q0q0 = q0 * q0;
                q0q1 = q0 * q1;
                q0q2 = q0 * q2;
                q0q3 = q0 * q3;
                q1q1 = q1 * q1;
                q1q2 = q1 * q2;
                q1q3 = q1 * q3;
                q2q2 = q2 * q2;
                q2q3 = q2 * q3;
                q3q3 = q3 * q3;

                // Reference direction of Earth's magnetic field
                hx = 2.0f * (mx * (0.5f - q2q2 - q3q3) + my * (q1q2 - q0q3) + mz * (q1q3 + q0q2));
                hy = 2.0f * (mx * (q1q2 + q0q3) + my * (0.5f - q1q1 - q3q3) + mz * (q2q3 - q0q1));
                bx = Math.Sqrt(hx * hx + hy * hy);
                bz = 2.0f * (mx * (q1q3 - q0q2) + my * (q2q3 + q0q1) + mz * (0.5f - q1q1 - q2q2));

                // Estimated direction of gravity and magnetic field
                halfvx = q1q3 - q0q2;
                halfvy = q0q1 + q2q3;
                halfvz = q0q0 - 0.5f + q3q3;
                halfwx = bx * (0.5f - q2q2 - q3q3) + bz * (q1q3 - q0q2);
                halfwy = bx * (q1q2 - q0q3) + bz * (q0q1 + q2q3);
                halfwz = bx * (q0q2 + q1q3) + bz * (0.5f - q1q1 - q2q2);

                // Error is sum of cross product between estimated direction and measured direction of field vectors
                halfex = (ay * halfvz - az * halfvy) + (my * halfwz - mz * halfwy);
                halfey = (az * halfvx - ax * halfvz) + (mz * halfwx - mx * halfwz);
                halfez = (ax * halfvy - ay * halfvx) + (mx * halfwy - my * halfwx);

                // Compute and apply integral feedback if enabled
                if (twoKi > 0.0f)
                {
                    integralFBx += twoKi * halfex * (1.0f / sampleFreq);    // integral error scaled by Ki
                    integralFBy += twoKi * halfey * (1.0f / sampleFreq);
                    integralFBz += twoKi * halfez * (1.0f / sampleFreq);
                    gx += integralFBx;      // apply integral feedback
                    gy += integralFBy;
                    gz += integralFBz;
                }
                else
                {
                    integralFBx = 0.0f;     // prevent integral windup
                    integralFBy = 0.0f;
                    integralFBz = 0.0f;
                }

                // Apply proportional feedback
                gx += twoKp * halfex;
                gy += twoKp * halfey;
                gz += twoKp * halfez;
            }

            // Integrate rate of change of quaternion
            gx *= (0.5 * (1.0 / sampleFreq));             // pre-multiply common factors
            gy *= (0.5 * (1.0 / sampleFreq));
            gz *= (0.5 * (1.0 / sampleFreq));
            qa = q0;
            qb = q1;
            qc = q2;
            q0 += (-qb * gx - qc * gy - q3 * gz);
            q1 += (qa * gx + qc * gz - q3 * gy);
            q2 += (qa * gy - qb * gz + q3 * gx);
            q3 += (qa * gz + qb * gy - qc * gx);

            // Normalise quaternion
            recipNorm = 1 / Math.Sqrt(q0 * q0 + q1 * q1 + q2 * q2 + q3 * q3);
            q0 *= recipNorm;
            q1 *= recipNorm;
            q2 *= recipNorm;
            q3 *= recipNorm;

            q0 = Math.Round(q0, 3);
            q1 = Math.Round(q1, 3);
            q2 = Math.Round(q2, 3);
            q3 = Math.Round(q3, 3);
        }

        //---------------------------------------------------------------------------------------------------
        // IMU algorithm update

        public void MahonyAHRSupdateIMU(double gx, double gy, double gz, double ax, double ay, double az, double sampleFrequency)
        {
            this.sampleFreq = sampleFrequency;
            double recipNorm;
            double halfvx, halfvy, halfvz;
            double halfex, halfey, halfez;
            double qa, qb, qc;

            // Compute feedback only if accelerometer measurement valid (avoids NaN in accelerometer normalisation)
            if (!((ax == 0.0) && (ay == 0.0) && (az == 0.0f)))
            {

                // Normalise accelerometer measurement
                recipNorm = 1 / Math.Sqrt(ax * ax + ay * ay + az * az);
                ax *= recipNorm;
                ay *= recipNorm;
                az *= recipNorm;

                // Estimated direction of gravity and vector perpendicular to magnetic flux
                halfvx = q1 * q3 - q0 * q2;
                halfvy = q0 * q1 + q2 * q3;
                halfvz = q0 * q0 - 0.5 + q3 * q3;

                // Error is sum of cross product between estimated and measured direction of gravity
                halfex = (ay * halfvz - az * halfvy);
                halfey = (az * halfvx - ax * halfvz);
                halfez = (ax * halfvy - ay * halfvx);

                // Compute and apply integral feedback if enabled
                if (twoKi > 0.0)
                {
                    integralFBx += twoKi * halfex * (1.0 / sampleFreq);    // integral error scaled by Ki
                    integralFBy += twoKi * halfey * (1.0 / sampleFreq);
                    integralFBz += twoKi * halfez * (1.0 / sampleFreq);
                    gx += integralFBx;      // apply integral feedback
                    gy += integralFBy;
                    gz += integralFBz;
                }
                else
                {
                    integralFBx = 0.0;     // prevent integral windup
                    integralFBy = 0.0;
                    integralFBz = 0.0;
                }

                // Apply proportional feedback
                gx += twoKp * halfex;
                gy += twoKp * halfey;
                gz += twoKp * halfez;
            }

            // Integrate rate of change of quaternion
            gx *= (0.5 * (1.0 / sampleFreq));             // pre-multiply common factors
            gy *= (0.5 * (1.0 / sampleFreq));
            gz *= (0.5 * (1.0 / sampleFreq));
            qa = q0;
            qb = q1;
            qc = q2;
            q0 += (-qb * gx - qc * gy - q3 * gz);
            q1 += (qa * gx + qc * gz - q3 * gy);
            q2 += (qa * gy - qb * gz + q3 * gx);
            q3 += (qa * gz + qb * gy - qc * gx);

            // Normalise quaternion
            recipNorm = 1 / Math.Sqrt(q0 * q0 + q1 * q1 + q2 * q2 + q3 * q3);
            q0 *= recipNorm;
            q1 *= recipNorm;
            q2 *= recipNorm;
            q3 *= recipNorm;

            q0 = Math.Round(q0, 3);
            q1 = Math.Round(q1, 3);
            q2 = Math.Round(q2, 3);
            q3 = Math.Round(q3, 3);
        }
    }
}

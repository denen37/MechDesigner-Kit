using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;


namespace BeltWindow.Models
{  
    public static class BeltCalculations
    {
        const double GRAVITY_CONSTANT = 9.81; 

        public static void CalculateLength(BeltProperties belt)
        {
            double sumPD = belt.SmallPulleySize + belt.BigPulleySize;
            double diffPD = belt.BigPulleySize - belt.SmallPulleySize;
            double D = belt.BigPulleySize;
            double d = belt.SmallPulleySize;
            double c = belt.CentreToCentreDistance * 1000;
            double ToRadians = PI / 180;
            double angA = belt.AngleOfContact_a * ToRadians;
            double angB = belt.AngleOfContact_b * ToRadians;
            double ans;

            if (belt.IsOpen)
            {
                ans = Sqrt((4 * c * c) - (diffPD * diffPD)) + 0.5 * (D * angB + d * angB);
            }
            else
            {
                ans = Sqrt((4 * c * c) - (sumPD * sumPD)) + 0.5 * sumPD * angA;
            }

            belt.BeltLength = ans / 1000;
  
        }

        public static void CalculateAnglesOfContact (BeltProperties belt)
        {
            double a, b;
            double D = belt.BigPulleySize + belt.SmallPulleySize;
            double d = belt.BigPulleySize - belt.SmallPulleySize;
            double c = belt.CentreToCentreDistance * 1000;
            double ToDegrees = 180 / PI;

            if (belt.IsOpen)
            {
                a = PI - 2 * Asin(d / (2 * c));
                b = PI + 2 * Asin(d / (2 * c));
            }
            else
            {
                a = b = PI + 2 * Asin(D / (2 * c));
            }

            a = a * ToDegrees;
            b = b * ToDegrees;

            belt.AngleOfContact_a = a;
            belt.AngleOfContact_b = b;
        }

        public static void CalculateDip(BeltProperties belt)
        {
            double c = belt.CentreToCentreDistance;
            double w = belt.SpecificWeight * belt.BeltWidth * belt.BeltThickness * 1e-6;
            double f = belt.InitialTension;
            double ans;

            ans = (c * c * w) / (8 * f);
            belt.Dip = ans * 1000;
        }

        public static void CalculateDesignPower(BeltProperties belt)
        {
            double P = belt.PowerTransmitted;
            double Nfs = belt.ServiceFactor;
            double Nd = belt.DesignFactor;

            belt.DesignPower = P * Nfs * Nd;
        }

        public static void CalculateFrictionDevelopment(BeltProperties belt)
        {
            double F1a = belt.TightTension;
            double F2 = belt.SlackTension;
            double Fc = belt.CentrifugalForce;
            double phi = (belt.AngleOfContact_a * PI) / 180;

            belt.FrictionDevelopment = (1 / phi) * Log((F1a - Fc) / (F2 - Fc));
        }

        public static void CalculateBeltWeight (BeltProperties belt)
        {
            double y = belt.SpecificWeight;
            double b = belt.BeltWidth;
            double t = belt.BeltThickness;
            double l = belt.BeltLength;

            belt.BeltWeight = (y * b * t ) / 1e6;
        }

        public static void CalculateLinearVelocity (BeltProperties belt)
        {
            double d = belt.SmallPulleySize;
            double n = belt.AngularVelocity / 60;

            belt.LinearVelocity_Small = (PI * d * n) / 1000;
        }

        public static void CalculateCentrifugalForce (BeltProperties belt)
        {
            double w = belt.BeltWeight;
            double v = belt.LinearVelocity_Small;
            double g = GRAVITY_CONSTANT;

            belt.CentrifugalForce = (w * v * v) / g;
        }

        public static void CalculateTorque (BeltProperties belt)
        {
            double h = belt.PowerTransmitted * 1000;
            double k = belt.ServiceFactor;
            double d = belt.DesignFactor;
            double n = belt.AngularVelocity / 60;

            belt.Torque = (h * k * d) / (2 * PI * n);
        }

        public static void CalculateNetTension(BeltProperties belt)
        {
            double t = belt.Torque;
            double d = belt.SmallPulleySize / 1000;

            belt.NetTension = (2 * t) / d;
        }

        public static void CalculateLargestAllowableTension(BeltProperties belt)
        {

            double b = belt.BeltWidth / 1000;
            double Cp = belt.PulleyCorrectionFactor;
            double Cv = belt.VelocityCorrectionFactor;
            double Fa = belt.AllowableTension;

            belt.LargestAllowableTension = b * Cp * Cv * Fa;
        }

        public static void CalculateTightTension(BeltProperties belt)
        {
            belt.TightTension = belt.LargestAllowableTension;
        }

        public static void CalculateSlackTension(BeltProperties belt)
        {
            double F1a = belt.TightTension;
            double Tnet = belt.NetTension;

            belt.SlackTension = F1a - Tnet;
        }

        public static void CalculateInitialTension(BeltProperties belt)
        {
            double F1 = belt.SlackTension;
            double F2 = belt.TightTension;
            double Fc = belt.CentrifugalForce;

            belt.InitialTension = (F1 + F2) / 2 - Fc;
        }

    }
}

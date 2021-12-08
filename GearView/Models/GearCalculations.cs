using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace GearWindow.Models
{
    public static class GearCalculations
    {
        private static int places = 2;
        public static double CalcNoGearTeeth(GearDrive drive)
        {
            double nG = drive.gear.AngVel;
            double np = drive.pinion.AngVel;
            double Np = drive.pinion.ToothCount;

            if (np > nG)
            {
                drive.GearRatio = Round(np / nG, places);
                return Round((Np * np / nG), places);
            }
            else 
            {
                drive.GearRatio = Round(nG / np, places);
                return Round((Np * nG / np), places); 
            }
        }

        public static double CalcOutputSpeed(GearDrive drive)
        {
            double Np = drive.pinion.ToothCount;
            double NG = drive.gear.ToothCount;
            double np = drive.pinion.AngVel;
            double angvel;
            if (NG > Np)
            {
                drive.GearRatio = Round(NG / Np, places);
                angvel = np * Np / NG;
                return Round(angvel, places);
            }
            else
            {
                drive.GearRatio = Round(Np / NG, places);
                angvel = np * NG / Np;
                return Round(angvel, places);
            }
        }
        public static void CalcTagentialForce(GearDrive drive)
        {
            double P = drive.Power;
            double n = drive.gear.AngVel;
            double D = drive.gear.PitchDiameter;

            drive.TangentialForce = Round((126000 * P / (n * D)));
        }

        public static void CalcNormalForce(GearDrive drive)
        {
            double w = drive.TangentialForce;
            double phi = drive.PressureAngle * (PI / 180);

            double Fn = w / Cos(phi);
            drive.NormalForce = Round(Fn);
        }

        public static void CalcRadialForce(GearDrive drive)
        {
            double w = drive.TangentialForce;
            double phi = drive.PressureAngle * (PI / 180);

            double Fr = w * Tan(phi);
            drive.RadialForce = Round(Fr);
        }

        public static void CalcDynamicFactor(GearDrive drive)
        {
            double C;
            int Av = drive.QualityNumber;
            double Vt = drive.PitchLineVel;
            double B = 0.25 * Pow((Av - 5.0), 0.667);

            if (drive.Unit == UnitSystem.English)
            { C = 50 + 56*(1 - B); }
            else
            { C = 3.5637 + 3.9914*(1 - B); }

            double Kv = Pow((C / (C + Sqrt(Vt))), -B);
            drive.DynamicFactor = Round(Kv, places);

            drive.MaxVelocity = Pow((C + (14 - Av)), 2);
        }

        public static void CalcBendingStress(Gear gear)
        {
            double Wt = gear.Drive.TangentialForce;
            double Pd = gear.Drive.DiametralPitch;
            double F = gear.Drive.FaceWidth;
            double Jg = gear.BendingStressGeometryFactor;
            double Ko = gear.Drive.OverLoadFactor;
            double Ks = gear.Drive.SizeFactor;
            double Km = gear.Drive.LoadDistributionFactor;
            double Kb = gear.RimThicknessFactor;
            double Kv = gear.Drive.DynamicFactor;

            double Sg = (Wt * Pd / F * Jg) * Ko * Ks * Km * Kb * Kv;
            gear.BendingStress = Round(Sg, places);
            
        }

        public static void CalcBendingStressGeometryFactor (Gear gear)
        {
            double Y = GearDbConnection.GetLewisFormFactor((int)gear.ToothCount);
            CalcFatigueStressConcFactor(gear);
            double Kf = gear.FatigueStressConcFactor;
            double MN = 1; // For Spur gears.

           double J = Y / Kf * MN;
            gear.BendingStressGeometryFactor = Round(J, places);
        }

        public static void CalcFatigueStressConcFactor(Gear gear)
        {
            double l = gear.Drive.Addendum + gear.Drive.Dedendum;
            double t = gear.Drive.ToothThickness;
            double b = gear.Drive.Dedendum;
            double d = gear.PitchDiameter;
            double phi = gear.Drive.PressureAngle * PI / 180;
            double rf = gear.Drive.FilletRaduis;
            double P = gear.Drive.DiametralPitch;

            //double H = 0.340 - 0.4583662 * phi;
            //double L = 0.316 - 0.4583662 * phi;
            //double M = 0.290 + 0.4583662 * phi;

            //double r = (Pow(b - rf, 2) / ((d / 2) + b - rf));
            double r = (1 / 1 - Sin(phi)) * ((PI / 4 * P) * Cos(phi) - b * Sin(phi));
            double Kf = 0.18 + Pow((t / r), 0.15) + Pow((t / l), 0.45);

            gear.FatigueStressConcFactor = Kf;

            //TODO - Fix the bug on fatigue stress concentration factor.
        }
        public static void GetElasticCoefficient(GearDrive drive)
        {
            throw new NotImplementedException();
        }

        public static void CalcContactStressGeometryfactor(GearDrive drive)
        {
            double Mg = drive.GearRatio;
            double Phi = drive.PressureAngle * PI / 180;
            double Mn = 1; // load sharing factor for spur gears;

            double I = (Cos(Phi) * Sin(Phi) / 2 * Mn) * (Mg / (Mg + 1)); // External gears.
            drive.ContactStressGeometryFactor = Round(I, places);

        }

        public static void CalcContactStress(Gear gear)
        {
            double Cp = gear.Drive.ElasticCoefficient;
            double Wt = gear.Drive.TangentialForce;
            double Pd = gear.Drive.DiametralPitch;
            double F = gear.Drive.FaceWidth;
            double I = gear.Drive.ContactStressGeometryFactor;
            double Ko = gear.Drive.OverLoadFactor;
            double Ks = gear.Drive.SizeFactor;
            double Km = gear.Drive.LoadDistributionFactor;
            double Kb = gear.Drive.pinion.RimThicknessFactor;
            double Kv = gear.Drive.DynamicFactor;

            double Sc = Cp * Sqrt((Wt * Ko * Kb * Km * Ks * Kv) / (F * Pd * I));
            gear.ContactStress = Round(Sc, places);
        }

        public static void CalcBendingStrengthStressCycleFactor (Gear gear)
        {
            double L = gear.LoadCycles;
            double Yn = 1;

            if (L <= 6 * Pow(10, 6))
            {
                switch (gear.Drive.HardnessType)
                {
                    case HardnessMethod.Case_carburized:
                        Yn = 6.1514 * Pow(L, -0.1192);
                        break;
                    case HardnessMethod.Nitrided:
                        Yn = 3.517 * Pow(L, -0.0817);
                        break;
                    case HardnessMethod.Through_hardened:
                        switch (gear.Drive.Hardness)
                        {
                            case 400:
                                Yn = 9.4518 * Pow(L, -0.418);
                                break;
                            case 250:
                                Yn = 4.9404 * Pow(L, -0.1045);
                                break;
                            case 160:
                                Yn = 2.3194 * Pow(L, -0.0538);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Yn = 1.3558 * Pow(L, -0.0178);
            }

            gear.BendingStressCycleFactor = Round(Yn, places);
        }

        public static void CalcContactStrengthStressCycleFactor (Gear gear)
        {
            double Zn = 1;
            double Nc = gear.LoadCycles;

            if (Nc <= Pow(10, 7))
            {
                switch (gear.Drive.HardnessType)
                {
                    
                    case HardnessMethod.Nitrided:
                        Zn = 1.249 * Pow(Nc, -0.0138);
                        break;
                    case HardnessMethod.Case_carburized:
                    case HardnessMethod.Through_hardened:
                        Zn = 2.466 * Pow(Nc, -0.056);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Zn = 1.4488 * Pow(Nc, -0.023);
            }

            gear.PittingStressCycleFactor = Round(Zn, places);
        }

        public static void GetAllowableBendingStress(GearDrive drive)
        {
            throw new NotImplementedException();
        }

        public static void GetAllowableContactStress(GearDrive drive)
        {
            throw new NotImplementedException();
        }

        public static void CalcAdjustedContactStress(GearDrive drive)
        {
            throw new NotImplementedException();
        }

        public static void CalcAdjustedBendingStress(GearDrive drive)
        {
            double Sat = drive.AllowableBendingStress;
            double Yn = drive.pinion.BendingStressCycleFactor;
            //double 
        }
    }
}

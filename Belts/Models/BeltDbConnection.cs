using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Dapper;


namespace BeltWindow.Models
{
    public static class BeltDbConnection
    {
        public static string CnnString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        public static void GetAllBeltData(BeltProperties belt)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(CnnString("FlatBeltDrive")))
            {
                dynamic output = null;

                try
                {
                    switch (belt.BeltMaterialId)
                    {
                        case 0:
                            output = connection.Query<dynamic>("dbo.spPolyamideBeltDataSelection",
                            new { Specification = belt.Specification, Size = belt.BeltThickness },
                            commandType: CommandType.StoredProcedure).Single();
                            break;

                        case 1:
                            output = connection.Query<dynamic>("dbo.spLeatherBeltDataSelection",
                            new { Specification = belt.Specification, Size = belt.BeltThickness },
                            commandType: CommandType.StoredProcedure).Single();
                            break;

                        case 2:
                            output = connection.Query<dynamic>("dbo.spUrethaneFlatBeltDataSelection",
                            new { Specification = belt.Specification, Size = belt.BeltThickness },
                            commandType: CommandType.StoredProcedure).Single();
                            break;
                        default:
                            break;
                    }

                    belt.SpecificWeight = (double)output.SpecificWeight;
                    //TODO - rewrite the Stored Procedure for urethane flat and round belts;
                    belt.MinSmallPulleySize = output.MinPulleyDiameter;

                    belt.AllowableTension = (double)output.AllowableTension;

                    belt.MaxFriction = (double)output.CoefficientOfFriction;
                }
                catch (SystemException e)
                {

                    MessageBox.Show($"{e.Message}\n\n For more Information. visit :{e.HelpLink}", "Database Connection Error");
                    return;
                }
            }
        }

        public static List<string> GetBeltSpecification(int selection)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(CnnString("FlatBeltDrive")))
            {
                List<string> output = new List<string>();

                try
                {
                    switch (selection)
                    {
                        case 0:
                            output = connection.Query<string>("dbo.spGetPolyamideSpecificaton_All",
                            commandType: CommandType.StoredProcedure).ToList();
                            break;

                        case 1:
                            output = connection.Query<string>("dbo.spGetLeatherSpecificaton_All",
                            commandType: CommandType.StoredProcedure).ToList();
                            break;

                        case 2:
                            output = connection.Query<string>("dbo.spGetUrethaneFlatSpecificaton_All",
                            commandType: CommandType.StoredProcedure).ToList();
                            break;

                        default:
                            break;
                    }

                    
                }
                catch (SystemException e)
                {

                    MessageBox.Show($"{e.Message}\n\n For more Information. visit :{e.HelpLink}", "Database Connection Error");
                }
                return output;
            }
        }


        public static List<float> GetBeltThicknessValues( int selection , String specification )
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(CnnString("FlatBeltDrive")))
            {
                List<float> output = new List<float>();

                try
                {
                    switch (selection)
                    {
                        case 0:
                            output = connection.Query<float>("dbo.spGetPolyamideBeltThickness_All", new { Specification = specification },
                            commandType: CommandType.StoredProcedure).ToList();
                            break;

                        case 1:
                            output = connection.Query<float>("dbo.spGetLeatherBeltThickness_All", new { Specification = specification },
                            commandType: CommandType.StoredProcedure).ToList();
                            break;

                        case 2:
                            output = connection.Query<float>("dbo.spGetUrethaneFlatBeltThickness_All", new { Specification = specification },
                            commandType: CommandType.StoredProcedure).ToList();
                            break;

                        default:
                            break;
                    }
                }
                catch (SystemException e)
                {

                    MessageBox.Show($"{e.Message}\n\n For more Information. visit :{e.HelpLink}", "Database Connection Error");
                }

                return output;
            }
        }


        public static double GetVelocityCorrectionFactor(BeltProperties belt)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(CnnString("FlatBeltDrive")))
            {
                double output = 1;

                try
                {
                    switch (belt.BeltMaterialId)
                    {
                        case 0:
                            break;

                        case 1:
                            output = connection.Query<float>("dbo.spGetLeatherVelocityCorrectionFactor",
                                new { @BeltThickness = belt.BeltThickness, @BeltSpeed = belt.LinearVelocity_Small },
                            commandType: CommandType.StoredProcedure).Single();
                            break;

                        case 2:
                        default:
                            break;
                    }
                }
                catch (SystemException e)
                {
                    MessageBox.Show($"{e.Message}\n\n For more Information. visit :{e.HelpLink}", "Database Connection Error");
                }

                return output;
            }

            }

        public static double GetPulleyCorrectionFactor(BeltProperties belt)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(CnnString("FlatBeltDrive")))
            {
                double output = 1;

                try
                {
                    switch (belt.BeltMaterialId)
                    {
                        case 0:
                            output = connection.Query<float>("dbo.spGetPolyamidePulleyCorrectionFactor",
                                new { @Specification = belt.Specification, @PulleyDiameter = belt.SmallPulleySize },
                            commandType: CommandType.StoredProcedure).Single();
                            break;

                        case 1:
                            output = connection.Query<float>("dbo.spGetLeatherPulleyCorrectionFactor", new { @PulleyDiameter = belt.SmallPulleySize },
                            commandType: CommandType.StoredProcedure).Single();
                            break;

                        case 2:
                        default:
                            break;
                    }
                }
                catch (SystemException e)
                {

                    MessageBox.Show($"{e.Message}\n\n For more Information. visit :{e.HelpLink}", "Database Connection Error");
                }

                return output;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using Dapper;
using System.Windows;

namespace GearWindow.Models
{
    public static class GearDbConnection
    {
        //Get the Database Configurations
        public static string CnnString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        public static int GetElasticCoefficient(string pinionMaterial, string GearMaterial)
        {
            int output = 0;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(CnnString("SpurGearDrive")))
            {
                try
                {
                    output = connection.QuerySingle<int>("dbo.spGetElasticCoefficient",
                        new { @PinionMaterial = pinionMaterial, @GearMaterial = GearMaterial },
                            commandType: CommandType.StoredProcedure);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Database Connection Failed");
                }
            }

            return output;
        }

        public static double GetLewisFormFactor(int numOfTeeth)
        {
            double output = 0;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(CnnString("SpurGearDrive")))
            {
                try
                {
                    output = connection.QuerySingle<double>("dbo.spGetFormFactor",
                        new { @num = numOfTeeth },
                            commandType: CommandType.StoredProcedure);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Database Connection Failed");
                }

                return output;
            }
        }

        public static List<OverloadFactors> GetOverloadFactors()
        {
            List<OverloadFactors> factors = new List<OverloadFactors>();
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(CnnString("SpurGearDrive")))
            {
                try
                {
                    factors = connection.Query<OverloadFactors>("SELECT * FROM dbo.OverloadFactors").ToList();
                }
                catch (SystemException e)
                {

                    MessageBox.Show(e.Message, "Database Connection Failed");
                }
            }

            return factors;
        }
    }
}

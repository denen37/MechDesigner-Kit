using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GearWindow.Models
{
    class Applications
    {
        public int Id { get; set; }

        public string Application { get; set; }

        public string DesignLife { get; set; }

        public int GetAverage()
        {
            string range = DesignLife;

            string [] hours = range.Split('-');

           string s1 =  hours[0].Replace(" ", string.Empty);
            string s2 = hours[1].Replace(" ", string.Empty);

            int a, b;
            try
            {
                bool validA = int.TryParse(s1, out a);
                bool validB = int.TryParse(s2, out b);

                if (validA == false || validB == false)
                {
                    throw new Exception($"Cannot convert value at {Id}");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return 0;
            }

            int avg = (a + b) / 2;
            return avg;
        }

        public int Average
        {
            get { return GetAverage(); }
        }
    }
}

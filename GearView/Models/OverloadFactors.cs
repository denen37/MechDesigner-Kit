using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GearWindow.Models
{
    public class OverloadFactors
    {
        public OverloadFactors() { }
   
        public int Id { get; set; }

        public string PowerSource { get; set; }

        public double Uniform { get; set; }

        public double ModerateShock { get; set; }

        public double LightShock { get; set; }

        
    }
}

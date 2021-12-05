using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeltWindow.Models
{
    public static class BeltCalculator
    {
        public static BeltProperties BeltManager(BeltProperties belt)
        {
            //BeltCalculations.CalculateLength(belt);

            //BeltCalculations.CalculateAnglesOfContact(belt);

            BeltCalculations.CalculateBeltWeight(belt);

            //BeltCalculations.CalculateLinearVelocity(belt);

            BeltCalculations.CalculateCentrifugalForce(belt);

            BeltCalculations.CalculateTorque(belt);

            BeltCalculations.CalculateNetTension(belt);

            BeltCalculations.CalculateLargestAllowableTension(belt);

            BeltCalculations.CalculateTightTension(belt);

            BeltCalculations.CalculateSlackTension(belt);

            BeltCalculations.CalculateInitialTension(belt);

            BeltCalculations.CalculateFrictionDevelopment(belt);

            BeltCalculations.CalculateDesignPower(belt);

            BeltCalculations.CalculateDip(belt);
           
            return belt;
        }
    }
}

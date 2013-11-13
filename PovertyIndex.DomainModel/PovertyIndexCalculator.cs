using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PovertyIndex.DomainModel
{
    public class PovertyIndexCalculator
    {
        private double alpha;

        public PovertyIndexCalculator(double alpha)
        {
            this.alpha = alpha;
        }

        public double Calculate(double sequenceEffect, double emergencyEffect)
        {
            return (alpha * sequenceEffect) + ((1.0 - alpha) * emergencyEffect);
        }
    }
}

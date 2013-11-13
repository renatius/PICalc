using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PovertyIndex.DomainModel
{
    /// <summary>
    /// The poverty status of an individual in a given year
    /// </summary>
    public class PovertyStatus
    {
        public int Year { get; set; }
        public bool IsPoor { get; set; }
        public double PovertyGap { get; set; }
    }
}

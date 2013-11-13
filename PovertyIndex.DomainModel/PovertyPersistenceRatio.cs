using System;
using FileHelpers;

namespace PovertyIndex.DomainModel
{
    [DelimitedRecord(";")]
    public sealed class PovertyPersistenceRatio
    {
        [FieldQuoted('"', QuoteMode.AlwaysQuoted, MultilineMode.AllowForRead)]
        private string country;
        public string Country
        {
            get
            {
                return country;
            }
            private set
            {
                country = value;
            }
        }
        private int lowYear;
        public int LowYear
        {
            get
            {
                return lowYear;
            }
            private set
            {
                lowYear = value;
            }
        }
        private int highYear;
        public int HighYear
        {
            get
            {
                return highYear;
            }
            private set
            {
                highYear = value;
            }
        }
        private int populationSize;
        public int PopulationSize
        {
            get
            {
                return populationSize;
            }
            private set
            {
                populationSize = value;
            }
        }
        private int highYearStillPoorCount;
        public int PoorInBothYears
        {
            get
            {
                return highYearStillPoorCount;
            }
            private set
            {
                highYearStillPoorCount = value;
            }
        }
        private double _value;
        public double PermanenceProbability
        {
            get
            {
                return _value;
            }
            private set
            {
                _value = value;
            }
        }

        // needed by FileHelpers Library
        public PovertyPersistenceRatio() { }

        public PovertyPersistenceRatio(
            string country,
            int lowYear,
            int highYear,
            int populationSize,
            int highYearStillPoorCount
        )
        {
            Precondition.Require(lowYear < highYear, "The low year argument must be less than the high year argument");
            Precondition.Require(populationSize >= 0, "the population size cannot be a negative value");
            Precondition.Require(highYearStillPoorCount >= 0, "the still poor count cannot be a negative value");
            Precondition.Require(highYearStillPoorCount <= populationSize, "The high year count cannot be greater than the low year count");            

            Country = country;
            LowYear = lowYear;
            HighYear = highYear;
            PopulationSize = populationSize;
            PoorInBothYears = highYearStillPoorCount;

            PermanenceProbability = PopulationSize > 0 ?
                ((double)PoorInBothYears) / ((double)PopulationSize) : 0;
        }
    }
}

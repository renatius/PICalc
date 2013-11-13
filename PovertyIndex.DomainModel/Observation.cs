using System;
using FileHelpers;

namespace PovertyIndex.DomainModel
{
    /// <summary>
    /// The "raw data" as provided by the flat file.
    /// </summary>
    [DelimitedRecord(";"), IgnoreFirst(1), IgnoreEmptyLines()]
    public sealed class Observation
    {
        private Int32 mYear;

        public Int32 Year
        {
            get { return mYear; }
            set { mYear = value; }
        }

        [FieldQuoted('"', QuoteMode.AlwaysQuoted, MultilineMode.AllowForRead)]
        private String mCountry;

        public String Country
        {
            get { return mCountry; }
            set { mCountry = value; }
        }


        private String mHouseholdId;

        public String HouseholdId
        {
            get { return mHouseholdId; }
            set { mHouseholdId = value; }
        }

        private String mPersonId;

        public String PersonId
        {
            get { return mPersonId; }
            set { mPersonId = value; }
        }


        private Int32 mIsPoor;

        public Int32 IsPoor
        {
            get { return mIsPoor; }
            set { mIsPoor = value; }
        }


        private double mPovertyGap;

        public double PovertyGap
        {
            get { return mPovertyGap; }
            set { mPovertyGap = value; }
        }
    }
}

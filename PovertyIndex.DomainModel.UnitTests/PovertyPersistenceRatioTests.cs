using NUnit.Framework;
namespace PovertyIndex.DomainModel.UnitTests
{
    [TestFixture]
    public class PovertyPersistenceRatioTests
    {
        PovertyPersistenceRatio ratio;
        string country = "denmark";
        int lowYear = 1994;
        int highYear = 2000;
        int lowYearCount = 600;
        int highYearCount = 200;

        [SetUp]
        public void SetUp()
        {
            ratio = new PovertyPersistenceRatio(country, lowYear, highYear, lowYearCount, highYearCount);
        }

        [Test]
        public void WhenCreatedCountryPropertyIsEqualToCountryArgument()
        {
            Assert.AreEqual(country, ratio.Country);            
        }

        [Test]
        public void WhenCreatedLowYearPropertyIsEqualToLowYearArgument()
        {
            Assert.AreEqual(lowYear, ratio.LowYear);
        }

        [Test]
        public void WhenCreatedHighYearPropertyIsEqualToHighYearArgument()
        {
            Assert.AreEqual(highYear, ratio.HighYear);
        }

        [Test]
        public void WhenCreatedLowYearPoorCountIsEqualToLowYearPoorCountArgument()
        {
            Assert.AreEqual(lowYearCount, ratio.PopulationSize);
        }

        [Test]
        public void WhenCreatedHighYearStillPoorCountIsEqualToHighYearStillPoorCountArgument()
        {
            Assert.AreEqual(highYearCount, ratio.PoorInBothYears);
        }

        [Test]
        public void WhenLowYearPoorCountIsZeroValueIsZero()
        {
            var ratio = new PovertyPersistenceRatio("XXX", 1999, 2000, 0, 0);
            Assert.AreEqual(0d, ratio.PermanenceProbability, double.Epsilon);
        }

        [Test]
        public void WhenCreatedValuePropertyIsEqualToRatioBetweenHighYearStillPoorCountAndLowYearPoorCount()
        {
            double expected = ((double)highYearCount)/((double)lowYearCount);
            Assert.AreEqual(expected, ratio.PermanenceProbability, double.Epsilon);
        }

        [Test, ExpectedException(typeof(BusinessException))]
        public void WhenLowYearIsNotLessThanHighYearABusinessExceptionIsThrown()
        {
            var ratio = new PovertyPersistenceRatio(country, 2000, 2000, lowYearCount, highYearCount);
        }

        [Test, ExpectedException(typeof(BusinessException))]
        public void WhenHighYearStillPoorCountIsGreaterThanLowYearPoorCountABusinessExceptionIsThrown()
        {
            var ratio = new PovertyPersistenceRatio(country, 1999, 2000, 1000, 1001);
        }

        [Test, ExpectedException(typeof(BusinessException))]
        public void WhenLowYearCountIsNegativeABusinessExceptionIsThrown()
        {
            var ratio = new PovertyPersistenceRatio(country, lowYear, highYear, -1, -1);
        }

        [Test, ExpectedException(typeof(BusinessException))]
        public void WhenHighYearCountIsNegativeABusinessExceptionIsThrown()
        {
            var ratio = new PovertyPersistenceRatio(country, lowYear, highYear, lowYearCount, -1);
        }
    }
}

using System.Collections.Generic;
using NUnit.Framework;

namespace PovertyIndex.DomainModel.UnitTests
{
    [TestFixture]
    public class PovertyPersistenceRatioCalculatorTests
    {
        const string italy = "italy";
        const string denmark = "denmark";
        string[] countries = { denmark, italy };
        string personId = "101";
        int[] years = { 1994, 1995, 1996 };
        Dictionary<int, bool> isPoor = new Dictionary<int, bool>();            

        List<Person> people;
        PovertyPersistenceRatioCalculator calculator;

        [SetUp]
        public void SetUp()
        {
            isPoor[1994] = true;
            isPoor[1995] = true;
            isPoor[1996] = false;

            people = new List<Person>();

            foreach (string country in countries)
            {
                Person p = new Person(country, personId);
                foreach (int year in years)
                {
                    p.AddObservation(year, isPoor[year], isPoor[year] ? 0.3 : 0);    
                }
                people.Add(p);
            }

            calculator = new PovertyPersistenceRatioCalculator(people);
        }

        [Test, ExpectedException(typeof(BusinessException))]
        public void WhenCreatedWithANullArgumentABusinessExceptionIsThrown()
        {
            var calculator = new PovertyPersistenceRatioCalculator(null);
        }

        [Test, ExpectedException(typeof(BusinessException))]
        public void WhenCreatedWithAnEmptyListArgumentABusinessExceptionIsThrown()
        {
            var calculator = new PovertyPersistenceRatioCalculator(new List<Person>());
        }

        [Test]
        public void WhenCalculatingOverTwoYearsOneRatioIsReturned()
        {
            var ratios = calculator.CalculateRatios(italy, 1994, 1995);
            Assert.AreEqual(1, ratios.Count);
        }

        [Test]
        public void WhenCalculatingOverTwoYearsOneRatioIsReturnedEvenWhenNoPeopleInSet()
        {
            var ratios = calculator.CalculateRatios("korea", 1994, 1995);
            Assert.AreEqual(1, ratios.Count);
        }

        [Test]
        public void WhenCalculatingOverThreeYearsThreeRatiosAreReturned()
        {
            var ratios = calculator.CalculateRatios(italy, 1994, 1996);
            Assert.AreEqual(3, ratios.Count);
        }

        [Test]
        public void WhenCalculatingOverThreeYearsThreeRatiosAreReturnedEvenWhenNoPeopleInSet()
        {
            var ratios = calculator.CalculateRatios("korea", 1994, 1996);
            Assert.AreEqual(3, ratios.Count);
        }
    }
}

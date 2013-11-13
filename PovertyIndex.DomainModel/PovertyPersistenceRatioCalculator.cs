using System;
using System.Linq;
using System.Collections.Generic;

namespace PovertyIndex.DomainModel
{
    public class PovertyPersistenceRatioCalculator
    {
        private List<Person> individuals;        
        
        public PovertyPersistenceRatioCalculator(List<Person> individuals)
        {
            Precondition.Require(individuals != null, "You must pass a list of individuals");
            Precondition.Require(individuals.Count > 0, "The list of individuals must not be empty");

            this.individuals = individuals;
        }

        public List<PovertyPersistenceRatio> CalculateRatios
        (
            string country, 
            int yearMin, 
            int yearMax 
        )
        {
            var ratios = new List<PovertyPersistenceRatio>();

            for (int lowYear = yearMin; lowYear < yearMax; lowYear++)
            {
                var people = from Person p in individuals
                             where p.Country == country
                             select p;

                var poors = from Person p in people
                            where p.IsPoorInYear(lowYear)
                            select p;

                for (int highYear = lowYear + 1; highYear <= yearMax; highYear++)                
                {
                    var stillPoors = from Person p in poors
                                     where p.IsPoorInYear(highYear)
                                     select p;

                    ratios.Add(new PovertyPersistenceRatio(country, lowYear, highYear, people.Count(), stillPoors.Count()));
                }
            }

            return ratios;
        }
    }
}

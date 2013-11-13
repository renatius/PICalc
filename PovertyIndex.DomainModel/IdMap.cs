using System;
using System.Collections.Generic;

namespace PovertyIndex.DomainModel
{
    public class IdMap
    {
        public IdMap()
        {
            map = new Dictionary<string, int>();
            generator = new Sequence();
        }

        public int GetIdFor(Observation observation)
        {
            string key = MakeSurrogateKey(observation.Country, observation.HouseholdId, observation.PersonId);
            return GetIdForKey(key);
        }

        private static string MakeSurrogateKey(string country, string householdId, string personId)
        {
            return string.Format("{0}_{2}", personId, householdId, country);
        }

        private int GetIdForKey(string key)
        {
            if (map.ContainsKey(key))
            {
                return map[key];
            }
            else
            {
                int id = generator.GetNextValue();
                map[key] = id;
                return id;
            }
        }

        private Dictionary<string, int> map;
        private Sequence generator;
    }
}

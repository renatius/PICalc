using System;
using System.Collections.Generic;

namespace PovertyIndex.DomainModel
{
    public class PersonFactory
    {
        public PersonFactory()
        {
            peopleCache = new Dictionary<int, Person>();
            idMap = new IdMap();
        }

        public Person CreateInstance(Observation observation)
        {
            int id = idMap.GetIdFor(observation);

            if (peopleCache.ContainsKey(id))
            {
                return peopleCache[id];
            }
            else
            {
                string country = observation.Country;
                string personId = observation.PersonId;
                Person person = new Person(country, personId);
                peopleCache[id] = person;
                return person;
            }
        }

        public List<Person> GetPeople()
        {
            return new List<Person>(peopleCache.Values);
        }

        Dictionary<int, Person> peopleCache;
        IdMap idMap;
    }
}

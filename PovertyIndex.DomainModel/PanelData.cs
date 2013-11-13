using System;
using System.Collections.Generic;
using System.Linq;
using FileHelpers;

namespace PovertyIndex.DomainModel
{
    /// <summary>
    /// class to hold panel data
    /// </summary>
    public class PanelData
    {
        /// <summary>
        /// The list of observations read from the flat file
        /// </summary>
        public List<Observation> Observations
        {
            get { return observations; }
        }

        /// <summary>
        /// The list of individuals described in the panel
        /// </summary>
        public List<Person> People
        {
            get { return people; }
        }

        /// <summary>
        /// The list of errors found in the panel data
        /// </summary>
        public List<PanelError> Errors
        {
            get { return errors; }
        }

        /// <summary>
        /// Can this data be used to calculate the poverty index?
        /// </summary>
        public bool IsValid
        {
            get { return 0 == Errors.Count; }
        }        

        /// <summary>
        /// The calculated poverty persistence ratios
        /// </summary>
        public List<PovertyPersistenceRatio> PovertyPersistenceRatios
        {
            get { return povertyPersistenceRatios; }
        }

        public void LoadFromFile(string filename)
        {
            Precondition.Require(!string.IsNullOrEmpty(filename), 
                "Please specify an input file");

            try
            {
                LoadObservations(filename);
                GatherObservationStatistics();
                ValidateObservations();
                PopulatePeopleList();                
                ValidatePeopleList();

                if (IsValid)
                {
                    CalculatePovertyPersistenceRatios();
                    CalculateEmergencyEffects();
                    CalculateSequenceEffects();
                    CalculateBossertIndexes();
                    CalculateBCD2Indexes();
                }
            }
            catch (Exception exc)
            {
                string errorMessage = string.Format("Error reading panel data from file '{1}':\n{0}",
                    exc.Message, filename);
                throw new BusinessException(errorMessage, exc);
            }
        }

        private void AddError(string message)
        {
            errors.Add(new PanelError(message));
        }

        private void LoadObservations(string filename)
        {
            Observation[] tmp = LoadObservationsFromFile(filename);
            RaiseErrorIfNoObservationFound(tmp);
            observations = new List<Observation>(tmp);            
            Postcondition.Ensure(observations.Count == tmp.Length, "Failed loading observations");
        }

        private static void RaiseErrorIfNoObservationFound(Observation[] tmp)
        {
            if (tmp == null || tmp.Length == 0)
            {
                string errorMessage = "no observations found";
                throw new BusinessException(errorMessage);
            }
        }

        private static Observation[] LoadObservationsFromFile(string filename)
        {
            var reader = new FileHelperEngine<Observation>();
            Observation[] tmp = reader.ReadFile(filename);
            return tmp;
        }

        private void GatherObservationStatistics()
        {
            yearMin = (from Observation x in observations
                       select x.Year).Min();

            yearMax = (from Observation x in observations
                       select x.Year).Max();

            yearSpan = 1 + (yearMax - yearMin);
        }

        private void ValidateObservations()
        {
            observations.ForEach(obs => ValidateObservation(obs));

            if (!(yearMin < yearMax))
            {
                AddError("The panel observations must span more than 1 year");
            }

            if (yearSpan > BinomialCoefficientChooseTwo.MaxN)
            {
                string errorMessage =
                    string.Format(
                        "The oldest and the most recent observation are separated by more than {0} years",
                        BinomialCoefficientChooseTwo.MaxN);
                AddError(errorMessage);
            }
        }

        private static void ValidateObservation(Observation observation)
        {
            if (string.IsNullOrEmpty(observation.Country))
            {
                throw new BusinessException("country field in observation cannot be null");
            }

            if (string.IsNullOrEmpty(observation.HouseholdId))
            {
                throw new BusinessException("householdId field in observation cannot be null");
            }

            if (string.IsNullOrEmpty(observation.PersonId))
            {
                throw new BusinessException("personId field in observation cannot be null");
            }            
        }

        private void PopulatePeopleList()
        {
            PersonFactory factory = new PersonFactory();

            observations.ForEach(obs =>
            {
                Person person = factory.CreateInstance(obs);
                person.AddObservation(obs.Year, obs.IsPoor == 1, obs.PovertyGap);
            });

            people = factory.GetPeople();
        }

        private void ValidatePeopleList()
        {
            people.ForEach(p => ValidatePerson(p));
        }

        private static string GetIdStringFor(Person p)
        {
            return string.Format("({0}, {1})", p.Country, p.PersonId);
        }

        private void ValidatePerson(Person p)
        {
            if (p.MinYear != yearMin)
            {
                AddError(string.Format("{0}: first observation year does not correspond to first year for panel", 
                    GetIdStringFor(p)));
            }

            if (p.MaxYear != yearMax)
            {
                AddError(string.Format("{0}: last observation year does not correspond to last year for panel", 
                    GetIdStringFor(p)));
            }

            if (p.ObservationCount != yearSpan)
            {
                AddError(string.Format("{0}: observations do not cover the entire span",
                    GetIdStringFor(p)));
            }
        }

        private void CalculatePovertyPersistenceRatios()
        {
            var ratios = new List<PovertyPersistenceRatio>();
            var countries = (from Person p in people
                             select p.Country).Distinct(); 


            foreach (var country in countries)
            {
                var calculator = new PovertyPersistenceRatioCalculator(people);
                ratios.AddRange(calculator.CalculateRatios(country, yearMin, yearMax));
            }

            povertyPersistenceRatios = ratios;
        }

        private void CalculateSequenceEffects()
        {
            var poors = from p in people
                        where p.IsEverPoor
                        select p;

            var calculator = new SequenceEffectCalculator(yearMin, yearMax, povertyPersistenceRatios);

            foreach (var p in poors)
            {
                sequenceEffects[p] = calculator.CalculateFor(p);
            }
        }

        private void CalculateEmergencyEffects()
        {
            var poors = from p in people
                        where p.IsEverPoor
                        select p;

            var calculator = new EmergencyEffectCalculator(yearMin, yearMax);

            foreach (var p in poors)
            {
                emergencyEffects[p] = calculator.CalculateFor(p);
            }
        }

        private void CalculateBossertIndexes()
        {
            var poors = from p in people
                        where p.IsEverPoor
                        select p;

            foreach (var p in poors)
            {
                bossertIndexes[p] = p.GetBossertIndex();
            }
        }

        public void CalculateBCD2Indexes()
        {
            var poors = from p in people
                        where p.IsEverPoor
                        select p;

            foreach (var p in poors)
            {
                bcd2Indexes[p] = p.GetBossert2Index();
            }            
        }

        private SequenceEffect GetSequenceEffect(Person p)
        {
            return sequenceEffects[p];
        }

        private double GetEmergencyEffect(Person p)
        {
            return emergencyEffects[p];
        }

        /*
        private double GetBossetIndex(Person p)
        {
            return bossertIndexes[p];
        }

        private double GetBCD2Index(Person p)
        {
            return bcd2Indexes[p];
        }
        */

        public List<PovertyIndexResult> CalculatePovertyIndex(double alpha)
        {
            Precondition.Require(IsValid,
                "Cannot calculate Poverty Index when panel data contain errors");
            
            var result = new List<PovertyIndexResult>();
            
            var poors = from p in people
                        where p.IsEverPoor
                        select p;

            var calculator = new PovertyIndexCalculator(alpha);

            foreach (var p in poors)
            {
                var sequenceEffect = GetSequenceEffect(p);
                double emergencyEffect = GetEmergencyEffect(p);                

                var pi = new PovertyIndexResult()
                                {
                                    Country = p.Country,
                                    PersonId = p.PersonId,
                                    PovertySequence = p.GetPovertyVectorAsStringBetween(yearMin, yearMax),
                                    PovertyGapSequence = p.GetPovertyGapVectorAsStringBetween(yearMin, yearMax),
                                    PovertyGapAverage = p.GetPovertyGapAverage(),
                                    MaxSpell = p.GetMaxSpell(),
                                    SequenceEffect1 = sequenceEffect.V1,
                                    SequenceEffect2 = sequenceEffect.V2,
                                    SequenceEffect3 = sequenceEffect.V3,
                                    SequenceEffect4 = sequenceEffect.V4,
                                    SequenceEffect5 = sequenceEffect.V5,
                                    EmergencyEffect = emergencyEffect,
                                    // BossertIndex = GetBossetIndex(p),
                                    // BCD2 = GetBCD2Index(p),
                                    Alpha = alpha,
                                    SE_EE_1 = calculator.Calculate(sequenceEffect.V1, emergencyEffect),
                                    SE_EE_2 = calculator.Calculate(sequenceEffect.V2, emergencyEffect),
                                    SE_EE_3 = calculator.Calculate(sequenceEffect.V3, emergencyEffect),
                                    SE_EE_4 = calculator.Calculate(sequenceEffect.V4, emergencyEffect),
                                    SE_EE_5 = calculator.Calculate(sequenceEffect.V5, emergencyEffect)
                                };

                result.Add(pi);
            }

            return result;
        }

        private List<PanelError> errors = new List<PanelError>();
        private List<Observation> observations = new List<Observation>();
        private List<Person> people = new List<Person>();
        private List<PovertyPersistenceRatio> povertyPersistenceRatios = new List<PovertyPersistenceRatio>();
        private Dictionary<Person, SequenceEffect> sequenceEffects = new Dictionary<Person, SequenceEffect>();
        private Dictionary<Person, double> emergencyEffects = new Dictionary<Person, double>();
        private Dictionary<Person, double> bossertIndexes = new Dictionary<Person, double>();
        private Dictionary<Person, double> bcd2Indexes = new Dictionary<Person, double>();

        // observation stats
        private int yearMin = int.MaxValue;
        private int yearMax = int.MinValue;
        private int yearSpan = 0;
    }
}

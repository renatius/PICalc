namespace PovertyIndex.DomainModel
{
    public class Sequence
    {
        private int counter;

        public Sequence()            
        {
            counter = 0;
        }

        public int GetNextValue()
        {
            try
            {
                checked
                {
                    counter += 1;
                }
                return counter;
            }
            catch (System.OverflowException ex)
            {
                throw new BusinessException(ex.Message);
            }
        }
    }
}

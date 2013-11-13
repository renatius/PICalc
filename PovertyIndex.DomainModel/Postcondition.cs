namespace PovertyIndex.DomainModel
{
    public class Postcondition
    {
        public static void Ensure(bool condition, string errorMessage)
        {
            if (!condition)
            {
                throw new BusinessException(errorMessage);
            }
        }
    }
}

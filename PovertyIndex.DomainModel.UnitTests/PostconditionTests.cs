using NUnit.Framework;

namespace PovertyIndex.DomainModel.UnitTests
{
    [TestFixture]
    public class PostconditionTests
    {
        [Test]
        public void WhenConditionIsTrueNoExceptionIsThrown()
        {
            bool condition = 1 == 1;
            Postcondition.Ensure(condition, "No Error");
            Assert.IsTrue(condition);
        }

        [Test, ExpectedException(typeof(BusinessException))]
        public void WhenConditionIsFalseABusinessExceptionIsRaised()
        {
            bool condition = 1 == 0;
            Postcondition.Ensure(condition, "Error Message");
        }

        [Test]
        public void WhenConditionIsFalseTheExceptionMessageIsEqualToTheMessageArgument()
        {
            string errorMessage = "abcdef#12345";
            try
            {
                bool condition = 1 == 0;
                Postcondition.Ensure(condition, errorMessage);
            }
            catch (BusinessException ex)
            {
                Assert.AreEqual(errorMessage, ex.Message);
            }
        }
    }
}

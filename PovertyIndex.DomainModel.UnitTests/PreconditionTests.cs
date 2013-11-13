using NUnit.Framework;

namespace PovertyIndex.DomainModel.UnitTests
{
    [TestFixture]
    public class PreconditionTests
    {
        [Test]
        public void WhenConditionIsTrueNoExceptionIsThrown()
        {
            bool condition = 1 == 1;
            Precondition.Require(condition, "No Error");
            Assert.IsTrue(condition);
        }

        [Test, ExpectedException(typeof(BusinessException))]
        public void WhenConditionIsFalseABusinessExceptionIsRaised()
        {
            bool condition = 1 == 0;
            Precondition.Require(condition, "Error Message");
        }

        [Test]
        public void WhenConditionIsFalseTheExceptionMessageIsEqualToTheMessageArgument()
        {
            string errorMessage = "abcdef#12345";
            try
            {
                bool condition = 1 == 0;
                Precondition.Require(condition, errorMessage);
            }
            catch (BusinessException ex)
            {
                Assert.AreEqual(errorMessage, ex.Message);
            }
        }
    }
}

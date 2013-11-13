using NUnit.Framework;
namespace PovertyIndex.DomainModel.UnitTests
{
    [TestFixture]
    public class SequenceTests
    {
        Sequence sequence = new Sequence();

        [Test]
        public void WhenCalledFirstTimeReturns1()
        {
            Assert.AreEqual(1, sequence.GetNextValue());
        }

        [Test]
        public void WhenGetNextValueIsCalledTheReturnedValueIsGreaterThanThePreviousReturnedValue()
        {
            int oldValue = sequence.GetNextValue();

            for (int i = 0; i < 1000; i++)
            {
                int newValue = sequence.GetNextValue();
                Assert.IsTrue(newValue > oldValue);
                oldValue = newValue;
            }
        }

        [Explicit]
        [Test, ExpectedException(typeof(BusinessException))]
        public void WhenSequenceOverflowsABusinessExceptionIsThrown()
        {
            while (int.MaxValue != sequence.GetNextValue())
            {
                // do nothing
            }
            sequence.GetNextValue();
        }

    }
}


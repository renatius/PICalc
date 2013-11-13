using NUnit.Framework;

namespace PovertyIndex.DomainModel.UnitTests
{
    [TestFixture]
    public class BinomialCoefficientChooseTwoTests
    {

        [Test, ExpectedException(typeof(BusinessException))]
        public void WhenNIsZeroOfThrowsABusinessException()
        {
            BinomialCoefficientChooseTwo.Of(0);
        }

        [Test, ExpectedException(typeof(BusinessException))]
        public void WhenNIsOneOfThrowsABusinessException()
        {
            BinomialCoefficientChooseTwo.Of(1);
        }

        [Test, ExpectedException(typeof(BusinessException))]
        public void WhenNIsNegativeOfThrowsABusinessException()
        {
            BinomialCoefficientChooseTwo.Of(-1);
        }

        [Test, ExpectedException(typeof(BusinessException))]
        public void WhenNExceedsMaxNOfThrowsABusinessException()
        {
            BinomialCoefficientChooseTwo.Of(BinomialCoefficientChooseTwo.MaxN + 1);
        }

        [Test]
        public void WhenNIsMaxNOfReturnsAValue()
        {
            int bc = BinomialCoefficientChooseTwo.Of(BinomialCoefficientChooseTwo.MaxN);
            Assert.IsTrue(0 != bc);
        }

        [Test]
        public void WhenNIsMinNOfReturnsAValue()
        {
            int bc = BinomialCoefficientChooseTwo.Of(BinomialCoefficientChooseTwo.MinN);
            Assert.IsTrue(0 != bc);
        }

        [Test]
        public void WhenNIs2NOfReturns1()
        {
            int bc = BinomialCoefficientChooseTwo.Of(2);
            Assert.AreEqual(1, bc);
        }

        [Test]
        public void WhenNIs7NOfReturns21()
        {
            int bc = BinomialCoefficientChooseTwo.Of(7);
            Assert.AreEqual(21, bc);
        }

    }

}

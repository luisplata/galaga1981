using NUnit.Framework;

namespace V2.Tests
{
    public class HighScoreStoreTests
    {
        [Test]
        public void ShouldReplace_Higher_True()
        {
            Assert.IsTrue(HighScoreStore.ShouldReplace(6000, 3000));
        }

        [Test]
        public void ShouldReplace_Lower_False()
        {
            Assert.IsFalse(HighScoreStore.ShouldReplace(3000, 5000));
        }

        [Test]
        public void ShouldReplace_Equal_False()
        {
            Assert.IsFalse(HighScoreStore.ShouldReplace(5000, 5000));
        }

        [Test]
        public void ShouldReplace_Negative_False()
        {
            Assert.IsFalse(HighScoreStore.ShouldReplace(-1, 0));
        }
    }
}

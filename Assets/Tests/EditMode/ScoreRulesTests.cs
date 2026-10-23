using NUnit.Framework;

namespace V2.Tests
{
    public class ScoreRulesTests
    {
        [TestCase(150, 225)]
        [TestCase(100, 150)]
        [TestCase(50, 75)]
        public void Apply_Inside_ReturnsOnePointFive(int basePoints, int expected)
        {
            Assert.AreEqual(expected,
                ScoreRules.ApplyProximity(basePoints, 2.0f, ScoreRules.NearRadius));
        }

        [Test]
        public void Apply_AtExactRadius_Bonuses()
        {
            Assert.AreEqual(225,
                ScoreRules.ApplyProximity(150, ScoreRules.NearRadius, ScoreRules.NearRadius));
        }

        [Test]
        public void Apply_Outside_ReturnsBase()
        {
            Assert.AreEqual(150,
                ScoreRules.ApplyProximity(150, ScoreRules.NearRadius + 0.01f, ScoreRules.NearRadius));
        }
    }
}

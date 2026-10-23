using NUnit.Framework;

namespace V2.Tests
{
    public class DifficultyTests
    {
        private const float Epsilon = 0.001f;

        [Test]
        public void ForLevel_1_ReturnsBase()
        {
            DifficultySettings d = Difficulty.ForLevel(1);
            Assert.AreEqual(5f, d.DiveCooldownMin, Epsilon);
            Assert.AreEqual(8f, d.DiveCooldownMax, Epsilon);
            Assert.AreEqual(1f, d.DiveSpeedMul, Epsilon);
            Assert.AreEqual(0.35f, d.SpawnInterval, Epsilon);
            Assert.AreEqual(2, d.MaxDivers);
            Assert.AreEqual(10f, d.TractorCooldownMin, Epsilon);
            Assert.AreEqual(14f, d.TractorCooldownMax, Epsilon);
        }

        [Test]
        public void ForLevel_5_MatchesCurve()
        {
            DifficultySettings d = Difficulty.ForLevel(5);
            Assert.AreEqual(3.582f, d.DiveCooldownMin, 0.01f);
            Assert.AreEqual(5.731f, d.DiveCooldownMax, 0.01f);
            Assert.AreEqual(1.2f, d.DiveSpeedMul, Epsilon);
            Assert.AreEqual(0.310f, d.SpawnInterval, 0.01f);
            Assert.AreEqual(3, d.MaxDivers);
            Assert.AreEqual(8.145f, d.TractorCooldownMin, 0.01f);
            Assert.AreEqual(11.403f, d.TractorCooldownMax, 0.01f);
        }

        [Test]
        public void ForLevel_10_ClampsAtFloors()
        {
            DifficultySettings d = Difficulty.ForLevel(10);
            Assert.AreEqual(2.5f, d.DiveCooldownMin, Epsilon);
            Assert.AreEqual(4f, d.DiveCooldownMax, Epsilon);
            Assert.AreEqual(1.45f, d.DiveSpeedMul, Epsilon);
            Assert.AreEqual(0.266f, d.SpawnInterval, 0.01f);
            Assert.AreEqual(4, d.MaxDivers);
            Assert.AreEqual(6.30f, d.TractorCooldownMin, 0.02f);
            Assert.AreEqual(8.82f, d.TractorCooldownMax, 0.02f);
        }

        [Test]
        public void ForLevel_20_ClampsAtAllCeilings()
        {
            DifficultySettings d = Difficulty.ForLevel(20);
            Assert.AreEqual(2.5f, d.DiveCooldownMin, Epsilon);
            Assert.AreEqual(4f, d.DiveCooldownMax, Epsilon);
            Assert.AreEqual(1.5f, d.DiveSpeedMul, Epsilon);
            Assert.AreEqual(0.20f, d.SpawnInterval, Epsilon);
            Assert.AreEqual(4, d.MaxDivers);
            Assert.AreEqual(6f, d.TractorCooldownMin, Epsilon);
            Assert.AreEqual(8f, d.TractorCooldownMax, Epsilon);
        }

        [Test]
        public void ForLevel_100_IdenticalToLevel20()
        {
            AssertSame(Difficulty.ForLevel(20), Difficulty.ForLevel(100));
        }

        [Test]
        public void ForLevel_0_TreatedAsLevel1()
        {
            AssertSame(Difficulty.ForLevel(1), Difficulty.ForLevel(0));
            AssertSame(Difficulty.ForLevel(1), Difficulty.ForLevel(-7));
        }

        [Test]
        public void ForLevel_ThreatIsMonotonicNonDecreasing()
        {
            DifficultySettings previous = Difficulty.ForLevel(1);
            for (int level = 2; level <= 30; level++)
            {
                DifficultySettings current = Difficulty.ForLevel(level);
                Assert.LessOrEqual(current.DiveCooldownMin, previous.DiveCooldownMin + Epsilon);
                Assert.LessOrEqual(current.DiveCooldownMax, previous.DiveCooldownMax + Epsilon);
                Assert.LessOrEqual(current.SpawnInterval, previous.SpawnInterval + Epsilon);
                Assert.LessOrEqual(current.TractorCooldownMin, previous.TractorCooldownMin + Epsilon);
                Assert.LessOrEqual(current.TractorCooldownMax, previous.TractorCooldownMax + Epsilon);
                Assert.GreaterOrEqual(current.DiveSpeedMul, previous.DiveSpeedMul - Epsilon);
                Assert.GreaterOrEqual(current.MaxDivers, previous.MaxDivers);
                previous = current;
            }
        }

        private static void AssertSame(DifficultySettings a, DifficultySettings b)
        {
            Assert.AreEqual(a.DiveCooldownMin, b.DiveCooldownMin, Epsilon);
            Assert.AreEqual(a.DiveCooldownMax, b.DiveCooldownMax, Epsilon);
            Assert.AreEqual(a.DiveSpeedMul, b.DiveSpeedMul, Epsilon);
            Assert.AreEqual(a.SpawnInterval, b.SpawnInterval, Epsilon);
            Assert.AreEqual(a.MaxDivers, b.MaxDivers);
            Assert.AreEqual(a.TractorCooldownMin, b.TractorCooldownMin, Epsilon);
            Assert.AreEqual(a.TractorCooldownMax, b.TractorCooldownMax, Epsilon);
        }
    }
}

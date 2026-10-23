using NUnit.Framework;

namespace V2.Tests
{
    public class RunStateTests
    {
        [Test]
        public void Start_HasMaxLives()
        {
            var state = new RunState();
            Assert.AreEqual(RunState.MaxLives, state.Lives);
            Assert.AreEqual(3, state.Lives);
        }

        [Test]
        public void LoseLife_DecrementsOnce()
        {
            var state = new RunState();
            Assert.IsTrue(state.TryLoseLife());
            Assert.AreEqual(2, state.Lives);
        }

        [Test]
        public void LoseLife_AtZero_StaysZero()
        {
            var state = new RunState();
            Assert.IsTrue(state.TryLoseLife());
            Assert.IsTrue(state.TryLoseLife());
            Assert.IsTrue(state.TryLoseLife());
            Assert.IsFalse(state.TryLoseLife());
            Assert.AreEqual(0, state.Lives);
        }

        [Test]
        public void Reset_RestoresMaxLives()
        {
            var state = new RunState();
            state.TryLoseLife();
            state.TryLoseLife();
            state.Reset();
            Assert.AreEqual(RunState.MaxLives, state.Lives);
        }

        [Test]
        public void IsDead_TrueOnlyAtZero()
        {
            var state = new RunState();
            Assert.IsFalse(state.IsDead);
            state.TryLoseLife();
            Assert.IsFalse(state.IsDead);
            state.TryLoseLife();
            Assert.IsFalse(state.IsDead);
            state.TryLoseLife();
            Assert.IsTrue(state.IsDead);
        }
    }
}

using NUnit.Framework;
using UnityEngine;

namespace V2.Tests
{
    public class HitMathTests
    {
        [Test]
        public void WithinSquared_Inside_True()
        {
            Assert.IsTrue(HitMath.WithinSquared(new Vector2(0f, 0f), new Vector2(0.3f, 0f),
                HitMath.PlayerHitRadius));
        }

        [Test]
        public void WithinSquared_AtExactRadius_True()
        {
            Assert.IsTrue(HitMath.WithinSquared(new Vector2(0f, 0f), new Vector2(0.5f, 0f),
                HitMath.PlayerHitRadius));
        }

        [Test]
        public void WithinSquared_Outside_False()
        {
            Assert.IsFalse(HitMath.WithinSquared(new Vector2(0f, 0f), new Vector2(0.51f, 0f),
                HitMath.PlayerHitRadius));
        }

        [Test]
        public void WithinSquared_VerticalMiss_False()
        {
            Assert.IsFalse(HitMath.WithinSquared(new Vector2(0f, 0f), new Vector2(0f, 0.6f),
                HitMath.PlayerHitRadius));
        }
    }
}

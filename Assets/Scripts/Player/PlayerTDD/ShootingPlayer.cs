using System.Collections;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ShootingPlayer
    {
        IControllerShoot shootingView;
        LogicShooting logic;
        [SetUp]
        public void SetUp()
        {
            shootingView = Substitute.For<IControllerShoot>();
            logic = new LogicShooting(shootingView);
        }
        // A Test behaves as an ordinary method
        [Test]
        public void ShootingPlayerSimple_fire()
        {
            logic.IsPlayerFire(true, 0);
            
            shootingView.Received(1).PlayerShootingBullet();
        }

        [TestCase(2)]
        [TestCase(3)]
        public void WhenPlayerIsFireAndHaveMoreTwoOrMoreBullet_NotShooting(int countBullet)
        {
            logic.IsPlayerFire(true, countBullet);
            
            shootingView.Received(0).PlayerShootingBullet();
        }

        [Test]
        public void WhenPlayerIsNotShootAndCeroBullet_NotShooting()
        {
            logic.IsPlayerFire(false, 0);
            
            shootingView.Received(0).PlayerShootingBullet();
        }
    }
}

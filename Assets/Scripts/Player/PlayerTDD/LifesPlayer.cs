using System.Collections;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class LifesPlayer
    {
        ILifeOfPlayerControllerView lifeOfPlayerControllerView;
        LogicLifeOfPlayer lifes;

        [SetUp]
        public void SetUp()
        {
            lifeOfPlayerControllerView = Substitute.For<ILifeOfPlayerControllerView>();
            lifes = new LogicLifeOfPlayer(lifeOfPlayerControllerView);
        }
        // A Test behaves as an ordinary method
        [Test]
        public void LifesPlayerSimpleConstructor()
        {
            // Use the Assert class to test conditions
            Assert.AreEqual(true,lifes.EstaVivo);
        }

        [Test]
        public void WhenPlayerIsCollisionWithEnemi_Died()
        {
            //action
            lifes.Murio(true,false);
            
            //assert
            lifeOfPlayerControllerView.Received(1).PlayerDied();
        }
        
        [Test]
        public void WhenPlayerIsCollisionWithBulletEnemi_Died()
        {
            //action
            lifes.Murio(false,true);
            
            //assert
            lifeOfPlayerControllerView.Received(1).PlayerDied();
        }
        
        [Test]
        public void WhenPlayerIsNotCollisionWithEnemiOrBulletEnemi_NotDied()
        {
            //action
            lifes.Murio(false,false);
            
            //assert
            lifeOfPlayerControllerView.Received(0).PlayerDied();
        }
    }
}

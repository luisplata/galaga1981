using System.Collections;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PointsPLayer
    {
        IControllerPointsView controllerPointsView;
        IPlayerPrefsAdapter playerPrefsAdapter;
        LogicPointsPlayer logic;
        
        [SetUp]
        public void SetUp()
        {
            controllerPointsView = Substitute.For<IControllerPointsView>();
            playerPrefsAdapter = Substitute.For<IPlayerPrefsAdapter>();
        }
        // A Test behaves as an ordinary method
        [Test]
        public void PlayerCheckPointAndNotExist_whenCreateScoreWhithCeroPoints()
        {
            playerPrefsAdapter.HasKey("score").Returns(false);
            logic = new LogicPointsPlayer(controllerPointsView, playerPrefsAdapter);
            //assert
            playerPrefsAdapter.Received(1).SetInt("score",0);
        }

        [Test]
        public void PlayerCheckPointsAndExist_whenReturnNumberOfPoints()
        {
            playerPrefsAdapter.HasKey("score").Returns(true);
            playerPrefsAdapter.GetInt("score").Returns(100);
            logic = new LogicPointsPlayer(controllerPointsView, playerPrefsAdapter);
            //assert
            Assert.AreEqual(100,logic.GetPoints());
        }

        [Test]
        public void PlayerKillEnemi_whenPointIsUp()
        {
            playerPrefsAdapter.HasKey("score").Returns(true);
            playerPrefsAdapter.GetInt("score").Returns(100);
            logic = new LogicPointsPlayer(controllerPointsView, playerPrefsAdapter);

            //action
            logic.PointsUp(2);
            
            //assert
            playerPrefsAdapter.Received(1).SetInt("score",102);
            controllerPointsView.Received().ShowPuntuaction(102);
        }
    }
}

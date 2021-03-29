using System.Collections;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class MovmentPlayer
    {
        IControllerMov controllerMov;
        LogicMovment logic;
        [SetUp]
        public void SetUp()
        {
            controllerMov = Substitute.For<IControllerMov>();
            logic = new LogicMovment(controllerMov);
        }

        [TearDown]
        public void TearDown()
        {
            
        }
        // A Test behaves as an ordinary method
        [Test]
        public void MovmentPlayerSimpleMov()
        {
            logic.MovePlayer(0);
            controllerMov.Received(1).MovePlayer(Arg.Any<Vector2>());
        }

        [TestCase(1,1)]
        [TestCase(0,0)]
        [TestCase(-1,-1)]
        [TestCase(-0.5f,-0.5f)]
        [TestCase(0.5f,0.5f)]
        public void MovmentPlayerInAxisX_returnAxisX(float movment, float expected)
        {
            logic.MovePlayer(movment);
            var vectorExpected = new Vector2(expected,0);
            controllerMov.Received().MovePlayer(vectorExpected);
        }
    }
}

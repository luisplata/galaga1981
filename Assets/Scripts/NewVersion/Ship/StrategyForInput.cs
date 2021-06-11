using System;
using UnityEngine;
using Utils;

namespace NewVersion.Ship
{
    public class StrategyForInput
    {
        private readonly TypeOfInput typeOfInput;
        private readonly ShipControllerMediator shipControllerMediator;
        private readonly AnimationCurve curveMovement;

        public StrategyForInput(TypeOfInput typeOfInput,ShipControllerMediator shipControllerMediator, AnimationCurve curveMovement)
        {
            this.typeOfInput = typeOfInput;
            this.shipControllerMediator = shipControllerMediator;
            this.curveMovement = curveMovement;
        }
        

        public IInputAdapter GetInput()
        {
            switch (typeOfInput)
            {
                case TypeOfInput.TouchInput:
                    return new InputTouchAdapter(Camera.main, shipControllerMediator.PointToFollow, shipControllerMediator.gameObject);
                case TypeOfInput.MachineInput:
                    return new InputMachine(curveMovement);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
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
            return typeOfInput switch
            {
                TypeOfInput.TouchInput => new InputTouchAdapter(Camera.main, shipControllerMediator),
                TypeOfInput.MachineInput => new InputMachine(curveMovement),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
using System;
using UnityEngine;
using Utils;

namespace NewVersion.Ship
{
    public class StrategyForInput
    {
        private readonly TypeOfInput typeOfInput;
        private readonly ShipControllerMediator shipControllerMediator;

        public StrategyForInput(TypeOfInput typeOfInput,ShipControllerMediator shipControllerMediator)
        {
            this.typeOfInput = typeOfInput;
            this.shipControllerMediator = shipControllerMediator;
        }
        

        public IInputAdapter GetInput()
        {
            switch (typeOfInput)
            {
                case TypeOfInput.TouchInput:
                    return new InputTouchAdapter(Camera.main, shipControllerMediator.PointToFollow, shipControllerMediator.gameObject);
                case TypeOfInput.MachineInput:
                    return new InputMachine();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
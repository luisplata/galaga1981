using UnityEngine;

public class LogicMovment
{
    private readonly IControllerMov controllerMov;

    public LogicMovment(IControllerMov controllerMov)
    {
        this.controllerMov = controllerMov;
    }

    public void MovePlayer(float directionJoistic)
    {
        var direccion = new Vector2(directionJoistic,0);
        if(directionJoistic == 0)
        {
            direccion = Vector2.zero;
        }
        controllerMov.MovePlayer(direccion);
    }
}
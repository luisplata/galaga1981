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
        if(directionJoistic != 0)
        {
            Vector2 direccion = new Vector2(directionJoistic,0);
            //lo movemos
            controllerMov.MovePlayer(direccion);
        }
        else
        {
            controllerMov.MovePlayer(Vector2.zero);
        }
    }
}
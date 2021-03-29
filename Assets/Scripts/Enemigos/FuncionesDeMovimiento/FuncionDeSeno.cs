using UnityEngine;
using System.Collections;

public class FuncionDeSeno : FuncionDeMovimiento
{

    public FuncionDeSeno()
    {
        limiteInferior = -2;
        limiteSuperior = 2;
    }
    public override float EjecutarFuncion(float x)
    {
        return -2 - (Mathf.Pow(x, 4)) + (4 * Mathf.Pow(x, 2));
    }
}

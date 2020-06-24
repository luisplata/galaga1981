using UnityEngine;
using System.Collections;

public class FuncionTangenteCuadradoMenosDos : FuncionDeMovimiento
{
    public FuncionTangenteCuadradoMenosDos()
    {
        limiteInferior = -3.6f;
        limiteSuperior = 3.6f;
    }
    public override float EjecutarFuncion(float x)
    {
        //return Mathf.Tan(Mathf.Pow(x, 2)) - 2;
        return Mathf.Sin(x) * Mathf.Pow(x, 2) * Mathf.Sin(x) - 1;
    }
}

using UnityEngine;

public abstract class Enemigo : MonoBehaviour
{
    public int valor;
    public string tipo;
    public float speed;
    public MovimientoDeFormacionDeEnemigos estacionamiento;
    public bool estaMuerto;
    public bool cansadoDeEsperar;
    public int stage;
    public int life;
    public int realLife;

    public void Config(int stagecomming)
    {
        stage = stagecomming;
        int levelsPerIncrement = 5;
        int increment = Mathf.FloorToInt((stagecomming - 1) / levelsPerIncrement);
        var healthGrowthRate = Mathf.Pow(1.5f, increment);
        realLife = Mathf.RoundToInt(Mathf.Max(0, life * healthGrowthRate));
        Debug.Log($"realLife: {realLife} from {tipo}");

        if (stagecomming > 1)
        {
            valor = Mathf.RoundToInt(valor * Mathf.Pow(1.1f, stagecomming));
        }
    }



}

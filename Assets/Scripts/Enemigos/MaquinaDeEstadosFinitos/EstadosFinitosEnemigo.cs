using UnityEngine;
using System.Collections;
using System;

public abstract class EstadosFinitosEnemigo : MonoBehaviour
{

    public abstract void Salir();
    private Vector2 cardinalidadDelFantasma;
    public virtual void Start()
    {
    }

    public abstract void Update();

    public abstract Type VerficarTransiciones();

    public virtual Vector2 GetCardinalidad()
    {
        return this.cardinalidadDelFantasma;
    }


    public void VerificarCambios()
    {
        Type estadoACambiar = VerficarTransiciones();
        if (estadoACambiar != this.GetType())
        {
            CambiarEstado(estadoACambiar);
        }
    }

    public void CambiarEstado(Type nuevoEstado)
    {
        //salir del estado actual
        Salir();
        //agregamos el componente
        gameObject.AddComponent(nuevoEstado);
        //destuimos el estado viejo
        Destroy(this);
    }
}

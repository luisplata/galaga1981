using System;
using UnityEngine;

public abstract class EstadosFinitos : MonoBehaviour
{
    public Escenario escenario;
    public abstract void Salir();
    private Vector2 cardinalidadDelFantasma;
    public ComportamientoEscenario comportamiento;
    public ControladorDeGrupos controladorDeGrupos;
    public ControadorDeVidasDeUI vidasUI;
    protected IInputAdapter input;
    public virtual void Start()
    {
        input = GetComponent<InputStragety>().GetInput();
        comportamiento = GetComponent<ComportamientoEscenario>();
        escenario = comportamiento.escenario;
        controladorDeGrupos = GetComponent<ControladorDeGrupos>();
        vidasUI = GetComponent<ControadorDeVidasDeUI>();
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
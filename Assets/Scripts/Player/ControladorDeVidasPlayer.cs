using System;
using UnityEngine;
using System.Collections;

public class ControladorDeVidasPlayer : MonoBehaviour, ILifeOfPlayerControllerView
{
    [field: SerializeField]
    public bool EstaVivo
    {
        get { return logicLifeOfPlayer.EstaVivo; }
        set { logicLifeOfPlayer.EstaVivo = value; }
    }

    [field: SerializeField] public int Vidas { get; set; } = 2;
    [SerializeField] private GameObject salidaDeSonido;
    [SerializeField] private AudioClip explosion;
    private LogicLifeOfPlayer logicLifeOfPlayer;

    private void Start()
    {
        logicLifeOfPlayer = new LogicLifeOfPlayer(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        logicLifeOfPlayer.Murio(collision.gameObject.CompareTag("Enemigo"), collision.gameObject.CompareTag("BalaEnemigo"));
    }
    

    public void PlayerDied()
    {
        salidaDeSonido.GetComponent<AudioSource>().PlayOneShot(explosion);
        GetComponent<Animator>().SetBool("estaMuerto", true);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<ControladorDeMovimiento>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        logicLifeOfPlayer.Murio(collision.CompareTag("Enemigo"), collision.CompareTag("BalaEnemigo"));
    }
    
    public void Init()
    {
        logicLifeOfPlayer.EstaVivo = true;
        GetComponent<Animator>().SetBool("estaMuerto", false);
    }

    /// <summary>
    /// This method is for PlayerAnimator
    /// </summary>
    public void KillerPlayer()
    {
        logicLifeOfPlayer.EstaVivo = false;
    }
}
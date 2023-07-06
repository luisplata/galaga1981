using System;
using UnityEngine;
using Utils;

public class ControllerShooting: MonoBehaviour , IControllerShoot
{
    public float speedDisparo;
    public GameObject salidaDeSonido;
    public GameObject bala;
    public AudioClip disparo;
    private IInputAdapter input;
    private AudioSource audioSource;
    private LogicShooting logicShooting;

    private void Start()
    {
        audioSource = salidaDeSonido.GetComponent<AudioSource>();
        input = GetComponent<InputStragety>().GetInput();
        logicShooting = new LogicShooting(this);
    }

    private void Update()
    {
        logicShooting.IsPlayerFire(input.GetButton("Fire"),GameObject.FindGameObjectsWithTag("BalaPlayer").Length);
    }
    
    public void PlayerShootingBullet()
    {
        GameObject disparoInstanciado = Instantiate(bala, transform);
        disparoInstanciado.tag = "BalaPlayer";
        var position = disparoInstanciado.transform.position;
        position = new Vector2(position.x + 0.058f, position.y);
        disparoInstanciado.transform.position = position;
        //le damos velocidad
        disparoInstanciado.GetComponent<Rigidbody2D>().velocity = Vector2.up * speedDisparo;
        audioSource.PlayOneShot(disparo);
    }

    public void AddBullets(int countOfBulletsMore)
    {
        logicShooting.AddMaxOfBullets(countOfBulletsMore);
    }
}
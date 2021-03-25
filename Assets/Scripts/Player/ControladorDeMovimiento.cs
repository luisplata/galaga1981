using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControladorDeMovimiento : MonoBehaviour
{
    public float speed, speedDisparo;
    public GameObject bala, salidaDeSonido;
    public AudioClip disparo;
    [SerializeField] private TextMeshProUGUI log;

    private IInputAdapter input;

    private Rigidbody2D rigidbody2D;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        input = GetComponent<InputStragety>().GetInput();
    }

    // Update is called once per frame
    void Update()
    {
        var directionJoistic = input.GetDirection().x;
        //log.text += directionJoistic + "\n";
        if(directionJoistic != 0)
        {
            Vector2 direccion = new Vector2(directionJoistic,0);
            /*if(directionJoistic < 0)
            {
                //es a la izq
                direccion = Vector2.left;
            }
            else
            {
                //es a la der
                direccion = Vector2.right;
            }*/
            //lo movemos
            rigidbody2D.velocity = direccion * (speed * Time.deltaTime);
        }
        else
        {
            rigidbody2D.velocity = Vector2.zero;
        }
        if (input.GetButton("Fire") && GameObject.FindGameObjectsWithTag("BalaPlayer").Length < 2)
        {
            //disparamos
            GameObject disparoInstanciado = Instantiate(bala, transform);
            disparoInstanciado.tag = "BalaPlayer";
            disparoInstanciado.transform.position = new Vector2(disparoInstanciado.transform.position.x + 0.058f, disparoInstanciado.transform.position.y);
            //le damos velocidad
            disparoInstanciado.GetComponent<Rigidbody2D>().velocity = Vector2.up * speedDisparo;
            salidaDeSonido.GetComponent<AudioSource>().PlayOneShot(disparo);
        }
    }
}

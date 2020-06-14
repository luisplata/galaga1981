using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorDeMovimiento : MonoBehaviour
{
    public float speed, speedDisparo;
    public GameObject bala;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxis("Horizontal") != 0)
        {
            Vector2 direccion;
            //es porque se esta moviendo
            //ahora determinamos la direccion
            if(Input.GetAxis("Horizontal") < 0)
            {
                //es a la izq
                direccion = Vector2.left;
            }
            else
            {
                //es a la der
                direccion = Vector2.right;
            }
            //lo movemos
            GetComponent<Rigidbody2D>().velocity = direccion * speed;
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button1)) && GameObject.FindGameObjectsWithTag("BalaPlayer").Length < 2)
        {
            //disparamos
            GameObject disparoInstanciado = Instantiate(bala, transform);
            disparoInstanciado.tag = "BalaPlayer";
            disparoInstanciado.transform.position = new Vector2(disparoInstanciado.transform.position.x + 0.058f, disparoInstanciado.transform.position.y);
            //le damos velocidad
            disparoInstanciado.GetComponent<Rigidbody2D>().velocity = Vector2.up * speedDisparo;
        }
    }
}

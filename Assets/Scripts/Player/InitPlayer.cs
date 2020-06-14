using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitPlayer : MonoBehaviour
{
    //vamosm a listar los componentes que necesito usar de este player
    SpriteRenderer sr;
    Rigidbody2D rb;
    Animator ani;
    Collider2D col;
    ControladorDeMovimiento cm;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        cm = GetComponent<ControladorDeMovimiento>();
        DetenerPlayer();
    }

    public void InitializarPlayer()
    {
        sr.enabled = true;
        ani.enabled = true;
        col.enabled = true;
        cm.enabled = true;
    }
    public void DetenerPlayer()
    {
        sr.enabled = false;
        ani.enabled = false;
        col.enabled = false;
        cm.enabled = false;
    }
}

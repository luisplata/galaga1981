using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorBala : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Paredes"))
        {
            Destroy(gameObject);
        }
    }
}
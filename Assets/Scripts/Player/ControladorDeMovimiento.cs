using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControladorDeMovimiento : MonoBehaviour, IControllerMov
{
    public float speed;
    private IInputAdapter input;
    private Rigidbody2D rigidbody2D;
    private LogicMovment logicMovment;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        input = GetComponent<InputStragety>().GetInput();
        logicMovment = new LogicMovment(this);
    }

    // Update is called once per frame
    void Update()
    {
        var directionJoistic = input.GetDirection().x;
        logicMovment.MovePlayer(directionJoistic);
    }
    

    public void MovePlayer(Vector2 direction)
    {
        rigidbody2D.velocity = direction * (speed * Time.deltaTime);
    }
}
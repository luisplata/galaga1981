using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private GameObject pointToFollow;
    [SerializeField] private float velocity;
    private Rigidbody2D rb;
    private IPlayer _player;
    private bool isConfigurated = false;

    public void Configure(IPlayer player)
    {
        _player = player;
        rb = GetComponent<Rigidbody2D>();
        isConfigurated = true;
    }

    private void GoToThePoint()
    {
        if (!isConfigurated) return;
        var diff = pointToFollow.transform.position - transform.position;
        rb.velocity = diff * velocity;
        _player.IsMovement();
    }

    public void MovePointTo(Vector2 worldPosition)
    {
        pointToFollow.transform.position = worldPosition;
        GoToThePoint();
    }
}

using System;
using UnityEngine;

public abstract class ControllerOfPowerUp : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;
    [SerializeField] private AudioClip powerUpSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        rb.velocity = Vector2.down * speed;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        OnPlayerCollision(other.gameObject);
        audioSource.PlayOneShot(powerUpSound);
        spriteRenderer.enabled = false;
        Destroy(gameObject,1);
    }

    protected abstract void OnPlayerCollision(GameObject player);
}
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    [SerializeField] float speed      = 5f;
    [SerializeField] float jumpForce  = 7f;
    bool isGrounded = false;

    [SerializeField] LayerMask groundLayer;
    Animator miAnimator;
    Rigidbody2D rb;
    [SerializeField] private List<SpriteRenderer> sprite1;
    [SerializeField] private GameObject sword;

    private float speedMultiplier = 1f;  // modificado por ClockItem

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        miAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        // Velocidad afectada por el multiplicador del reloj
        rb.velocity = new Vector2(horizontalInput * speed * speedMultiplier, rb.velocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        // Flip del sprite
        foreach (var sprite in sprite1)
        {
            if (horizontalInput > 0) sprite.flipX = true;
            if (horizontalInput < 0) sprite.flipX = false;
        }

        // Animación de correr
        miAnimator.SetBool("Run", horizontalInput != 0);

        // Detección de suelo
        Collider2D col = GetComponent<Collider2D>();
        isGrounded = Physics2D.OverlapCircle(
            transform.position - transform.up * ((col.bounds.extents.y / transform.localScale.y - col.offset.y) * transform.localScale.y),
            0.01f,
            groundLayer
        );
    }

    // Llamado por ClockItem para afectar la velocidad
    public void SetSpeedMultiplier(float multiplier) => speedMultiplier = multiplier;
}
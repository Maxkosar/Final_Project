using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] float speed     = 5f;
    [SerializeField] float jumpForce = 7f;

    [Header("Teclas configurables")]
    [SerializeField] private KeyCode keyLeft  = KeyCode.A;
    [SerializeField] private KeyCode keyRight = KeyCode.D;
    [SerializeField] private KeyCode keyJump  = KeyCode.Space;

    [Header("Referencias")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] private List<SpriteRenderer> sprite1;
    [SerializeField] private GameObject sword;

    private bool    isGrounded      = false;
    private float   speedMultiplier = 1f;
    private Animator    miAnimator;
    private Rigidbody2D rb;

    void Start()
    {
        rb         = GetComponent<Rigidbody2D>();
        miAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        // Movimiento horizontal con teclas configurables
        float horizontalInput = 0f;
        if (Input.GetKey(keyRight)) horizontalInput =  1f;
        if (Input.GetKey(keyLeft))  horizontalInput = -1f;

        rb.velocity = new Vector2(horizontalInput * speed * speedMultiplier, rb.velocity.y);

        // Salto
        if (Input.GetKeyDown(keyJump) && isGrounded)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        // Flip del sprite
        foreach (var sprite in sprite1)
        {
            if (horizontalInput > 0) sprite.flipX = true;
            if (horizontalInput < 0) sprite.flipX = false;
        }

        // Animación
        miAnimator.SetBool("Run", horizontalInput != 0);

        // Detección de suelo
        Collider2D col = GetComponent<Collider2D>();
        isGrounded = Physics2D.OverlapCircle(
            transform.position - transform.up * ((col.bounds.extents.y / transform.localScale.y - col.offset.y) * transform.localScale.y),
            0.01f,
            groundLayer
        );
    }

    public void SetSpeedMultiplier(float multiplier) => speedMultiplier = multiplier;
}
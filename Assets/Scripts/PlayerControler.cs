using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    [SerializeField] float speed=5f;
    [SerializeField] float jumpForce=7f;
    bool isGrounded = false;
    [SerializeField] LayerMask groundLayer; 
    Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sprite1;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite1 = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

    // Controles de personaje
    rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
    if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
    {
    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    if (horizontalInput > 0)
    {
        sprite1.flipX = !sprite1.flipX;

    }
     // Almacenando el colisionador en una variable separada para facilitar su uso
    Collider2D col = GetComponent<Collider2D>();
 // Creando un área circular debajo de los pies del personaje
    isGrounded = Physics2D.OverlapCircle(transform.position - transform.up * ((col.bounds.extents.y / transform.localScale.y - col.offset.y) * transform.localScale.y), 0.01f, groundLayer);
    }
}

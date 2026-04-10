using UnityEngine;
using System.Collections;

public class Bandit : MonoBehaviour {

    [SerializeField] float      m_speed = 4.0f;
    [SerializeField] float      m_runSpeed = 7.0f; // Nueva velocidad para correr
    [SerializeField] float      m_jumpForce = 7.5f;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Sensor_Bandit       m_groundSensor;
    private bool                m_grounded = false;
    private bool                m_combatIdle = false;
    private bool                m_isDead = false;

    private int                 m_jumpCount = 0;
    private int                 m_maxJumps = 2; 

    void Start () {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Bandit>();
    }
    
    void Update () {
        // --- Detección de Suelo ---
        if (!m_grounded && m_groundSensor.State()) {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
            m_jumpCount = 0;
        }

        if (m_grounded && !m_groundSensor.State()) {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            if(m_jumpCount == 0) m_jumpCount = 1; 
        }

        // --- Manejo de Sprint (Shift) ---
        // Usamos GetKey (sin "Down") para que detecte mientras se mantiene presionada
        float currentSpeed = m_speed;
        if (Input.GetKey(KeyCode.LeftShift)) {
            currentSpeed = m_runSpeed;
        }

        // --- Manejo de Input y Movimiento ---
        float inputX = Input.GetAxis("Horizontal");

        if (inputX > 0)
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (inputX < 0)
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        // Aplicamos la velocidad actual (normal o carrera)
        m_body2d.velocity = new Vector2(inputX * currentSpeed, m_body2d.velocity.y);
        m_animator.SetFloat("AirSpeed", m_body2d.velocity.y);

        // --- Manejo de Acciones ---
        if (Input.GetKeyDown("e")) {
            if(!m_isDead) m_animator.SetTrigger("Death");
            else m_animator.SetTrigger("Recover");
            m_isDead = !m_isDead;
        }
        else if (Input.GetKeyDown("q"))
            m_animator.SetTrigger("Hurt");

        else if(Input.GetMouseButtonDown(0))
            m_animator.SetTrigger("Attack");

        else if (Input.GetKeyDown("f"))
            m_combatIdle = !m_combatIdle;

        // --- Doble Salto ---
        else if (Input.GetKeyDown("space") && (m_grounded || m_jumpCount < m_maxJumps)) {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
            m_jumpCount++;
        }

        // --- Estados de Animación ---
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
            m_animator.SetInteger("AnimState", 2); // Caminar/Correr

        else if (m_combatIdle)
            m_animator.SetInteger("AnimState", 1);

        else
            m_animator.SetInteger("AnimState", 0);
    }
}
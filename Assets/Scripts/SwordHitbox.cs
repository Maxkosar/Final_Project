using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    private SwordAttack  swordAttack;
    private PlayerHealth ownerHealth;  // el PlayerHealth del dueño de la espada

    void Start()
    {
        swordAttack  = GetComponentInParent<SwordAttack>();
        ownerHealth  = GetComponentInParent<PlayerHealth>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.root == transform.root) return;

        PlayerHealth targetHealth = col.GetComponent<PlayerHealth>();
        if (targetHealth != null)
        {
            // Pasamos al dueño como "killer" para que ArenaController lo registre
            targetHealth.TakeDamage(swordAttack.GetDamage(), ownerHealth);
        }
    }
}
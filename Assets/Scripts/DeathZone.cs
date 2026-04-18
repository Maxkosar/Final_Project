using UnityEngine;

public class DeathZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        // Busca PlayerHealth en el objeto o en cualquier padre
        // Así funciona aunque el collider sea de un hijo como GroundSensor
        PlayerHealth health = col.GetComponentInParent<PlayerHealth>();

        if (health != null && !health.IsDead())
        {
            Debug.Log($"DeathZone: {col.gameObject.name} cayó al vacío");
            health.FallDeath();
        }
    }
}
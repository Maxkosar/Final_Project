using UnityEngine;

public class HealthItem : Item
{
    [Header("Curación")]
    [SerializeField] private float healAmount = 30f;

    protected override void ApplyEffect(GameObject player)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
            health.Heal(healAmount);
    }
}

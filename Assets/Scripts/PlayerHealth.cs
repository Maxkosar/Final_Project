using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Zona de muerte")]
    [SerializeField] private float deathYPosition = -10f;  // si cae más abajo de esto, muere

    private float        currentHealth;
    private bool         isDead = false;
    private PlayerHealth lastAttacker;
    private bool         isFallDeath = false;

    private PlayerStar playerStar;

    public UnityEvent<float, float> OnHealthChanged;
    public UnityEvent<PlayerHealth> OnDeath;
    public UnityEvent               OnFallDeath;

    void Start()
    {
        playerStar    = GetComponent<PlayerStar>();
        currentHealth = maxHealth;
        OnHealthChanged.Invoke(currentHealth, maxHealth);
    }

    void Update()
    {
        // Si cae por debajo del límite Y, muere
        if (!isDead && transform.position.y < deathYPosition)
        {
            Debug.Log($"{gameObject.name} cayó al vacío en Y: {transform.position.y}");
            FallDeath();
        }
    }

    public void TakeDamage(float amount, PlayerHealth attacker = null)
    {
        if (isDead) return;
        if (playerStar != null && playerStar.IsInvincible()) return;

        if (attacker != null)
            lastAttacker = attacker;

        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);
        OnHealthChanged.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
            Die(lastAttacker);
    }

    public void FallDeath()
    {
        if (isDead) return;

        isDead      = true;
        isFallDeath = true;

        Debug.Log($"FallDeath ejecutado en {gameObject.name}");

        OnFallDeath.Invoke();
        OnDeath.Invoke(null);
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        OnHealthChanged.Invoke(currentHealth, maxHealth);
    }

    private void Die(PlayerHealth killer)
    {
        isDead = true;
        OnDeath.Invoke(killer);
    }

    public void Revive()
    {
        isDead        = false;
        isFallDeath   = false;
        currentHealth = maxHealth;
        lastAttacker  = null;
        OnHealthChanged.Invoke(currentHealth, maxHealth);
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth()     => maxHealth;
    public bool  IsDead()           => isDead;
    public bool  IsFallDeath()      => isFallDeath;
}
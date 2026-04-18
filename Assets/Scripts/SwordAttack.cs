using System.Collections;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    [Header("Configuración del Ataque")]
    [SerializeField] private float attackDamage   = 20f;
    [SerializeField] private float attackDuration = 0.2f;
    [SerializeField] private float attackCooldown = 0.5f;

    [Header("Referencias")]
    [SerializeField] private GameObject swordHitbox;
    [SerializeField] private KeyCode attackKey = KeyCode.Z;

    private bool  canAttack        = true;
    private float damageMultiplier = 1f;  // modificado por AttackItem
    private Animator miAnimator;

    void Start()
    {
        miAnimator = GetComponent<Animator>();
        if (swordHitbox != null)
            swordHitbox.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(attackKey) && canAttack)
            StartCoroutine(PerformAttack());
    }

    IEnumerator PerformAttack()
    {
        canAttack = false;

        if (swordHitbox != null) swordHitbox.SetActive(true);
        if (miAnimator != null)  miAnimator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackDuration);

        if (swordHitbox != null) swordHitbox.SetActive(false);

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }

    // El daño real = daño base × multiplicador (modificado por AttackItem)
    public float GetDamage()                           => attackDamage * damageMultiplier;
    public void  SetDamageMultiplier(float multiplier) => damageMultiplier = multiplier;
}
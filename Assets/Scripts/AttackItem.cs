using System.Collections;
using UnityEngine;

public class AttackItem : Item
{
    [Header("Ataque Potenciado")]
    [SerializeField] private float damageMultiplier = 2f;  // duplica el daño

    protected override void ApplyEffect(GameObject player)
    {
        SwordAttack sword = player.GetComponent<SwordAttack>();
        if (sword != null)
            player.GetComponent<MonoBehaviour>().StartCoroutine(BoostAttack(sword));
    }

    IEnumerator BoostAttack(SwordAttack sword)
    {
        sword.SetDamageMultiplier(damageMultiplier);
        Debug.Log($"Ataque potenciado x{damageMultiplier} por {duration} segundos");

        yield return new WaitForSeconds(duration);

        sword.SetDamageMultiplier(1f); // volvemos al daño normal
        Debug.Log("Ataque volvió a la normalidad");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockItem : Item
{
    [Header("Efecto del Reloj")]
    [SerializeField] private float speedBoost     = 1.5f;  // multiplicador de velocidad para el que lo agarra
    [SerializeField] private float slowMultiplier = 0.5f;  // multiplicador de velocidad para los rivales

    protected override void ApplyEffect(GameObject player)
    {
        player.GetComponent<MonoBehaviour>().StartCoroutine(ApplyClock(player));
    }

    IEnumerator ApplyClock(GameObject player)
    {
        PlayerControler myController = player.GetComponent<PlayerControler>();

        // Buscamos todos los jugadores en la escena
        List<PlayerControler> allPlayers = new List<PlayerControler>(
            FindObjectsOfType<PlayerControler>()
        );

        // Aplicamos efectos
        foreach (var pc in allPlayers)
        {
            if (pc.gameObject == player)
                pc.SetSpeedMultiplier(speedBoost);    // quien lo agarró: más rápido
            else
                pc.SetSpeedMultiplier(slowMultiplier); // rivales: más lentos
        }

        Debug.Log($"Reloj activo: {duration} segundos");
        yield return new WaitForSeconds(duration);

        // Revertimos todo
        foreach (var pc in allPlayers)
            pc.SetSpeedMultiplier(1f);

        Debug.Log("Reloj terminó, velocidades normales");
    }
}
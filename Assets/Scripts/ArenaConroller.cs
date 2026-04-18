using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaController : MonoBehaviour
{
    [Header("Jugadores")]
    [SerializeField] private List<PlayerHealth> players;

    [Header("Referencias")]
    [SerializeField] private PlayerCanvas playerCanvas;
    [SerializeField] private ItemSpawner  itemSpawner;
    [SerializeField] private ArenaHUD     arenaHUD;

    [Header("Respawn")]
    [SerializeField] private float respawnDelay  = 3f;
    [SerializeField] private float respawnHeight = 10f;

    [Header("Penalidad por caída")]
    [SerializeField] private int fallPenalty = 5;

    private Dictionary<PlayerHealth, int> scores = new Dictionary<PlayerHealth, int>();
    private bool gameOver = false;

    void Start()
    {
        foreach (var player in players)
        {
            scores[player] = 0;
            player.OnDeath.AddListener((killer) => OnPlayerDied(player, killer));
            Debug.Log($"ArenaController suscrito a: {player.gameObject.name}");
        }

        if (arenaHUD != null)
        {
            arenaHUD.InitializePlayers(players);
            arenaHUD.UpdateScores(scores);
        }
    }

    private void OnPlayerDied(PlayerHealth deadPlayer, PlayerHealth killer)
    {
        Debug.Log($"OnPlayerDied llamado: {deadPlayer.gameObject.name}");
        if (gameOver) return;

        if (killer == null)
        {
            scores[deadPlayer] = Mathf.Max(0, scores[deadPlayer] - fallPenalty);
            Debug.Log($"{deadPlayer.gameObject.name} cayó al vacío. -{fallPenalty} pts. Total: {scores[deadPlayer]}");
        }
        else
        {
            scores[killer]++;
            Debug.Log($"{killer.gameObject.name} eliminó a {deadPlayer.gameObject.name}. Puntaje: {scores[killer]}");
        }

        if (arenaHUD != null)
            arenaHUD.UpdateScores(scores);

        StartCoroutine(RespawnPlayer(deadPlayer));
    }

    private IEnumerator RespawnPlayer(PlayerHealth player)
    {
        player.gameObject.SetActive(false);

        yield return new WaitForSeconds(respawnDelay);

        Vector3 spawnPos = new Vector3(
            transform.position.x,
            transform.position.y + respawnHeight,
            0f
        );

        player.transform.position = spawnPos;
        player.gameObject.SetActive(true);
        player.Revive();

        Debug.Log($"{player.gameObject.name} respawneó");
    }

    public void OnTimeUp()
    {
        if (gameOver) return;
        StartCoroutine(EndGame());
    }

    private IEnumerator EndGame()
    {
        gameOver = true;
        itemSpawner.StopSpawning();

        PlayerHealth winner   = null;
        int          topScore = -1;
        bool         tie      = false;

        foreach (var kvp in scores)
        {
            if (kvp.Value > topScore)
            {
                topScore = kvp.Value;
                winner   = kvp.Key;
                tie      = false;
            }
            else if (kvp.Value == topScore)
            {
                tie = true;
            }
        }

        if (tie)
            Debug.Log("¡Tiempo agotado! Empate.");
        else if (winner != null)
            Debug.Log($"¡Ganó {winner.gameObject.name} con {topScore} puntos!");

        if (arenaHUD != null)
            arenaHUD.ShowWinner(tie ? "¡Empate!" : $"¡Ganó {winner.gameObject.name}!");

        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
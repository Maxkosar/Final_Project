using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArenaHUD : MonoBehaviour
{
    [System.Serializable]
    public class PlayerPanel
    {
        public string          playerName;
        public TextMeshProUGUI scoreText;
        public Image           itemImage;
        public Sprite          defaultSprite;
    }

    [Header("Paneles de jugadores (uno por esquina)")]
    [SerializeField] private List<PlayerPanel> panels;

    [Header("UI Final")]
    [SerializeField] private TextMeshProUGUI winnerText;

    private Dictionary<PlayerHealth, int> playerIndexMap = new Dictionary<PlayerHealth, int>();

    void Start()
    {
        if (winnerText != null)
            winnerText.gameObject.SetActive(false);

        foreach (var panel in panels)
        {
            if (panel.itemImage != null)
            {
                panel.itemImage.enabled = true;  // siempre visible
                panel.itemImage.sprite  = panel.defaultSprite;
                panel.itemImage.color   = panel.defaultSprite != null ? Color.white : Color.clear;
            }
            if (panel.scoreText != null)
                panel.scoreText.text = "0 pts";
        }
    }

    public void InitializePlayers(List<PlayerHealth> players)
    {
        for (int i = 0; i < players.Count && i < panels.Count; i++)
        {
            playerIndexMap[players[i]] = i;

            PlayerItemTracker tracker = players[i].GetComponent<PlayerItemTracker>();
            if (tracker != null)
            {
                int index = i;
                tracker.OnItemChanged.AddListener((sprite) => OnItemChanged(index, sprite));
            }

            if (panels[i].scoreText != null)
                panels[i].scoreText.text = "0 pts";
        }
    }

    public void UpdateScores(Dictionary<PlayerHealth, int> scores)
    {
        foreach (var kvp in scores)
        {
            if (playerIndexMap.TryGetValue(kvp.Key, out int index))
            {
                if (index < panels.Count && panels[index].scoreText != null)
                    panels[index].scoreText.text = $"{kvp.Value} pts";
            }
        }
    }

    private void OnItemChanged(int playerIndex, Sprite sprite)
    {
        if (playerIndex >= panels.Count) return;

        PlayerPanel panel = panels[playerIndex];
        if (panel.itemImage == null) return;

        panel.itemImage.enabled = true;

        if (sprite != null)
        {
            // Hay item activo — mostramos su sprite
            panel.itemImage.sprite = sprite;
            panel.itemImage.color  = Color.white;
        }
        else
        {
            // No hay item — mostramos default o transparente
            panel.itemImage.sprite = panel.defaultSprite;
            panel.itemImage.color  = panel.defaultSprite != null ? Color.white : Color.clear;
        }
    }

    public void ShowWinner(string message)
    {
        if (winnerText == null) return;
        winnerText.gameObject.SetActive(true);
        winnerText.text = message;
    }
}
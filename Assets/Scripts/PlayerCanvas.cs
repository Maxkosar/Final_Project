using UnityEngine;
using TMPro;

public class PlayerCanvas : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Timer Settings")]
    [SerializeField] private float totalTime = 180f; // 3 minutos en segundos

    private float currentTime;
    private bool isRunning = true;

    void Start()
    {
        // Si no se asignó por Inspector, lo busca automáticamente
        if (timerText == null)
            timerText = GetComponentInChildren<TextMeshProUGUI>();

        currentTime = totalTime;
        UpdateDisplay(currentTime);
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isRunning = false;
            OnTimerFinished();
        }

        UpdateDisplay(currentTime);
    }

    private void UpdateDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void OnTimerFinished()
    {
        timerText.text = "00:00";
        Debug.Log("¡Tiempo agotado!");
        // Aquí podés agregar lógica: fin de partida, evento, etc.
    }

    // Métodos públicos para controlar el timer desde otros scripts
    public void PauseTimer()  => isRunning = false;
    public void ResumeTimer() => isRunning = true;
    public void ResetTimer()  { currentTime = totalTime; isRunning = true; }
}
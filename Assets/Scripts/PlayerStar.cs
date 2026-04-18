using System.Collections;
using UnityEngine;

public class PlayerStar : MonoBehaviour
{
    public float starDuration = 10f;
    public AudioClip starMusic;
    public AudioClip normalMusic;

    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource    = GetComponent<AudioSource>();
    }

    public void ActivateStar()
    {
        if (isInvincible) StopAllCoroutines();
        StartCoroutine(StarEffect());
    }

    IEnumerator StarEffect()
    {
        isInvincible = true;
        audioSource.clip = starMusic;
        audioSource.Play();

        StartCoroutine(FlashColors());

        yield return new WaitForSeconds(starDuration);

        isInvincible           = false;
        spriteRenderer.color   = Color.white;
        audioSource.clip       = normalMusic;
        audioSource.Play();
    }

    IEnumerator FlashColors()
    {
        Color[] colors = { Color.red, Color.yellow, Color.green, Color.cyan, Color.magenta };
        int i = 0;
        while (isInvincible)
        {
            spriteRenderer.color = colors[i % colors.Length];
            i++;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public bool  IsInvincible() => isInvincible;
    public float GetDuration()  => starDuration;  // usado por StarItem para limpiar la UI
}
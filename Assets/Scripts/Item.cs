using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [Header("Configuración Base")]
    [SerializeField] protected float duration = 5f;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private Sprite itemSprite;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (itemSprite == null)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) itemSprite = sr.sprite;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        // Le pasamos la duración al tracker para que él maneje el timer
        PlayerItemTracker tracker = col.GetComponentInParent<PlayerItemTracker>();
        if (tracker != null)
            tracker.SetCurrentItem(itemSprite, duration);

        ApplyEffect(col.gameObject);

        if (pickupSound != null && audioSource != null)
            audioSource.PlayOneShot(pickupSound);

        Destroy(gameObject);  // destruimos el item, el tracker maneja el resto
    }

    protected abstract void ApplyEffect(GameObject player);
}
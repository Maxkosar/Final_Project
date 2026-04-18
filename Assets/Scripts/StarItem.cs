using UnityEngine;

public class StarItem : MonoBehaviour
{
    [SerializeField] private Sprite itemSprite;

    void Awake()
    {
        if (itemSprite == null)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) itemSprite = sr.sprite;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        PlayerStar star = col.GetComponentInParent<PlayerStar>();
        if (star != null)
            star.ActivateStar();

        // El tracker maneja el timer internamente
        PlayerItemTracker tracker = col.GetComponentInParent<PlayerItemTracker>();
        if (tracker != null)
            tracker.SetCurrentItem(itemSprite, star != null ? star.GetDuration() : 10f);

        Destroy(gameObject);
    }
}
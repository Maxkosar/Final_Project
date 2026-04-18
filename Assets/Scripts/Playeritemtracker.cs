using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerItemTracker : MonoBehaviour
{
    public UnityEvent<Sprite> OnItemChanged;

    private Sprite currentItemSprite;
    private Coroutine clearCoroutine;

    public void SetCurrentItem(Sprite itemSprite, float duration = 0f)
    {
        // Si había un item anterior, cancelamos su timer
        if (clearCoroutine != null)
            StopCoroutine(clearCoroutine);

        currentItemSprite = itemSprite;
        OnItemChanged.Invoke(currentItemSprite);

        // Si tiene duración, iniciamos el timer de limpieza desde acá
        if (duration > 0f)
            clearCoroutine = StartCoroutine(ClearAfterDuration(duration));
    }

    public void ClearCurrentItem()
    {
        if (clearCoroutine != null)
        {
            StopCoroutine(clearCoroutine);
            clearCoroutine = null;
        }
        currentItemSprite = null;
        OnItemChanged.Invoke(null);
    }

    IEnumerator ClearAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        currentItemSprite = null;
        OnItemChanged.Invoke(null);
        clearCoroutine = null;
    }

    public Sprite GetCurrentItem() => currentItemSprite;
}
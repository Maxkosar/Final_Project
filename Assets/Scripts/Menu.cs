using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Header("Referencia al componente Image")]
    public Image targetImage;

    [Header("Sprites")]
    public Sprite originalSprite;
    public Sprite alternateSprite;

    void Start()
    {
        if (originalSprite == null && targetImage != null)
            originalSprite = targetImage.sprite;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (targetImage != null && alternateSprite != null)
                targetImage.sprite = alternateSprite;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (targetImage != null && originalSprite != null)
                targetImage.sprite = originalSprite;
        }
    }
}
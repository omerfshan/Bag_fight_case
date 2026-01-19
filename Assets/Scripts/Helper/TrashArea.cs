using UnityEngine;
using UnityEngine.UI;

public class TrashArea : MonoBehaviour
{
    private Image trashImage;

    [SerializeField] private Sprite trashOpen;
    [SerializeField] private Sprite trashClose;

    void Awake()
    {
        trashImage = transform.GetChild(0).GetComponent<Image>();
    }

    public void SetOpen()
    {
        if (trashImage != null)
            trashImage.sprite = trashOpen;
    }

    public void SetClose()
    {
        if (trashImage != null)
            trashImage.sprite = trashClose;
    }
}

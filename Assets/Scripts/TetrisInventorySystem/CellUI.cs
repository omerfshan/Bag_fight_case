using UnityEngine;
using UnityEngine.UI;

public class CellUI : MonoBehaviour
{
    public Image img;
    public bool is_filled;

    private void Awake()
    {
        img = GetComponent<Image>();
    }

    public void SetEmpty()   // BOŞ → GRİ
    {
        img.color = Color.gray;
    }

    public void SetFilled()  // DOLU → BEYAZ
    {
        img.color = Color.white;
    }
}

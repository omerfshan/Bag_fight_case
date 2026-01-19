using UnityEngine;
using UnityEngine.UI;

public class CellUI : MonoBehaviour
{
    public Image img;
    public bool is_filled;

   
    public static readonly Color EmptyColor  = new Color(0.7f, 0.7f, 0.7f); 
    public static readonly Color FilledColor = Color.white;                  
    public static readonly Color ValidColor  = new Color(0.2f, 1f, 0.2f);   
    public static readonly Color InvalidColor= new Color(1f, 0.3f, 0.3f);    

    private void Awake()
    {
        img = GetComponent<Image>();
    }

   
    public void SetEmpty()
    {
        img.color = EmptyColor;
    }

   
    public void SetFilled()
    {
        img.color = FilledColor;
    }

   
    public void SetValid()
    {
        img.color = ValidColor;
    }

 
    public void SetInvalid()
    {
        img.color = InvalidColor;
    }
}

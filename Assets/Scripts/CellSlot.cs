using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CellSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform rect;
    private Image sprite;

    public bool fill;
    public int FID = -1;

    public Color normal_color;
    public Color hover_color;
    public Color fill_color;
    public Color fillHover_color = Color.red;

    void Start()
    {
        if (!sprite) sprite = GetComponent<Image>();
        if (!rect) rect = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!fill) sprite.color = hover_color;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!fill) sprite.color = normal_color;
    }

    public void setFilled(int fid)
    {
        FID = fid;
        fill = true;
        sprite.color = fill_color;
    }

    public void setEmpty()
    {
        FID = -1;
        fill = false;
        sprite.color = normal_color;
    }

    public void UpdateCell()
    {
        sprite.color = fill ? fill_color : normal_color;
    }

    private Rect GetWorldSpaceRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        Vector2 min = new Vector2(corners.Min(c => c.x), corners.Min(c => c.y));
        Vector2 size = new Vector2(
            corners.Max(c => c.x) - min.x,
            corners.Max(c => c.y) - min.y
        );

        return new Rect(min, size);
    }

    public bool IsOverlaps(RectTransform rectB, int id)
    {
        Rect rectAWorld = GetWorldSpaceRect(rect);
        Rect rectBWorld = GetWorldSpaceRect(rectB);

        bool hover = rectAWorld.Overlaps(rectBWorld);

        if (hover)
        {
            if (!fill)
            {
                sprite.color = hover_color;
            }
            else if (id != FID)
            {
                sprite.color = fillHover_color;
            }
        }
        else
        {
            sprite.color = fill ? fill_color : normal_color;
        }

        return hover;
    }
}

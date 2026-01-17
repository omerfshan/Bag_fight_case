using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int ID;
    public GridInventory inv;
    public bool canRotate;
    public bool isInInventory;

    private bool isDrag;
    private Vector2 CurrentPos;
    private Vector2 offset;
    private Vector2 localPosition;

    private RectTransform rect;
    private Image sprite;

    HashSet<CellSlot> cells = new HashSet<CellSlot>();
    HashSet<CellSlot> oldCells = new HashSet<CellSlot>();
    HashSet<Vector2> tp = new HashSet<Vector2>();

    Transform CurrentParent;

    void Start()
    {
        ID = Mathf.RoundToInt(Random.Range(0, 1000000));
        sprite = GetComponent<Image>();

        rect = GetComponent<RectTransform>();
        CurrentPos = rect.anchoredPosition;
        CurrentParent = transform.parent;
    }
    void Update()
{
    if (isDrag)
    {
        if (canRotate && Input.mouseScrollDelta.y >= 1 || canRotate && Input.mouseScrollDelta.y <= -1)
        {
            if (rect.pivot.x == 0)
            {
                rect.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                rect.pivot = new Vector2(1, 1);
            }
            else if (rect.pivot.x == 1)
            {
                rect.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                rect.pivot = new Vector2(0, 1);
            }
        }

        rect.anchoredPosition = localPosition - offset;
        UpdateSlots();
    }
}


    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.parent = inv.transform;
        rect.anchorMin = new Vector2(.5f, .5f);
        rect.anchorMax = new Vector2(.5f, .5f);

        for (int i = 0; i < inv.cells.Count; i++)
        {
            var c = inv.cells[i];
            if (c.FID == ID)
            {
                c.setEmpty();
            }
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            inv.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out offset
        );

        offset -= rect.anchoredPosition;
        isDrag = true;
        sprite.raycastTarget = false;
        CurrentPos = rect.anchoredPosition;
    }


    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            inv.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out localPosition
        );

        rect.anchoredPosition = localPosition - offset;

        tp.Clear();
        cells.Clear();

        for (int i = 0; i < inv.cells.Count; i++)
        {
            var c = inv.cells[i];

            if (c.IsOverlaps(rect, ID))
            {
                Vector2 cellLocalPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    inv.GetComponent<RectTransform>(),
                    c.rect.position,
                    eventData.pressEventCamera,
                    out cellLocalPosition
                );

                tp.Add(cellLocalPosition);
            }
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
        sprite.raycastTarget = true;

        if (tp.Count < 1)
        {
            if (!isInInventory)
                transform.parent = CurrentParent;
            else
            {
                rect.anchoredPosition = CurrentPos;

                foreach (var c in oldCells)
                    c.setFilled(ID);
            }

            foreach (var c in cells)
                c.setFilled(ID);

            return;
        }

        rect.anchoredPosition = tp.First();

        for (int i = 0; i < inv.cells.Count; i++)
        {
            var c = inv.cells[i];
            if (c.IsOverlaps(rect, ID))
                cells.Add(c);
        }

        foreach (var c in cells)
        {
            if (c.FID != -1 && c.FID != ID || c.fill)
            {
                if (!isInInventory)
                    transform.parent = CurrentParent;
                else
                {
                    rect.anchoredPosition = CurrentPos;

                    foreach (var c2 in oldCells)
                        c2.setFilled(ID);
                }

                foreach (var c2 in cells)
                    c2.UpdateCell();

                return;
            }
        }
        foreach (var c in cells)
        c.setFilled(ID);

        inv.AddItem(this);
        CurrentPos = tp.First();

        oldCells.Clear();
        foreach (var c in cells)
            oldCells.Add(c);

    }
  private  void UpdateSlots()
{
    foreach (var c2 in cells)
        c2.UpdateCell();

    tp.Clear();
    cells.Clear();

    foreach (var c in inv.cells)
    {
        if (c.IsOverlaps(rect, ID))
        {
            cells.Add(c);

            Vector2 cellLocalPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                inv.GetComponent<RectTransform>(),
                c.rect.position,
                null,
                out cellLocalPosition
            );

            tp.Add(cellLocalPosition);
        }
    }
}

}

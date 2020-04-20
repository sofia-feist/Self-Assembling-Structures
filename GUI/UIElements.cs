using UnityEngine;
using UnityEngine.EventSystems;

public class UIElements : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public static bool isBeingDragged = false;



    public void OnPointerDown(PointerEventData eventData)
    {
        isBeingDragged = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isBeingDragged = false;
    }


    public void OnBeginDrag(PointerEventData data)
    {
        isBeingDragged = true;
    }

    public void OnDrag(PointerEventData data)
    {
        isBeingDragged = true;
    }

    public void OnEndDrag(PointerEventData data)
    {
        isBeingDragged = false;
    }
}


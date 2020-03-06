using UnityEngine;
using UnityEngine.EventSystems;

public class SpeedSlider : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public bool isBeingDragged = false;



    public void OnBeginDrag(PointerEventData data)
    {
        isBeingDragged = true;
    }

    public void OnEndDrag(PointerEventData data)
    {
        isBeingDragged = false;
    }
}


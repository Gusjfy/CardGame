using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform parentToReturnTo = null;

    public void OnBeginDrag(PointerEventData eventData)
    {
        this.parentToReturnTo = transform.parent;
        transform.SetParent(this.parentToReturnTo.parent);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.SetParent(parentToReturnTo);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public Transform GetParentToReturnTo()
    {
        return this.parentToReturnTo;
    }
    
    public void SetParentToReturnTo(Transform ParentToReturnTo)
    {
        this.parentToReturnTo = ParentToReturnTo;
    }
}

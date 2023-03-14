using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drop : MonoBehaviour, IDropHandler
{
    
    public void OnDrop(PointerEventData eventData)
    {
        Drag script = eventData.pointerDrag.GetComponent<Drag>();
        script.SetParentToReturnTo(transform);
    }
}

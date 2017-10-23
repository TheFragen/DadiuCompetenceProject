using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PropagatePointerEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private UnityEvent OnPointerEnterEvent, OnPointerExitEvent;

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject[] any = eventData.hovered.Where(o => o.activeSelf).ToArray();
        if (any.Length > 0)
        {
            OnPointerEnterEvent.Invoke();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitEvent.Invoke();
    }
}

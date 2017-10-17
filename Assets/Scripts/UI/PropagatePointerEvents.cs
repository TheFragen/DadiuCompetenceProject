using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PropagatePointerEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private UnityEvent OnPointerEnterEvent, OnPointerExitEvent;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterEvent.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitEvent.Invoke();
    }
}

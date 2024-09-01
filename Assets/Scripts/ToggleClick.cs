using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class ToggleClick : MonoBehaviour, IPointerClickHandler
{

    public UnityEvent OnClickToggle;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickToggle?.Invoke();
    }
}

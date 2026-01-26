using UnityEngine;
using UnityEngine.EventSystems;

public class ClickToClose : MonoBehaviour, IPointerDownHandler
{
    public GameObject popupToClose;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (popupToClose != null)
        {
            popupToClose.SetActive(false);
        }
    }
}
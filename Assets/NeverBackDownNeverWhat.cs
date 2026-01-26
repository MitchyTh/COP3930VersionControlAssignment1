using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;     // ← needed for IPointerDownHandler
using System.Collections.Generic;

public class RandomPopup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button triggerButton;          // your existing button that shows the popup
    [SerializeField] private GameObject popupPanel;         // the whole popup panel (initially inactive)
    [SerializeField] private Image displayImage;            // the Image component that shows the random picture

    [Header("Images")]
    [SerializeField] private List<Sprite> popupSprites = new List<Sprite>();  // drag your sprites here

    [Header("Optional - Close on background click")]
    [SerializeField] private Image backgroundOverlay;       // drag the semi-transparent background Image here
    // (make sure this Image has "Raycast Target" checked in the Inspector)

    void Awake()
    {
        if (triggerButton == null)
        {
            Debug.LogError("Trigger button not assigned!", this);
            return;
        }

        triggerButton.onClick.AddListener(ShowRandomPopup);

        // Optional A: Close popup when clicking the background overlay
        if (backgroundOverlay != null)
        {
            // Add EventTrigger component if it doesn't exist yet (can also do this in prefab/scene)
            var trigger = backgroundOverlay.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = backgroundOverlay.gameObject.AddComponent<EventTrigger>();
            }

            // We use the simpler IPointerDownHandler approach instead of EventTrigger
            // So we attach a small helper component (see below) or handle it directly
            var closer = backgroundOverlay.gameObject.GetComponent<ClickToClose>();
            if (closer == null)
            {
                closer = backgroundOverlay.gameObject.AddComponent<ClickToClose>();
            }
            closer.popupToClose = popupPanel;
        }
        else
        {
            Debug.LogWarning("Background overlay not assigned → clicking background won't close the popup.", this);
        }
    }

    private void ShowRandomPopup()
    {
        if (popupSprites == null || popupSprites.Count == 0)
        {
            Debug.LogWarning("No popup images assigned!", this);
            return;
        }

        int randomIndex = Random.Range(0, popupSprites.Count);
        displayImage.sprite = popupSprites[randomIndex];

        popupPanel.SetActive(true);
    }

    // Public method so the close button (if you have one) and background can call it
    public void HidePopup()
    {
        popupPanel.SetActive(false);
    }
}
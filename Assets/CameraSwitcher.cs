using UnityEngine;

public class CameraSwitchTrigger : MonoBehaviour
{
    [SerializeField] private Camera cameraToEnable;    // drag the new camera here
    [SerializeField] private Camera cameraToDisable;   // usually your main camera

    // Optional: if you want to switch back when leaving the area
    //[SerializeField] private bool switchBackOnExit = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (cameraToDisable != null)
            cameraToDisable.gameObject.SetActive(false);   // or .enabled = false

        if (cameraToEnable != null)
            cameraToEnable.gameObject.SetActive(true);     // or .enabled = true

        // Optional: update the main camera tag/reference if other scripts use Camera.main
        // cameraToEnable.tag = "MainCamera";
    }

}
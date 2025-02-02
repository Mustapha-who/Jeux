using UnityEngine;

public class PanelOpener : MonoBehaviour
{
    public GameObject panel; // Unique panel for this object
    private static GameObject activePanel = null; // Currently open panel
    private static PanelOpener activePanelOpener = null; // Track active opener

    private Camera playerCamera; // The player's camera
    private float raycastDistance = 3f; // Raycast distance to check for object interaction



    void Start()
    {
        playerCamera = Camera.main; // Get the player's camera
        if (panel != null)
        {
            panel.SetActive(false); // Ensure panel starts hidden
        }
        
    }

    private void Update()
    {
        RaycastHit hit;

        // Cast a ray from the center of the screen (camera)
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, raycastDistance)) // Ray hits something within range
        {
            if (hit.collider.gameObject == gameObject && Input.GetKeyDown(KeyCode.F)) // Check if the ray hit this object and F is pressed
            {
                TogglePanel();
            }
        }
        
    }

    void TogglePanel()
    {
        if (panel != null)
        {
            // If another panel is active, close it before toggling this one
            if (activePanel != null && activePanel != panel)
            {
                activePanel.SetActive(false);
            }

            // If the current panel is already open, close it; otherwise, open it
            bool isActive = !panel.activeSelf;
            panel.SetActive(isActive);

            // Update the active panel reference
            activePanel = isActive ? panel : null;
        }
    }
}

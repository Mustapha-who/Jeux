using UnityEngine;
using UnityEngine.UI; 
public class Playerinteract : MonoBehaviour
{

    public float playerReach = 3f;
    public GameObject panel;
    public PanelOpener panelOpener;
    Interactable currentInteractable;
    // Update is called once per frame
    void Update()
    {
        CheckInteraction();
         if (Input.GetKeyDown(KeyCode.F)) // Check if F is pressed
        {
            
        }
    }
    

    

    public void CheckInteraction()
    {
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray,out hit, playerReach))
        {
            if (hit.collider.tag == "Interactable")// if looking at an interactable object.
            {
             Interactable newInteractable = hit.collider.GetComponent<Interactable>();   
            

            if (newInteractable.enabled)
            {
                SetNewCurrentInteractable(newInteractable);
            }

            else //if nothing in reach
            {
            DisableCurrentInteractable();
            }
            

        }

            else 
        {
            DisableCurrentInteractable();
        }
            
        }
        else //if nothing in reach
        {
            DisableCurrentInteractable();
        }

        
    }

    void SetNewCurrentInteractable(Interactable newInteractable){
        currentInteractable = newInteractable;
        currentInteractable.EnableOutline();
        HUDController.instance.EnableInteractionText(currentInteractable.message);
    }

    void DisableCurrentInteractable()
    {
        HUDController.instance.DisableInteractionText();
        if (currentInteractable)
        {
            currentInteractable.DisableOutline();
            currentInteractable = null;
        }
    }
}

using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
public static HUDController instance;
private void Awake(){
    instance = this;
}

[SerializeField] TMP_Text interactionText;

public void EnableInteractionText(string text)
{
    interactionText.text = text + " Press F";
    interactionText.gameObject.SetActive(true);
}
public void DisableInteractionText(){
    interactionText.gameObject.SetActive(false);
}

public bool IsInteractionTextEnabled()
{
    return interactionText.gameObject.activeSelf; // Check if the interaction text is active
}


}

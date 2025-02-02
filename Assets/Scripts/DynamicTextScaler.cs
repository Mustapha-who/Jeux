using UnityEngine;
using TMPro;  // For TextMeshPro, if you're using Unity's default Text, use UnityEngine.UI;

public class DynamicTextScaler : MonoBehaviour
{
    public TextMeshProUGUI panelText; // Reference to TextMeshProUGUI
    public RectTransform panelRect; // Reference to the panel's RectTransform

    public float baseFontSize = 24f; // The base font size when the panel is at a certain size
    public float sizeFactor = 0.1f; // Factor to scale font size based on the panel's size

    void Update()
    {
        if (panelText != null && panelRect != null)
        {
            ScaleText();
        }
    }

    void ScaleText()
    {
        // Get the width or height of the panel
        float panelWidth = panelRect.rect.width;

        // Scale font size based on panel width (you can also use height if preferred)
        float newFontSize = baseFontSize + (panelWidth * sizeFactor);

        // Set the new font size to the text
        panelText.fontSize = newFontSize;
    }
}

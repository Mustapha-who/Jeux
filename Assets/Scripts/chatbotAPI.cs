using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;  // For UnityWebRequest
using System.Collections;
using System.Text;             // For Encoding
using TMPro;

public class chatbotAPI : MonoBehaviour
{
    [Header("UI References")]
    public InputField userInputField;      // Drag your InputField here
    public Text responseText;              // Drag your Text (or TextMeshProUGUI) here

    [Header("API Settings")]
    public string apiURL = "http://127.0.0.1:8000/chat";  // Your FastAPI endpoint
    public string apiKey = "my_secure_api_key_123";       // Must match API_KEY in your FastAPI code

    /// <summary>
    /// Called by your "Send" button's OnClick event
    /// </summary>
    public void OnSendButtonClicked()
    {
        string userMessage = userInputField.text;
        if (!string.IsNullOrEmpty(userMessage))
        {
            // Start the coroutine to send data to the API
            StartCoroutine(SendMessageToAPI(userMessage));
        }
    }

    /// <summary>
    /// Coroutine to send POST request to the FastAPI endpoint
    /// </summary>
    IEnumerator SendMessageToAPI(string userMessage)
    {
        // 1. Create request body (must match ChatRequest in your Python code)
        ChatRequest chatRequest = new ChatRequest { message = userMessage };
        string jsonBody = JsonUtility.ToJson(chatRequest);

        // 2. Prepare UnityWebRequest
        using (UnityWebRequest webRequest = new UnityWebRequest(apiURL, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            // 3. Set headers
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("api_key", apiKey);

            // 4. Send request, yield until done
            yield return webRequest.SendWebRequest();

            // 5. Check for network or HTTP errors
            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
                responseText.text = "Error: " + webRequest.error;
            }
            else
            {
                // 6. Parse the JSON response
                string jsonResponse = webRequest.downloadHandler.text;
                ChatResponse chatResponse = JsonUtility.FromJson<ChatResponse>(jsonResponse);

                // 7. Display the response to your UI text
                responseText.text = chatResponse.response;
            }
        }
    }
}

// Classes to mirror your Python Pydantic models
[System.Serializable]
public class ChatRequest
{
    public string message;
}

[System.Serializable]
public class ChatResponse
{
    public string response;
}
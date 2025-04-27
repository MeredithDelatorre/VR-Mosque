using UnityEngine;
using TMPro;  // Needed for TextMeshPro

public class ColliderTextChange : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI panelText;    // The Text field inside your Canvas

    [Header("New Text")]
    [TextArea]
    public string newMessage = "This is the updated message after first collision!";

    private bool hasTriggered = false; // 🔥 New flag to track if it already triggered

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            Debug.Log("✅ Sphere collided! Updating panel text...");

            if (panelText != null)
            {
                panelText.text = newMessage;
            }
            else
            {
                Debug.LogWarning("⚠️ Panel Text is not assigned in the inspector!");
            }

            hasTriggered = true; // ✅ Now block future triggers
        }
    }
}

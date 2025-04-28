using UnityEngine;
using TMPro;  

public class ColliderTextChange : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI panelText;  

    [Header("New Text")]
    [TextArea]
    public string newMessage = "This is the updated message after first collision!";

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            Debug.Log("Sphere collided! Updating panel text...");

            if (panelText != null)
            {
                panelText.text = newMessage;
            }
            else
            {
                Debug.LogWarning("⚠Panel Text is not assigned in the inspector!");
            }

            hasTriggered = true;
        }
    }
}

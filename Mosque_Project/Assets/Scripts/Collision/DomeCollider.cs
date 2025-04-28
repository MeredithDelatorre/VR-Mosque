using UnityEngine;
using TMPro;

public class DomeCollider : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI panelText;    

    [Header("Text Messages")]
    [TextArea]
    public string newMessage = "This is the updated message after first collision!";
    [TextArea]
    public string exitMessage = "Thanks for visiting.";

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
                Debug.LogWarning("Panel Text is not assigned in the inspector!");
            }

            hasTriggered = true; // Block future triggers
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited! Updating panel text...");

            if (panelText != null)
            {
                panelText.text = exitMessage;
            }
            else
            {
                Debug.LogWarning("Panel Text is not assigned in the inspector!");
            }
        }
    }
}

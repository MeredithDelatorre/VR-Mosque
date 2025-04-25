using UnityEngine;
using TMPro; // Required for TextMeshPro

public class ScrollInteraction : MonoBehaviour
{
    [TextArea(3, 10)]
    public string infoText;

    public GameObject infoPanel; // Reference to inactive panel
    public TextMeshProUGUI infoTextField;

    public void ActivateInfo()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(true);

            if (infoTextField != null)
            {
                infoTextField.text = infoText;
            }
        }
        else
        {
            Debug.LogWarning("InfoPanel reference missing!");
        }
        Debug.Log("Trying to activate panel: " + infoPanel.name);
    }
}
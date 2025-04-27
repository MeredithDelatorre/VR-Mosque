using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
public class ScrollInteraction : MonoBehaviour
{
    [Header("Primary Panel Settings")]
    public Canvas primaryCanvas;
    public TextMeshProUGUI primaryText;
    [TextArea]
    public string primaryMessage;

    [Header("Secondary Panel Settings")]
    public TextMeshProUGUI secondaryText;
    [TextArea]
    public string secondaryMessage;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        interactable.selectEntered.AddListener(OnSelectScroll);
    }

    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnSelectScroll);
    }

    private void OnSelectScroll(SelectEnterEventArgs args)
    {
        Debug.Log("✅ Scroll clicked!");

        // Activate primary panel and update text
        if (primaryCanvas != null)
        {
            primaryCanvas.enabled = true;
        }
        if (primaryText != null)
        {
            primaryText.text = primaryMessage;
        }

        // Activate secondary panel and update text
        if (secondaryText != null)
        {
            secondaryText.text = secondaryMessage;
        }
    }
}


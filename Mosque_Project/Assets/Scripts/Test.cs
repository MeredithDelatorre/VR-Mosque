using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


public class TestScrollInteraction : MonoBehaviour
{
    void Start()
    {
        XRSimpleInteractable backChange = GetComponent<XRSimpleInteractable>();
        backChange.selectEntered.AddListener(PrintMessage);
    }

    void PrintMessage(SelectEnterEventArgs arg)
    {
        Debug.Log("✅ Scroll was selected!");
    }

}

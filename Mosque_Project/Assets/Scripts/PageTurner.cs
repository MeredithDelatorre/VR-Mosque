using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PageTurner : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public GameObject panel;    // The panel that holds the text
    public Button nextButton;   // The button the user clicks

    private int currentPage = 1;

    void Start()
    {
        textComponent.pageToDisplay = currentPage;
    }

    public void NextPage()
    {
        if (currentPage < textComponent.textInfo.pageCount)
        {
            currentPage++;
            textComponent.pageToDisplay = currentPage;
        }
        else
        {
            // If already on the last page, hide panel and button
            panel.SetActive(false);
            nextButton.gameObject.SetActive(false);
        }
    }
}



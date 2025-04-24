using UnityEngine;

public class ScrollDomePopUp : MonoBehaviour
{
    [SerializeField] Canvas toggleCanvas;
    bool toggle = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        toggleCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void openClose(){
        if(toggle == false){
            toggle = true;
            toggleCanvas.enabled = true;
        }else{
            toggle = false;
            toggleCanvas.enabled = false;
        }
    }
}

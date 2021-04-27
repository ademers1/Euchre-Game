using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitButton : MonoBehaviour
{

    public GameObject quitPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        quitPanel.SetActive(true);
    }

    public void NoClicked()
    {
        quitPanel.SetActive(false);
    }

    public void YesClicked()
    {
        Application.Quit();
    }
}

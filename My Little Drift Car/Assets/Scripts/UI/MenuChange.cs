using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuChange : MonoBehaviour
{
    public GameObject uipanel;
    private bool uiActive;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            if(!uiActive)
            {
                uipanel.SetActive(true);
                uiActive = true;
            }
            else
            {
                uipanel.SetActive(false);
                uiActive = false;
            }
        }
    }

    public void changescene()
    {
        SceneManager.LoadScene("Test_Level_1");
    }

    public void exit()
    {
        SceneManager.LoadScene("Menu");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    public GameObject s14drift,s2kdrift,ui;

    public void s14d()
    {
        Debug.Log("S14 Drift Pressed");
        ui.SetActive(false);
        Instantiate(s14drift);
    }
    public void s2kd()
    {
        Debug.Log("S2K Drift Pressed");
        ui.SetActive(false);
        Instantiate(s2kdrift);
    }
}

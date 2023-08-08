using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Gamemanager : MonoBehaviour
{
    private int selector,final;
    public GameObject first,second;
    public GameObject[] cars;
    public Vector3 pos = new Vector3(330.2f,21.48f,65f);
    public bool photon;

    public void s14()
    {
        selector = 1;
        Debug.Log("S14 Pressed");
        first.SetActive(false);
        second.SetActive(true);
        //if (photon)
        //    PhotonNetwork.Instantiate(s14drift.name, pos, Quaternion.identity);
        //else
        //    Instantiate(s14drift,pos, Quaternion.identity);
    }
    public void s2k()
    {
        selector = 2;
        Debug.Log("S2K Pressed");
        first.SetActive(false);
        second.SetActive(true);
        //if (photon)
        //    PhotonNetwork.Instantiate(s2kdrift.name, pos, Quaternion.identity);
        //else
        //    Instantiate(s2kdrift);
    }

    public void wrx()
    {
        selector = 3;
        Debug.Log("WRX Pressed");
        first.SetActive(false);
        second.SetActive(true);
    }

    public void r34()
    {
        selector = 4;
        Debug.Log("R34 Pressed");
        first.SetActive(false);
        second.SetActive(true);
    }

    public void back()
    {
        selector = 0;
        Debug.Log("Back pressed");
        second.SetActive(false);
        first.SetActive(true);
    }

    public void race()
    {
        final = selector + 4;
    }

    public void drift()
    {
        final = selector;
    }
}

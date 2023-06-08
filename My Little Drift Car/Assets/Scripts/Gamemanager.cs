using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Gamemanager : MonoBehaviour
{
    public GameObject s14drift,s2kdrift,ui;
    public Vector3 pos = new Vector3(330.2f,21.48f,65f);
    public bool photon;

    public void s14d()
    {
        Debug.Log("S14 Drift Pressed");
        ui.SetActive(false);
        if (photon)
            PhotonNetwork.Instantiate(s14drift.name, pos, Quaternion.identity);
        else
            Instantiate(s14drift,pos, Quaternion.identity);
    }
    public void s2kd()
    {
        Debug.Log("S2K Drift Pressed");
        ui.SetActive(false);
        if (photon)
            PhotonNetwork.Instantiate(s2kdrift.name, pos, Quaternion.identity);
        else
            Instantiate(s2kdrift);
    }
}

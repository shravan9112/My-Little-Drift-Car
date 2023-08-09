using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class CreateAndJoinRoom : MonoBehaviourPunCallbacks
{
    public GameObject createInput;
    public GameObject joinInput;

    public void information()
    {
        Application.OpenURL("https://drive.google.com/file/d/13rDMi9lhK9XNN1Fo0-egGBS2EuO0QYmR/view?usp=sharing");
    }

    public void consent()
    {
        Application.OpenURL("https://drive.google.com/file/d/1uu3VdC_TmMwZubUCPRe6DeBRCCAiNY19/view?usp=sharing");
    }
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createInput.GetComponent<TMP_InputField>().text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.GetComponent<TMP_InputField>().text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Scene 1");
    }
}

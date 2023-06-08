using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class CreateAndJoinRoom : MonoBehaviourPunCallbacks
{
    public GameObject createInput;
    public GameObject joinInput;

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
        PhotonNetwork.LoadLevel("s1");
    }
}

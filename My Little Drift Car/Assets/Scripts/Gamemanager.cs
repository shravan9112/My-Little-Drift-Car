using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Gamemanager : MonoBehaviour
{
    private int selector,final;
    public GameObject first,second,ui,hints,panel,credits;
    public GameObject[] cars,showcars;
    public float time,maxspeed;
    public Vector3 pos = new Vector3(330.2f,21.48f,65f);
    public bool photon,exit,runtimer;
    public TMP_Text timeText;

    public void escape()
    {
        ui.SetActive(true);
        exit = false;
    }
    private void Awake()
    {
        maxspeed = 0;
        time = 0;
        runtimer = true;
    }

    private void Update()
    {
        check();
        if(runtimer == true)
        {
            time = time + Time.deltaTime;
            timeText.text = time.ToString("F1");
        } 
        if (exit == true)
            escape();
    }

    public void quit()
    {
        runtimer = false;
        first.SetActive(false);
        panel.SetActive(false);
        credits.SetActive(true);
    }

    public void backtomenu()
    {
        runtimer = true;
        credits.SetActive(false);
        first.SetActive(true);
        panel.SetActive(true);
    }

    public void website()
    {
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSee5BTWim4k7lpEi8UCGTpTi23AWJm-_qfS3igZkt_jrotCXA/viewform?usp=sf_link");
        Debug.Log("Survey clicked");
        Application.Quit();
    }

    public void check()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            hints.SetActive(true);
        }
        if(Input.GetKeyUp(KeyCode.Tab))
        {
            hints.SetActive(false);
        }
    }

    public void s14()
    {
        selector = 1;
        showcars[0].SetActive(true);
        Debug.Log("S14 Pressed");
        first.SetActive(false);
        second.SetActive(true);
    }
    public void s2k()
    {
        selector = 2;
        showcars[1].SetActive(true);
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
        showcars[2].SetActive(true);
        Debug.Log("WRX Pressed");
        first.SetActive(false);
        second.SetActive(true);
    }

    public void r34()
    {
        selector = 4;
        showcars[3].SetActive(true);
        Debug.Log("R34 Pressed");
        first.SetActive(false);
        second.SetActive(true);
    }

    public void back()
    {
        selector = 0;
        for(int i =0;i<4;i++)
        {
            showcars[i].SetActive(false);
        }
        Debug.Log("Back pressed");
        second.SetActive(false);
        first.SetActive(true);
    }

    public void race()
    {
        final = selector + 3;
        ui.SetActive(false);
        if (photon)
            PhotonNetwork.Instantiate(cars[final].name, pos, Quaternion.identity);
        else
            Instantiate(cars[final], pos, Quaternion.identity);
    }

    public void drift()
    {
        final = selector-1;
        ui.SetActive(false);
        if (photon)
            PhotonNetwork.Instantiate(cars[final].name, pos, Quaternion.identity);
        else
            Instantiate(cars[final], pos, Quaternion.identity);
    }
}

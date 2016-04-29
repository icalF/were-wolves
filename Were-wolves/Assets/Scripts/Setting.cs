using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Setting : MonoBehaviour
{

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void setPort(Text text)
    {
        if(text!=null)
        PlayerPrefs.SetInt("port", Convert.ToInt32(text.GetComponent<Text>().text));
    }
    public void setIP(Text text)
    {
        PlayerPrefs.SetString("ipaddress", text.GetComponent<Text>().text);
    }
    public void setNight()
    {
        PlayerPrefs.SetString("time", "night");
    }
    public void setNoon()
    {
        PlayerPrefs.SetString("time", "noon");
    }

}

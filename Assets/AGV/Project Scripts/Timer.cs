using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public GameObject time_data;
    public bool count_time;

    private float StartTime;
    private float PauseTime = 0;

    private void Start()
    {
        StartTime = Time.time;
        time_data.SetActive(true);
        time_data.GetComponent<TMPro.TextMeshPro>().text = "";
    }

    private void OnEnable()
    {
        StartTime = Time.time;
    }

    void Update()
    {
        if (count_time)
        {
            float TimerControl = Time.time - StartTime;
            string mins = ((int)TimerControl / 60).ToString("00");
            string segs = (TimerControl % 60).ToString("00");
            string milisegs = ((TimerControl * 100) % 100).ToString("00");

            string TimerString = string.Format("{00}:{01}:{02}", mins, segs, milisegs);

            time_data.GetComponent<TMPro.TextMeshPro>().text = TimerString.ToString();
        }       
    }

    public void StartCountTime()
    {
        StartTime = Time.time;
        count_time = true;
    }

    public void StopCountTime()
    {
        count_time = false;
        PauseTime = Time.time - StartTime;
    }

    public void ResumeCountTime()
    {
        count_time = true;
        StartTime = (Time.time - PauseTime);
        //There´s an erro where resetting after finishing a sthe time does not update propperly
    }

    public void ResetTime()
    {
        count_time = false;
        time_data.GetComponent<TMPro.TextMeshPro>().text = "";
    }
}
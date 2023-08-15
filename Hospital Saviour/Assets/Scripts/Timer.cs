using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;


public class Timer : MonoBehaviour
{
    [Range(0, 59)]
    public int minutes;
    [Range(0, 59)]
    public int seconds;

    public Text standardText;
    public Image dialSlider;

    bool timerRunning = false;
    bool timerPaused = false;
    public double timeRemaining;
    

    private void Awake()
    {
        if(!standardText)
        if(GetComponent<Text>())
        {
            standardText = GetComponent<Text>();
        }
        if(!dialSlider)
        if(GetComponent<Image>())
        {
            dialSlider = GetComponent<Image>();
        }
            dialSlider.fillAmount = 1f;
    }
    void Start()
    {
        standardText.text = DisplayFormattedTime(ReturnTotalSeconds());
    }
    void Update()
    {
        if(timerRunning)
        {
            if (timeRemaining > 0.02)
            {
                timeRemaining -= Time.deltaTime;
                DisplayInTextObject();
            }
            else
            {
                timeRemaining = 0;
                timerRunning = false;
                DisplayInTextObject();
            }
            float timeRangeClamped = Mathf.InverseLerp(ReturnTotalSeconds(), 0, (float)timeRemaining);
            dialSlider.fillAmount = Mathf.Lerp(1, 0, timeRangeClamped);
        }
    }

    private void DisplayInTextObject()
    { 
        standardText.text = DisplayFormattedTime(timeRemaining);
    }

    public void StartTimer()
    {
        if (!timerRunning && !timerPaused)
        {
            timerPaused = false;
        
            timeRemaining = ReturnTotalSeconds();
            DisplayInTextObject();
            dialSlider.fillAmount = 1f;
            
            timerRunning = true;
            timeRemaining = minutes * 60;
            timeRemaining += seconds;

            DisplayFormattedTime(timeRemaining);
        }
    }

    public float ReturnTotalSeconds()
    {
        float totalTimeSet;
        totalTimeSet = minutes * 60;
        totalTimeSet += seconds;
        return totalTimeSet;
    }

    public string DisplayFormattedTime(double remainingSeconds)
    {
        string convertedNumber;
        float minutes, seconds;
        minutes = Mathf.FloorToInt(((float)remainingSeconds / 60));
        seconds = Mathf.FloorToInt((float)remainingSeconds - ((float)minutes * 60));

        string MinutesFormat()
        {
            string minutesFormatted;
            minutesFormatted = string.Format("{0:00}", minutes);
            minutesFormatted += ":";
            return minutesFormatted;
        }
        string SecondsFormat()
        {
            string secondsFormatted; 
            secondsFormatted = string.Format("{0:00}", seconds);              
            return secondsFormatted;
        }
        

        convertedNumber = MinutesFormat() + SecondsFormat();

        return convertedNumber;
    }

    public void SetTimeFromSeconds(int seconds)
    {
        minutes = seconds / 60;
        this.seconds = seconds % 60;
    }
}

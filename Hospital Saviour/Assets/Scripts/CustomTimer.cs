using UnityEngine;
using UnityEngine.UI;


// Inspired by https://gamedevbeginner.com/how-to-make-countdown-timer-in-unity-minutes-seconds/
public class CustomTimer : MonoBehaviour
{
    public int minutes;
    public int seconds;
    public Text timerText;
    public Image dialSlider;
    bool isRunning = false;
    bool isPaused = false;
    public double remaining;


    private void Awake()
    {
        if (!timerText)
            if (GetComponent<Text>())
            {
                timerText = GetComponent<Text>();
            }

        if (!dialSlider)
            if (GetComponent<Image>())
            {
                dialSlider = GetComponent<Image>();
            }

        dialSlider.fillAmount = 1f;
    }

    void Start()
    {
        Debug.Log(timerText);
        timerText.text = DisplayTime(ReturnTotalSeconds());
    }

    void Update()
    {
        if (isRunning)
        {
            if (remaining > 0.02)
            {
                remaining -= Time.deltaTime;
                Display();
            }
            else
            {
                remaining = 0;
                isRunning = false;
                Display();
            }

            float timeRange = Mathf.InverseLerp(ReturnTotalSeconds(), 0, (float)remaining);
            dialSlider.fillAmount = Mathf.Lerp(1, 0, timeRange);
        }
    }

    private void Display()
    {
        timerText.text = DisplayTime(remaining);
    }

    public void StartTimer()
    {
        if (!isRunning && !isPaused)
        {
            isPaused = false;

            remaining = ReturnTotalSeconds();
            Display();
            dialSlider.fillAmount = 1f;

            isRunning = true;
            remaining = minutes * 60;
            remaining += seconds;

            DisplayTime(remaining);
        }
    }

    public float ReturnTotalSeconds()
    {
        float totalTimeSet;
        totalTimeSet = minutes * 60;
        totalTimeSet += seconds;
        return totalTimeSet;
    }

    public string DisplayTime(double remainingSeconds)
    {
        string time, mFormatted, sFormatted;
        float minutes, seconds;
        minutes = Mathf.FloorToInt((float)remainingSeconds / 60);
        seconds = Mathf.FloorToInt((float)remainingSeconds - (minutes * 60));

        mFormatted = string.Format("{0:00}", minutes) + ":";
        sFormatted = string.Format("{0:00}", seconds);

        time = mFormatted + sFormatted;
        
        Debug.Log(time);

        return time;
    }

    public void SetTimeFromSeconds(int seconds)
    {
        minutes = seconds / 60;
        this.seconds = seconds % 60;
    }
}
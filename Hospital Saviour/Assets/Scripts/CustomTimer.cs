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

    // Executed on start scene
    private void Awake()
    {
        // Following two IFs are separate to avoid edge cases when one was able to initialize and another wasn't
        // If timerText is empty i.e. timer is not initialized -- create component
        if (!timerText)
            if (GetComponent<Text>())
            {
                timerText = GetComponent<Text>();
            }
        
        // If dialSlider is empty i.e. timer is not initialized -- create component
        if (!dialSlider)
            if (GetComponent<Image>())
            {
                dialSlider = GetComponent<Image>();
            }
        
        dialSlider.fillAmount = 1f;
    }

    void Start()
    {
        // Assign to timer text amount of time left
        timerText.text = DisplayTime(ReturnTotalSeconds());
    }

    // Executed every frame/tick
    void Update()
    {
        // Guard check that timer is running
        if (isRunning)
        {
            // If there is time left deduct deltaTime (usually 1 sec) from remaining time and trigger display to update
            // text. Magic 0.02 number help this to work consistently because of floats and double arithmetics
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

            // Extrapolate remaining time to 360 dial 
            float timeRange = Mathf.InverseLerp(ReturnTotalSeconds(), 0, (float)remaining);
            dialSlider.fillAmount = Mathf.Lerp(1, 0, timeRange);
        }
    }

    private void Display()
    {
        // Update text with remaining time
        timerText.text = DisplayTime(remaining);
    }

    // Custom function to start timer manually
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

    // Helper function to convert minutes and seconds to total seconds
    public float ReturnTotalSeconds()
    {
        float totalTimeSet;
        totalTimeSet = minutes * 60;
        totalTimeSet += seconds;
        return totalTimeSet;
    }
    
    // Function to format time to minutes and seconds
    public string DisplayTime(double remainingSeconds)
    {
        string time, mFormatted, sFormatted;
        float minutes, seconds;
        minutes = Mathf.FloorToInt((float)remainingSeconds / 60);
        seconds = Mathf.FloorToInt((float)remainingSeconds - (minutes * 60));

        mFormatted = string.Format("{0:00}", minutes) + ":";
        sFormatted = string.Format("{0:00}", seconds);

        time = mFormatted + sFormatted;

        return time;
    }

    // Function to programmatically set time
    public void SetTimeFromSeconds(int seconds)
    {
        minutes = seconds / 60;
        this.seconds = seconds % 60;
    }
}
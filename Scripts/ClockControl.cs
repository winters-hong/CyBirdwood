using System;
using UnityEngine;
using UnityEngine.UI;

public class ClockControl : MonoBehaviour
{
    public bool realtime = false;
    public bool isPause = false;

    [Header("Timespan(min) for One Day")]
    public float minPerDay = 1; //1 day real time = 5 min in game
    private int date = 1; //set initial date

    [Header("Skybox")]
    public GameObject sky;
    private Vector3 skydegree;

    private float degreesPerHour,
        degreesPerMinute,
        degreesPerSecond;
    public double initialHour,
        initialMinute,
        initialSecond;
    private double gapHour = 0f,
        gapMinute = 0f,
        gapSecond = 0f;
    private bool getStopTime = false;
    private TimeSpan stopTime;
    private double oldTotal;

    void InitialDegrees()
    {
        if (realtime)
        {
            //1 day equals to 5 min, speedup 24*60/5
            degreesPerHour = 30f;
            degreesPerMinute = 6f;
            degreesPerSecond = 6f;
        }
        else
        {
            //1 day equals to 5 min, speedup 24*60/5
            degreesPerHour = 30f * (24 * 60 / minPerDay);
            degreesPerMinute = 360f * (24 * 60 / minPerDay);
            degreesPerSecond = 1 * (24 * 60 / minPerDay);
        }
    }

    void InitialTime()
    {
        TimeSpan initialTime = DateTime.Now.TimeOfDay;
        if (realtime)
        {
            initialHour = 0;
            initialMinute = 0;
            initialSecond = 0;
        }
        else
        {
            initialHour = initialTime.TotalHours;
            initialMinute = initialTime.TotalMinutes;
            initialSecond = initialTime.TotalSeconds;
        }
    }
    void GetGapTime()
    {
        TimeSpan currentTime = DateTime.Now.TimeOfDay;

        if (getStopTime == false)
        {
            stopTime = DateTime.Now.TimeOfDay; //get pause time
            getStopTime = true;
        }

        gapHour = currentTime.TotalHours - stopTime.TotalHours; //get timespan of current time and pause time
        gapMinute = currentTime.TotalMinutes - stopTime.TotalMinutes;
        gapSecond = currentTime.TotalSeconds - stopTime.TotalSeconds;
    }

    private void Start()
    {
        InitialDegrees();
        InitialTime();
    }

    void Update()
    {
        if (realtime)
        {
            UpdateContinuous();
        }
        else
        {
            if (isPause)
            {
                GetGapTime();
            }
            else
            {
                if (getStopTime == true)
                {
                    getStopTime = false;
                    initialHour += gapHour;
                }
                UpdateContinuous();
            }
        }

        sky.transform.rotation = Quaternion.Euler(skydegree);
    }

    void UpdateContinuous()
    {
        TimeSpan time = DateTime.Now.TimeOfDay;

        //skybox
        skydegree.x = ((float)((time.TotalHours - initialHour) * degreesPerHour) / 2) % 360;

        date = 1 + (int)(((float)((time.TotalHours - initialHour) * degreesPerHour) / 2) / 360);
    }

    public float GetSkybox_X()
    {
        return skydegree.x;
    }
    public double ReturnDeltaTime()
    {
        // if (oldTotal == null)
        // {
        //     Debug.Log("awsl");
        //     oldTotal = initialHour;
        // }

        TimeSpan time = DateTime.Now.TimeOfDay;
        double delta = (time.TotalHours - oldTotal);
        oldTotal = time.TotalHours;

        Debug.Log(delta);
        return delta;
    }

    public void ResetSky()
    {
        InitialDegrees();
        InitialTime();
    }

    public void DayNightSwap(float sec)
    {
        isPause = false;
        StartCoroutine(
            DelayToInvoke.DelayToInvokeDo(
                () =>
                {
                    isPause = true;
                },
                sec
            )
        );
    }
}

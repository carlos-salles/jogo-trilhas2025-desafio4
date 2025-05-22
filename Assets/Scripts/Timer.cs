using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField]
    bool autoStart;
    [SerializeField, Min(0f)]
    float duration;

    public bool IsRunning { get; private set; }
    public bool IsStopped { get => !IsRunning; }
    public float TimeLeft { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        if (autoStart) { StartTimer(); }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsRunning) { return; }

        TimeLeft -= Time.deltaTime;
        if (TimeLeft <= 0)
        {
            TimeLeft = 0;
            IsRunning = false;
        }
    }

    public void StartTimer()
    {
        StartTimer(duration);
    }
    public void StartTimer(float duration)
    {
        this.duration = duration;
        TimeLeft = this.duration;
        IsRunning = true;
    }
    public void PauseTimer()
    {
        IsRunning = false;
    }
}

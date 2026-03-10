using UnityEngine;

public class SurvivalTimerRewind : RewindAbstract
{
    private SurvivalTimer survivalTimer;
    private CircularBuffer<float> trackedTime;

    private void Start()
    {
        survivalTimer = GetComponent<SurvivalTimer>();
        trackedTime = new CircularBuffer<float>();
    }

    public override void Track()
    {
        trackedTime.WriteLastValue(survivalTimer.elapsedTime);
    }

    public override void Rewind(float seconds)
    {
        survivalTimer.elapsedTime = trackedTime.ReadFromBuffer(seconds);
    }
}
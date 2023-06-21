using UnityEngine;

public class Timer
{
    private float _tickTime;

    public Timer(float tickTime)
    {
        _tickTime = tickTime;
    }

    public bool IsTicked()
    {
        return Time.time % _tickTime < (Time.time - Time.deltaTime) % _tickTime;
    }
}

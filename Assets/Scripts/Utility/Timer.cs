using UnityEngine;

public class Timer
{
    private float elapsed;
    private float duration;


    public Timer(float duration)
    {
        SetDuration(duration);
        Reset();
    }

    public void AccrueTime(float deltaTime)
    {
        elapsed += deltaTime;
    }

    public bool IsElapsed()
    {
        if (elapsed > duration)
        {
            elapsed = 0;
            return true;
        }

        return false;
    }

    public void Reset()
    {
        elapsed = 0f;
    }

    public void SetDuration(float newDuration)
    {
        duration = Mathf.Max(0.0001f, newDuration); // 0 ¹æÁö
    }
}

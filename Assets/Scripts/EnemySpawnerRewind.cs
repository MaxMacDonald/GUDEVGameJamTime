using UnityEngine;

public class EnemySpawnerRewind : RewindAbstract
{
    private EnemySpawner enemySpawner;
    private CircularBuffer<float> trackedTime;
    private CircularBuffer<int> trackedIndex;

    private void Start()
    {
        enemySpawner = GetComponent<EnemySpawner>();
        trackedTime = new CircularBuffer<float>();
        trackedIndex = new CircularBuffer<int>();
    }

    public override void Track()
    {
        trackedTime.WriteLastValue(enemySpawner.elapsedTime);
        trackedIndex.WriteLastValue(enemySpawner.nextEventIndex);
    }

    public override void Rewind(float seconds)
    {
        enemySpawner.elapsedTime = trackedTime.ReadFromBuffer(seconds);
        enemySpawner.nextEventIndex = trackedIndex.ReadFromBuffer(seconds);
    }
}
using UnityEngine;

public class EnemyRewind : RewindAbstract
{
    private EnemyShooter enemyShooter;
    private CircularBuffer<float> trackedHealth;


    private void Awake()
    {
        enemyShooter = GetComponent<EnemyShooter>();
        trackedHealth = new CircularBuffer<float>();

    }

    public override void Track()
    {
        trackedHealth.WriteLastValue(enemyShooter.health);

    }

    public override void Rewind(float seconds)
    {
        enemyShooter.health = trackedHealth.ReadFromBuffer(seconds);
    }
}
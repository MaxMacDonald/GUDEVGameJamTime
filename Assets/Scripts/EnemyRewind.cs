using UnityEngine;

public class EnemyRewind : RewindAbstract
{
    private IEnemy enemy;
    private CircularBuffer<float> trackedHealth;


    private void Awake()
    {
        enemy = GetComponent<IEnemy>();
        if (enemy == null)
            Debug.LogError($"EnemyRewind on {gameObject.name} couldn't find an IEnemy component!");

    }

    private void Start()
    {
        trackedHealth = new CircularBuffer<float>();

    }

    public override void Track()
    {
        if (enemy == null) return;
        trackedHealth.WriteLastValue(enemy.Health);

    }

    public override void Rewind(float seconds)
    {
        if (enemy == null) return;
        enemy.Health = trackedHealth.ReadFromBuffer(seconds);
    }
}
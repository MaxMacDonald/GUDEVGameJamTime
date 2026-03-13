using UnityEngine;

public class EnemyBossRewind : RewindAbstract
{
    private EnemyBossPhase1 enemyBoss;
    private CircularBuffer<float> trackedHealth;
    private CircularBuffer<float> trackedIsDead;
    private CircularBuffer<float> trackedCooldown;

    private void Awake()
    {
        enemyBoss = GetComponent<EnemyBossPhase1>();
        if (enemyBoss == null)
            Debug.LogError($"EnemyHexRewind on {gameObject.name} couldn't find EnemyHex!");
    }

    private void Start()
    {
        trackedHealth = new CircularBuffer<float>();
        trackedIsDead = new CircularBuffer<float>();
        trackedCooldown = new CircularBuffer<float>();
    }

    public override void Track()
    {
        if (enemyBoss == null) return;
        trackedHealth.WriteLastValue(enemyBoss.health);
        trackedIsDead.WriteLastValue(enemyBoss.isDead ? 1f : 0f);
        trackedCooldown.WriteLastValue(enemyBoss.spawnCooldown);
    }

    public override void Rewind(float seconds)
    {
        if (enemyBoss == null) return;
        enemyBoss.health = trackedHealth.ReadFromBuffer(seconds);
        enemyBoss.isDead = trackedIsDead.ReadFromBuffer(seconds) > 0.5f;
        enemyBoss.spawnCooldown = trackedCooldown.ReadFromBuffer(seconds);
    }
}
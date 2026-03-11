public interface IEnemy
{
    float Health { get; set; }
    bool IsDead { get; set; }
    void TakeDamage(float damage);
    float MoveSpeed { get; set; }
    bool InsideArena { get; set; }
    void Die();
}
using UnityEngine;

public class SpringPoint
{
    public float position;      // current displacement from rest
    public float velocity;
    public float targetPosition = 0f;

    public void Update(float springConstant, float damping)
    {
        float force = -springConstant * (position - targetPosition) - damping * velocity;
        velocity += force * Time.fixedDeltaTime;
        position += velocity * Time.fixedDeltaTime;
    }

    public void Hit(float force)
    {
        velocity += force;
    }
}
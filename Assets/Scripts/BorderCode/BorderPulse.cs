using UnityEngine;

public class BorderPulse : MonoBehaviour
{
    [Header("Timing")]
    public float pulseInterval = 1f;
    public float pulseVariation = 0.1f;

    [Header("Impact Settings")]
    public int impactCount = 8;
    public float impactForce = 3f;
    public bool randomPositions = true;

    [Header("Pattern")]
    public bool doubleBeat = false;
    public float doubleBeatDelay = 0.15f;

    public EllipseBorder ellipseBorder;
    private float timer;


    [Header("Button Pulse")]
    public RectTransform buttonTransform;
    public float buttonPulseScale = 1.05f;
    public float buttonPulseSpeed = 8f;

    private Vector3 buttonOriginalScale;
    private bool buttonPulsing = false;

    void Start()
    {
        
        timer = pulseInterval;
        if (buttonTransform != null)
            buttonOriginalScale = buttonTransform.localScale;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            if (doubleBeat)
            {
                Pulse(impactForce);
                Invoke(nameof(SecondBeat), doubleBeatDelay);
            }
            else
            {
                Pulse(impactForce);
            }

            timer = pulseInterval + Random.Range(-pulseVariation, pulseVariation);
        }
        if (buttonPulsing && buttonTransform != null)
        {
            buttonTransform.localScale = Vector3.Lerp(
                buttonTransform.localScale,
                buttonOriginalScale,
                Time.deltaTime * buttonPulseSpeed
            );

            if (Vector3.Distance(buttonTransform.localScale, buttonOriginalScale) < 0.001f)
            {
                buttonTransform.localScale = buttonOriginalScale;
                buttonPulsing = false;
            }
        }
    }

    void SecondBeat()
    {
        Pulse(impactForce * 0.6f);
    }

    void Pulse(float force)
    {
        if (buttonTransform != null)
        {
            buttonTransform.localScale = buttonOriginalScale * buttonPulseScale;
            buttonPulsing = true;
        }
        if (randomPositions)
        {
            for (int i = 0; i < impactCount; i++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);
                Vector2 hitPos = new Vector2(
                    ellipseBorder.radiusX * Mathf.Cos(angle),
                    ellipseBorder.radiusY * Mathf.Sin(angle)
                );
                ellipseBorder.Ripple(hitPos, force, true);
            }
        }
        else
        {
            float angleStep = Mathf.PI * 2f / impactCount;
            for (int i = 0; i < impactCount; i++)
            {
                float angle = i * angleStep;
                Vector2 hitPos = new Vector2(
                    ellipseBorder.radiusX * Mathf.Cos(angle),
                    ellipseBorder.radiusY * Mathf.Sin(angle)
                );
                ellipseBorder.Ripple(hitPos, force, false);
            }
        }
    }
}
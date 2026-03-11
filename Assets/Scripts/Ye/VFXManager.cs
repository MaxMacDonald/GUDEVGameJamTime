using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;
    public GameObject enemyDeathVFXPrefab;
    public GameObject EnemyGetHitVFXPrefab;
    public float destroyDelay = 2f;

    private void Awake()
    {
        Instance = this;
    }

    
    public void PlayEnemyDeathVFX(Vector3 position)
    {
        GameObject vfx = Instantiate(enemyDeathVFXPrefab, position, Quaternion.identity, transform);
        Destroy(vfx, destroyDelay);

    }

    public void PlayHitVFX(Vector3 position, Quaternion rotation)
    {
        GameObject vfx = Instantiate(EnemyGetHitVFXPrefab, position, rotation, transform);
        Destroy(vfx, destroyDelay);

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

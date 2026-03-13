using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;
    public GameObject enemyDeathVFXPrefab;
    public GameObject enemyDeathRingVFXPrefab;
    public GameObject enemyGetHitVFXPrefab;
    public GameObject bulletHitWallVFXPrefab;


    public float destroyDelay = 2f;

    private void Awake()
    {
        Instance = this;
    }

    
    public void PlayEnemyDeathVFX(Vector3 position)
    {
        GameObject vfx = Instantiate(enemyDeathVFXPrefab, position, Quaternion.identity, transform);
        Destroy(vfx, destroyDelay);

        GameObject vfx2 = Instantiate(enemyDeathRingVFXPrefab, position, Quaternion.identity, transform);
        //Made by Feel, will destroy itself after playing animation, so no need to destroy here


        SFXManager.Instance.PlayEnemyDieFeedbacks();
    }

    public void PlayHitVFX(Vector3 position, Quaternion rotation)
    {
        GameObject vfx = Instantiate(enemyGetHitVFXPrefab, position, rotation, transform);
        Destroy(vfx, destroyDelay);

        SFXManager.Instance.PlayEnemyGetHitSoundFeedbacks();
    }

    public void PlayHitWallVFX(Vector3 position)
    {
        GameObject vfx = Instantiate(bulletHitWallVFXPrefab, position, Quaternion.identity, transform);
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

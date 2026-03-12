using UnityEngine;

public class SFXManager : MonoBehaviour
{

    public static SFXManager Instance;
    public AudioClip enemyDeathSFXPrefab;


    private void Awake()
    {
        Instance = this;
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

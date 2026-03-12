using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.Events;

public class SFXManager : MonoBehaviour
{

    public static SFXManager Instance;
    public MMF_Player enemyDieFeedback;



    public void PlayEnemyDieFeedbacks()
    {
        enemyDieFeedback?.PlayFeedbacks();
    }

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

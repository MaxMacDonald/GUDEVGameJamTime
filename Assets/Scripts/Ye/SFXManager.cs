using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class SFXManager : MonoBehaviour
{

    public static SFXManager Instance;
    public MMF_Player enemyDieFeedback;
    public MMF_Player rewindFeedback;
    public MMF_Player enemyGetHitSoundFeedback;

    [SerializeField] private AudioMixerSnapshot normalSnapshot;
    [SerializeField] private AudioMixerSnapshot rewindSnapshot;

    public float enterTime = 0.02f;
    public float exitTime = 5.0f;

    public void PlayEnemyDieFeedbacks()
    {
        enemyDieFeedback?.PlayFeedbacks();
    }
    public void PlayEnemyGetHitSoundFeedbacks()
    {
        enemyGetHitSoundFeedback?.PlayFeedbacks();
    }


    public void PlayRewindFeedbacks()
    {
        BGMAudioMixerStartRewind();
        rewindFeedback?.PlayFeedbacks();
    }

    public void StopRewindFeedbacks()
    {
        BGMAudioMixerStopRewind();
        rewindFeedback?.StopFeedbacks();
    }

    private void BGMAudioMixerStartRewind()
    {
        rewindSnapshot.TransitionTo(enterTime);
    }

    private void BGMAudioMixerStopRewind()
    {
        normalSnapshot.TransitionTo(exitTime);
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

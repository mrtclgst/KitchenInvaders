using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    [SerializeField] private StoveCounter _stoveCounter;
    [SerializeField] private float _warningSoundTimerMax;
    private AudioSource _audioSource;
    private float _warningSoundTimer;
    private bool _playWarningSound;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        _stoveCounter.Event_OnStateChanged += OnStateChanged;
        _stoveCounter.Event_OnProgressChanged += OnProgressChanged;
    }
    private void Update()
    {
        if (_playWarningSound)
        {
            _warningSoundTimer -= Time.deltaTime;
            if (_warningSoundTimer < 0)
            {
                SoundManager.Instance.PlayWarningSound(_stoveCounter.transform.position);
                _warningSoundTimer = _warningSoundTimerMax;
            }
        }
    }

    private void OnProgressChanged(object sender, IHasProgress.Event_OnProgressChangedArgs e)
    {
        float progressValueWillBeWarned = 0.5f;
        _playWarningSound = _stoveCounter.IsFrying() && e.ProgressNormalized > progressValueWillBeWarned;
    }

    private void OnStateChanged(object sender, StoveCounter.Event_OnStateChangedArgs e)
    {
        bool playSound = e.state is StoveCounter.State.Frying or StoveCounter.State.Burning;
        if (playSound)
        {
            _audioSource.Play();
        }
        else
        {
            _audioSource.Pause();
        }


    }

}

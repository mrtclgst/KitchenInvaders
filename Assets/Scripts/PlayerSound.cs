using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private AudioClipReferencesSO _audioClipReferencesSO;
    [SerializeField] private float _footstepSoundInterval = 0.1f;
    [SerializeField] private Player _player;

    private AudioSource _audioSource;
    private float _footstepSoundTimer;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        _footstepSoundTimer -= Time.deltaTime;
        if (_footstepSoundTimer < 0)
        {
            if (_player.IsWalking())
            {
                _audioSource.PlayOneShot(_audioClipReferencesSO.GetFootstepSounds()[Random.Range(0, _audioClipReferencesSO.GetFootstepSounds().Length)]);
                _footstepSoundTimer = _footstepSoundInterval;
            }
        }
    }
}

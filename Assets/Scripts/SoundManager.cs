using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private const string PLAYER_PREFS_SOUND_EFFECT_VOLUME_KEY = "SoundEffectVolume";

    [SerializeField] private AudioClipReferencesSO _audioClipReferencesSO;
    [SerializeField] private float _recipeSuccessVolume;
    [SerializeField] private float _recipeFailedVolume;

    private float _volume = 1f;

    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECT_VOLUME_KEY, 1);

        DeliveryManager.Instance.Event_OnRecipeSuccess += OnRecipeSuccess;
        DeliveryManager.Instance.Event_OnRecipeFailed += OnRecipeFailed;
        CuttingCounter.Event_OnAnyCut += OnAnyCut;

        BaseCounter.Event_OnAnyObjectPlacedHere += OnAnyObjectPlacedCounter;
        TrashCounter.Event_OnAnyObjectTrashed += OnAnyObjectTrashed;


        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.Event_OnPickedUp += OnPickedUp;
        }
        else
        {
            Player.Event_OnAnyPlayerSpawned += OnAnyPlayerSpawned;
        }

    }

    private void OnAnyPlayerSpawned(object sender, System.EventArgs e)
    {
        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.Event_OnPickedUp -= OnPickedUp;
            Player.LocalInstance.Event_OnPickedUp += OnPickedUp;
        }
    }

    private void OnAnyObjectTrashed(object sender, System.EventArgs e)
    {
        PlaySound(_audioClipReferencesSO.GetTrashSounds(), (sender as TrashCounter).transform.position);
    }

    private void OnAnyObjectPlacedCounter(object sender, System.EventArgs e)
    {
        PlaySound(_audioClipReferencesSO.GetObjectPutDownSounds(), (sender as BaseCounter).transform.position);
    }

    private void OnPickedUp(object sender, System.EventArgs e)
    {
        PlaySound(_audioClipReferencesSO.GetObjectPickUpSounds(), Player.LocalInstance.transform.position);
    }

    private void OnAnyCut(object sender, System.EventArgs e)
    {
        Vector3 soundPosition = (sender as CuttingCounter).transform.position;
        PlaySound(_audioClipReferencesSO.GetChopSounds(), soundPosition);
    }

    private void OnRecipeSuccess(object sender, System.EventArgs e)
    {
        PlaySound(_audioClipReferencesSO.GetDeliverySuccessSounds(), Camera.main.transform.position, _recipeSuccessVolume);
    }
    private void OnRecipeFailed(object sender, System.EventArgs e)
    {
        PlaySound(_audioClipReferencesSO.GetDeliveryFailSounds(), Camera.main.transform.position, _recipeFailedVolume);
    }

    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1)
    {
        PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volume);
    }
    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * _volume);
    }

    internal void PlayCountdownSound()
    {
        PlaySound(_audioClipReferencesSO.GetWarningSounds(), Vector3.zero);
    }
    internal void PlayWarningSound(Vector3 position)
    {
        PlaySound(_audioClipReferencesSO.GetWarningSounds(), position);
    }

    public void ChangeVolume()
    {
        _volume += 0.1f;
        if (_volume > 1f)
        {
            _volume = 0f;
        }

        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECT_VOLUME_KEY, _volume);
        PlayerPrefs.Save();
    }
    public float GetVolume()
    {
        return _volume;
    }
}

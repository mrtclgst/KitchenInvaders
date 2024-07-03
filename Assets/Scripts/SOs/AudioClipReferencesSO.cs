using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "KitchenInvaders", menuName = "ScriptableObjects/AudioClipReferencesSO")]
public class AudioClipReferencesSO : ScriptableObject
{
    [SerializeField] private AudioClip[] chop;
    [SerializeField] private AudioClip[] deliverySuccess;
    [SerializeField] private AudioClip[] deliveryFail;
    [SerializeField] private AudioClip[] footstep;
    [SerializeField] private AudioClip[] objectPickUp;
    [SerializeField] private AudioClip[] objectPutDown;
    [SerializeField] private AudioClip[] trash;
    [SerializeField] private AudioClip[] warning;
    [SerializeField] private AudioClip stoveSizzle;

    public AudioClip[] GetChopSounds()
    {
        return chop;
    }
    public AudioClip[] GetDeliveryFailSounds() { return deliveryFail; }
    public AudioClip[] GetDeliverySuccessSounds() { return deliverySuccess; }
    public AudioClip[] GetFootstepSounds() { return footstep; }
    public AudioClip[] GetObjectPickUpSounds() { return objectPickUp; }
    public AudioClip[] GetObjectPutDownSounds() { return objectPutDown; }
    public AudioClip[] GetTrashSounds() { return trash; }
    public AudioClip[] GetWarningSounds() { return warning; }
    public AudioClip GetStoveSizzleSound() { return stoveSizzle; }


}
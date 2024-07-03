using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    private const string ANIM_BOOL_WALKING = "IsWalking";
    private const string ANIM_TRIGGER_CHOPPING = "IsChopping";
    private const string ANIM_BOOL_HOLDING = "IsHolding";

    public Animator _animator;
    [SerializeField] public Player _player;


    //private void Awake()
    //{
    //    _animator = GetComponentInChildren<Animator>();
    //}
    private void OnEnable()
    {
        _player.Event_OnChopEvent += Player_OnSelectedCounterChanged;
    }
    private void OnDisable()
    {
    }
    private void Update()
    {
        if (!IsOwner)
            return;

        _animator.SetBool(ANIM_BOOL_WALKING, _player.IsWalking());
        _animator.SetBool(ANIM_BOOL_HOLDING, _player.IsHoldingKitchenObject());
    }
    private void Player_OnSelectedCounterChanged(object sender, EventArgs e)
    {
        Debug.Log("chop");
        _animator.SetTrigger(ANIM_TRIGGER_CHOPPING);
    }
}

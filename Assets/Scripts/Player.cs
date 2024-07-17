using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour, IKitchenObjectParent
{
    public static event EventHandler Event_OnAnyPlayerSpawned;

    public static void ResetStaticData()
    {
        Event_OnAnyPlayerSpawned = null;
    }

    public static Player LocalInstance { get; private set; }

    public event EventHandler<Event_OnSelectedCounterChangedEventArgs> Event_OnSelectedCounterChanged;
    public class Event_OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter SelectedCounter;
    }
    public event EventHandler Event_OnPickedUp;
    public event EventHandler Event_OnChopEvent;


    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private LayerMask _countersLayerMask;
    [SerializeField] private LayerMask _collisionsLayerMask;
    [SerializeField] private Transform _kitchenObjectHoldPoint;
    [SerializeField] private List<Vector3> _spawnPositionList;
    [SerializeField] private PlayerVisual _playerVisual;

    private bool _isWalking = false;
    private Vector3 _lastInteractionDir;
    private BaseCounter _selectedCounter;
    private KitchenObject _kitchenObject;

    private void Start()
    {
        GameInput.Instance.Event_OnInteractPerformed += OnInteractPerformed;
        GameInput.Instance.Event_OnInteractAlternatePerformed += OnInteractAlternatePerformed;

        PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        _playerVisual.SetPlayerVisual(playerData.characterVisualId);
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }

        transform.position = _spawnPositionList[KitchenGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
        Event_OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    private void Update()
    {
        if (!IsOwner)
            return;
        HandleMovement();
        //HandleMovementServerAuth();
        HandleInteractions();
    }
    private void HandleInteractions()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);
        if (moveDir != Vector3.zero)
        {
            _lastInteractionDir = moveDir;
        }
        float interactDistance = 1.5f;
        if (Physics.Raycast(transform.position, _lastInteractionDir, out RaycastHit hit, interactDistance, _countersLayerMask))
        {
            if (hit.transform.TryGetComponent(out BaseCounter counter))
            {
                //clearCounter.Interact();
                if (_selectedCounter != counter)
                {
                    SetSelectedCounter(counter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }
    // private void HandleMovementServerAuth()
    // {
    //     Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
    //     HandleMovementServerRpc(inputVector);
    // }

    // [ServerRpc(RequireOwnership = false)]
    // private void HandleMovementServerRpc(Vector2 inputVector)
    // {
    //     Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);
    //     Vector3 rotateDir = new Vector3(inputVector.x, 0, inputVector.y);

    //     //player'in bir sonraki framede kat edecegi mesafe
    //     float moveDistance = _moveSpeed * Time.deltaTime;
    //     float playerRadius = 0.7f;
    //     float playerHeight = 1.8f;         //capsule'un alt noktasi // ust noktasi
    //     bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

    //     if (!canMove)
    //     {
    //         //Attemp to move in x axis
    //         Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
    //         canMove = (Mathf.Abs(moveDirX.x) > 0.5f) &&
    //             !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

    //         if (canMove)
    //         {
    //             moveDir = moveDirX;
    //         }
    //         else
    //         {
    //             //Attemp to move in z axis
    //             Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
    //             canMove = (Mathf.Abs(moveDirX.z) > 0.5f) &&
    //                 !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);
    //             if (canMove)
    //             {
    //                 moveDir = moveDirZ;
    //             }
    //         }
    //     }

    //     if (canMove)
    //     {
    //         transform.position += moveDir * _moveSpeed * Time.deltaTime;
    //     }

    //     _isWalking = moveDir.magnitude > 0;

    //     float rotationSpeed = 10f;
    //     transform.forward = Vector3.Slerp(transform.forward, rotateDir, Time.deltaTime * rotationSpeed);
    // }
    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);
        Vector3 rotateDir = new Vector3(inputVector.x, 0, inputVector.y);

        //player'in bir sonraki framede kat edecegi mesafe
        float moveDistance = _moveSpeed * Time.deltaTime;
        float playerRadius = 0.5f;
        //float playerHeight = 1.8f;         //capsule'un alt noktasi // ust noktasi
        bool canMove = !Physics.BoxCast(
                    transform.position,
                    Vector3.one * playerRadius,
                    moveDir,
                    Quaternion.identity,
                    moveDistance,
                    _collisionsLayerMask);

        if (!canMove)
        {
            //Attemp to move in x axis
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = (Mathf.Abs(moveDirX.x) > 0.5f) &&
                !Physics.BoxCast(
                    transform.position,
                    Vector3.one * playerRadius,
                    moveDirX,
                    Quaternion.identity,
                    moveDistance,
                    _collisionsLayerMask);

            if (canMove)
            {
                moveDir = moveDirX;
            }
            else
            {
                //Attemp to move in z axis
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = (Mathf.Abs(moveDirZ.z) > 0.5f) &&
                    !Physics.BoxCast(
                    transform.position,
                    Vector3.one * playerRadius,
                    moveDirZ,
                    Quaternion.identity,
                    moveDistance,
                    _collisionsLayerMask);
                if (canMove)
                {
                    moveDir = moveDirZ;
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDir * _moveSpeed * Time.deltaTime;
        }

        _isWalking = moveDir.magnitude > 0;

        float rotationSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, rotateDir, Time.deltaTime * rotationSpeed);
    }
    private void OnInteractPerformed(object sender, EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying()) return;
        if (_selectedCounter != null)
        {
            _selectedCounter.Interact(this);
        }
    }
    private void OnInteractAlternatePerformed(object sender, EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying()) return;

        if (_selectedCounter != null)
        {
            _selectedCounter.InteractAlternate(this);
            //if (_selectedCounter.HasKitchenObject()
            //    && _selectedCounter is CuttingCounter)
            //{
            //    Event_OnChopEvent?.Invoke(this, EventArgs.Empty);
            //}
        }
    }
    private void NetworkManager_OnClientDisconnectCallback(ulong clientID)
    {
        if (clientID == OwnerClientId && HasKitchenObject())
        {
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
        }
    }
    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        _selectedCounter = selectedCounter;
        Event_OnSelectedCounterChanged?.Invoke(this,
            new Event_OnSelectedCounterChangedEventArgs { SelectedCounter = selectedCounter });

    }
    public bool IsWalking()
    {
        return _isWalking;
    }
    public bool IsHoldingKitchenObject()
    {
        return _kitchenObject != null;
    }
    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        _kitchenObject = kitchenObject;
        if (_kitchenObject != null)
        {
            Event_OnPickedUp?.Invoke(this, EventArgs.Empty);
        }
    }
    public KitchenObject GetKitchenObject()
    {
        return _kitchenObject;
    }
    public Transform GetCounterTopPoint()
    {
        return _kitchenObjectHoldPoint;
    }
    public bool HasKitchenObject()
    {
        return _kitchenObject != null;
    }
    public void ClearKitchenObject()
    {
        _kitchenObject = null;
    }
    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
    public Transform GetKitchenObjectFollowTransform()
    {
        return _kitchenObjectHoldPoint;
    }
}

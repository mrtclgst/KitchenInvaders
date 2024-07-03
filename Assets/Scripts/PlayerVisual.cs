using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private PlayerVisualContainerSO _playerVisualContainerSO;
    [SerializeField] private GameObject _playerVisualGO;
    [SerializeField] private PlayerAnimator _playerAnimator;

    //[SerializeField] private MeshRenderer _headMeshRenderer;
    //[SerializeField] private MeshRenderer _bodyMeshRenderer;

    //private Material _material;

    private void Awake()
    {
        //_material = new Material(_headMeshRenderer.material);
        //_headMeshRenderer.material = _material;
        //_bodyMeshRenderer.material = _material;
    }
    private void Start()
    {
    }

    //public void SetPlayerColor(Color color)
    //{
    //    _material.color = color;
    //}

    public void SetPlayerVisual(int characterIndex)
    {
        if (_playerVisualGO.transform.childCount > 0)
        {
            Destroy(_playerVisualGO.gameObject);
        }
        _playerVisualGO = 
            Instantiate(_playerVisualContainerSO.m_PlayerVisualList[characterIndex], transform);
        _playerVisualGO.transform.localPosition = Vector3.zero;
        _playerVisualGO.transform.localRotation = Quaternion.identity;
        if (_playerAnimator != null)
        {
           _playerAnimator._animator = _playerVisualGO.GetComponentInChildren<Animator>();
        }
    }
}
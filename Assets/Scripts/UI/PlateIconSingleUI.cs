using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlateIconSingleUI : MonoBehaviour
{
    [SerializeField] private Image _iconImage;

    public void SetKitchenObjectSO(KitchenObjectSO kitchenObjectSO)
    {
        _iconImage.sprite = kitchenObjectSO.GetIcon();
    }
}

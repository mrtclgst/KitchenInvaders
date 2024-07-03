using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _recipeNameText;
    [SerializeField] private Transform _iconContainer;
    [SerializeField] private Transform _iconTemplate;
    private void Awake()
    {
        _iconTemplate.gameObject.SetActive(false);
    }

    public void SetRecipeUI(RecipeSO recipeSO)
    {
        _recipeNameText.text = recipeSO.GetRecipeName();
        foreach (Transform child in _iconContainer)
        {
            if (child == _iconTemplate)
                continue;

            Destroy(child.gameObject);
        }

        foreach (KitchenObjectSO kitchenObjectSO in recipeSO.GetKitchenObjectSOList())
        {
            Transform iconUI = Instantiate(_iconTemplate, _iconContainer);
            iconUI.gameObject.SetActive(true);
            iconUI.GetComponent<Image>().sprite = kitchenObjectSO.GetIcon();
        }
    }

}

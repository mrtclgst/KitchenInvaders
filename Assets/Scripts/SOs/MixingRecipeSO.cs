using UnityEngine;

[CreateAssetMenu(fileName = "KitchenInvaders", menuName = "ScriptableObjects/MixingRecipeSO", order = 1)]
public class MixingRecipeSO : ScriptableObject
{
    [SerializeField] private KitchenObjectSO _input;
    [SerializeField] private KitchenObjectSO _output;
    [SerializeField] private float _mixingTimeMax;

    internal float GetFryingTimeMax()
    {
        return _mixingTimeMax;
    }

    internal KitchenObjectSO GetInput()
    {
        return _input;
    }

    internal KitchenObjectSO GetOutput()
    {
        return _output;
    }
}
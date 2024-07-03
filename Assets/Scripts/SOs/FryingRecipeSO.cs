using UnityEngine;

[CreateAssetMenu(fileName = "KitchenInvaders", menuName = "ScriptableObjects/FryingRecipeSO", order = 1)]
public class FryingRecipeSO : ScriptableObject
{
    [SerializeField] private KitchenObjectSO _input;
    [SerializeField] private KitchenObjectSO _output;
    [SerializeField] private float _fryingTimeMax;

    public KitchenObjectSO GetInput()
    {
        return _input;
    }
    public KitchenObjectSO GetOutput()
    {
        return _output;
    }
    public float GetFryingTimeMax()
    {
        return _fryingTimeMax;
    }
}
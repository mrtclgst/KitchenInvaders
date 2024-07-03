using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerVisualContainer", menuName = "ScriptableObjects/PlayerVisualContainer", order = 1)]
public class PlayerVisualContainerSO : ScriptableObject
{
    public List<GameObject> m_PlayerVisualList;
}
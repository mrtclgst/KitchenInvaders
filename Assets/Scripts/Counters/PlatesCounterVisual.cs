using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{
    [SerializeField] private Transform _counterTopPoint;
    [SerializeField] private Transform _plateVisualPrefab;
    [SerializeField] private PlatesCounter _platesCounter;

    private List<Transform> _spawnedPlatesList;

    private void Awake()
    {
        _spawnedPlatesList = new List<Transform>();
    }

    private void Start()
    {
        _platesCounter.Event_OnPlateSpawned += OnPlateSpawned;
        _platesCounter.Event_OnPlateTaken += OnPlateTaken;

    }

    private void OnPlateSpawned(object sender, EventArgs e)
    {
        Transform plateVisualTransform = Instantiate(_plateVisualPrefab, _counterTopPoint);

        float plateOffsetY = 0.1f * _spawnedPlatesList.Count;
        plateVisualTransform.localPosition = new Vector3(0, plateOffsetY, 0);
        _spawnedPlatesList.Add(plateVisualTransform);
    }
    private void OnPlateTaken(object sender, EventArgs e)
    {
        Transform lastPlateVisual = _spawnedPlatesList[^1];
        _spawnedPlatesList.Remove(lastPlateVisual);
        Destroy(lastPlateVisual.gameObject);
    }
}
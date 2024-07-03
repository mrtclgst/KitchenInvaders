using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private GameObject _hasProgressGameObject;
    [SerializeField] private Image _progressBarImage;
    private IHasProgress _progressable;

    private void Start()
    {
        _progressable = _hasProgressGameObject.GetComponent<IHasProgress>();
        if (_progressable == null )
        {
            Debug.LogError("There is no IHasProgress at " + _hasProgressGameObject);
        }
        _progressable.Event_OnProgressChanged += OnProgressChanged;
        _progressBarImage.fillAmount = 0;
        Hide();
    }

    private void OnProgressChanged(object sender, IHasProgress.Event_OnProgressChangedArgs e)
    {
        _progressBarImage.fillAmount = e.ProgressNormalized;
        if (e.ProgressNormalized is 0 or 1)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
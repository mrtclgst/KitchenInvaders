using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveBurnWarningUI : MonoBehaviour
{
    [SerializeField] private StoveCounter _stoveCounter;

    private void Start()
    {
        _stoveCounter.Event_OnProgressChanged += OnProgressChanged;
        Hide();
    }

    private void OnProgressChanged(object sender, IHasProgress.Event_OnProgressChangedArgs e)
    {
        float progressValueWillBeWarned = 0.5f;
        bool show = _stoveCounter.IsFrying() && e.ProgressNormalized > progressValueWillBeWarned;

        if (show)
        {
            Show();
        }
        else
        {
            Hide();
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

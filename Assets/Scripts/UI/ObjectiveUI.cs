using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveUI : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);

        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;

    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        UpdateVisibility();
    }

    private void UpdateVisibility() {
        gameObject.SetActive(
            KitchenGameManager.Instance.IsGamePlaying());
    }

    private void OnDestroy()
    {
        if (KitchenGameManager.Instance != null) { 
        KitchenGameManager.Instance.OnStateChanged -= KitchenGameManager_OnStateChanged;
        }
    }
}

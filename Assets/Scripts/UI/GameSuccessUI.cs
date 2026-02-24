using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSuccessUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI recipesDeliveredText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;

    private void Awake()
    {
        retryButton.onClick.AddListener(() => {
            Time.timeScale = 1f; 
            Loader.Load(Loader.Scene.GameScene);
        });
        mainMenuButton.onClick.AddListener(() => {
            Time.timeScale = 1f; 
            Loader.Load(Loader.Scene.MainMenuScene);

        });
    }
    private void Start()
    {
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
        Hide();
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (KitchenGameManager.Instance.IsGameWon())
        {
            Show();
        }
        else {           
            Hide(); 
        }
    }

    private void Show() { 
    gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}

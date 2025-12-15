using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button buttonPlay;
    [SerializeField] private Button buttonSetting;
    [SerializeField] private Button buttonGuide;
    [SerializeField] private Button buttonShop;

    [SerializeField] private UIPopupGuide uIPopupGuide;
    [SerializeField] private UIPopupSetting uIPopupSetting;
    [SerializeField] private UIPopupShop uIPopupShop;
    [SerializeField] private Button buttonQuit;

    private void Start()
    {
        // Phát nhạc nền menu
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(AudioType.MenuMusic, fadeIn: true, fadeDuration: 1f);
        }

        InitializeButtons();
    }

    private void InitializeButtons()
    {
        if (buttonPlay != null)
        {
            buttonPlay.onClick.AddListener(OnPlayClicked);
        }

        if (buttonSetting != null)
        {
            buttonSetting.onClick.AddListener(OnSettingClicked);
        }

        if (buttonShop != null)
        {
            buttonShop.onClick.AddListener(OnShopClicked);
        }

        if (buttonGuide != null)
        {
            buttonGuide.onClick.AddListener(OnGuideClicked);
        }

        if (buttonQuit != null)
            buttonQuit.onClick.AddListener(OnQuitClicked);
    }

    private void OnPlayClicked()
    {
        AudioManager.Instance?.PlayOneShot(AudioType.UITap);
        EventBus.Publish(new OnPlayClicked());
        // load game scene or start game
        SceneManager.LoadScene(1);
    }

    private void OnSettingClicked()
    {
        AudioManager.Instance?.PlayOneShot(AudioType.UITap);
        EventBus.Publish(new OnSettingClicked());
        uIPopupSetting.Show();
    }

    private void OnShopClicked()
    {
        AudioManager.Instance?.PlayOneShot(AudioType.UITap);
        EventBus.Publish(new OnShopClicked());
        uIPopupShop.Show();
    }

    private void OnGuideClicked()
    {
        AudioManager.Instance?.PlayOneShot(AudioType.UITap);
        EventBus.Publish(new OnGuideClicked());
        uIPopupGuide.Show();
    }

    private void OnQuitClicked()
    {
        AudioManager.Instance?.PlayOneShot(AudioType.UITap);
        EventBus.Publish(new OnQuitClicked());

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Thoát Play Mode khi test trong Editor
#else
    Application.Quit(); // Thoát game khi build
#endif
    }
}

public struct OnPlayClicked { }
public struct OnSettingClicked { }
public struct OnShopClicked { }
public struct OnGuideClicked { }
public struct OnQuitClicked { }


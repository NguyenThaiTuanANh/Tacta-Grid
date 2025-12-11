using UnityEngine;
using UnityEngine.UI;

public class UIInGame : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button buttonPause;

    [SerializeField] private UIPauseMenu uIPauseMenu;
    public static UIInGame Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    void Start()
    {
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        if (buttonPause != null)
        {
            buttonPause.onClick.AddListener(OnPauseClicked);
        }
    }

    private void OnPauseClicked()
    {
        AudioManager.Instance?.PlayOneShot(AudioType.UITap);
        EventBus.Publish(new OnPauseClicked());
        uIPauseMenu.Show();
    }



    void Update()
    {

    }
}

public struct OnPauseClicked { }

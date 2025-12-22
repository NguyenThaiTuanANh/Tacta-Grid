using UnityEngine;

public class BonusManager : MonoBehaviour
{
    public static BonusManager Instance;
    [SerializeField] private Base playerBase;
    public BonusSpawner bonusSpawner;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Apply(BonusType type)
    {
        switch (type)
        {
            case BonusType.Heal:
                Heal();
                break;

            case BonusType.Shield:
                Shield();
                break;

            case BonusType.DoubleScore:
                DoubleScore();
                break;

            case BonusType.SlowTime:
                SlowTime();
                break;
        }
    }

    void Heal()
    {
        Debug.Log("❤️ Heal Player");
        playerBase.Heal(5);
    }

    void Shield()
    {
        Debug.Log("🛡 Shield ON");
        // Player.Instance.EnableShield(5f);
    }

    void DoubleScore()
    {
        Debug.Log("⭐ Double Score");
        // ScoreManager.Instance.DoubleScore(10f);
    }

    void SlowTime()
    {
        Debug.Log("⏳ Slow Time");
        Time.timeScale = 0.5f;
        Invoke(nameof(ResetTime), 3f);
    }

    void ResetTime()
    {
        Time.timeScale = 1f;
    }
}

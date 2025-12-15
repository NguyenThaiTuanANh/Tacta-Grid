using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// Thành - có HP, game over khi HP = 0
/// </summary>
public class Base : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Visual")]
    [SerializeField] private TMPro.TextMeshProUGUI healthText; // 3D text
    [SerializeField] private Slider healthSlider; // UI Slider để hiển thị thanh máu
                                                  // Health bar có thể dùng 3D Canvas hoặc Billboard

    [Header("Events")]
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent OnBaseDestroyed;

    private Color originalColor;
    private Coroutine blinkCoroutine;
    [SerializeField] private Image healthFill;
    private bool gameOver = false;

    private void Start()
    {
        currentHealth = maxHealth;

        // Khởi tạo slider nếu có
        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }
        originalColor = healthFill.color;

        UpdateHealthDisplay();
    }

    /// <summary>
    /// Nhấp nháy xanh lá
    /// </summary>
    /// <param name="duration">Tổng thời gian nhấp nháy</param>
    /// <param name="speed">Tốc độ nhấp nháy</param>
    public void BlinkGreen(float duration = 1f, float speed = 6f)
    {
        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);

        blinkCoroutine = StartCoroutine(BlinkGreenRoutine(duration, speed));
    }

    private IEnumerator BlinkGreenRoutine(float duration, float speed)
    {
        float time = 0f;
        Color green = Color.green;

        while (time < duration)
        {
            float t = Mathf.PingPong(Time.time * speed, 1f);
            healthFill.color = Color.Lerp(originalColor, green, t);

            time += Time.deltaTime;
            yield return null;
        }

        healthFill.color = originalColor;
        blinkCoroutine = null;
    }

    /// <summary>
    /// Nhận sát thương
    /// </summary>
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        // Phát âm thanh BaseBroken khi base bị tấn công
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayOneShot(AudioType.BaseBroken);
        }

        UpdateHealthDisplay();
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0 && gameOver == false)
        {
            gameOver = true;
            OnBaseDestroyed?.Invoke();
        }
    }

    /// <summary>
    /// Hồi máu (nếu cần)
    /// </summary>
    public void Heal(float amount)
    {
        BlinkGreen(1.5f, 8f);
        currentHealth += amount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
        UpdateHealthDisplay();
        OnHealthChanged?.Invoke(currentHealth);
    }

    /// <summary>
    /// Cập nhật hiển thị máu (3D)
    /// </summary>
    private void UpdateHealthDisplay()
    {
        // Cập nhật text
        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(currentHealth)}/{Mathf.CeilToInt(maxHealth)}";
        }

        // Cập nhật slider
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    /// <summary>
    /// Kiểm tra còn sống không
    /// </summary>
    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    /// <summary>
    /// Lấy HP hiện tại
    /// </summary>
    public float GetCurrentHealth() => currentHealth;

    /// <summary>
    /// Lấy HP tối đa
    /// </summary>
    public float GetMaxHealth() => maxHealth;

    /// <summary>
    /// Set HP (dùng khi khởi tạo game với độ khó khác nhau)
    /// </summary>
    public void SetMaxHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = maxHealth;

        // Cập nhật slider max value
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
        }

        UpdateHealthDisplay();
    }
}


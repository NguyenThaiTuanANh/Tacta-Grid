using UnityEngine;
using UnityEngine.UI;

public class BGScaleLoading : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private RectTransform bgImage;

    [Header("Scale Settings")]
    [SerializeField] private Vector3 startScale = new Vector3(1f, 1f, 1f);
    [SerializeField] private Vector3 endScale = new Vector3(1.2f, 1.2f, 1f);
    [SerializeField] private float duration = 3f;

    private float _timer = 0f;

    void Start()
    {
        if (bgImage != null)
            bgImage.localScale = startScale;
    }

    void Update()
    {
        if (_timer < duration)
        {
            _timer += Time.deltaTime;
            float t = Mathf.Clamp01(_timer / duration);

            bgImage.localScale = Vector3.Lerp(startScale, endScale, t);
        }
    }
}

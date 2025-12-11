using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quản lý hệ thống hướng dẫn chơi game - hiển thị hint animation
/// Đảm bảo chỉ hiện 1 hint tại một thời điểm và mỗi hint chỉ hiện 1 lần mỗi session
/// Animation sẽ lặp lại cho đến khi người chơi thực hiện thao tác
/// </summary>
public class TutorialHintManager : MonoBehaviour
{
    public static TutorialHintManager Instance { get; private set; }

    [Header("Hint Prefab")]
    [SerializeField] private GameObject hintDotPrefab;

    [Header("Settings")]
    [SerializeField] private float hintDuration = 2f;

    public event Action OnHintCompleted;

    // Enum các loại hint
    public enum HintType
    {
        TetrisBlockPlacement,
        BlockExpandGridPlacement 
    }

    private HashSet<HintType> shownHints = new HashSet<HintType>();

    private GameObject currentHintObject = null;
    private TutorialHintAnimation currentHintAnimation = null;
    private bool isShowingHint = false;

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

    private void Update()
    {
        if (isShowingHint && currentHintAnimation != null)
        {
            if (DetectPlayerInput())
            {
                StopCurrentHint();
            }
        }
    }

    /// <summary>
    /// Phát hiện bất kỳ input nào từ người chơi
    /// </summary>
    private bool DetectPlayerInput()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            return true;
        }

        if (Input.touchCount > 0)
        {
            return true;
        }

        if (Input.anyKeyDown)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Dừng hint hiện tại
    /// </summary>
    private void StopCurrentHint()
    {
        if (currentHintAnimation != null)
        {
            currentHintAnimation.StopAnimation();
        }

        OnHintComplete();
    }

    /// <summary>
    /// Hiển thị hint cho tetris block - từ block đến vị trí hợp lệ gần nhất
    /// </summary>
    /// <param name="fromWorldPos">Vị trí bắt đầu (world position của tetris block)</param>
    /// <param name="toWorldPos">Vị trí kết thúc (vị trí hợp lệ gần nhất trên grid)</param>
    public void ShowTetrisBlockHint(Vector3 fromWorldPos, Vector3 toWorldPos)
    {
        if (shownHints.Contains(HintType.TetrisBlockPlacement))
        {
            return;
        }

        if (isShowingHint)
        {
            return;
        }

        shownHints.Add(HintType.TetrisBlockPlacement);

        StartHint(fromWorldPos, toWorldPos);
    }

    /// <summary>
    /// Hiển thị hint cho block expand grid - từ block đến vị trí đất liền gần nhất
    /// </summary>
    /// <param name="fromWorldPos">Vị trí bắt đầu (world position của block expand grid)</param>
    /// <param name="toWorldPos">Vị trí kết thúc (đất liền gần nhất)</param>
    public void ShowBlockExpandGridHint(Vector3 fromWorldPos, Vector3 toWorldPos)
    {
        if (shownHints.Contains(HintType.BlockExpandGridPlacement))
        {
            return;
        }

        if (isShowingHint)
        {
            return;
        }

        shownHints.Add(HintType.BlockExpandGridPlacement);

        // Spawn hint
        StartHint(fromWorldPos, toWorldPos);
    }

    /// <summary>
    /// Bắt đầu hiển thị hint animation
    /// </summary>
    private void StartHint(Vector3 fromWorldPos, Vector3 toWorldPos)
    {
        if (hintDotPrefab == null)
        {
            return;
        }

        isShowingHint = true;

        currentHintObject = Instantiate(hintDotPrefab);

        currentHintAnimation = currentHintObject.GetComponent<TutorialHintAnimation>();
        if (currentHintAnimation != null)
        {
            currentHintAnimation.Initialize(fromWorldPos, toWorldPos, hintDuration, OnHintComplete);
        }
        else
        {
            Destroy(currentHintObject);
            currentHintObject = null;
            currentHintAnimation = null;
            isShowingHint = false;
        }
    }

    /// <summary>
    /// Callback khi hint animation hoàn thành
    /// </summary>
    private void OnHintComplete()
    {
        if (currentHintObject != null)
        {
            Destroy(currentHintObject);
            currentHintObject = null;
        }

        currentHintAnimation = null;
        isShowingHint = false;

        OnHintCompleted?.Invoke();
    }

    /// <summary>
    /// Reset tất cả hints - dùng khi bắt đầu game mới
    /// </summary>
    public void ResetAllHints()
    {
        shownHints.Clear();

        if (currentHintAnimation != null)
        {
            currentHintAnimation.StopAnimation();
        }

        if (currentHintObject != null)
        {
            Destroy(currentHintObject);
            currentHintObject = null;
        }

        currentHintAnimation = null;
        isShowingHint = false;
    }

    /// <summary>
    /// Kiểm tra hint đã được hiển thị chưa
    /// </summary>
    public bool HasShownHint(HintType hintType)
    {
        return shownHints.Contains(hintType);
    }

    /// <summary>
    /// Kiểm tra có đang hiển thị hint không
    /// </summary>
    public bool IsShowingHint()
    {
        return isShowingHint;
    }
}

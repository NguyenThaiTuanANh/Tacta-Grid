using UnityEngine;

/// <summary>
/// Block mở rộng grid - có thể kéo thả vào cạnh grid để mở rộng (3D)
/// </summary>
public class BlockExpandGrid : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private TMPro.TextMeshPro expandText; // 3D text
    [SerializeField] private ParticleSystem particleEffectPrefab; // Particle effect khi sử dụng

    [Header("Input")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask gridLayerMask = -1;
    [SerializeField] private LayerMask nonGridLayerMask = -1; // Layer mask cho cellNonGrid

    private Vector3 originalPosition;
    private bool isDragging = false;
    private bool isUsed = false;
    private Plane dragPlane;
    public CellNonGrid nonGrid;
    private Grid _grid;
    private CellNonGrid _currentHighlight;
    private LayerMask _combinedMask;
    private Vector3 dragScreenOffset;


    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        dragPlane = new Plane(Vector3.up, transform.position);
        _grid = FindFirstObjectByType<Grid>();
        _combinedMask = gridLayerMask | nonGridLayerMask;
    }

    private void Start()
    {
        originalPosition = transform.position;
        if (expandText != null)
        {
            expandText.text = "Expand";
        }
    }

    private void Update()
    {
        if (isUsed) return;

        HandleInput();
    }

    private void HandleInput()
    {
        // Mouse input
        if (Input.GetMouseButtonDown(0))
        {
            OnPointerDown();
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            OnDrag();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnPointerUp();
        }
    }

    private void OnPointerDown()
    {
        if (isUsed) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
        {
            isDragging = true;
            originalPosition = transform.position;

            dragPlane = new Plane(-mainCamera.transform.forward, transform.position);

            // ✅ Tính offset
            Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position - transform.right - transform.up);
            dragScreenOffset = Input.mousePosition - screenPos;
        }
    }


    private void OnDrag()
    {
        if (isUsed || !isDragging) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition - dragScreenOffset);

        if (dragPlane.Raycast(ray, out float distance))
        {
            Debug.Log("hehe");
            transform.position = ray.GetPoint(distance);
            UpdateHighlight(ray);
        }
    }


    private void UpdateHighlight(Ray ray)
    {
        if (_grid == null) return;

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _combinedMask))
        {
            CellNonGrid cell = _grid.GetNonGridCellAtPosition(hit.point);

            if (cell != null && cell != _currentHighlight)
            {
                ClearHighlight();
                _currentHighlight = cell;
                var (gridIndex, cellPos) = _grid.WorldToGridPositionWithIndex(cell.transform.position);
                _currentHighlight.SetHighlight(_grid.IsAdjacent(gridIndex, cellPos));
            }
            else if (cell == null)
            {
                ClearHighlight();
            }
        }
        else
        {
            ClearHighlight();
        }
    }
    private void ClearHighlight()
    {
        if (_currentHighlight != null)
        {
            _currentHighlight.SetNormal();
            _currentHighlight = null;
        }
    }

    private void OnPointerUp()
    {
        if (isUsed || !isDragging) return;

        isDragging = false;

        if (_currentHighlight == null)
        {
            transform.position = originalPosition;
            return;
        }

        Vector3 pos = _currentHighlight.transform.position;

        ClearHighlight();

        if (_grid.ExpandGridAtWorldPosition(pos, 1))
        {
            AudioManager.Instance?.PlayOneShot(AudioType.Explosion);
            SpawnParticleEffect(pos);

            isUsed = true;
            Destroy(gameObject);
            return;
        }

        transform.position = originalPosition;
    }

    /// <summary>
    /// Spawn particle effect tại vị trí sử dụng block
    /// </summary>
    private void SpawnParticleEffect(Vector3 position)
    {
        if (particleEffectPrefab != null)
        {
            // Tạo particle tại vị trí
            ParticleSystem particleInstance = Instantiate(particleEffectPrefab, position, Quaternion.identity);

            // Disable looping để đảm bảo chỉ chạy 1 lần
            var main = particleInstance.main;
            main.loop = false;

            // Play particle
            particleInstance.Play();

            // Tự động destroy sau khi particle chạy xong (duration only, không cần thêm lifetime)
            Destroy(particleInstance.gameObject, particleInstance.main.duration);
        }
    }

    /// <summary>
    /// Kiểm tra đã được sử dụng chưa
    /// </summary>
    public bool IsUsed() => isUsed;
}


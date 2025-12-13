using UnityEngine;

public class CellNonGrid : MonoBehaviour
{
    [SerializeField] private Material cubeMaterialNormal;
    [SerializeField] private Material cubeMaterialVaildHighlight;
    [SerializeField] private Material cubeMaterialInVaildHighlight;
    [SerializeField] private Renderer cubeRenderer;

    private bool _isHighlight;
    private bool _isValid;

    void Awake()
    {
        SetNormal();
    }

    public void SetHighlight(bool isValid)
    {
        
        _isHighlight = true;
        _isValid = isValid;
        if (_isValid)
        {
            cubeRenderer.material = cubeMaterialVaildHighlight;
        }
        else
        {
            cubeRenderer.material = cubeMaterialInVaildHighlight;
        }
    }

    public void SetNormal()
    {
        cubeRenderer.material = cubeMaterialNormal;
        _isHighlight = false;
        _isValid = false;
    }
}

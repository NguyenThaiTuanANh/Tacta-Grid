using UnityEngine;

public enum BonusType
{
    Heal,
    Shield,
    DoubleScore,
    SlowTime
}


public class BonusItem : MonoBehaviour
{
    [Header("Config")]
    public BonusType bonusType;
    public float speed = 2f;
    public float lifeTime = 10f;
    public GameObject vfxHit;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        MoveDown();
        HandleClick();
    }

    void MoveDown()
    {
        // Trôi theo hướng DOWN của camera (ngược up)
        Vector3 moveDir = -mainCamera.transform.up;
        transform.position += moveDir * speed * Time.deltaTime;
    }

    void HandleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    ApplyBonus();
                }
            }
        }
    }

    void ApplyBonus()
    {
        BonusManager.Instance.Apply(bonusType);

        // Hiệu ứng click (optional)
        // Instantiate(pickupVFX, transform.position, Quaternion.identity);
        if (vfxHit != null)
        {
            var flashInstance = Instantiate(vfxHit, transform.position, Quaternion.identity);
            //flashInstance.transform.forward = gameObject.transform.forward;

            // Hủy flash sau khi hiệu ứng hoàn thành
            var flashPs = flashInstance.GetComponent<ParticleSystem>();
            if (flashPs != null)
            {
                Destroy(flashInstance, flashPs.main.duration);
            }
            else
            {
                var flashPsParts = flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(flashInstance, flashPsParts.main.duration);
            }
        }

        Destroy(gameObject);
    }
}

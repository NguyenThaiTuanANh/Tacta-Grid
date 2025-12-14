using UnityEngine;

public class BonusSpawner : MonoBehaviour
{
    public GameObject[] bonusPrefabs;
    public float spawnInterval = 3f;
    public float spawnDistance = 100f;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    public void Spawn()
    {
        Vector3 spawnPos =
            cam.transform.position +
            cam.transform.forward * (spawnDistance + Random.Range(-5, 5)) +
            cam.transform.up * 3f +
            cam.transform.right * Random.Range(-3f, 3f);

        Instantiate(
            bonusPrefabs[Random.Range(0, bonusPrefabs.Length)],
            spawnPos,
            Quaternion.identity
        );
        Debug.Log("aha");
    }
}

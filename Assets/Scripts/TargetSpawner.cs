using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetSpawner : MonoBehaviour
{
    [Header("Configuracion General")]
    public GameObject targetPrefab;
    public float spawnInterval = 1.5f;
    public float minDistance = 2f;
    public float maxDistance = 5f;
    public float targetRadius = 0.5f;
    public int maxSpawnAttempts = 10;

    [Header("Configuracion de Vida")]
    public float minLifeTime = 2f;
    public float maxLifeTime = 4f;

    [Header("Configuracion de Movimiento")]
    [Range(0f, 1f)]
    public float movementChance = 0.5f;
    public float minSpeed = 2f;
    public float maxSpeed = 5f;
    public float minSwayDistance = 0.1f;
    public float maxSwayDistance = 0.4f;

    private Camera playerCamera;
    private List<MovingTargetData> activeMovingTargets = new List<MovingTargetData>();

    private class MovingTargetData
    {
        public GameObject targetObject;
        public Vector3 startPosition;
        public Vector3 rightVector;
        public float speed;
        public float distance;
        public float creationTime;
    }

    void Start()
    {
        playerCamera = Camera.main; 
        if (playerCamera == null) playerCamera = FindObjectOfType<Camera>();
        StartCoroutine(SpawnTargetsRoutine());
    }

    IEnumerator SpawnTargetsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnTarget();
        }
    }

    void SpawnTarget()
    {
        if (targetPrefab == null || playerCamera == null) return;

        Vector3 spawnPosition = Vector3.zero;
        bool positionFound = false;

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            float randomX = Random.Range(0.2f, 0.8f);
            float randomY = Random.Range(0.2f, 0.8f);
            float randomZ = Random.Range(minDistance, maxDistance);
            spawnPosition = playerCamera.ViewportToWorldPoint(new Vector3(randomX, randomY, randomZ));

            if (!Physics.CheckSphere(spawnPosition, targetRadius))
            {
                positionFound = true;
                break;
            }
        }

        if (!positionFound) return;

        GameObject newTargetObj = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);
        newTargetObj.transform.LookAt(playerCamera.transform);

        float lifeTime = Random.Range(minLifeTime, maxLifeTime);
        Destroy(newTargetObj, lifeTime);

        if (Random.value < movementChance)
        {
            MovingTargetData newData = new MovingTargetData();
            newData.targetObject = newTargetObj;
            newData.startPosition = spawnPosition;
            newData.rightVector = newTargetObj.transform.right;
            newData.speed = Random.Range(minSpeed, maxSpeed);
            newData.distance = Random.Range(minSwayDistance, maxSwayDistance);
            newData.creationTime = Time.time;

            activeMovingTargets.Add(newData);
        }
    }

    void Update()
    {
        for (int i = activeMovingTargets.Count - 1; i >= 0; i--)
        {
            MovingTargetData data = activeMovingTargets[i];

            if (data.targetObject == null)
            {
                activeMovingTargets.RemoveAt(i);
                continue;
            }

            float timeAlive = Time.time - data.creationTime;
            Vector3 offset = data.rightVector * Mathf.Sin(timeAlive * data.speed) * data.distance;
            
            data.targetObject.transform.position = data.startPosition + offset;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private Player player;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private EnemyScaling scaling;
    [SerializeField] private Camera gameCamera;
    [SerializeField] private SpriteRenderer floor;
    [SerializeField, Min(0.1f)] private float spawnInterval = 2f;
    [SerializeField, Min(1)] private int maxEnemies = 20;
    [SerializeField, Min(0.1f)] private float spawnDistance = 1f;
    [SerializeField, Min(0.1f)] private float spawnBandWidth = 2f;

    private bool isSpawning;
    private float spawnTimer;

    private void Update()
    {
        if (Time.timeScale <= 0f || !isSpawning || player == null || player.CurrentHealth <= 0)
        {
            return;
        }

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }
    }

    public void StartSpawning()
    {
        isSpawning = true;
        spawnTimer = spawnInterval;
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    public Enemy SpawnEnemy()
    {
        if (Time.timeScale <= 0f || !isSpawning || player == null || player.CurrentHealth <= 0)
        {
            return null;
        }

        if (enemyPrefab == null || gameCamera == null || floor == null || gameManager == null || scaling == null)
        {
            return null;
        }

        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude);

        if (enemies.Length >= maxEnemies)
        {
            return null;
        }

        Vector2 spawnPosition;

        if (!TrySelectSpawnPosition(out spawnPosition))
        {
            return null;
        }

        float elapsedTime = gameManager.ElapsedTime;
        int health = scaling.CalculateHealth(enemyPrefab.BaseHealth, elapsedTime);
        int damage = scaling.CalculateDamage(enemyPrefab.BaseDamage, elapsedTime);

        Enemy enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        enemy.Initialize(player.transform, health, damage);
        return enemy;
    }

    private bool TrySelectSpawnPosition(out Vector2 spawnPosition)
    {
        spawnPosition = Vector2.zero;

        CircleCollider2D enemyCollider = enemyPrefab.GetComponent<CircleCollider2D>();
        Vector3 enemyScale = enemyPrefab.transform.lossyScale;
        float largestScale = Mathf.Max(Mathf.Abs(enemyScale.x), Mathf.Abs(enemyScale.y));
        float enemyRadius = enemyCollider.radius * largestScale;
        float floorInset = enemyRadius + 0.05f;

        Bounds floorBounds = floor.bounds;
        Vector3 cameraPosition = gameCamera.transform.position;
        float halfCameraHeight = gameCamera.orthographicSize;
        float halfCameraWidth = halfCameraHeight * gameCamera.aspect;
        float viewMargin = enemyRadius + spawnDistance;

        float left = cameraPosition.x - halfCameraWidth - viewMargin;
        float right = cameraPosition.x + halfCameraWidth + viewMargin;
        float bottom = cameraPosition.y - halfCameraHeight - viewMargin;
        float top = cameraPosition.y + halfCameraHeight + viewMargin;

        List<Rect> spawnAreas = new List<Rect>();
        AddSpawnArea(spawnAreas, left - spawnBandWidth, bottom, left, top, floorBounds, floorInset);
        AddSpawnArea(spawnAreas, right, bottom, right + spawnBandWidth, top, floorBounds, floorInset);
        AddSpawnArea(spawnAreas, left, bottom - spawnBandWidth, right, bottom, floorBounds, floorInset);
        AddSpawnArea(spawnAreas, left, top, right, top + spawnBandWidth, floorBounds, floorInset);

        if (spawnAreas.Count == 0)
        {
            return false;
        }

        int areaIndex = Random.Range(0, spawnAreas.Count);
        Rect selectedArea = spawnAreas[areaIndex];
        float spawnX = Random.Range(selectedArea.xMin, selectedArea.xMax);
        float spawnY = Random.Range(selectedArea.yMin, selectedArea.yMax);
        spawnPosition = new Vector2(spawnX, spawnY);
        return true;
    }

    private void AddSpawnArea(List<Rect> spawnAreas, float minX, float minY, float maxX, float maxY,
        Bounds floorBounds, float floorInset)
    {
        minX = Mathf.Max(minX, floorBounds.min.x + floorInset);
        minY = Mathf.Max(minY, floorBounds.min.y + floorInset);
        maxX = Mathf.Min(maxX, floorBounds.max.x - floorInset);
        maxY = Mathf.Min(maxY, floorBounds.max.y - floorInset);

        if (minX < maxX && minY < maxY)
        {
            Rect spawnArea = Rect.MinMaxRect(minX, minY, maxX, maxY);
            spawnAreas.Add(spawnArea);
        }
    }
}

using UnityEngine;

[DefaultExecutionOrder(-50)]
[DisallowMultipleComponent]
public sealed class CollectibleSpawner: MonoBehaviour {
  [Header("Collectible")]
  [SerializeField]
  private Collectible collectiblePrefab;

  [SerializeField]
  private Collectible rareCollectiblePrefab;

  [SerializeField]
  [Range(0f,1f)]
  private float rareSpawnChance = 0.25f;

  [Header("Points d'apparition")]
  [SerializeField]
  private Transform[] spawnPoints;

  private void Awake() {
    if (collectiblePrefab == null) {
      Debug.LogError("CollectibleSpawner : aucun prefab de collectible n'est renseigné.",this);
      enabled = false;
      return;
    }

    foreach (Transform spawnPoint in spawnPoints) {
      if (spawnPoint == null) {
        continue;
      }
      Collectible prefabToSpawn = collectiblePrefab;

      if (rareCollectiblePrefab != null &&
          Random.value < rareSpawnChance) {
        prefabToSpawn = rareCollectiblePrefab;
      }
      Instantiate(prefabToSpawn,spawnPoint.position,spawnPoint.rotation,transform);
    }
  }
}
using UnityEngine;

[DefaultExecutionOrder(-50)]
[DisallowMultipleComponent]
public sealed class CollectibleSpawner: MonoBehaviour {
  [Header("Collectible")]
  [SerializeField]
  private Collectible collectiblePrefab;

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
      Instantiate(collectiblePrefab,spawnPoint.position,spawnPoint.rotation,transform);
    }
  }
}
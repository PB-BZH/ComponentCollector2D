using UnityEngine;

[DisallowMultipleComponent]
public sealed class GameEffects: MonoBehaviour {
  [Header("Particules")]
  [SerializeField]
  private ParticleSystem collectEffectPrefab;

  private GameManager _gameManager;

  private void OnValidate() {
    if (collectEffectPrefab == null) {
      Debug.LogWarning(
          "GameEffects : le prefab de particules de collecte n'est pas renseigné.",
          this);
    }
  }

  private void Awake() {
    _gameManager =
        FindFirstObjectByType<GameManager>();

    if (_gameManager == null) {
      Debug.LogError(
          "GameEffects : aucun GameManager actif n'a été trouvé dans la scène.",
          this);

      enabled = false;
      return;
    }

    if (collectEffectPrefab == null) {
      Debug.LogError(
          "GameEffects : le prefab de particules de collecte est absent.",
          this);

      enabled = false;
    }
  }

  private void OnEnable() {
    if (_gameManager != null) {
      _gameManager.CollectibleCollected +=
          OnCollectibleCollected;
    }
  }

  private void OnDisable() {
    if (_gameManager != null) {
      _gameManager.CollectibleCollected -=
          OnCollectibleCollected;
    }
  }

  private void OnCollectibleCollected(
      Vector3 position,
      int pointValue) {
    if (collectEffectPrefab == null) {
      return;
    }

    ParticleSystem effect = Instantiate(
        collectEffectPrefab,
        position,
        Quaternion.identity);

    if (pointValue > 1) {
      effect.transform.localScale =
          Vector3.one * 1.5f;
    }

    effect.Play();
  }
}
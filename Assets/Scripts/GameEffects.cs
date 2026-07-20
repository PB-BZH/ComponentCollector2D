using UnityEngine;

[DisallowMultipleComponent]
public sealed class GameEffects: MonoBehaviour {
  [Header("Particules")]
  [SerializeField]
  private ParticleSystem collectEffectPrefab;

  [SerializeField]
  private ParticleSystem damageEffectPrefab;

  [SerializeField]
  private ParticleSystem hazardDestroyedEffectPrefab;

  private GameManager _gameManager;

  private void OnValidate() {
    if (collectEffectPrefab == null) {
      Debug.LogWarning("GameEffects : le prefab de particules de collecte n'est pas renseigné.",this);
    }
  }

  private void Awake() {
    _gameManager = FindAnyObjectByType<GameManager>();

    if (_gameManager == null) {
      Debug.LogError("GameEffects : aucun GameManager actif n'a été trouvé dans la scène.",this);
      enabled = false;
      return;
    }

    if (collectEffectPrefab == null) {
      Debug.LogError("GameEffects : le prefab de particules de collecte est absent.",this);
      enabled = false;
    }
  }

  private void OnEnable() {
    if (_gameManager != null) {
      _gameManager.CollectibleCollected += OnCollectibleCollected;
      _gameManager.PlayerDamaged += OnPlayerDamaged;
      _gameManager.HazardDestroyed += OnHazardDestroyed;
    }
  }

  private void OnDisable() {
    if (_gameManager != null) {
      _gameManager.CollectibleCollected -= OnCollectibleCollected;
      _gameManager.PlayerDamaged -= OnPlayerDamaged;
      _gameManager.HazardDestroyed -= OnHazardDestroyed;
    }
  }

  private void OnHazardDestroyed(
    Vector3 position,
    int points) {
    if (hazardDestroyedEffectPrefab == null) {
      Debug.LogWarning(
          "GameEffects : le prefab de destruction de danger n'est pas renseigné.",
          this);

      return;
    }

    ParticleSystem effect = Instantiate(
        hazardDestroyedEffectPrefab,
        position,
        Quaternion.identity);

    effect.Play();
  }

  private void OnPlayerDamaged(Vector3 position) {
    if (damageEffectPrefab == null) {
      Debug.LogWarning("GameEffects : le prefab de dégât n'est pas renseigné.",this);
      return;
    }

    ParticleSystem effect = Instantiate(damageEffectPrefab,position,Quaternion.identity);
    effect.Play();
  }

  private void OnCollectibleCollected(Vector3 position,int pointValue) {
    if (collectEffectPrefab == null) {
      return;
    }

    ParticleSystem effect = Instantiate(collectEffectPrefab,position,Quaternion.identity);

    if (pointValue > 1) {
      effect.transform.localScale = Vector3.one * 1.5f;
    }

    effect.Play();
  }
}
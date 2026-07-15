using UnityEngine;

[DisallowMultipleComponent]
public sealed class GameEffects: MonoBehaviour {
  [Header("Gestionnaire")]
  [SerializeField]
  private GameManager gameManager;

  [Header("Particules")]
  [SerializeField]
  private ParticleSystem collectEffectPrefab;

  private void OnValidate() {
    if (gameManager == null ||
        collectEffectPrefab == null) {
      Debug.LogWarning(
          "GameEffects : GameManager ou prefab de particules non renseigné.",
          this);
    }
  }

  private void OnEnable() {
    if (gameManager != null) {
      gameManager.CollectibleCollected +=
          OnCollectibleCollected;
    }
  }

  private void OnDisable() {
    if (gameManager != null) {
      gameManager.CollectibleCollected -=
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

    // Le collectible rare produit un effet légèrement plus grand.
    if (pointValue > 1) {
      effect.transform.localScale =
          Vector3.one * 1.5f;
    }

    effect.Play();
  }
}
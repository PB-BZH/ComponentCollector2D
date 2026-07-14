using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class Collectible: MonoBehaviour {
  private GameManager _gameManager;
  private bool _isCollected;

  private void Awake() {
    _gameManager = FindAnyObjectByType<GameManager>();

    if (_gameManager == null) {
      Debug.LogError(
          "Aucun GameManager actif n'a été trouvé dans la scène.",
          this);
    }
  }

  private void Reset() {
    Collider2D collectibleCollider = GetComponent<Collider2D>();
    collectibleCollider.isTrigger = true;
  }

  private void OnTriggerEnter2D(Collider2D other) {
    if (_isCollected ||
        !other.TryGetComponent<PlayerController>(out _)) {
      return;
    }

    _isCollected = true;

    if (_gameManager != null) {
      _gameManager.RegisterCollectible();
    }

    Destroy(gameObject);
  }
}
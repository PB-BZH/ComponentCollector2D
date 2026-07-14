using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class Collectible: MonoBehaviour {
  public event Action<Collectible> Collected;

  private bool _isCollected;

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

    Collected?.Invoke(this);

    Destroy(gameObject);
  }
}
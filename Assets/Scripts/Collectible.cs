using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class Collectible: MonoBehaviour {
  public event Action<Collectible> Collected;

  [Header("Valeur")]
  [SerializeField]
  [Min(1)]
  private int pointValue = 1;

  [Header("Animation")]
  [SerializeField]
  [Min(0f)]
  private float rotationSpeed = 90f;

  public int PointValue => pointValue;

  private bool _isCollected;

  private void Update() {
    transform.Rotate(xAngle: 0f,yAngle: 0f,zAngle: rotationSpeed * Time.deltaTime);
  }

  private void Reset() {
    Collider2D collectibleCollider = GetComponent<Collider2D>();
    collectibleCollider.isTrigger = true;
  }

  private void OnTriggerEnter2D(Collider2D other) {
    if (_isCollected || !other.TryGetComponent<PlayerController>(out _)) {
      return;
    }

    _isCollected = true;

    Collected?.Invoke(this);

    Destroy(gameObject);
  }
}

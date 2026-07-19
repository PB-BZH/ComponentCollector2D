using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
public sealed class PlayerProjectile: MonoBehaviour {
  [Header("Déplacement")]
  [SerializeField]
  [Min(0.1f)]
  private float speed = 10f;

  [Header("Durée de vie")]
  [SerializeField]
  [Min(0.1f)]
  private float lifetime = 2f;

  [Header("Impacts")]
  [SerializeField]
  private LayerMask impactLayers;

  private Rigidbody2D _rigidbody;

  private void Awake() {
    _rigidbody = GetComponent<Rigidbody2D>();
  }

  private void OnEnable() {
    Destroy(gameObject,lifetime);
  }

  public void Initialize(Vector2 direction) {
    Vector2 normalizedDirection =
        direction.sqrMagnitude > 0.0001f
            ? direction.normalized
            : Vector2.right;

    _rigidbody.linearVelocity =
        normalizedDirection * speed;
  }

  private void OnTriggerEnter2D(Collider2D other) {
    int otherLayerMask =
        1 << other.gameObject.layer;

    if ((impactLayers.value & otherLayerMask) == 0) {
      return;
    }

    MovingHazard hazard =
        other.GetComponentInParent<MovingHazard>();

    if (hazard != null) {
      hazard.ReceiveProjectileHit();
    }

    Destroy(gameObject);
  }
}
using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public sealed class MovingHazard: MonoBehaviour {
  public event Action<MovingHazard,PlayerController> PlayerHit;
  [Header("Déplacement")]
  [SerializeField]
  [Min(0f)]
  private float speed = 2f;

  [SerializeField]
  private Vector2 travelOffset = new(8f,0f);

  private Rigidbody2D _rigidbody;

  private Vector2 _startPosition;
  private Vector2 _endPosition;

  private float _pathLength;
  private float _travelledDistance;

  private void Awake() {
    _rigidbody = GetComponent<Rigidbody2D>();

    _startPosition = _rigidbody.position;
    _endPosition = _startPosition + travelOffset;

    _pathLength = Vector2.Distance(_startPosition,_endPosition);
  }

  private void FixedUpdate() {
    if (_pathLength <= Mathf.Epsilon || speed <= 0f) {
      return;
    }

    _travelledDistance += speed * Time.fixedDeltaTime;

    float distanceOnPath = Mathf.PingPong(_travelledDistance,_pathLength);
    float progress = distanceOnPath / _pathLength;

    Vector2 targetPosition = Vector2.Lerp(_startPosition,_endPosition,progress);

    _rigidbody.MovePosition(targetPosition);
  }

  private void OnTriggerEnter2D(Collider2D other) {
    if (!other.TryGetComponent<PlayerController>(out PlayerController playerController)) {
      return;
    }

    PlayerHit?.Invoke(this,playerController);
  }

  private void OnDrawGizmosSelected() {
    Vector3 startPosition = Application.isPlaying
      ? (Vector3)_startPosition
      : transform.position;

    Vector3 endPosition = startPosition + (Vector3)travelOffset;

    Gizmos.DrawLine(startPosition,endPosition);
    Gizmos.DrawWireSphere(startPosition,0.15f);
    Gizmos.DrawWireSphere(endPosition,0.15f);
  }
}

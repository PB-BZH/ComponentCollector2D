using UnityEngine;

[RequireComponent(typeof(Camera))]
[DisallowMultipleComponent]
public sealed class CameraFollow: MonoBehaviour {
  [Header("Cible")]
  [SerializeField]
  private Transform target;

  [Header("Limites")]
  [SerializeField]
  private BoxCollider2D cameraBounds;

  [Header("Décalage")]
  [SerializeField]
  private Vector3 offset = new Vector3(0f,0f,-10f);

  [Header("Fluidité")]
  [SerializeField]
  [Min(0.01f)]
  private float smoothTime = 0.2f;

  private Vector3 _velocity;
  private Camera _camera;

  private void Awake() {
    _camera = GetComponent<Camera>();
  }

  private void LateUpdate() {
    if (target == null ||
        cameraBounds == null ||
        _camera == null) {
      return;
    }

    Vector3 targetPosition =
        target.position + offset;

    targetPosition =
      ClampPositionToBounds(targetPosition);

    transform.position = Vector3.SmoothDamp(
        transform.position,
        targetPosition,
        ref _velocity,
        smoothTime);
  }

  private Vector3 ClampPositionToBounds(Vector3 position) {
    Bounds bounds = cameraBounds.bounds;

    float halfHeight =
        _camera.orthographicSize;

    float halfWidth =
        halfHeight * _camera.aspect;

    float minX =
        bounds.min.x + halfWidth;

    float maxX =
        bounds.max.x - halfWidth;

    float minY =
        bounds.min.y + halfHeight;

    float maxY =
        bounds.max.y - halfHeight;

    position.x =
        Mathf.Clamp(position.x,minX,maxX);

    position.y =
        Mathf.Clamp(position.y,minY,maxY);

    return position;
  }
}
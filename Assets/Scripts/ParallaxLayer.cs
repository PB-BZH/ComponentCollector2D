using UnityEngine;

[DisallowMultipleComponent]
public sealed class ParallaxLayer: MonoBehaviour {
  [Header("Références")]
  [SerializeField]
  private Transform cameraTransform;

  [Header("Parallaxe")]
  [SerializeField]
  [Range(0f,1f)]
  private float cameraFollowFactor = 0.8f;

  private Vector3 _initialLayerPosition;
  private Vector3 _initialCameraPosition;

  private void Awake() {
    if (cameraTransform == null) {
      Debug.LogError(
          "ParallaxLayer : la caméra n'est pas renseignée.",
          this);

      enabled = false;
      return;
    }

    _initialLayerPosition = transform.position;
    _initialCameraPosition = cameraTransform.position;
  }

  private void LateUpdate() {
    Vector3 cameraMovement =
        cameraTransform.position -
        _initialCameraPosition;

    transform.position =
        _initialLayerPosition +
        new Vector3(
            cameraMovement.x * cameraFollowFactor,
            cameraMovement.y * cameraFollowFactor,
            0f);
  }
}
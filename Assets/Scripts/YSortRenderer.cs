using UnityEngine;

[DisallowMultipleComponent]
public sealed class YSortRenderer: MonoBehaviour {
  [Header("Rendu")]
  [SerializeField]
  private SpriteRenderer targetRenderer;

  [Header("Tri vertical")]
  [SerializeField]
  [Min(1)]
  private int precision = 100;

  [SerializeField]
  private int orderOffset;

  private void Reset() {
    targetRenderer = GetComponentInChildren<SpriteRenderer>();
  }

  private void Awake() {
    if (targetRenderer == null) {
      targetRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    if (targetRenderer == null) {
      Debug.LogError("YSortRenderer : aucun SpriteRenderer n'a été trouvé.",this);
      enabled = false;
      return;
    }

    UpdateSortingOrder();
  }

  private void LateUpdate() {
    UpdateSortingOrder();
  }

  private void UpdateSortingOrder() {
    targetRenderer.sortingOrder = orderOffset - Mathf.RoundToInt(transform.position.y * precision);
  }
}
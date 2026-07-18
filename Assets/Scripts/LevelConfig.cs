using UnityEngine;

[CreateAssetMenu(
    fileName = "LevelConfig",
    menuName = "Component Collector/Level Config")]
public sealed class LevelConfig: ScriptableObject {
  [Header("Identification")]
  [SerializeField]
  [Min(1)]
  private int levelNumber = 1;

  [Header("Règles")]
  [SerializeField]
  [Min(1f)]
  private float gameDuration = 45f;

  public int LevelNumber => levelNumber;

  public float GameDuration => gameDuration;
}
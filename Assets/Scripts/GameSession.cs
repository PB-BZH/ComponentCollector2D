using UnityEngine;

[DefaultExecutionOrder(-100)]
[DisallowMultipleComponent]
public sealed class GameSession: MonoBehaviour {
  public static GameSession Instance { get; private set; }

  [Header("Nouvelle partie")]
  [SerializeField]
  [Min(1)]
  private int startingLives = 3;

  public int StartingLives => startingLives;

  public int RemainingLives { get; private set; }

  public int Score { get; private set; }

  private int _levelStartScore;
  private int _levelStartLives;

  private void OnValidate() {
    startingLives = Mathf.Max(1,startingLives);
  }

  private void Awake() {
    // Une session persistante existe déjà :
    // l'instance provenant de la nouvelle scène est supprimée.
    if (Instance != null && Instance != this) {
      Destroy(gameObject);
      return;
    }

    Instance = this;

    DontDestroyOnLoad(gameObject);

    BeginNewGame();
  }

  public void BeginNewGame() {
    Score = 0;
    RemainingLives = startingLives;

    SaveLevelCheckpoint();
  }

  public void BeginLevel() {
    SaveLevelCheckpoint();
  }

  public void AddScore(int points) {
    Score += Mathf.Max(0,points);
  }

  public bool TryLoseLife() {
    RemainingLives = Mathf.Max(0,RemainingLives - 1);
    return RemainingLives > 0;
  }

  public void RestoreLevelCheckpoint() {
    Score = _levelStartScore;
    RemainingLives = _levelStartLives;
  }

  private void SaveLevelCheckpoint() {
    _levelStartScore = Score;
    _levelStartLives = RemainingLives;
  }

  private void OnDestroy() {
    if (Instance == this) {
      Instance = null;
    }
  }
}
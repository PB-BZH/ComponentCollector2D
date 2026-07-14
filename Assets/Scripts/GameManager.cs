using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public sealed class GameManager: MonoBehaviour {
  [Header("Partie")]
  [SerializeField]
  [Min(1f)]
  private float gameDuration = 45f;

  [SerializeField]
  private PlayerController playerController;

  public event Action<int,int,int> ScoreChanged;
  public event Action<int> TimerChanged;
  public event Action<GameResult> GameFinished;

  private Collectible[] _collectibles =
      Array.Empty<Collectible>();

  private int _collectedCount;
  private int _totalCollectibles;
  private int _score;

  private float _remainingTime;
  private int _lastPublishedSecond = -1;

  private bool _gameFinished;

  private void OnValidate() {
    gameDuration = Mathf.Max(1f,gameDuration);

    if (playerController == null) {
      Debug.LogWarning(
          "GameManager : PlayerController n'est pas renseigné.",
          this);
    }
  }

  private void Awake() {
    _collectibles =
        FindObjectsByType<Collectible>(
            FindObjectsInactive.Exclude);

    _totalCollectibles = _collectibles.Length;
    _remainingTime = gameDuration;

    if (!ValidateReferences()) {
      enabled = false;
      return;
    }

    foreach (Collectible collectible in _collectibles) {
      collectible.Collected += OnCollectibleCollected;
    }
  }

  private void Start() {
    PublishScore();
    PublishTimer(force: true);
  }

  private void Update() {
    if (Keyboard.current?.escapeKey.wasPressedThisFrame == true) {
      Application.Quit();
      return;
    }

    if (_gameFinished) {
      return;
    }

    _remainingTime -= Time.deltaTime;

    if (_remainingTime <= 0f) {
      _remainingTime = 0f;

      PublishTimer(force: true);
      FinishGame(GameResult.TimeExpired);

      return;
    }

    PublishTimer();
  }

  private void OnCollectibleCollected(
      Collectible collectible) {
    if (_gameFinished) {
      return;
    }

    _collectedCount++;
    _score += collectible.PointValue;

    PublishScore();

    if (_collectedCount >= _totalCollectibles) {
      FinishGame(GameResult.Victory);
    }
  }

  private void PublishScore() {
    ScoreChanged?.Invoke(
        _collectedCount,
        _totalCollectibles,
        _score);
  }

  private void PublishTimer(bool force = false) {
    int displayedSecond =
        Mathf.CeilToInt(_remainingTime);

    if (!force &&
        displayedSecond == _lastPublishedSecond) {
      return;
    }

    _lastPublishedSecond = displayedSecond;

    TimerChanged?.Invoke(displayedSecond);
  }

  private void FinishGame(GameResult result) {
    if (_gameFinished) {
      return;
    }

    _gameFinished = true;

    if (playerController != null) {
      playerController.enabled = false;
    }

    GameFinished?.Invoke(result);

    Debug.Log(
        result == GameResult.Victory
            ? "Tous les composants ont été ramassés."
            : "Temps écoulé.");
  }

  public void ReplayCurrentScene() {
    Scene currentScene =
        SceneManager.GetActiveScene();

    SceneManager.LoadScene(
        currentScene.buildIndex);
  }

  private bool ValidateReferences() {
    bool isValid = true;

    if (playerController == null) {
      Debug.LogError(
          "La référence PlayerController n'est pas renseignée.",
          this);

      isValid = false;
    }

    if (_totalCollectibles == 0) {
      Debug.LogWarning(
          "Aucun Collectible actif n'a été trouvé dans la scène.",
          this);
    }

    return isValid;
  }

  private void OnDestroy() {
    foreach (Collectible collectible in _collectibles) {
      if (collectible != null) {
        collectible.Collected -=
            OnCollectibleCollected;
      }
    }
  }
}
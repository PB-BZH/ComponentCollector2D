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

  [Header("Interface")]
  [SerializeField]
  private GameUI gameUI;

  private Collectible[] _collectibles =
      Array.Empty<Collectible>();

  private int _collectedCount;
  private int _totalCollectibles;

  private float _remainingTime;
  private bool _gameFinished;

  private int _score;

  private void Awake() {
    Application.runInBackground = true;
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

    gameUI.Initialize(
        _totalCollectibles,
        _remainingTime);
  }

  private void OnValidate() {
    gameDuration = Mathf.Max(1f,gameDuration);

    if (playerController == null || gameUI == null) {
      Debug.LogWarning(
          "GameManager : PlayerController ou GameUI n'est pas renseigné.",
          this);
    }
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

      gameUI.UpdateTimer(_remainingTime);

      FinishGame(
          "Temps écoulé !",
          isVictory: false);

      return;
    }

    gameUI.UpdateTimer(_remainingTime);
  }

  private void OnCollectibleCollected(
      Collectible collectible) {
    RegisterCollectible(collectible.PointValue);
  }

  private void RegisterCollectible(int pointValue) {
    if (_gameFinished) {
      return;
    }

    _collectedCount++;
    _score += pointValue;

    gameUI.UpdateScore(
        _collectedCount,
        _totalCollectibles,
        _score);

    if (_collectedCount >= _totalCollectibles) {
      FinishGame(
          "Mission accomplie !",
          isVictory: true);
    }
  }

  public void ReplayCurrentScene() {
    Scene currentScene =
        SceneManager.GetActiveScene();

    SceneManager.LoadScene(
        currentScene.buildIndex);
  }

  private void FinishGame(
      string message,
      bool isVictory) {
    if (_gameFinished) {
      return;
    }

    _gameFinished = true;

    gameUI.ShowEndPanel(message);

    if (playerController != null) {
      playerController.enabled = false;
    }

    Debug.Log(
        isVictory
            ? "Tous les composants ont été ramassés."
            : "Temps écoulé.");
  }

  private bool ValidateReferences() {
    bool isValid = true;

    if (playerController == null) {
      Debug.LogError(
          "La référence PlayerController n'est pas renseignée.",
          this);

      isValid = false;
    }

    if (gameUI == null) {
      Debug.LogError(
          "La référence GameUI n'est pas renseignée.",
          this);

      isValid = false;
    }
    else if (!gameUI.ValidateReferences()) {
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
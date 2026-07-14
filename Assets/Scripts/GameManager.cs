using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public sealed class GameManager: MonoBehaviour {
  [Header("Interface principale")]
  [SerializeField]
  private TMP_Text scoreText;

  [SerializeField]
  private GameObject joystickRoot;

  [SerializeField]
  private TMP_Text timerText;

  [Header("Écran de fin")]
  [SerializeField]
  private GameObject endPanel;

  [SerializeField]
  private TMP_Text endMessageText;

  [Header("Partie")]
  [SerializeField]
  [Min(1f)]
  private float gameDuration = 45f;

  [SerializeField]
  private PlayerController playerController;

  private int _collectedCount;
  private int _totalCollectibles;

  private float _remainingTime;
  private bool _gameFinished;

  private void Awake() {
    _totalCollectibles =
        FindObjectsByType<Collectible>(
            FindObjectsInactive.Exclude).Length;

    _remainingTime = gameDuration;

    if (!ValidateReferences()) {
      enabled = false;
      return;
    }

    endPanel.SetActive(false);

    if (joystickRoot != null) {
      joystickRoot.SetActive(true);
    }

    RefreshScore();
    RefreshTimer();
  }

  private void Update() {
    if (Keyboard.current?.escapeKey.wasPressedThisFrame == true) {
      Application.Quit();
      return;
    }

    if (_gameFinished) {
      return;
    }

    if (_gameFinished) {
      return;
    }

    _remainingTime -= Time.deltaTime;

    if (_remainingTime <= 0f) {
      _remainingTime = 0f;
      RefreshTimer();

      FinishGame(
          "Temps écoulé !",
          isVictory: false);

      return;
    }

    RefreshTimer();
  }

  public void RegisterCollectible() {
    if (_gameFinished) {
      return;
    }

    _collectedCount++;
    RefreshScore();

    if (_collectedCount >= _totalCollectibles) {
      FinishGame(
          "Mission accomplie !",
          isVictory: true);
    }
  }

  public void ReplayCurrentScene() {
    Scene currentScene = SceneManager.GetActiveScene();
    SceneManager.LoadScene(currentScene.buildIndex);
  }

  private void FinishGame(string message,bool isVictory) {
    if (_gameFinished) {
      return;
    }

    _gameFinished = true;

    endMessageText.text = message;
    endPanel.SetActive(true);

    if (joystickRoot != null) {
      joystickRoot.SetActive(false);
    }

    if (playerController != null) {
      playerController.enabled = false;
    }

    Debug.Log(
        isVictory
            ? "Tous les composants ont été ramassés."
            : "Temps écoulé.");
  }

  private void RefreshScore() {
    scoreText.text =
        $"Composants : {_collectedCount} / {_totalCollectibles}";
  }

  private void RefreshTimer() {
    int displayedTime = Mathf.CeilToInt(_remainingTime);

    timerText.text = $"Temps : {displayedTime}";
  }

  private bool ValidateReferences() {
    bool isValid = true;

    if (scoreText == null) {
      Debug.LogError(
          "La référence ScoreText n'est pas renseignée.",
          this);

      isValid = false;
    }

    if (timerText == null) {
      Debug.LogError(
          "La référence TimerText n'est pas renseignée.",
          this);

      isValid = false;
    }

    if (endPanel == null) {
      Debug.LogError(
          "La référence EndPanel n'est pas renseignée.",
          this);

      isValid = false;
    }

    if (endMessageText == null) {
      Debug.LogError(
          "La référence EndMessageText n'est pas renseignée.",
          this);

      isValid = false;
    }

    if (playerController == null) {
      Debug.LogError(
          "La référence PlayerController n'est pas renseignée.",
          this);

      isValid = false;
    }

    if (joystickRoot == null) {
      Debug.LogWarning(
          "JoystickRoot n'est pas renseigné. Le jeu fonctionnera sans joystick tactile.",
          this);
    }

    return isValid;
  }
}
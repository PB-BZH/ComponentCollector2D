using TMPro;
using UnityEngine;

public sealed class GameUI: MonoBehaviour {
  [SerializeField]
  private TMP_Text livesText;

  [Header("Gestionnaire de partie")]
  [SerializeField]
  private GameManager gameManager;

  [Header("Interface principale")]
  [SerializeField]
  private TMP_Text scoreText;

  [SerializeField]
  private TMP_Text timerText;

  [Header("Écran de fin")]
  [SerializeField]
  private GameObject endPanel;

  [SerializeField]
  private TMP_Text endMessageText;

  [Header("Commandes tactiles")]
  [SerializeField]
  private GameObject joystickRoot;

  private void OnValidate() {
    if (gameManager == null ||
        scoreText == null ||
        timerText == null ||
        livesText == null ||
        endPanel == null ||
        endMessageText == null) {
      Debug.LogWarning(
          "GameUI : une ou plusieurs références obligatoires ne sont pas renseignées.",
          this);
    }
  }

  private void Awake() {
    if (!ValidateReferences()) {
      enabled = false;
      return;
    }

    endPanel.SetActive(false);

    if (joystickRoot != null) {
      joystickRoot.SetActive(true);
    }
  }

  private void OnEnable() {
    if (gameManager == null) {
      return;
    }

    gameManager.ScoreChanged += OnScoreChanged;
    gameManager.TimerChanged += OnTimerChanged;
    gameManager.GameFinished += OnGameFinished;
    gameManager.LivesChanged += OnLivesChanged;
  }

  private void OnDisable() {
    if (gameManager == null) {
      return;
    }

    gameManager.ScoreChanged -= OnScoreChanged;
    gameManager.TimerChanged -= OnTimerChanged;
    gameManager.GameFinished -= OnGameFinished;
    gameManager.LivesChanged -= OnLivesChanged;
  }

  private void OnLivesChanged(
    int remainingLives,
    int totalLives) {
    livesText.text =
        $"Vies : {remainingLives} / {totalLives}";
  }

  private void OnScoreChanged(
      int collectedCount,
      int totalCollectibles,
      int score) {
    scoreText.text =
        $"Composants : {collectedCount} / {totalCollectibles}   Score : {score}";
  }

  private void OnTimerChanged(int remainingSeconds) {
    timerText.text =
        $"Temps : {remainingSeconds}";
  }

  private void OnGameFinished(GameResult result) {
    if (joystickRoot != null) {
      joystickRoot.SetActive(false);
    }

    switch (result) {
      case GameResult.Victory:
        endMessageText.text =
            "Mission accomplie !";
        break;

      case GameResult.TimeExpired:
        endMessageText.text =
            "Temps écoulé !";
        break;

      case GameResult.NoLives:
        endMessageText.text =
            "Plus de vies !";
        break;

      default:
        endMessageText.text =
            "Partie terminée";
        break;
    }

    endPanel.SetActive(true);
  }

  public bool ValidateReferences() {
    bool isValid = true;

    if (gameManager == null) {
      Debug.LogError(
          "La référence GameManager n'est pas renseignée dans GameUI.",
          this);

      isValid = false;
    }

    if (scoreText == null) {
      Debug.LogError(
          "La référence ScoreText n'est pas renseignée dans GameUI.",
          this);

      isValid = false;
    }

    if (timerText == null) {
      Debug.LogError(
          "La référence TimerText n'est pas renseignée dans GameUI.",
          this);

      isValid = false;
    }

    if (endPanel == null) {
      Debug.LogError(
          "La référence EndPanel n'est pas renseignée dans GameUI.",
          this);

      isValid = false;
    }

    if (endMessageText == null) {
      Debug.LogError(
          "La référence EndMessageText n'est pas renseignée dans GameUI.",
          this);

      isValid = false;
    }

    if (livesText == null) {
      Debug.LogError(
          "La référence LivesText n'est pas renseignée dans GameUI.",
          this);

      isValid = false;
    }

    Canvas gameCanvas =
        GetComponentInParent<Canvas>();

    if (gameCanvas == null) {
      Debug.LogError(
          "GameUI doit être attaché à un Canvas ou à l'un de ses enfants.",
          this);

      isValid = false;
    }

    if (endPanel != null) {
      Canvas endPanelCanvas =
          endPanel.GetComponentInParent<Canvas>();

      if (endPanelCanvas == null) {
        Debug.LogError(
            "EndPanel doit être placé sous un Canvas.",
            endPanel);

        isValid = false;
      }
      else if (gameCanvas != null &&
               endPanelCanvas != gameCanvas) {
        Debug.LogError(
            "EndPanel n'appartient pas au même Canvas que GameUI.",
            endPanel);

        isValid = false;
      }
    }

    if (joystickRoot == null) {
      Debug.LogWarning(
          "JoystickRoot n'est pas renseigné. Le jeu fonctionnera sans joystick tactile.",
          this);
    }

    return isValid;
  }
}
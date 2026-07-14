using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[DisallowMultipleComponent]
public sealed class GameUI: MonoBehaviour {
  [Header("Interface principale")]
  [SerializeField]
  private TMP_Text scoreText;

  [SerializeField]
  private TMP_Text timerText;

  [SerializeField]
  private TMP_Text livesText;

  [Header("Écran de fin")]
  [SerializeField]
  private GameObject endPanel;

  [SerializeField]
  private TMP_Text endMessageText;

  [SerializeField]
  private Button endActionButton;

  [SerializeField]
  private TMP_Text endActionButtonText;

  [Header("Commandes tactiles")]
  [SerializeField]
  private GameObject joystickRoot;

  private GameManager _gameManager;

  private void OnValidate() {
    if (scoreText == null ||
        timerText == null ||
        livesText == null ||
        endPanel == null ||
        endMessageText == null ||
        endActionButton == null ||
        endActionButtonText == null) {
      Debug.LogWarning(
          "GameUI : une ou plusieurs références obligatoires ne sont pas renseignées.",
          this);
    }
  }

  private void Awake() {
    _gameManager =
        FindFirstObjectByType<GameManager>();

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
    if (_gameManager != null) {
      _gameManager.ScoreChanged += OnScoreChanged;
      _gameManager.TimerChanged += OnTimerChanged;
      _gameManager.LivesChanged += OnLivesChanged;
      _gameManager.GameFinished += OnGameFinished;
    }

    if (endActionButton != null) {
      endActionButton.onClick.AddListener(
          OnEndActionButtonClicked);
    }
  }

  private void OnDisable() {
    if (_gameManager != null) {
      _gameManager.ScoreChanged -= OnScoreChanged;
      _gameManager.TimerChanged -= OnTimerChanged;
      _gameManager.LivesChanged -= OnLivesChanged;
      _gameManager.GameFinished -= OnGameFinished;
    }

    if (endActionButton != null) {
      endActionButton.onClick.RemoveListener(
          OnEndActionButtonClicked);
    }
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

  private void OnLivesChanged(
      int remainingLives,
      int totalLives) {
    livesText.text =
        $"Vies : {remainingLives} / {totalLives}";
  }

  private void OnGameFinished(GameResult result) {
    if (joystickRoot != null) {
      joystickRoot.SetActive(false);
    }

    switch (result) {
      case GameResult.Victory:
        endMessageText.text =
            "Mission accomplie !";

        endActionButtonText.text =
            "Menu principal";
        break;

      case GameResult.TimeExpired:
        endMessageText.text =
            "Temps écoulé !";

        endActionButtonText.text =
            "Rejouer";
        break;

      case GameResult.NoLives:
        endMessageText.text =
            "Plus de vies !";

        endActionButtonText.text =
            "Rejouer";
        break;

      default:
        endMessageText.text =
            "Partie terminée";

        endActionButtonText.text =
            "Rejouer";
        break;
    }

    endPanel.SetActive(true);
  }

  private void OnEndActionButtonClicked() {
    if (_gameManager == null) {
      Debug.LogError(
          "Impossible d'exécuter l'action de fin : GameManager est absent.",
          this);

      return;
    }

    _gameManager.HandleEndAction();
  }

  private bool ValidateReferences() {
    bool isValid = true;

    if (_gameManager == null) {
      Debug.LogError(
          "Aucun GameManager actif n'a été trouvé dans la scène.",
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

    if (livesText == null) {
      Debug.LogError(
          "La référence LivesText n'est pas renseignée dans GameUI.",
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

    if (endActionButton == null) {
      Debug.LogError(
          "La référence EndActionButton n'est pas renseignée dans GameUI.",
          this);

      isValid = false;
    }

    if (endActionButtonText == null) {
      Debug.LogError(
          "La référence EndActionButtonText n'est pas renseignée dans GameUI.",
          this);

      isValid = false;
    }

    Canvas gameCanvas = GetComponent<Canvas>();

    if (gameCanvas == null) {
      Debug.LogError(
          "GameUI doit être attaché au Canvas.",
          this);

      isValid = false;
    }

    if (endPanel != null &&
        endPanel.GetComponentInParent<Canvas>() != gameCanvas) {
      Debug.LogError(
          "EndPanel n'appartient pas au Canvas de GameUI.",
          endPanel);

      isValid = false;
    }

    if (joystickRoot == null) {
      Debug.LogWarning(
          "JoystickRoot n'est pas renseigné. Le jeu fonctionnera sans joystick tactile.",
          this);
    }
    else if (joystickRoot.GetComponentInParent<Canvas>() !=
             gameCanvas) {
      Debug.LogWarning(
          "JoystickRoot n'appartient pas au Canvas de GameUI.",
          joystickRoot);
    }

    return isValid;
  }
}
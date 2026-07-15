using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[DisallowMultipleComponent]
public sealed class GameUI: MonoBehaviour {
  [Header("Avertissement du chronomètre")]

  [SerializeField]
  [Min(0)]
  private int timerWarningThreshold = 10;

  [SerializeField]
  private Color timerNormalColor = Color.white;

  [SerializeField]
  private Color timerWarningColor = Color.red;

  [SerializeField]
  [Min(0.1f)]
  private float timerBlinkInterval = 0.4f;

  [Header("Pause")]
  [SerializeField]
  private GameObject pausePanel;

  [SerializeField]
  private Button pauseButton;

  [SerializeField]
  private Button resumeButton;

  [SerializeField]
  private Button restartLevelButton;

  [SerializeField]
  private Button mainMenuButton;

  [Header("Interface principale")]
  [SerializeField]
  private TMP_Text scoreText;

  [SerializeField]
  private TMP_Text collectedText;

  [SerializeField]
  private TMP_Text timerText;

  [SerializeField]
  private TMP_Text livesText;

  [SerializeField]
  private TMP_Text levelText;

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
  private Coroutine _timerBlinkCoroutine;

  private void OnValidate() {
    if (scoreText == null ||
        timerText == null ||
        livesText == null ||
        endPanel == null ||
        endMessageText == null ||
        endActionButton == null ||
        endActionButtonText == null ||
        pausePanel == null ||
        pauseButton == null ||
        resumeButton == null ||
        restartLevelButton == null ||
        mainMenuButton == null
     ) {
      Debug.LogWarning("GameUI : une ou plusieurs références obligatoires ne sont pas renseignées.",this);
    }
  }

  private void Awake() {
    _gameManager = FindAnyObjectByType<GameManager>();

    if (!ValidateReferences()) {
      enabled = false;
      return;
    }

    levelText.text = $"Niveau : {_gameManager.LevelNumber}";

    endPanel.SetActive(false);
    pausePanel.SetActive(false);
    pauseButton.gameObject.SetActive(true);

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
      _gameManager.PauseChanged += OnPauseChanged;
      pauseButton.onClick.AddListener(OnPauseButtonClicked);
      resumeButton.onClick.AddListener(OnResumeButtonClicked);
      restartLevelButton.onClick.AddListener(OnRestartLevelButtonClicked);
      mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
    }

    if (endActionButton != null) {
      endActionButton.onClick.AddListener(OnEndActionButtonClicked);
    }
  }

  private void OnDisable() {
    if (_gameManager != null) {
      _gameManager.ScoreChanged -= OnScoreChanged;
      _gameManager.TimerChanged -= OnTimerChanged;
      _gameManager.LivesChanged -= OnLivesChanged;
      _gameManager.GameFinished -= OnGameFinished;
      _gameManager.PauseChanged -= OnPauseChanged;
      pauseButton.onClick.RemoveListener(OnPauseButtonClicked);
      resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
      restartLevelButton.onClick.RemoveListener(OnRestartLevelButtonClicked);
      mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);
      StopTimerBlinking();
    }

    if (endActionButton != null) {
      endActionButton.onClick.RemoveListener(OnEndActionButtonClicked);
    }
  }

  private void OnPauseChanged(bool isPaused) {
    pausePanel.SetActive(isPaused);
    pauseButton.gameObject.SetActive(!isPaused);

    if (joystickRoot != null) {
      joystickRoot.SetActive(!isPaused);
    }
  }

  private void OnPauseButtonClicked() {
    _gameManager.TogglePause();
  }

  private void OnResumeButtonClicked() {
    _gameManager.ResumeGame();
  }

  private void OnRestartLevelButtonClicked() {
    _gameManager.ReplayCurrentScene();
  }

  private void OnMainMenuButtonClicked() {
    _gameManager.ReturnToMainMenu();
  }

  private void OnScoreChanged(int collectedCount,int totalCollectibles,int score) {
    collectedText.text = $"Composants : {collectedCount} / {totalCollectibles}";
    scoreText.text = $"Score : {score}";
  }

  private void OnTimerChanged(int remainingSeconds) {
    timerText.text = $"Temps : {remainingSeconds}";

    bool isWarning =
        remainingSeconds <= timerWarningThreshold;

    timerText.color =
        isWarning
            ? timerWarningColor
            : timerNormalColor;

    if (isWarning && _timerBlinkCoroutine == null) {
      _timerBlinkCoroutine =
          StartCoroutine(BlinkTimer());
    }
    else if (!isWarning) {
      StopTimerBlinking();
    }
  }

  private void StopTimerBlinking() {
    if (_timerBlinkCoroutine != null) {
      StopCoroutine(_timerBlinkCoroutine);
      _timerBlinkCoroutine = null;
    }

    timerText.enabled = true;
  }

  private void OnLivesChanged(
      int remainingLives,
      int totalLives) {
    livesText.text =
        $"Vies : {remainingLives} / {totalLives}";
  }

  private void OnGameFinished(GameResult result) {
    pausePanel.SetActive(false);
    pauseButton.gameObject.SetActive(false);

    if (joystickRoot != null) {
      joystickRoot.SetActive(false);
    }

    switch (result) {
      case GameResult.Victory:
        endMessageText.text = "Mission accomplie !";
        endActionButtonText.text = "Menu principal";
        break;

      case GameResult.TimeExpired:
        endMessageText.text = "Temps écoulé !";
        endActionButtonText.text = "Rejouer";
        break;

      case GameResult.NoLives:
        endMessageText.text = "Plus de vies !";
        endActionButtonText.text = "Rejouer";
        break;

      default:
        endMessageText.text = "Partie terminée";
        endActionButtonText.text = "Rejouer";
        break;
    }

    endPanel.SetActive(true);
  }

  private void OnEndActionButtonClicked() {
    if (_gameManager == null) {
      Debug.LogError("Impossible d'exécuter l'action de fin : GameManager est absent.",this);
      return;
    }
    _gameManager.HandleEndAction();
  }

  private bool ValidateReferences() {
    bool isValid = true;
    if (pausePanel == null) {
      Debug.LogError(
          "La référence PausePanel n'est pas renseignée dans GameUI.",
          this);

      isValid = false;
    }

    if (pauseButton == null) {
      Debug.LogError(
          "La référence PauseButton n'est pas renseignée dans GameUI.",
          this);

      isValid = false;
    }

    if (resumeButton == null) {
      Debug.LogError(
          "La référence ResumeButton n'est pas renseignée dans GameUI.",
          this);

      isValid = false;
    }

    if (restartLevelButton == null) {
      Debug.LogError(
          "La référence RestartLevelButton n'est pas renseignée dans GameUI.",
          this);

      isValid = false;
    }

    if (mainMenuButton == null) {
      Debug.LogError(
          "La référence MainMenuButton n'est pas renseignée dans GameUI.",
          this);

      isValid = false;
    }

    if (_gameManager == null) {
      Debug.LogError("Aucun GameManager actif n'a été trouvé dans la scène.",this);
      isValid = false;
    }

    if (scoreText == null) {
      Debug.LogError("La référence ScoreText n'est pas renseignée dans GameUI.",this);
      isValid = false;
    }

    if (timerText == null) {
      Debug.LogError("La référence TimerText n'est pas renseignée dans GameUI.",this);
      isValid = false;
    }

    if (livesText == null) {
      Debug.LogError("La référence LivesText n'est pas renseignée dans GameUI.",this);
      isValid = false;
    }

    if (endPanel == null) {
      Debug.LogError("La référence EndPanel n'est pas renseignée dans GameUI.",this);
      isValid = false;
    }

    if (endMessageText == null) {
      Debug.LogError("La référence EndMessageText n'est pas renseignée dans GameUI.",this);
      isValid = false;
    }

    if (endActionButton == null) {
      Debug.LogError("La référence EndActionButton n'est pas renseignée dans GameUI.",this);
      isValid = false;
    }

    if (endActionButtonText == null) {
      Debug.LogError("La référence EndActionButtonText n'est pas renseignée dans GameUI.",this);
      isValid = false;
    }

    Canvas gameCanvas = GetComponent<Canvas>();

    if (gameCanvas == null) {
      Debug.LogError("GameUI doit être attaché au Canvas.",this);
      isValid = false;
    }

    if (endPanel != null &&
        endPanel.GetComponentInParent<Canvas>() != gameCanvas) {
      Debug.LogError("EndPanel n'appartient pas au Canvas de GameUI.",endPanel);
      isValid = false;
    }

    if (joystickRoot == null) {
      Debug.LogWarning("JoystickRoot n'est pas renseigné. Le jeu fonctionnera sans joystick tactile.",this);
    }
    else if (joystickRoot.GetComponentInParent<Canvas>() != gameCanvas) {
      Debug.LogWarning("JoystickRoot n'appartient pas au Canvas de GameUI.",joystickRoot);
    }
    return isValid;
  }

  private IEnumerator BlinkTimer() {
    while (true) {
      timerText.enabled = !timerText.enabled;
      yield return new WaitForSecondsRealtime(timerBlinkInterval);
    }
  }
}
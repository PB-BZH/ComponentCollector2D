using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public sealed class GameManager: MonoBehaviour {
  [Header("Navigation")]
  [SerializeField]
  [Min(0)]
  private int mainMenuBuildIndex = 0;

  [SerializeField]
  [Min(0f)]
  private float invulnerabilityDuration = 1.5f;

  [Header("Partie")]
  [SerializeField]
  [Min(1f)]
  private float gameDuration = 45f;

  [SerializeField]
  private PlayerController playerController;

  public event Action<int,int,int> ScoreChanged;
  public event Action<int> TimerChanged;
  public event Action<GameResult> GameFinished;
  public event Action<bool> PauseChanged;
  public event Action<int> PointsGained;
  public event Action LifeLost;
  public event Action<Vector3,int> CollectibleCollected;
  public event Action<Vector3> PlayerDamaged;

  private Collectible[] _collectibles = Array.Empty<Collectible>();
  private MovingHazard[] _hazards = Array.Empty<MovingHazard>();

  private int _collectedCount;
  private int _totalCollectibles;
  private GameSession _gameSession;
  private GameResult? _lastGameResult;
  private bool _isPaused;

  public bool IsPaused => _isPaused;

  private float _remainingTime;
  private int _lastPublishedSecond = -1;

  private bool _gameFinished;
  public event Action<int,int> LivesChanged;

  private void OnValidate() {
    mainMenuBuildIndex = Mathf.Max(0,mainMenuBuildIndex);
    gameDuration = Mathf.Max(1f,gameDuration);
    invulnerabilityDuration = Mathf.Max(0f,invulnerabilityDuration);

    if (playerController == null) {
      Debug.LogWarning("GameManager : PlayerController n'est pas renseigné.",this);
    }
  }

  private void Awake() {
    Time.timeScale = 1f;
    _isPaused = false;
    _gameSession = GameSession.Instance;
    _collectibles = FindObjectsByType<Collectible>(FindObjectsInactive.Exclude);
    _hazards = FindObjectsByType<MovingHazard>(FindObjectsInactive.Exclude);
    _totalCollectibles = _collectibles.Length;
    _remainingTime = gameDuration;

    if (!ValidateReferences()) {
      enabled = false;
      return;
    }

    // Enregistre le score et les vies présents
    // au début de ce niveau.
    _gameSession.BeginLevel();

    foreach (Collectible collectible in _collectibles) {
      collectible.Collected += OnCollectibleCollected;
    }

    foreach (MovingHazard hazard in _hazards) {
      hazard.PlayerHit += OnPlayerHit;
    }
  }

  private void OnPlayerHit(MovingHazard hazard,PlayerController hitPlayer) {
    if (_gameFinished || hitPlayer.IsInvulnerable) {
      return;
    }

    bool hasRemainingLives = _gameSession.TryLoseLife();
    PublishLives();
    LifeLost?.Invoke();
    PlayerDamaged?.Invoke(hitPlayer.transform.position);
    Debug.Log($"Danger touché. Vies restantes : {_gameSession.RemainingLives}",hazard);

    if (!hasRemainingLives) {
      FinishGame(GameResult.NoLives);
      return;
    }

    hitPlayer.Respawn(invulnerabilityDuration);
  }

  private void Start() {
    PublishScore();
    PublishTimer(force: true);
    PublishLives();
  }

  private void PublishLives() {
    LivesChanged?.Invoke(_gameSession.RemainingLives,_gameSession.StartingLives);
  }

  private void Update() {
    if (Keyboard.current?.escapeKey.wasPressedThisFrame == true) {
      TogglePause();
      return;
    }

    if (_gameFinished || _isPaused) {
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

  public void TogglePause() {
    if (_gameFinished) {
      return;
    }

    SetPaused(!_isPaused);
  }

  public void ResumeGame() {
    SetPaused(false);
  }

  private void SetPaused(bool isPaused) {
    if (_gameFinished || _isPaused == isPaused) {
      return;
    }

    _isPaused = isPaused;

    Time.timeScale = _isPaused
        ? 0f
        : 1f;

    PauseChanged?.Invoke(_isPaused);
  }

  private void OnCollectibleCollected(Collectible collectible) {
    if (_gameFinished) {
      return;
    }

    _collectedCount++;

    _gameSession.AddScore(collectible.PointValue);
    PointsGained?.Invoke(collectible.PointValue);
    CollectibleCollected?.Invoke(collectible.transform.position,collectible.PointValue);

    PublishScore();

    if (_collectedCount >= _totalCollectibles) {
      if (!TryLoadNextLevel()) {
        FinishGame(GameResult.Victory);
      }
    }
  }

  private bool TryLoadNextLevel() {
    Scene currentScene = SceneManager.GetActiveScene();
    int nextSceneBuildIndex = currentScene.buildIndex + 1;
    bool nextSceneExists = nextSceneBuildIndex < SceneManager.sceneCountInBuildSettings;
    if (!nextSceneExists) {
      return false;
    }
    _gameFinished = true;
    Debug.Log($"Chargement du niveau suivant, index {nextSceneBuildIndex}.");
    SetPaused(false);
    SceneManager.LoadScene(nextSceneBuildIndex);
    return true;
  }

  private void PublishScore() {
    ScoreChanged?.Invoke(_collectedCount,_totalCollectibles,_gameSession.Score);
  }

  private void PublishTimer(bool force = false) {
    int displayedSecond = Mathf.CeilToInt(_remainingTime);

    if (!force && displayedSecond == _lastPublishedSecond) {
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
    _lastGameResult = result;

    if (playerController != null) {
      playerController.enabled = false;
    }

    GameFinished?.Invoke(result);

    Debug.Log(
        result == GameResult.Victory
            ? "Tous les niveaux sont terminés."
            : result == GameResult.TimeExpired
                ? "Temps écoulé."
                : "Plus de vies.");
  }

  public void HandleEndAction() {
    if (_lastGameResult == GameResult.Victory) {
      ReturnToMainMenu();
      return;
    }

    ReplayCurrentScene();
  }

  public void ReturnToMainMenu() {
    SetPaused(false);

    if (mainMenuBuildIndex >=
        SceneManager.sceneCountInBuildSettings) {
      Debug.LogError(
          $"L'index du menu ({mainMenuBuildIndex}) " +
          "n'existe pas dans la Scene List.",
          this);

      return;
    }

    SceneManager.LoadScene(
        mainMenuBuildIndex);
  }

  public void ReplayCurrentScene() {
    SetPaused(false);

    _gameSession.RestoreLevelCheckpoint();

    Scene currentScene = SceneManager.GetActiveScene();
    SceneManager.LoadScene(currentScene.buildIndex);
  }

  private bool ValidateReferences() {
    bool isValid = true;

    if (playerController == null) {
      Debug.LogError("La référence PlayerController n'est pas renseignée.",this);
      isValid = false;
    }

    if (_gameSession == null) {
      Debug.LogError("Aucune GameSession active n'a été trouvée.",this);
      isValid = false;
    }

    if (_totalCollectibles == 0) {
      Debug.LogWarning("Aucun Collectible actif n'a été trouvé dans la scène.",this);
    }

    return isValid;
  }

  private void OnDestroy() {
    if (_isPaused) {
      Time.timeScale = 1f;
    }

    foreach (Collectible collectible in _collectibles) {
      if (collectible != null) {
        collectible.Collected -= OnCollectibleCollected;
      }
    }

    foreach (MovingHazard hazard in _hazards) {
      if (hazard != null) {
        hazard.PlayerHit -= OnPlayerHit;
      }
    }
  }
}
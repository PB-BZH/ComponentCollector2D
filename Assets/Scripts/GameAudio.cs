using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
[DisallowMultipleComponent]
public sealed class GameAudio: MonoBehaviour {
  [Header("Effets sonores")]
  [SerializeField]
  private AudioClip collectClip;

  [SerializeField]
  private AudioClip damageClip;

  [SerializeField]
  private AudioClip victoryClip;

  [SerializeField]
  private AudioClip timeExpiredClip;

  [SerializeField]
  private AudioClip noLivesClip;

  [Header("Compte à rebours")]
  [SerializeField]
  [Min(1)]
  private int countdownStartSecond = 10;

  [SerializeField]
  private AudioClip countdownClip;
  private int _lastCountdownSecond = -1;

  [SerializeField]
  private AudioClip finalCountdownClip;

  [SerializeField]
  [Min(1)]
  private int finalCountdownStartSecond = 3;

  [Header("Volumes")]
  [SerializeField]
  [Range(0f,1f)]
  private float collectVolume = 0.7f;

  [SerializeField]
  [Range(0f,1f)]
  private float timeExpiredVolume = 1f;

  [SerializeField]
  [Range(0f,1f)]
  private float noLivesVolume = 1f;

  [SerializeField]
  [Range(0f,1f)]
  private float damageVolume = 0.8f;

  [SerializeField]
  [Range(0f,1f)]
  private float victoryVolume = 0.8f;

  private AudioSource _audioSource;
  private GameManager _gameManager;

  private void OnValidate() {
    if (collectClip == null ||
        damageClip == null ||
        victoryClip == null) {
      Debug.LogWarning(
          "GameAudio : un ou plusieurs effets sonores " +
          "ne sont pas renseignés.",
          this);
    }
  }

  private void Awake() {
    _audioSource =
        GetComponent<AudioSource>();

    _audioSource.playOnAwake = false;
    _audioSource.loop = false;

    // Effets sonores non spatialisés :
    // même volume partout dans la scène.
    _audioSource.spatialBlend = 0f;
  }

  private void OnEnable() {
    SceneManager.sceneLoaded +=
        OnSceneLoaded;

    AttachToCurrentGameManager();
  }

  private void OnDisable() {
    SceneManager.sceneLoaded -=
        OnSceneLoaded;

    DetachFromGameManager();
  }

  private void OnSceneLoaded(
      Scene scene,
      LoadSceneMode loadMode) {
    AttachToCurrentGameManager();
  }

  private void AttachToCurrentGameManager() {
    DetachFromGameManager();

    _gameManager =
        FindAnyObjectByType<GameManager>();

    // Il est normal de ne pas en trouver dans MainMenu.
    if (_gameManager == null) {
      return;
    }

    _gameManager.PointsGained += OnPointsGained;
    _gameManager.LifeLost += OnLifeLost;
    _gameManager.GameFinished += OnGameFinished;
    _gameManager.TimerChanged += OnTimerChanged;
    _lastCountdownSecond = -1;
  }

  private void DetachFromGameManager() {
    if (_gameManager != null) {
      _gameManager.PointsGained -= OnPointsGained;
      _gameManager.LifeLost -= OnLifeLost;
      _gameManager.GameFinished -= OnGameFinished;
      _gameManager.TimerChanged -= OnTimerChanged;
      _lastCountdownSecond = -1;
    }

    _gameManager = null;
  }

  private void OnTimerChanged(int remainingSeconds) {
    if (remainingSeconds <= 0 ||
        remainingSeconds > countdownStartSecond) {
      return;
    }

    if (remainingSeconds == _lastCountdownSecond) {
      return;
    }

    _lastCountdownSecond = remainingSeconds;

    AudioClip clipToPlay =
      remainingSeconds <= finalCountdownStartSecond && finalCountdownClip != null
        ? finalCountdownClip
        : countdownClip;

    if (clipToPlay == null) {
      Debug.LogWarning(
          "GameAudio : aucun son de compte à rebours n'est renseigné.",
          this);

      return;
    }

    _audioSource.PlayOneShot(clipToPlay);
  }

  private void OnPointsGained(int pointValue) {
    if (collectClip == null) {
      return;
    }
    // Le rare possède une tonalité légèrement plus haute.
    _audioSource.pitch = pointValue > 1 ? 1.2f : 1f;

    _audioSource.PlayOneShot(collectClip,collectVolume);
    _audioSource.pitch = 1f;
  }

  private void OnLifeLost() {
    PlayClip(
        damageClip,
        damageVolume);
  }

  private void OnGameFinished(GameResult result) {
    switch (result) {
      case GameResult.Victory:
        PlayClip(victoryClip,victoryVolume);
        break;

      case GameResult.TimeExpired:
        PlayClip(timeExpiredClip,timeExpiredVolume);
        break;

      case GameResult.NoLives:
        PlayClip(noLivesClip,noLivesVolume);
        break;
    }
  }

  private void PlayClip(
      AudioClip clip,
      float volume) {
    if (clip == null) {
      return;
    }

    _audioSource.pitch = 1f;

    _audioSource.PlayOneShot(
        clip,
        volume);
  }
}
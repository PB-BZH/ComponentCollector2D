using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class MainMenuController: MonoBehaviour {
  [SerializeField]
  private TMP_Text bestScoreText;

  [Header("Boutons")]
  [SerializeField]
  private Button newGameButton;

  [SerializeField]
  private Button quitButton;

  [Header("Navigation")]
  [SerializeField]
  [Min(0)]
  private int firstLevelBuildIndex = 1;

  private void OnValidate() {
    firstLevelBuildIndex = Mathf.Max(0,firstLevelBuildIndex);
    if (newGameButton == null || quitButton == null) {
      Debug.LogWarning("MainMenuController : les boutons ne sont pas tous renseignés.",this);
    }
  }

  private void Awake() {
    Time.timeScale = 1f;
    if (!ValidateReferences()) {
      enabled = false;
    }
  }

  private void Start() {
    if (bestScoreText == null) {
      Debug.LogWarning("MainMenuController : BestScoreText n'est pas renseigné.",this);
      return;
    }

    if (GameSession.Instance == null) {
      Debug.LogWarning("MainMenuController : aucune GameSession disponible.",this);
      return;
    }

    bestScoreText.text = $"Meilleur score : {GameSession.Instance.BestScore}";
  }

  private void OnEnable() {
    if (newGameButton != null) {
      newGameButton.onClick.AddListener(StartNewGame);
    }

    if (quitButton != null) {
      quitButton.onClick.AddListener(QuitGame);
    }
  }

  private void OnDisable() {
    if (newGameButton != null) {
      newGameButton.onClick.RemoveListener(StartNewGame);
    }

    if (quitButton != null) {
      quitButton.onClick.RemoveListener(QuitGame);
    }
  }

  private void StartNewGame() {
    if (GameSession.Instance == null) {
      Debug.LogError("Impossible de démarrer : aucune GameSession active.",this);
      return;
    }

    if (firstLevelBuildIndex >= SceneManager.sceneCountInBuildSettings) {
      Debug.LogError(
          $"L'index du premier niveau ({firstLevelBuildIndex}) " +
          "n'existe pas dans la Scene List.",
          this);

      return;
    }

    GameSession.Instance.BeginNewGame();
    SceneManager.LoadScene(firstLevelBuildIndex);
  }

  private void QuitGame() {
    Debug.Log("Fermeture de l'application.");
    Application.Quit();
  }

  private bool ValidateReferences() {
    bool isValid = true;

    if (newGameButton == null) {
      Debug.LogError("La référence NewGameButton n'est pas renseignée.",this);
      isValid = false;
    }

    if (quitButton == null) {
      Debug.LogError("La référence QuitButton n'est pas renseignée.",this);
      isValid = false;
    }

    return isValid;
  }
}
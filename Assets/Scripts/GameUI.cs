using TMPro;
using UnityEngine;

public sealed class GameUI: MonoBehaviour {
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

  public bool ValidateReferences() {
    bool isValid = true;

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

    if (joystickRoot == null) {
      Debug.LogWarning(
          "JoystickRoot n'est pas renseigné. Le jeu fonctionnera sans joystick tactile.",
          this);
    }

    return isValid;
  }

  public void Initialize(
      int totalCollectibles,
      float remainingTime) {
    endPanel.SetActive(false);

    if (joystickRoot != null) {
      joystickRoot.SetActive(true);
    }

    UpdateScore(
        collectedCount: 0,
        totalCollectibles);

    UpdateTimer(remainingTime);
  }

  public void UpdateScore(
      int collectedCount,
      int totalCollectibles) {
    scoreText.text =
        $"Composants : {collectedCount} / {totalCollectibles}";
  }

  public void UpdateTimer(float remainingTime) {
    int displayedTime = Mathf.CeilToInt(remainingTime);

    timerText.text = $"Temps : {displayedTime}";
  }

  public void ShowEndPanel(string message) {
    if (joystickRoot != null) {
      joystickRoot.SetActive(false);
    }

    endMessageText.text = message;
    endPanel.SetActive(true);
  }
}
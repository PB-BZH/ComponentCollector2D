using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public sealed class PlayerController: MonoBehaviour {
  [Header("Déplacement")]
  [SerializeField]
  [Min(0f)]
  private float speed = 5f;

  [Header("Entrées")]
  [SerializeField]
  private InputActionReference moveAction;

  [Header("Invulnérabilité")]
  [SerializeField]
  [Min(0.05f)]
  private float blinkInterval = 0.12f;

  private Rigidbody2D _rigidbody;
  private SpriteRenderer _spriteRenderer;

  private Vector2 _movementInput;
  private Vector2 _spawnPosition;

  private Coroutine _invulnerabilityCoroutine;

  public bool IsInvulnerable { get; private set; }

  private void Awake() {
    _rigidbody = GetComponent<Rigidbody2D>();
    _spriteRenderer = GetComponent<SpriteRenderer>();

    _spawnPosition = _rigidbody.position;
  }

  private void OnEnable() {
    if (moveAction == null || moveAction.action == null) {
      Debug.LogError(
          "L'action de déplacement Move n'est pas renseignée.",
          this);

      enabled = false;
      return;
    }

    moveAction.action.Enable();
  }

  private void Update() {
    Vector2 input =
        moveAction.action.ReadValue<Vector2>();

    _movementInput =
        Vector2.ClampMagnitude(input,1f);
  }

  private void FixedUpdate() {
    _rigidbody.linearVelocity =
        _movementInput * speed;
  }

  public void Respawn(float invulnerabilityDuration) {
    if (_invulnerabilityCoroutine != null) {
      StopCoroutine(_invulnerabilityCoroutine);
    }

    _movementInput = Vector2.zero;
    _rigidbody.linearVelocity = Vector2.zero;

    // Téléportation vers la position initiale.
    _rigidbody.position = _spawnPosition;

    _invulnerabilityCoroutine =
        StartCoroutine(
            InvulnerabilityRoutine(
                invulnerabilityDuration));
  }

  private IEnumerator InvulnerabilityRoutine(
      float duration) {
    IsInvulnerable = true;

    float elapsedTime = 0f;

    while (elapsedTime < duration) {
      _spriteRenderer.enabled =
          !_spriteRenderer.enabled;

      yield return new WaitForSeconds(
          blinkInterval);

      elapsedTime += blinkInterval;
    }

    _spriteRenderer.enabled = true;
    IsInvulnerable = false;
    _invulnerabilityCoroutine = null;
  }

  private void OnDisable() {
    if (moveAction != null &&
        moveAction.action != null) {
      moveAction.action.Disable();
    }

    if (_invulnerabilityCoroutine != null) {
      StopCoroutine(
          _invulnerabilityCoroutine);

      _invulnerabilityCoroutine = null;
    }

    IsInvulnerable = false;
    _spriteRenderer.enabled = true;

    _movementInput = Vector2.zero;

    if (_rigidbody != null) {
      _rigidbody.linearVelocity =
          Vector2.zero;
    }
  }
}
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class PlayerController: MonoBehaviour {
  [Header("Déplacement")]
  [SerializeField]
  [Min(0f)]
  private float speed = 5f;

  [Header("Entrées")]
  [SerializeField]
  private InputActionReference moveAction;

  [Header("Tir")]
  [SerializeField]
  private InputActionReference fireAction;

  [SerializeField]
  private PlayerProjectile projectilePrefab;

  [SerializeField]
  private Transform firePoint;

  [SerializeField]
  [Min(0f)]
  private float firePointDistance = 0.65f;

  [Header("Invulnérabilité")]
  [SerializeField]
  [Min(0.05f)]
  private float blinkInterval = 0.12f;

  private Rigidbody2D _rigidbody;
  private SpriteRenderer _spriteRenderer;
  private Animator _animator;

  private Vector2 _movementInput;
  private Vector2 _spawnPosition;
  private Vector2 _lastMoveDirection = Vector2.right;

  private Coroutine _invulnerabilityCoroutine;

  public bool IsInvulnerable { get; private set; }

  private void Awake() {
    _rigidbody = GetComponent<Rigidbody2D>();
    _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    _animator = GetComponentInChildren<Animator>();

    _spawnPosition = _rigidbody.position;
  }

  private void OnEnable() {
    if (moveAction == null ||
        moveAction.action == null) {
      Debug.LogError("L'action de déplacement Move n'est pas renseignée.",this);
      enabled = false;
      return;
    }

    if (fireAction == null ||
        fireAction.action == null) {
      Debug.LogError("L'action de tir Fire n'est pas renseignée.",this);
      enabled = false;
      return;
    }

    fireAction.action.performed += OnFirePerformed;

    moveAction.action.Enable();
    fireAction.action.Enable();
  }

  private void Update() {
    Vector2 input = moveAction.action.ReadValue<Vector2>();

    _movementInput = Vector2.ClampMagnitude(input,1f);

    if (_movementInput.sqrMagnitude > 0.01f) {
      _lastMoveDirection = _movementInput.normalized;
    }

    bool isMoving = _movementInput.sqrMagnitude > 0.01f;
    _animator.SetBool("IsMoving",isMoving);
  }

  private void FixedUpdate() {
    _rigidbody.linearVelocity = _movementInput * speed;
  }

  public void Respawn(float invulnerabilityDuration) {
    if (_invulnerabilityCoroutine != null) {
      StopCoroutine(_invulnerabilityCoroutine);
    }

    _movementInput = Vector2.zero;

    if (_movementInput.sqrMagnitude > 0.01f) {
      _lastMoveDirection =
          _movementInput.normalized;
    }

    _rigidbody.linearVelocity = Vector2.zero;

    // Téléportation vers la position initiale.
    _rigidbody.position = _spawnPosition;

    _invulnerabilityCoroutine = StartCoroutine(InvulnerabilityRoutine(invulnerabilityDuration));
  }

  private IEnumerator InvulnerabilityRoutine(
      float duration) {
    IsInvulnerable = true;

    float elapsedTime = 0f;

    while (elapsedTime < duration) {
      _spriteRenderer.enabled = !_spriteRenderer.enabled;
      yield return new WaitForSeconds(blinkInterval);
      elapsedTime += blinkInterval;
    }

    _spriteRenderer.enabled = true;
    IsInvulnerable = false;
    _invulnerabilityCoroutine = null;
  }

  private void OnDisable() {
    if (fireAction != null &&
        fireAction.action != null) {
      fireAction.action.performed -= OnFirePerformed;
      fireAction.action.Disable();
    }

    if (moveAction != null &&
        moveAction.action != null) {
      moveAction.action.Disable();
    }

    if (_invulnerabilityCoroutine != null) {
      StopCoroutine(_invulnerabilityCoroutine);
      _invulnerabilityCoroutine = null;
    }

    IsInvulnerable = false;
    _spriteRenderer.enabled = true;

    _movementInput = Vector2.zero;

    if (_rigidbody != null) {
      _rigidbody.linearVelocity = Vector2.zero;
    }
  }

  private void OnFirePerformed(
      InputAction.CallbackContext context) {
    if (projectilePrefab == null) {
      Debug.LogWarning("PlayerController : le prefab du projectile n'est pas renseigné.",this);
      return;
    }

    if (firePoint == null) {
      Debug.LogWarning("PlayerController : le point de tir n'est pas renseigné.",this);
      return;
    }

    Vector2 currentInput =
    moveAction.action.ReadValue<Vector2>();

    Vector2 fireDirection =
        currentInput.sqrMagnitude > 0.01f
            ? currentInput.normalized
            : _lastMoveDirection;

    firePoint.localPosition =
    new Vector3(
        fireDirection.x,
        fireDirection.y,
        0f)
    * firePointDistance;

    PlayerProjectile projectile = Instantiate(
        projectilePrefab,
        firePoint.position,
        Quaternion.identity);

    projectile.Initialize(fireDirection);
  }
}
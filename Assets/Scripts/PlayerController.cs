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

  private Rigidbody2D _rigidbody;
  private Vector2 _movementInput;

  private void Awake() {
    _rigidbody = GetComponent<Rigidbody2D>();
  }

  private void OnEnable() {
    if (moveAction == null || moveAction.action == null) {
      Debug.LogError("L'action de déplacement Move n'est pas renseignée.",this);
      enabled = false;
      return;
    }
    moveAction.action.Enable();
  }

  private void Update() {
    Vector2 input = moveAction.action.ReadValue<Vector2>();

    // Préserve la progressivité d'un joystick analogique
    // tout en limitant sa longueur maximale à 1.
    _movementInput = Vector2.ClampMagnitude(input,1f);
  }

  private void FixedUpdate() {
    _rigidbody.linearVelocity = _movementInput * speed;
  }

  private void OnDisable() {
    if (moveAction != null && moveAction.action != null) {
      moveAction.action.Disable();
    }
    _movementInput = Vector2.zero;
    if (_rigidbody != null) {
      _rigidbody.linearVelocity = Vector2.zero;
    }
  }
}
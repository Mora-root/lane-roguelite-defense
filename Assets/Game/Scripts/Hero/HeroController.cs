using UnityEngine;

public sealed class HeroController : MonoBehaviour
{
    [SerializeField] private Team team = Team.Player;
    [SerializeField] private Health health;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private bool canMove = true;
    [SerializeField] private bool useKeyboardInput = true;
    [SerializeField] private float gravity = -20f;

    private Vector2 moveInput;
    private float verticalVelocity;
    private bool hasLoggedMissingController;

    public Team Team => team;
    public Health Health => health;
    public bool CanMove => canMove;

    private void Awake()
    {
        if (health == null)
        {
            health = GetComponent<Health>();
        }

        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        if (characterController == null)
        {
            LogMissingControllerWarning();
        }
    }

    private void Update()
    {
        if (useKeyboardInput)
        {
            SetMoveInput(new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")));
        }

        Move();
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = Vector2.ClampMagnitude(input, 1f);
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
    }

    private void Move()
    {
        if (!canMove || (health != null && health.IsDead))
        {
            return;
        }

        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y);

        if (movement.sqrMagnitude > 0f)
        {
            RotateTowards(movement);
        }

        if (characterController == null)
        {
            transform.position += movement * (moveSpeed * Time.deltaTime);
            LogMissingControllerWarning();
            return;
        }

        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        movement *= moveSpeed;
        movement.y = verticalVelocity;
        characterController.Move(movement * Time.deltaTime);
    }

    private void RotateTowards(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime);
    }

    private void LogMissingControllerWarning()
    {
        if (hasLoggedMissingController)
        {
            return;
        }

        hasLoggedMissingController = true;
        Debug.LogWarning(
            "HeroController is missing a CharacterController. Using Transform movement as a fallback.",
            this);
    }
}

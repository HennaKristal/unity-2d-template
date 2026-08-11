using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class DemoCharacterController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 6.0f;
    [SerializeField] private float jumpForce = 12.0f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.6f, 0.1f);
    [SerializeField] private LayerMask groundLayer;


    private Rigidbody2D rigidbodyComponent;
    private bool isGrounded;


    private void Awake()
    {
        rigidbodyComponent = GetComponent<Rigidbody2D>();
    }


    private void Update()
    {
        CheckGround();

        if (InputManager.Instance.JumpPress && isGrounded)
        {
            Jump();
        }
    }


    private void FixedUpdate()
    {
        Move();
    }


    private void Move()
    {
        Vector2 velocity = rigidbodyComponent.linearVelocity;

        velocity.x = InputManager.Instance.Move.x * movementSpeed;

        rigidbodyComponent.linearVelocity = velocity;
    }


    private void Jump()
    {
        Vector2 velocity = rigidbodyComponent.linearVelocity;

        velocity.y = jumpForce;

        rigidbodyComponent.linearVelocity = velocity;
    }


    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapBox(
            groundCheck.position,
            groundCheckSize,
            0.0f,
            groundLayer
        );
    }


    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}
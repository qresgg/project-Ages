using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _acceleration = 10f;
    [SerializeField] private float _rotationSpeed = 4020f;
    [SerializeField] private float _stopDistance = 1f;

    private Rigidbody2D _rb;
    private Camera _mainCamera;

    private Vector2 _inputDirection;
    private Vector2 _mouseDirection;
    private Vector2 _moveDirection;
    private Vector2 _lookDirection;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _mainCamera = Camera.main;
        if (_mainCamera == null)
            Debug.LogError("Camera.main is not exist!!!!!!!!!!!!!!!!!!!!!!!!!!!");
    }

    private void Update()
    {
        ReadInput();
        CalculateDirections();
        RotateTowardsLookDirection();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void ReadInput()
    {
        _inputDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
    }

    private void CalculateDirections()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(_mainCamera.transform.position.z - transform.position.z);

        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        _mouseDirection = (mouseWorldPos - transform.position).normalized;

        float distanceToMouse = Vector2.Distance(transform.position, mouseWorldPos);

        if (Input.GetMouseButton(0) && distanceToMouse > _stopDistance)
            _moveDirection = _mouseDirection;
        else if (_inputDirection != Vector2.zero)
            _moveDirection = _inputDirection;
        else
            _moveDirection = Vector2.zero;

        if (Input.GetMouseButton(0))
            _lookDirection = _mouseDirection;
        else if (_inputDirection != Vector2.zero)
            _lookDirection = _inputDirection;
    }

    private void RotateTowardsLookDirection()
    {
        if (_lookDirection != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(_lookDirection.y, _lookDirection.x) * Mathf.Rad2Deg - 90f;
            float newAngle = Mathf.LerpAngle(_rb.rotation, targetAngle, _rotationSpeed * Time.deltaTime / 360f);
            _rb.MoveRotation(newAngle);
        }
    }

    private void MovePlayer()
    {
        Vector2 targetVelocity = _moveDirection * _moveSpeed;
        _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, targetVelocity, _acceleration * Time.fixedDeltaTime);
    }
}
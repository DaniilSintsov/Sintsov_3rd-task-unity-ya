using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerDirection
{
    Left = -1,
    Right = 1
}

[RequireComponent(typeof(Rigidbody2D))]
public class Slide : MonoBehaviour
{
    private const float MinMoveDistance = 0.001f;
    private const float ShellRadius = 0.01f;

    [SerializeField] private float _minGroundNormalY = 0.65f;
    [SerializeField] private float _gravityModifier = 1f;
    [SerializeField] private Vector2 _velocity;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _speed;
    [SerializeField] private AnimationCurve _yAnimation;
    [SerializeField] private AnimationCurve _xAnimation;
    [SerializeField] private int _jumpHeight;
    [SerializeField] private int _jumpLength;

    private Rigidbody2D _rb2d;

    private Vector2 _groundNormal;
    private Vector2 _targetVelocity;
    private bool _grounded;
    private PlayerDirection _playerDir;
    private ContactFilter2D _contactFilter;
    private RaycastHit2D[] _hitBuffer = new RaycastHit2D[16];
    private List<RaycastHit2D> _hitBufferList = new List<RaycastHit2D>(16);

    private void OnEnable()
    {
        _rb2d = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _contactFilter.useTriggers = false;
        _contactFilter.SetLayerMask(_layerMask);
        _contactFilter.useLayerMask = true;
    }

    private void Update()
    {
        Vector2 alongSurface = Vector2.Perpendicular(_groundNormal);
        _targetVelocity = alongSurface * _speed;

        Debug.DrawRay(_rb2d.position, _groundNormal, Color.blue);
        Debug.DrawRay(_rb2d.position, Vector2.up, Color.red);

        if (Input.GetAxis("Jump") > 0 && _grounded)
        {
            StartCoroutine(AnimateJumpRoutine(_rb2d.transform));
        }

        SetPlayerDir(ref _playerDir);
    }

    private void FixedUpdate()
    {
        _velocity += Physics2D.gravity * (_gravityModifier * Time.deltaTime);
        _velocity.x = _targetVelocity.x;
        _grounded = false;

        Vector2 deltaPosition = _velocity * Time.deltaTime;
        Vector2 moveAlongGround = new Vector2(_groundNormal.y, _groundNormal.x);
        Vector2 move = moveAlongGround * (deltaPosition.x * -(int)_playerDir);

        Movement(move, false);

        move = Vector2.up * deltaPosition.y;

        Movement(move, true);
    }

    private void SetPlayerDir(ref PlayerDirection playerDir)
    {
        var crossOfGroundNormalAndVertical = Vector3.Cross(_groundNormal, Vector2.up);

        if (crossOfGroundNormalAndVertical.z > 0)
        {
            playerDir = PlayerDirection.Right;
            _rb2d.gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (crossOfGroundNormalAndVertical.z < 0)
        {
            playerDir = PlayerDirection.Left;
            _rb2d.gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    private IEnumerator AnimateJumpRoutine(Transform jumper)
    {
        var expiredSeconds = 0f;

        Vector3 startPosition = jumper.position;

        while (expiredSeconds < 1)
        {
            expiredSeconds += Time.deltaTime;

            jumper.position = startPosition + new Vector3(
                _xAnimation.Evaluate(expiredSeconds) * _jumpLength * (int)_playerDir,
                _yAnimation.Evaluate(expiredSeconds) * _jumpHeight, 0);
            yield return null;
        }
    }

    private void Movement(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;

        if (distance > MinMoveDistance)
        {
            int count = _rb2d.Cast(move, _contactFilter, _hitBuffer, distance + ShellRadius);

            _hitBufferList.Clear();

            for (int i = 0; i < count; i++)
            {
                _hitBufferList.Add(_hitBuffer[i]);
            }

            for (int i = 0; i < _hitBufferList.Count; i++)
            {
                Vector2 currentNormal = _hitBufferList[i].normal;
                if (currentNormal.y > _minGroundNormalY)
                {
                    _grounded = true;

                    if (yMovement)
                    {
                        _groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(_velocity, currentNormal);
                if (projection < 0)
                {
                    _velocity = _velocity - projection * currentNormal;
                }

                float modifiedDistance = _hitBufferList[i].distance - ShellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }

        _rb2d.position = _rb2d.position + move.normalized * distance;
    }
}

using System;
using UnityEngine;

public class RockPlayer : MonoBehaviour
{
    [SerializeField] private float _initialThrust;
    [SerializeField] private float _speed;

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _rigidbody.AddForce(transform.right * _initialThrust);
    }

    private void Update()
    {
        var moveHorizontal = Input.GetAxis("Horizontal");

        if (Math.Abs(moveHorizontal) > 0)
        {
            _rigidbody.AddForce(transform.right * (moveHorizontal * _speed));
        }
    }
}

using UnityEngine;

public class PinPlayer : MonoBehaviour
{
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.TryGetComponent(out RopeAnchor ropeAnchor))
            return;

        var joint = ropeAnchor.gameObject.GetComponent<HingeJoint2D>();
        float xConnectedAnchorPosition = 0.005999653f;
        float yConnectedAnchorPosition = 0.9706504f;

        joint.connectedBody = _rigidbody;
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = new Vector2(xConnectedAnchorPosition, yConnectedAnchorPosition);
    }
}

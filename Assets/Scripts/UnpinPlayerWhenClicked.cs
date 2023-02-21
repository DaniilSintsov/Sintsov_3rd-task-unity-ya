using UnityEngine;

public class UnpinPlayerWhenClicked : MonoBehaviour
{
    private HingeJoint2D _hingeJoint;

    private void Awake()
    {
        _hingeJoint = GetComponent<HingeJoint2D>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _hingeJoint.connectedBody = null;
            _hingeJoint.autoConfigureConnectedAnchor = true;
        }
    }
}

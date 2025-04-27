using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class Arrow : MonoBehaviour
{
    public float speed = 15f; // Speed of the arrow
    public Transform tip; // The tip of the arrow

    private Rigidbody rb;
    private bool _inAir = false;
    private Vector3 _lastPosition = Vector3.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PullInteraction.PullActionReleased += Release;

        Stop();
    }
    private void OnDestroy()
    {
        PullInteraction.PullActionReleased -= Release;
    }
    private void Release(float value)
    {
        PullInteraction.PullActionReleased -= Release;
        gameObject.transform.parent = null;
        _inAir = true;
        SetPhysics(true);

        Vector3 force = transform.forward * value * speed;
        rb.AddForce(force, ForceMode.Impulse);

        StartCoroutine(RotateWithVelocity());

        _lastPosition = tip.position;
    }
    private IEnumerator RotateWithVelocity()
    {
        yield return new WaitForEndOfFrame(); // Wait for the next frame to ensure the arrow has a velocity
        while (_inAir)
        {
            Quaternion newRotation = Quaternion.LookRotation(rb.velocity, transform.up);
            transform.rotation = newRotation;
            yield return null;
        }
    }
    private void FixedUpdate()
    {
        if (_inAir)
        {
            CheckCollision();
            _lastPosition = tip.position;
        }
    }

    private void CheckCollision()
    {
        if (Physics.Linecast(_lastPosition, tip.position, out RaycastHit hitInfo))
        {
            if (hitInfo.transform.gameObject.layer != 8)
            {
                if (hitInfo.transform.TryGetComponent(out Rigidbody body))
                {

                    rb.interpolation = RigidbodyInterpolation.None;
                    transform.parent = hitInfo.transform;
                    body.AddForce(rb.velocity, ForceMode.Impulse);
                }
                Stop();
            }

        }
    }
    private void Stop()
    {
        _inAir = false;
        SetPhysics(false);
    }
    private void SetPhysics(bool usePhysics)
    {
        rb.useGravity = usePhysics;
        rb.isKinematic = !usePhysics;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetToStrike : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float targetSize = 10f;
    [SerializeField] private float targetSpeed = 10f;
    public Rigidbody Rb => rb;

    void Update()
    {
        var dir = new Vector3(Mathf.Cos(Time.time * targetSpeed) * targetSize, Mathf.Sin(Time.time * targetSpeed) * targetSize);

        rb.velocity = dir;
    }

    public void Explode() => Destroy(gameObject);
}

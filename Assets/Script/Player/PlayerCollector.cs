using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PlayerCollector : MonoBehaviour
{
    PlayerStats player;
    CircleCollider2D detector;
    public float pullSpeed;

    private void Start()
    {
        player = GetComponentInParent<PlayerStats>();
    }

    public void SetRadius(float radius)
    {
        if (!detector)
        {
            detector = GetComponent<CircleCollider2D>();
        }
        detector.radius = radius;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent(out Pickup p))
        {
            p.Collect(player, pullSpeed);
        }
    }
}

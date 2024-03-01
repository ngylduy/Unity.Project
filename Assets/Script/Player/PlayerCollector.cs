using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    PlayerStats playerStats;
    CircleCollider2D circleCollider;
    public float pullSpeed;

    private void Start() {
        playerStats = FindObjectOfType<PlayerStats>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Update() {
        circleCollider.radius = playerStats.CurrentMagnet;
    }
    
    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.TryGetComponent(out ICollectible collectible)) {
            Rigidbody2D rb = col.gameObject.GetComponent<Rigidbody2D>();
            Vector2 foreceDir = (transform.position - col.transform.position).normalized;
            rb.AddForce(foreceDir * pullSpeed);

            collectible.Collect();
        }
    }
}

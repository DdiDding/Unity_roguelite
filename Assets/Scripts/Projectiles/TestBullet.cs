using UnityEngine;

public class TestBullet : MonoBehaviour
{
    public float speed = 5f;

    void Start()
    {
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();

        rigidbody.gravityScale = 0f;
        rigidbody.linearVelocity = (Vector2)transform.right * speed;
    }

}

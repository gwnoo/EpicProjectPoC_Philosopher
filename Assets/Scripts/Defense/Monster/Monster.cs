using UnityEngine;

public class Monster : MonoBehaviour
{
    public float moveSpeed = 5f;
    private bool _isMoving = true;

    void Update()
    {
        if (_isMoving)
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            _isMoving = false;
            Debug.Log($"{gameObject.name} 벽에 도착하여 정지함.");
        }
    }
}

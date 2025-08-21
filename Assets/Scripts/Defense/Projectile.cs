using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float lifetime = 2f;
    private float speed = 50f;
    public int damage = 5;

    private LineRenderer lineRenderer;
    private Color startColor = Color.white;
    public Color endColor = Color.black;

    private Rigidbody2D rb;
    private Vector3 previousPosition;

    private void Start()
    {
        Destroy(gameObject, lifetime);

        previousPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.startColor = startColor;
            lineRenderer.endColor = endColor;
        }
    }

    private void Update()
    {
        Vector3 currentPosition = transform.position;

        CheckCollision(previousPosition, currentPosition);
        previousPosition = currentPosition;

        rb.linearVelocity = transform.right * speed;

        if (lineRenderer != null)
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, currentPosition);
        }
    }

    private void CheckCollision(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        RaycastHit2D hit = Physics2D.Raycast(start, direction, direction.magnitude);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                MonsterStats monster = hit.collider.GetComponent<MonsterStats>();
                if (monster != null)
                {
                    monster.TakeDamage(damage);
                }
                Destroy(gameObject);
            }
        }
    }
}

using UnityEngine;

public class Boomerang : MonoBehaviour
{
    public float throwSpeed = 10f;
    public float returnSpeed = 5f;
    public float maxDistance = 5f;
    public float interactableTouchCheckRadius = 5f;
    private Vector2 startPosition;
    private bool returning = false;
    private Rigidbody2D rb;
    private Rigidbody2D playerRb;

    void Start()
    {
        returning = false;
    }

    void Update()
    {
        if (!returning && Vector2.Distance(startPosition, rb.position) >= maxDistance)
        {
            returning = true;
        }

        if (returning)
        {
            ReturnBoomerang();
        }
    }

    public void ThrowBoomerang(Vector2 direction, Rigidbody2D playerRigidBody, CharacterControllerData controllerData)
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = rb.position;
        playerRb = playerRigidBody;
        rb.velocity = direction * throwSpeed;
        float torque = Random.Range(.1f, controllerData.torsoThrowTorque);
        rb.AddTorque(torque, ForceMode2D.Impulse);
        returning = false;
    }

    void ReturnBoomerang()
    {
        Vector2 direction = (playerRb.position - rb.position).normalized;
        rb.velocity = direction * returnSpeed;

        Vector2 distance = playerRb.position - rb.position;
        bool isTouching = distance.sqrMagnitude < interactableTouchCheckRadius * interactableTouchCheckRadius;
#if UNITY_EDITOR
        DebugDraw.DrawWireCircle(rb.position, transform.rotation, interactableTouchCheckRadius, isTouching ? Color.red : Color.white);
#endif
        if (isTouching)
        {
            returning = false;
            rb.velocity = Vector2.zero;
            GameObject.Destroy(gameObject);
        }

    }
}

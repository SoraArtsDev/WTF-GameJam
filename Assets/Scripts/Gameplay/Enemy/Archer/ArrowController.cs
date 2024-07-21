using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Sora.Environment
{
    public class ArrowController : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Events.SoraEvent gameOverEvent;
        [SerializeField] private float xVelocity;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Managers.GameManager.instance.OnGameOver(this, null);
            collision.gameObject.SetActive(false);
        }

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {
            rb.velocity = new Vector2(xVelocity, rb.velocity.y);
        }
    }
}

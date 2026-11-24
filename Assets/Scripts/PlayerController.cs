using UnityEngine;

namespace Aurora
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PlayerController : MonoBehaviour
    {
        public float speed = 4.3f;
        Rigidbody2D body; Vector2 movement, facing = Vector2.down;
        float nextAttack; SpriteRenderer sprite;
        void Awake() { body = GetComponent<Rigidbody2D>(); sprite = GetComponent<SpriteRenderer>(); }
        void Update()
        {
            var game = GameSession.Instance;
            if (game.Paused) { movement = Vector2.zero; return; }
            movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            if (movement.sqrMagnitude > 0) { facing = movement; if (Mathf.Abs(movement.x) > .1f) sprite.flipX = movement.x < 0; }
            sprite.color = Time.time < game.InvulnerableUntil && Mathf.FloorToInt(Time.time * 12) % 2 == 0 ? new Color(1, 1, 1, .35f) : Color.white;
            sprite.sortingOrder = 10 + Mathf.RoundToInt(-transform.position.y * 2);
            if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextAttack) Attack();
            if (Input.GetKeyDown(KeyCode.E)) Interact();
            if (Input.GetKeyDown(KeyCode.Q)) game.Potion();
            Interactable nearest = Nearest(); game.HUD.SetInteraction(nearest != null ? "E · " + nearest.label : "");
        }
        void FixedUpdate() { body.MovePosition(body.position + movement * speed * Time.fixedDeltaTime); }
        void Attack()
        {
            nextAttack = Time.time + .32f;
            foreach (var enemy in FindObjectsByType<EnemyController>(FindObjectsSortMode.None))
            {
                Vector2 delta = (Vector2)enemy.transform.position - body.position;
                if (delta.magnitude <= 1.65f && Vector2.Dot(facing, delta.normalized) > -.1f && !Physics2D.Linecast(body.position, enemy.transform.position, 1 << 8)) enemy.Hit(1);
            }
            var slash = new GameObject("Arco da espada"); slash.transform.position = (Vector2)transform.position + facing * .7f;
            var line = slash.AddComponent<LineRenderer>(); line.useWorldSpace = false; line.positionCount = 9;
            line.startWidth = .13f; line.endWidth = .04f; line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = new Color(.7f, 1, .9f); line.endColor = new Color(1, .8f, .3f); line.sortingOrder = 100;
            float angle = Mathf.Atan2(facing.y, facing.x);
            for (int i = 0; i < 9; i++) { float a = angle - 1.1f + i * .275f; line.SetPosition(i, new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0) * .8f); }
            Destroy(line.material, .16f); Destroy(slash, .16f);
        }
        Interactable Nearest()
        {
            Interactable closest = null; float distance = 1.65f;
            foreach (var item in FindObjectsByType<Interactable>(FindObjectsSortMode.None))
            {
                float current = Vector2.Distance(transform.position, item.transform.position);
                if (current < distance) { closest = item; distance = current; }
            }
            return closest;
        }
        void Interact() { var item = Nearest(); if (item != null) item.Use(); }
    }
}

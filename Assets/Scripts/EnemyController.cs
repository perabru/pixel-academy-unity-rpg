using UnityEngine;

namespace Aurora
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class EnemyController : MonoBehaviour
    {
        public string id; public bool boss;
        int hp, maxHp; float nextShot, nextHit, contactTime; Rigidbody2D body;
        SpriteRenderer sprite; Vector2 home; bool dead;
        void Start() { hp = maxHp = boss ? 18 : 3; body = GetComponent<Rigidbody2D>(); sprite = GetComponent<SpriteRenderer>(); home = body.position; nextShot = Time.time + 2; }
        void FixedUpdate()
        {
            var game = GameSession.Instance; if (dead || game.Paused || game.Player == null) return;
            Vector2 delta = (Vector2)game.Player.transform.position - body.position;
            bool active = boss ? game.State.gateOpen && game.Player.transform.position.y > -10 : delta.magnitude < 6;
            if (active)
            {
                float speed = boss ? (hp <= maxHp / 2 ? 1.8f : 1.1f) : 1.65f;
                if (delta.magnitude > .7f) body.MovePosition(body.position + delta.normalized * speed * Time.fixedDeltaTime);
                if (boss && Time.time > nextShot)
                {
                    nextShot = Time.time + (hp <= maxHp / 2 ? 1.1f : 2f);
                    for (int i = -1; i <= 1; i++)
                    {
                        var shot = game.World.CreateSprite("Eco do Sentinela", "crystal", body.position, 80);
                        shot.transform.localScale = Vector3.one * .35f;
                        var bullet = shot.AddComponent<BossProjectile>(); bullet.direction = Quaternion.Euler(0, 0, i * 20) * delta.normalized;
                    }
                }
            }
            else if (Vector2.Distance(body.position, home) > .25f) body.MovePosition(Vector2.MoveTowards(body.position, home, Time.fixedDeltaTime));
            sprite.sortingOrder = 10 + Mathf.RoundToInt(-transform.position.y * 2);
            sprite.color = Time.time < nextHit ? new Color(1, .55f, .55f) : Color.white;
            if (boss && active) game.HUD.SetBoss(hp, maxHp);
            if (delta.magnitude < (boss ? 1.1f : .75f) && Time.time > contactTime) { contactTime = Time.time + .8f; game.Hurt(boss ? 2 : 1); }
        }
        public void Hit(int damage)
        {
            var game = GameSession.Instance;
            if (dead || Time.time < nextHit || (boss && (!game.State.gateOpen || game.Player.transform.position.y <= -10))) return;
            hp -= damage; nextHit = Time.time + .2f; AudioFeedback.Play("hit");
            if (hp > 0) return;
            dead = true; game.State.Collect(id); game.State.coins += boss ? 20 : 3;
            if (boss) { game.HUD.SetBoss(0, 0); game.Win(); }
            else game.HUD.Toast("+3 moedas");
            Destroy(gameObject);
        }
    }
    public sealed class BossProjectile : MonoBehaviour
    {
        public Vector2 direction; float lifetime = 4;
        void Update()
        {
            var game = GameSession.Instance; if (game.Paused) return;
            transform.position += (Vector3)(direction * (3.5f * Time.deltaTime)); lifetime -= Time.deltaTime;
            if (lifetime <= 0 || game.World.IsBlocked(transform.position)) { Destroy(gameObject); return; }
            if (game.Player != null && Vector2.Distance(transform.position, game.Player.transform.position) < .4f) { game.Hurt(1); Destroy(gameObject); }
        }
    }
}

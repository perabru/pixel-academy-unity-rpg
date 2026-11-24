using System.Collections.Generic;
using UnityEngine;

namespace Aurora
{
    public sealed class WorldBuilder : MonoBehaviour
    {
        public const int Width = 32, Height = 22;
        Transform root; GameObject gate;
        readonly Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
        readonly List<Interactable> runes = new List<Interactable>();
        readonly HashSet<Vector2Int> blocked = new HashSet<Vector2Int>();
        public string[] Rows { get; private set; }
        public string ZoneName(int zone) => zone == 0 ? "Vila da Aurora" : zone == 1 ? "Bosque dos Ecos" : "Templo do Farol";
        public Vector2 SafeSpawn(int zone) => zone == 2 ? new Vector2(5, -17) : new Vector2(5, -8);
        public bool IsBlocked(Vector2 point) => blocked.Contains(new Vector2Int(Mathf.RoundToInt(point.x), Mathf.RoundToInt(-point.y))) || point.x < 1 || point.x > 30 || point.y > -1 || point.y < -20;
        public Sprite Art(string name)
        {
            if (sprites.TryGetValue(name, out var sprite)) return sprite;
            var tex = Resources.Load<Texture2D>("Art/" + name);
            if (tex == null) { Debug.LogError("Asset ausente: " + name); return null; }
            tex.filterMode = FilterMode.Point;
            sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f), 32);
            sprites[name] = sprite; return sprite;
        }
        public GameObject CreateSprite(string name, string art, Vector2 position, int order = 0)
        {
            var obj = new GameObject(name); obj.transform.SetParent(root); obj.transform.position = position;
            var sr = obj.AddComponent<SpriteRenderer>(); sr.sprite = Art(art); sr.sortingOrder = order; return obj;
        }
        void Solid(string name, string art, int x, int y)
        {
            var obj = CreateSprite(name, art, new Vector2(x, -y), art == "tree" ? 3 : 1);
            obj.layer = 8; obj.AddComponent<BoxCollider2D>().size = Vector2.one;
            blocked.Add(new Vector2Int(x, y));
        }
        public void Build(int zone)
        {
            if (root != null) { root.gameObject.SetActive(false); Destroy(root.gameObject); }
            root = new GameObject(ZoneName(zone)).transform; runes.Clear(); blocked.Clear(); gate = null;
            char[][] map = new char[Height][];
            for (int y = 0; y < Height; y++)
            {
                map[y] = new char[Width];
                for (int x = 0; x < Width; x++)
                {
                    char tile = x == 0 || y == 0 || x == Width - 1 || y == Height - 1 ? '#' : '.';
                    if (zone == 0 && x > 10 && x < 19 && y > 2 && y < 6) tile = '#';
                    if (zone == 1 && ((x == 13 && y > 3 && y < 13) || (y == 15 && x > 17 && x < 28))) tile = '#';
                    if (zone == 1 && x > 20 && x < 26 && y > 2 && y < 7) tile = '~';
                    if (zone == 2 && y == 10 && x != 16) tile = '#';
                    map[y][x] = tile;
                    CreateSprite("Piso " + x + "," + y, zone == 2 ? "floor" : (y == 8 || x == 5 ? "path" : "grass"), new Vector2(x, -y), -10);
                    if (tile == '#') Solid("Obstáculo", zone == 2 ? "wall" : "tree", x, y);
                    if (tile == '~') Solid("Água", "water", x, y);
                }
            }
            Rows = new string[Height]; for (int y = 0; y < Height; y++) Rows[y] = new string(map[y]);
            if (zone == 0)
            {
                Add("iris", "elder", 11, 8, InteractionKind.Elder, "Íris · guardiã");
                Add("altar-vila", "shrine", 5, 6, InteractionKind.Shrine, "Altar · descansar e salvar");
                Add("saida-bosque", "gate", 29, 8, InteractionKind.Portal, "Bosque dos Ecos", 1);
                Add("bau-vila", "chest", 7, 15, InteractionKind.Chest, "Baú de viagem");
            }
            if (zone == 1)
            {
                Add("volta-vila", "gate", 2, 8, InteractionKind.Portal, "Vila da Aurora", 0);
                Add("entrada-templo", "gate", 29, 18, InteractionKind.Portal, "Templo do Farol", 2);
                Add("cristal-1", "crystal", 9, 5, InteractionKind.Crystal, "Cristal da clareira");
                Add("cristal-2", "crystal", 18, 4, InteractionKind.Crystal, "Cristal do lago");
                Add("cristal-3", "crystal", 26, 18, InteractionKind.Crystal, "Cristal das raízes");
                Add("bau-bosque", "chest", 20, 18, InteractionKind.Chest, "Suprimentos");
                Add("altar-bosque", "shrine", 16, 14, InteractionKind.Shrine, "Altar do bosque");
                SpawnEnemy("slime-1", 9, 7); SpawnEnemy("slime-2", 18, 6); SpawnEnemy("slime-3", 26, 19);
                SpawnEnemy("slime-4", 8, 17); SpawnEnemy("slime-5", 20, 12);
            }
            if (zone == 2)
            {
                Add("volta-bosque", "gate", 3, 18, InteractionKind.Portal, "Voltar ao bosque", 1);
                Add("altar-templo", "shrine", 7, 18, InteractionKind.Shrine, "Altar do templo");
                runes.Add(Add("runa-sol", "switch_off", 11, 15, InteractionKind.Rune, "Runa do Sol", 0));
                runes.Add(Add("runa-lua", "switch_off", 16, 15, InteractionKind.Rune, "Runa da Lua", 1));
                runes.Add(Add("runa-estrela", "switch_off", 21, 15, InteractionKind.Rune, "Runa da Estrela", 2));
                gate = CreateSprite("Selo do templo", "gate", new Vector2(16, -10), 4);
                gate.layer = 8; gate.AddComponent<BoxCollider2D>(); if (GameSession.Instance.State.gateOpen) OpenGate();
                else blocked.Add(new Vector2Int(16, 10));
                SpawnEnemy("sentinela", 16, 5, true); RefreshRunes();
            }
        }
        Interactable Add(string id, string art, int x, int y, InteractionKind kind, string label, int target = 0)
        {
            if ((kind == InteractionKind.Crystal || kind == InteractionKind.Chest) && GameSession.Instance.State.collectedIds.Contains(id)) return null;
            var obj = CreateSprite(label, art, new Vector2(x, -y), 3);
            var item = obj.AddComponent<Interactable>(); item.id = id; item.kind = kind; item.label = label; item.target = target; return item;
        }
        void SpawnEnemy(string id, int x, int y, bool boss = false)
        {
            if (GameSession.Instance.State.collectedIds.Contains(id) || (boss && GameSession.Instance.State.bossDefeated)) return;
            var obj = CreateSprite(id, boss ? "sentinel" : "slime", new Vector2(x, -y), 8);
            if (boss) obj.transform.localScale = Vector3.one * 1.8f;
            var body = obj.AddComponent<Rigidbody2D>(); body.gravityScale = 0; body.freezeRotation = true;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            obj.AddComponent<CircleCollider2D>().radius = boss ? .33f : .3f;
            var enemy = obj.AddComponent<EnemyController>(); enemy.id = id; enemy.boss = boss;
        }
        public void OpenGate() { if (gate != null) gate.SetActive(false); blocked.Remove(new Vector2Int(16, 10)); }
        public void RefreshRunes()
        {
            foreach (var rune in runes) if (rune != null) rune.GetComponent<SpriteRenderer>().sprite = Art(rune.target < GameSession.Instance.RuneProgress ? "switch_on" : "switch_off");
        }
    }
}

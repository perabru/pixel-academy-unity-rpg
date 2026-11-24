using System;
using UnityEngine;

namespace Aurora
{
    public sealed class GameSession : MonoBehaviour
    {
        public static GameSession Instance { get; private set; }
        public GameState State { get; private set; } = new GameState();
        public PlayerController Player { get; private set; }
        public GameHUD HUD { get; private set; }
        public WorldBuilder World { get; private set; }
        public bool Paused { get; private set; }
        public bool Started { get; private set; }
        public float InvulnerableUntil { get; private set; }
        public int RuneProgress { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void StartGame()
        {
            if (Instance == null) new GameObject("GameSession").AddComponent<GameSession>();
        }
        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this; DontDestroyOnLoad(gameObject);
            AudioListener.pause = false; Time.timeScale = 1;
            gameObject.AddComponent<AudioFeedback>();
            World = gameObject.AddComponent<WorldBuilder>();
            HUD = gameObject.AddComponent<GameHUD>();
        }
        void Start() { HUD.ShowMenu(); }
        void Update()
        {
            if (!Started) return;
            if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
            if (Input.GetKeyDown(KeyCode.Tab)) HUD.ToggleInventory();
        }
        public void Begin(bool continueGame)
        {
            State = continueGame ? SaveSystem.Load() : new GameState();
            Started = true; SetPaused(false); HUD.ClosePanel();
            Enter(State.sceneIndex, new Vector2(State.positionX, State.positionY), false);
        }
        public void Enter(int zone, Vector2 spawn, bool save = true)
        {
            State.sceneIndex = zone; RuneProgress = State.gateOpen ? 3 : 0;
            World.Build(zone);
            if (World.IsBlocked(spawn)) spawn = World.SafeSpawn(zone);
            var actor = World.CreateSprite("Exploradora", "hero", spawn, 10);
            var body = actor.AddComponent<Rigidbody2D>(); body.gravityScale = 0;
            body.freezeRotation = true; body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            var collider = actor.AddComponent<BoxCollider2D>(); collider.size = new Vector2(.45f, .4f); collider.offset = new Vector2(0, -.22f);
            Player = actor.AddComponent<PlayerController>();
            var camera = Camera.main;
            if (camera == null)
            {
                var cameraObject = new GameObject("Main Camera"); cameraObject.tag = "MainCamera";
                camera = cameraObject.AddComponent<Camera>(); cameraObject.AddComponent<AudioListener>();
            }
            camera.orthographic = true; camera.orthographicSize = 5.6f; camera.backgroundColor = new Color(.035f, .065f, .09f);
            var follow = camera.GetComponent<CameraFollow>() ?? camera.gameObject.AddComponent<CameraFollow>(); follow.target = actor.transform; follow.Snap();
            InvulnerableUntil = Time.time + 1.5f; HUD.Toast(World.ZoneName(zone));
            if (save) Save();
        }
        public void Save()
        {
            if (!Started || Player == null || State.hp <= 0) return;
            State.positionX = Player.transform.position.x; State.positionY = Player.transform.position.y;
            try { SaveSystem.Save(State); HUD.Toast("Progresso salvo neste computador."); }
            catch (Exception ex) { Debug.LogWarning(ex); HUD.Toast("Não foi possível salvar. Verifique o espaço e as permissões da pasta."); }
        }
        public void Hurt(int damage)
        {
            if (Paused || Time.time < InvulnerableUntil || !State.Damage(damage)) return;
            InvulnerableUntil = Time.time + 1; AudioFeedback.Play("hit");
            if (State.hp <= 0) { SetPaused(true); HUD.ShowDefeat(); }
        }
        public void Respawn()
        {
            State.hp = State.maxHp; State.coins = Mathf.Max(0, State.coins - 3);
            HUD.ClosePanel(); SetPaused(false); Enter(0, World.SafeSpawn(0));
        }
        public void Potion()
        {
            if (State.UsePotion()) { AudioFeedback.Play("pickup"); HUD.Toast("Poção usada: +3 pontos de vida."); }
            else HUD.Toast(State.potions == 0 ? "Sem poções. Troque 5 moedas com Íris." : "Sua vida já está cheia.");
        }
        public void Rune(int index)
        {
            if (State.gateOpen) return;
            if (index == RuneProgress) { RuneProgress++; AudioFeedback.Play("pickup"); }
            else { RuneProgress = index == 0 ? 1 : 0; HUD.Toast("A sequência recomeçou: Sol, Lua, Estrela."); }
            if (RuneProgress == 3) { State.gateOpen = true; World.OpenGate(); HUD.Toast("O selo se abriu. O Sentinela despertou."); AudioFeedback.Play("door"); Save(); }
            World.RefreshRunes();
        }
        public void Win()
        {
            if (State.bossDefeated) return;
            State.bossDefeated = true; State.hp = State.maxHp; State.coins += 30; Save();
            AudioFeedback.Play("win"); SetPaused(true); HUD.ShowVictory();
        }
        public void TogglePause()
        {
            if (State.hp <= 0) return;
            SetPaused(!Paused); if (Paused) HUD.ShowPause(); else HUD.ClosePanel();
        }
        public void SetPaused(bool value) { Paused = value; Time.timeScale = value ? 0 : 1; }
        void OnApplicationPause(bool pause) { if (pause && Started && State.hp > 0) { Save(); SetPaused(true); HUD.ShowPause(); } }
        void OnApplicationQuit() { Save(); }
        void OnDestroy() { if (Instance == this) { Instance = null; Time.timeScale = 1; } }
    }
}

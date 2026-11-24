using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Aurora
{
    public sealed class GameHUD : MonoBehaviour
    {
        Canvas canvas; Font font; RectTransform panel; Text stats, objective, hint, toast, boss;
        float toastUntil; bool inventory;
        readonly Color ink = new Color(.91f, .95f, .93f), gold = new Color(.98f, .79f, .41f);
        void Awake()
        {
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var root = new GameObject("Interface", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            root.transform.SetParent(transform); canvas = root.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = root.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720); scaler.matchWidthOrHeight = .5f;
            if (FindAnyObjectByType<EventSystem>() == null) new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            var top = Box(root.transform, "HUD", new Vector2(20, -20), new Vector2(580, 112), new Vector2(0, 1), new Color(.03f, .09f, .13f, .93f));
            stats = TextAt(top, "Estado", "", new Vector2(18, -14), new Vector2(545, 32), 22, gold);
            objective = TextAt(top, "Missão", "", new Vector2(18, -52), new Vector2(545, 52), 18, ink);
            boss = TextAt(root.transform, "Chefe", "", new Vector2(-360, -22), new Vector2(340, 42), 21, gold, new Vector2(1, 1));
            hint = TextAt(root.transform, "Interação", "", new Vector2(-400, 100), new Vector2(800, 35), 22, gold, new Vector2(.5f, 0)); hint.alignment = TextAnchor.MiddleCenter;
            toast = TextAt(root.transform, "Aviso", "", new Vector2(-500, 150), new Vector2(1000, 60), 22, ink, new Vector2(.5f, 0)); toast.alignment = TextAnchor.MiddleCenter;
            var controls = TextAt(root.transform, "Controles", "WASD / setas · mover     Espaço · atacar     E · interagir     Q · poção     Tab · mochila     Esc · pausa", new Vector2(-600, 35), new Vector2(1200, 35), 17, ink, new Vector2(.5f, 0)); controls.alignment = TextAnchor.MiddleCenter;
        }
        void Update()
        {
            var game = GameSession.Instance;
            if (!game.Started) { stats.text = "ECOS DE AURORA"; objective.text = "Uma aventura original · Pixel Academy"; return; }
            var s = game.State; stats.text = "VIDA " + s.hp + "/" + s.maxHp + "     MOEDAS " + s.coins + "     POÇÕES " + s.potions;
            objective.text = s.Objective();
            if (Time.unscaledTime > toastUntil) toast.text = "";
            if (s.sceneIndex != 2) boss.text = "";
        }
        RectTransform Box(Transform parent, string name, Vector2 position, Vector2 size, Vector2 anchor, Color color)
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(Image)); obj.transform.SetParent(parent, false);
            var r = obj.GetComponent<RectTransform>(); r.anchorMin = r.anchorMax = anchor; r.pivot = new Vector2(0, 1); r.anchoredPosition = position; r.sizeDelta = size;
            obj.GetComponent<Image>().color = color; return r;
        }
        Text TextAt(Transform parent, string name, string value, Vector2 position, Vector2 size, int sizeFont, Color color, Vector2? anchor = null)
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(Text)); obj.transform.SetParent(parent, false);
            var r = obj.GetComponent<RectTransform>(); r.anchorMin = r.anchorMax = anchor ?? new Vector2(0, 1); r.pivot = new Vector2(0, 1); r.anchoredPosition = position; r.sizeDelta = size;
            var t = obj.GetComponent<Text>(); t.font = font; t.text = value; t.fontSize = sizeFont; t.color = color; t.supportRichText = false; t.raycastTarget = false;
            t.horizontalOverflow = HorizontalWrapMode.Wrap; t.verticalOverflow = VerticalWrapMode.Overflow; return t;
        }
        void OpenPanel(string title, string body)
        {
            ClosePanel();
            panel = Box(canvas.transform, "Painel", new Vector2(-360, 265), new Vector2(720, 530), new Vector2(.5f, .5f), new Color(.025f, .065f, .10f, .985f));
            Box(panel, "Detalhe", Vector2.zero, new Vector2(720, 5), new Vector2(0, 1), gold);
            TextAt(panel, "Título", title, new Vector2(36, -32), new Vector2(648, 62), 36, gold);
            TextAt(panel, "Texto", body, new Vector2(36, -108), new Vector2(648, 200), 23, ink);
        }
        void Button(string title, int row, UnityAction action)
        {
            var r = Box(panel, title, new Vector2(36, -318 - row * 58), new Vector2(648, 48), new Vector2(0, 1), new Color(.12f, .30f, .32f));
            var button = r.gameObject.AddComponent<Button>(); button.onClick.AddListener(action);
            var text = TextAt(r, "Rótulo", title, Vector2.zero, r.sizeDelta, 22, ink); text.alignment = TextAnchor.MiddleCenter;
            if (row == 0 && EventSystem.current != null) EventSystem.current.SetSelectedGameObject(r.gameObject);
        }
        public void ShowMenu()
        {
            OpenPanel("ECOS DE AURORA", "O farol se apagou. Entre no bosque, recupere três cristais e descubra o segredo do templo.\n\nUma aventura 2D original para aprender Unity, criada para a Pixel Academy.");
            Button("Nova aventura", 0, () => GameSession.Instance.Begin(false));
            if (SaveSystem.Exists) Button("Continuar do salvamento", 1, () => GameSession.Instance.Begin(true));
            Button("Sair", 2, Application.Quit);
        }
        public void ShowPause()
        {
            OpenPanel("Uma pausa na jornada", "O jogo está pausado. O salvamento fica neste computador.\n\nAtalhos: Q para poção, E para interação e Tab para ver a mochila.");
            Button("Continuar", 0, () => GameSession.Instance.TogglePause());
            Button("Salvar agora", 1, () => GameSession.Instance.Save());
            Button("Salvar e sair", 2, () => { GameSession.Instance.Save(); Application.Quit(); });
        }
        public void Dialogue(string title, string message)
        { GameSession.Instance.SetPaused(true); OpenPanel(title, message); Button("Continuar", 0, () => { ClosePanel(); GameSession.Instance.SetPaused(false); }); }
        public void ShowShop()
        {
            GameSession.Instance.SetPaused(true);
            OpenPanel("Íris · provisões", GameSession.Instance.State.Objective() + "\n\nCada poção custa 5 moedas e recupera 3 pontos de vida. Descanse nos altares para recuperar toda a vida e salvar.");
            Button("Comprar poção · 5 moedas", 0, () => { bool bought = GameSession.Instance.State.BuyPotion(); Toast(bought ? "Poção guardada na mochila." : "Moedas insuficientes ou mochila cheia."); if (bought) GameSession.Instance.Save(); });
            Button("Voltar à aventura", 1, () => { ClosePanel(); GameSession.Instance.SetPaused(false); });
        }
        public void ToggleInventory()
        {
            var game = GameSession.Instance;
            if (game.State.hp <= 0) return;
            if (inventory) { ClosePanel(); game.SetPaused(false); return; }
            if (game.Paused) return;
            game.SetPaused(true); var s = game.State;
            OpenPanel("Mochila", "Poções: " + s.potions + "\nMoedas: " + s.coins + "\nCristais: " + s.crystals + "/3\nChave do templo: " + (s.templeKey ? "obtida" : "a encontrar")); inventory = true;
            Button("Usar poção e voltar", 0, () => { game.Potion(); ClosePanel(); game.SetPaused(false); });
            Button("Fechar mochila", 1, () => { ClosePanel(); game.SetPaused(false); });
        }
        public void ShowDefeat()
        { OpenPanel("A luz ainda espera", "Você perdeu suas forças. Volte ao altar da vila com vida completa. A recuperação custa até 3 moedas; os cristais e as conquistas permanecem."); Button("Voltar ao altar", 0, () => GameSession.Instance.Respawn()); }
        public void ShowVictory()
        { OpenPanel("AURORA VOLTOU A BRILHAR", "O Sentinela caiu e a luz do farol alcança o bosque outra vez. Sua aventura foi salva.\n\nVolte a Íris para encerrar a história. Obrigado por jogar!"); Button("Continuar explorando", 0, () => { ClosePanel(); GameSession.Instance.SetPaused(false); }); }
        public void ClosePanel() { if (panel != null) { panel.gameObject.SetActive(false); Destroy(panel.gameObject); panel = null; } inventory = false; }
        public void Toast(string message) { toast.text = message; toastUntil = Time.unscaledTime + 4; }
        public void SetInteraction(string message) { hint.text = message; }
        public void SetBoss(int hp, int max) { boss.text = max > 0 ? "SENTINELA  " + hp + " / " + max : ""; }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArcadeHud : MonoBehaviour
{
    private static readonly Color Cyan = new Color(0.36f, 0.94f, 0.93f);
    private static readonly Color Muted = new Color(0.48f, 0.61f, 0.7f);
    private static readonly Color Ink = new Color(0.018f, 0.032f, 0.06f, 0.96f);
    private Font _font;
    private Sprite _barSprite;
    private RectTransform _root;
    private Text _score;
    private Text _best;
    private Text _sector;
    private Text _time;
    private Text _health;
    private Text _bomb;
    private Text _combo;
    private Text _auto;
    private Text _notice;
    private Text _noticeDetail;
    private Text _sound;
    private Image _healthFill;
    private Image _bombFill;
    private Image _comboFill;
    private Image _flash;
    private CanvasGroup _noticeGroup;
    private GameObject _overlay;
    private Text _overlayTitle;
    private Text _overlayDetail;
    private Button _resume;
    private GameSession _session;
    private float _nextRefresh;
    private RectTransform _topBar;
    private RectTransform _bottomBar;
    private RectTransform _card;
    private Text _scoreCaption;
    private Text _controls;
    private Button _pauseButton;
    private Button _soundButton;
    private LayoutState[] _wideLayout;
    private Vector2 _layoutSize;

    // Keep the authored landscape layout so resizing does not accumulate offsets.
    private sealed class LayoutState
    {
        public RectTransform Rect;
        public Vector2 AnchorMin, AnchorMax, OffsetMin, OffsetMax;
        public Text Text;
        public int FontSize;
        public TextAnchor Alignment;

        public void Restore()
        {
            Rect.anchorMin = AnchorMin;
            Rect.anchorMax = AnchorMax;
            Rect.offsetMin = OffsetMin;
            Rect.offsetMax = OffsetMax;
            if (Text == null) return;
            Text.fontSize = FontSize;
            Text.resizeTextMaxSize = FontSize;
            Text.alignment = Alignment;
        }
    }

    private void Start()
    {
        _session = GetComponent<GameSession>();
        _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        _barSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f, 1f);
        GameObject canvasObject = new GameObject("Flight HUD", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(transform, false);
        _root = canvasObject.GetComponent<RectTransform>();
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);
        scaler.matchWidthOrHeight = 0.5f;
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject events = new GameObject("UI Input", typeof(EventSystem), typeof(StandaloneInputModule));
            events.transform.SetParent(transform, false);
        }

        RectTransform top = Panel(_root, "Top bar", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -82), Vector2.zero, Ink);
        Panel(top, "Accent", Vector2.zero, Vector2.right, Vector2.zero, new Vector2(0, 1), new Color(0.2f, 0.65f, 0.7f, 0.5f));
        _scoreCaption = Label(top, "SCORE", 12, Muted, TextAnchor.MiddleLeft, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(28, 11), new Vector2(190, 29));
        _score = Label(top, "000000", 29, Color.white, TextAnchor.MiddleLeft, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(26, -25), new Vector2(210, 12));
        _best = Label(top, "BEST  000000", 12, Muted, TextAnchor.MiddleLeft, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(220, -19), new Vector2(400, 10));
        _sector = Label(top, "SECTOR 01", 16, Cyan, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-110, 0), new Vector2(110, 25));
        _time = Label(top, "00:00  /  DEEP SPACE", 11, Muted, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-130, -23), new Vector2(130, -2));
        _health = Label(top, "HULL  10 / 10", 13, Color.white, TextAnchor.MiddleLeft, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-325, 2), new Vector2(-165, 28));
        _healthFill = Bar(top, "Hull", new Vector2(1, 0.5f), new Vector2(-325, -18), new Vector2(150, 7), Cyan);
        _bomb = Label(top, "B  /  PULSE READY", 12, Cyan, TextAnchor.MiddleLeft, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-150, 2), new Vector2(-16, 28));
        _bombFill = Bar(top, "Bomb", new Vector2(1, 0.5f), new Vector2(-150, -18), new Vector2(122, 7), Cyan);

        RectTransform bottom = Panel(_root, "Controls", Vector2.zero, Vector2.right, Vector2.zero, new Vector2(0, 48), Ink);
        _controls = Label(bottom, "WASD / ARROWS  MOVE     SHIFT  FOCUS     SPACE  FIRE     B  BOMB", 12, Muted, TextAnchor.MiddleLeft,
            Vector2.zero, new Vector2(0.65f, 1), new Vector2(26, 0), new Vector2(0, 0));
        _auto = Label(bottom, "1  AUTO OFF", 12, Cyan, TextAnchor.MiddleRight, new Vector2(1, 0), Vector2.one, new Vector2(-420, 0), new Vector2(-270, 0));
        Button pause = MakeButton(bottom, "P  PAUSE", new Vector2(1, 0.5f), new Vector2(-195, 0), new Vector2(116, 30), () => _session.TogglePause());
        pause.GetComponent<Image>().color = new Color(0.1f, 0.18f, 0.23f);
        Button sound = MakeButton(bottom, "M  SOUND ON", new Vector2(1, 0.5f), new Vector2(-70, 0), new Vector2(116, 30), () => _session.ToggleSound());
        _sound = sound.GetComponentInChildren<Text>();

        _combo = Label(_root, "", 18, Cyan, TextAnchor.MiddleCenter, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(-170, 70), new Vector2(170, 98));
        _comboFill = Bar(_root, "Combo window", new Vector2(0.5f, 0), new Vector2(-70, 65), new Vector2(140, 2), Cyan);
        RectTransform notice = Panel(_root, "Announcement", new Vector2(0.5f, 0.76f), new Vector2(0.5f, 0.76f), new Vector2(-280, -44), new Vector2(280, 44), Color.clear);
        _noticeGroup = notice.gameObject.AddComponent<CanvasGroup>();
        _notice = Label(notice, "", 30, Color.white, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, new Vector2(0, 24), Vector2.zero);
        _noticeDetail = Label(notice, "", 12, Cyan, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Vector2.zero, new Vector2(0, -36));
        _flash = Panel(_root, "Damage flash", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, Color.clear).GetComponent<Image>();

        RectTransform overlay = Panel(_root, "Pause and results", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Color(0.005f, 0.012f, 0.035f, 0.88f));
        overlay.GetComponent<Image>().raycastTarget = true;
        _overlay = overlay.gameObject;
        RectTransform card = Panel(overlay, "Briefing card", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-245, -175), new Vector2(245, 175), Ink);
        Panel(card, "Card accent", new Vector2(0, 1), Vector2.one, new Vector2(0, -3), Vector2.zero, Cyan);
        Label(card, "O R B I T   B R E A K E R", 12, Cyan, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, new Vector2(24, 270), new Vector2(-24, -36));
        _overlayTitle = Label(card, "PAUSED", 39, Color.white, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, new Vector2(24, 185), new Vector2(-24, -83));
        _overlayDetail = Label(card, "", 15, Muted, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, new Vector2(24, 115), new Vector2(-24, -158));
        _resume = MakeButton(card, "RESUME  /  P", new Vector2(0.5f, 0), new Vector2(0, 87), new Vector2(300, 40), () => _session.TogglePause());
        MakeButton(card, "RETRY  /  R", new Vector2(0.5f, 0), new Vector2(0, 37), new Vector2(300, 40), () => _session.Restart());
        _overlay.SetActive(false);
        _topBar = top;
        _bottomBar = bottom;
        _card = card;
        _pauseButton = pause;
        _soundButton = sound;
        CaptureWideLayout();
        Canvas.ForceUpdateCanvases();
        ApplyResponsiveLayout();
        Refresh();
    }

    private void CaptureWideLayout()
    {
        RectTransform[] rects = _root.GetComponentsInChildren<RectTransform>(true);
        _wideLayout = new LayoutState[rects.Length - 1];
        for (int i = 1; i < rects.Length; i++)
        {
            RectTransform rect = rects[i];
            Text text = rect.GetComponent<Text>();
            _wideLayout[i - 1] = new LayoutState
            {
                Rect = rect, AnchorMin = rect.anchorMin, AnchorMax = rect.anchorMax,
                OffsetMin = rect.offsetMin, OffsetMax = rect.offsetMax, Text = text,
                FontSize = text != null ? text.fontSize : 0,
                Alignment = text != null ? text.alignment : TextAnchor.MiddleCenter
            };
        }
    }

    private void LateUpdate()
    {
        if (_root != null && _wideLayout != null && _root.rect.size != _layoutSize)
            ApplyResponsiveLayout();
    }

    private void ApplyResponsiveLayout()
    {
        _layoutSize = _root.rect.size;
        foreach (LayoutState state in _wideLayout) state.Restore();
        _controls.text = "WASD / ARROWS  MOVE     SHIFT  FOCUS     SPACE  FIRE     B  BOMB";
        // CanvasScaler gives us logical UI units, independent of the device resolution.
        if (_layoutSize.x >= 1000f) return;

        _topBar.offsetMin = new Vector2(0, -166);
        Place(_scoreCaption.rectTransform, 0, 0.5f, 1, 24, -30, -12, -10);
        Place(_score.rectTransform, 0, 0.5f, 1, 22, -73, -12, -32);
        Place(_best.rectTransform, 0, 0.5f, 1, 24, -97, -12, -75);
        Place(_sector.rectTransform, 0.5f, 1, 1, 12, -43, -24, -12);
        Place(_time.rectTransform, 0.5f, 1, 1, 12, -73, -24, -47);
        _sector.alignment = _time.alignment = TextAnchor.MiddleRight;
        Place(_health.rectTransform, 0, 0.5f, 1, 24, -135, -12, -107);
        Place(_bomb.rectTransform, 0.5f, 1, 1, 12, -135, -24, -107);
        Place((RectTransform)_healthFill.transform.parent, 0, 0.5f, 1, 24, -150, -12, -143);
        Place((RectTransform)_bombFill.transform.parent, 0.5f, 1, 1, 12, -150, -24, -143);
        SetFontSize(_scoreCaption, 15);
        SetFontSize(_score, 34);
        SetFontSize(_best, 16);
        SetFontSize(_sector, 22);
        SetFontSize(_time, 16);
        SetFontSize(_health, 19);
        SetFontSize(_bomb, 19);

        _bottomBar.offsetMax = new Vector2(0, 142);
        _controls.text = "WASD / ARROWS  MOVE     SHIFT  FOCUS\nSPACE  FIRE     B  BOMB";
        Place(_controls.rectTransform, 0, 1, 0, 24, 72, -24, 132);
        _controls.alignment = TextAnchor.MiddleCenter;
        SetFontSize(_controls, 18);
        Place(_auto.rectTransform, 0, 1f / 3f, 0, 20, 16, -6, 58);
        _auto.alignment = TextAnchor.MiddleCenter;
        Place((RectTransform)_pauseButton.transform, 1f / 3f, 2f / 3f, 0, 6, 16, -6, 58);
        Place((RectTransform)_soundButton.transform, 2f / 3f, 1, 0, 6, 16, -20, 58);
        SetFontSize(_auto, 17);
        SetFontSize(_pauseButton.GetComponentInChildren<Text>(), 17);
        SetFontSize(_sound, 17);
        Place(_combo.rectTransform, 0, 1, 0, 24, 159, -24, 191);
        SetFontSize(_combo, 21);
        Place((RectTransform)_comboFill.transform.parent, 0.35f, 0.65f, 0, 0, 153, 0, 156);
        Place((RectTransform)_noticeGroup.transform, 0, 1, 0.76f, 24, -44, -24, 44);
        float cardWidth = Mathf.Min(490f, _layoutSize.x - 48f);
        _card.offsetMin = new Vector2(-cardWidth * 0.5f, -175);
        _card.offsetMax = new Vector2(cardWidth * 0.5f, 175);
    }

    private static void Place(RectTransform rect, float left, float right, float anchorY,
        float insetLeft, float bottom, float insetRight, float top)
    {
        rect.anchorMin = new Vector2(left, anchorY);
        rect.anchorMax = new Vector2(right, anchorY);
        rect.offsetMin = new Vector2(insetLeft, bottom);
        rect.offsetMax = new Vector2(insetRight, top);
    }

    private static void SetFontSize(Text text, int size)
    {
        text.fontSize = size;
        text.resizeTextMaxSize = size;
    }

    private void Update()
    {
        if (_session == null || _root == null) return;
        _noticeGroup.alpha = Mathf.Clamp01(_session.NoticeRemaining * 2f);
        _flash.color = new Color(1f, 0.15f, 0.2f, CombatFeedback.DamageFlash * 0.5f);
        _comboFill.fillAmount = _session.ComboRemaining;
        if (Time.unscaledTime < _nextRefresh) return;
        _nextRefresh = Time.unscaledTime + 0.08f;
        Refresh();
    }

    private void Refresh()
    {
        _score.text = _session.Score.ToString("000000");
        _best.text = "BEST  " + _session.BestScore.ToString("000000");
        _sector.text = "SECTOR " + _session.Sector.ToString("00");
        int seconds = Mathf.FloorToInt(_session.Elapsed);
        _time.text = (seconds / 60).ToString("00") + ":" + (seconds % 60).ToString("00") + "  /  DEEP SPACE";
        Player player = _session.Player;
        int health = player != null ? player.Health : 0;
        int maximum = player != null ? player.MaxHealth : 10;
        _health.text = "HULL  " + health + " / " + maximum;
        _healthFill.fillAmount = (float)health / maximum;
        _healthFill.color = health <= maximum * 0.3f ? new Color(1f, 0.35f, 0.4f) : Cyan;
        float cooldown = _session.Bomb != null ? _session.Bomb.CoolTimer : 0f;
        _bomb.text = cooldown <= 0f ? "B  /  PULSE READY" : "B  /  " + cooldown.ToString("0.0") + "s";
        _bombFill.fillAmount = _session.Bomb != null ? 1f - cooldown / _session.Bomb.FireRate : 0f;
        _bomb.color = cooldown <= 0f ? Cyan : Muted;
        _combo.text = _session.Combo > 1 ? _session.Combo + " CHAIN   /   x" + _session.Multiplier : "";
        _comboFill.transform.parent.gameObject.SetActive(_session.Combo > 1);
        _auto.text = "1  AUTO " + (_session.Fire != null && _session.Fire.AutoFireMode ? "ON" : "OFF");
        _sound.text = _session.IsMuted ? "M  SOUND OFF" : "M  SOUND ON";
        _notice.text = _session.Notice;
        _noticeDetail.text = _session.NoticeDetail;
        bool overlayVisible = _session.IsPaused || _session.IsGameOver;
        _overlay.SetActive(overlayVisible);
        if (!overlayVisible) return;
        _overlayTitle.text = _session.IsGameOver ? "RUN COMPLETE" : "PAUSED";
        _overlayDetail.text = _session.IsGameOver
            ? "SCORE  " + _session.Score.ToString("000000") + "    /    BEST  " + _session.BestScore.ToString("000000") + "\n" + _session.Kills + " TARGETS CLEARED   ·   SECTOR " + _session.Sector.ToString("00")
            : "Take a breath. Your flight is on hold.\nP / ESC to resume  ·  R to start a new run";
        _resume.gameObject.SetActive(!_session.IsGameOver);
    }

    private static RectTransform Panel(Transform parent, string name, Vector2 min, Vector2 max, Vector2 offsetMin, Vector2 offsetMax, Color color)
    {
        var obj = new GameObject(name, typeof(RectTransform), typeof(Image));
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
        Image image = obj.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return rect;
    }

    private Text Label(Transform parent, string content, int size, Color color, TextAnchor alignment, Vector2 min, Vector2 max, Vector2 offsetMin, Vector2 offsetMax)
    {
        var obj = new GameObject("Label", typeof(RectTransform), typeof(Text));
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
        Text text = obj.GetComponent<Text>();
        text.font = _font;
        text.fontSize = size;
        text.color = color;
        text.alignment = alignment;
        text.text = content;
        text.raycastTarget = false;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = Mathf.Min(size, 12);
        text.resizeTextMaxSize = size;
        return text;
    }

    private Image Bar(Transform parent, string name, Vector2 anchor, Vector2 position, Vector2 size, Color color)
    {
        RectTransform background = Panel(parent, name, anchor, anchor, position, position + size, new Color(0.12f, 0.22f, 0.28f));
        Image fill = Panel(background, "Fill", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, color).GetComponent<Image>();
        fill.sprite = _barSprite;
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillOrigin = 0;
        return fill;
    }

    private Button MakeButton(Transform parent, string label, Vector2 anchor, Vector2 center, Vector2 size, UnityEngine.Events.UnityAction action)
    {
        RectTransform rect = Panel(parent, label, anchor, anchor, center - size * 0.5f, center + size * 0.5f, new Color(0.08f, 0.23f, 0.28f));
        rect.GetComponent<Image>().raycastTarget = true;
        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = rect.GetComponent<Image>();
        // Space belongs to firing; clicking a menu must not leave a submit target selected.
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(0.6f, 1f, 1f);
        colors.pressedColor = new Color(0.35f, 0.7f, 0.7f);
        button.colors = colors;
        button.onClick.AddListener(action);
        Label(rect, label, 12, Cyan, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        return button;
    }

    private void OnDestroy()
    {
        if (_barSprite != null) Destroy(_barSprite);
    }
}

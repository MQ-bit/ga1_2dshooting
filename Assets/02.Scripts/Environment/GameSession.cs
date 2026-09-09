using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }
    public static bool InputBlocked => Instance != null && (Instance.IsPaused || Instance.IsGameOver);
    [SerializeField, Min(5f)] private float _sectorDuration = 25f;
    [SerializeField, Min(0.5f)] private float _comboWindow = 3f;
    public Player Player { get; private set; }
    public PlayerBomb Bomb { get; private set; }
    public PlayerFire Fire { get; private set; }
    public bool IsPaused { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsMuted { get; private set; }
    public int Score { get; private set; }
    public int BestScore { get; private set; }
    public int Kills { get; private set; }
    public int Combo { get; private set; }
    public int Multiplier => Mathf.Clamp(1 + Combo / 5, 1, 5);
    public int Sector => 1 + Mathf.FloorToInt(Elapsed / Mathf.Max(5f, _sectorDuration));
    public float Elapsed { get; private set; }
    public float ComboRemaining => Mathf.Clamp01((_comboUntil - Time.time) / _comboWindow);
    public string Notice { get; private set; }
    public string NoticeDetail { get; private set; }
    public float NoticeRemaining => Mathf.Max(0f, _noticeUntil - Time.unscaledTime);
    private float _comboUntil;
    private float _noticeUntil;
    private float _endTime;
    private int _sector = 1;
    private bool _restarting;
    private const string BestKey = "OrbitBreaker.BestScore";
    private const string MuteKey = "OrbitBreaker.Muted";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() { Instance = null; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Time.timeScale = 1f;
        BestScore = PlayerPrefs.GetInt(BestKey, 0);
        IsMuted = PlayerPrefs.GetInt(MuteKey, 0) == 1;
        gameObject.AddComponent<CombatFeedback>();
        gameObject.AddComponent<SpaceBackdrop>();
        gameObject.AddComponent<ArcadeHud>();
    }

    private void Start()
    {
        Player = FindFirstObjectByType<Player>();
        if (Player != null)
        {
            Bomb = Player.GetComponent<PlayerBomb>();
            Fire = Player.GetComponent<PlayerFire>();
        }
        Announce("ORBIT BREAKER", "SECTOR 01  /  CLEAR THE APPROACH", 3.5f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) ToggleSound();
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) TogglePause();
        if ((IsGameOver || IsPaused) && Input.GetKeyDown(KeyCode.R)) Restart();
        if (IsGameOver)
        {
            // Let the final explosion finish before freezing the playfield.
            if (Time.unscaledTime - _endTime > 0.65f) Time.timeScale = 0f;
            return;
        }
        if (IsPaused) return;
        Elapsed += Time.deltaTime;
        if (Time.time > _comboUntil) Combo = 0;
        if (Sector != _sector)
        {
            _sector = Sector;
            Announce("SECTOR " + Sector.ToString("00"), "ENEMY ACTIVITY INCREASING", 2.5f);
        }
    }

    public void RegisterKill()
    {
        if (InputBlocked) return;
        Combo = Time.time <= _comboUntil ? Combo + 1 : 1;
        _comboUntil = Time.time + _comboWindow;
        Kills++;
        Score += 100 * Multiplier;
        BestScore = Mathf.Max(BestScore, Score);
    }

    public void Announce(string title, string detail, float duration)
    {
        Notice = title;
        NoticeDetail = detail;
        _noticeUntil = Time.unscaledTime + duration;
    }

    public void EndRun()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        IsPaused = false;
        _endTime = Time.unscaledTime;
        SavePreferences();
    }

    public void TogglePause()
    {
        if (IsGameOver || _restarting) return;
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;
    }

    public void ToggleSound()
    {
        IsMuted = !IsMuted;
        PlayerPrefs.SetInt(MuteKey, IsMuted ? 1 : 0);
    }

    public void Restart()
    {
        if (_restarting) return;
        _restarting = true;
        SavePreferences();
        Time.timeScale = 1f;
        Scene scene = gameObject.scene;
        SceneManager.LoadSceneAsync(scene.buildIndex >= 0 ? scene.buildIndex : SceneManager.GetActiveScene().buildIndex);
    }

    private void SavePreferences()
    {
        PlayerPrefs.SetInt(BestKey, Mathf.Max(BestScore, PlayerPrefs.GetInt(BestKey, 0)));
        PlayerPrefs.SetInt(MuteKey, IsMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void OnApplicationFocus(bool focused)
    {
        if (!focused && !IsPaused && !IsGameOver) TogglePause();
    }

    private void OnDestroy()
    {
        if (Instance != this) return;
        SavePreferences();
        Time.timeScale = 1f;
        Instance = null;
    }
}

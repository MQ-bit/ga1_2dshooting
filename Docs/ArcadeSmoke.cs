// Copy into Assets/Editor of an isolated project, then run Unity with
// -batchmode -projectPath <clone> -executeMethod ArcadeSmoke.Run -logFile <log>.
// The original editor and its open scene are never controlled by this test.
using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public static class ArcadeSmoke
{
    private const string Active = "ArcadeSmoke.Active";
    private static double _started;
    private static int _stage;
    private static Enemy _bombTarget;
    private static Bullet _friendlyShot;
    private static int _scoreBeforeBomb;
    private static float _bombSpawnedAt;
    private static double _endedAt;

    static ArcadeSmoke()
    {
        if (SessionState.GetBool("ArcadeSmoke.FinishPending", false)) EditorApplication.delayCall += RestoreAndExit;
        if (SessionState.GetBool(Active, false)) EditorApplication.update += Tick;
    }

    public static void Run()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Run only in an isolated batch project.");
        Directory.CreateDirectory("Artifacts");
        File.WriteAllText("Artifacts/checks.txt", "");
        _started = 0;
        _stage = 0;
        SessionState.SetInt("ArcadeSmoke.Best", PlayerPrefs.GetInt("OrbitBreaker.BestScore", 0));
        SessionState.SetInt("ArcadeSmoke.Muted", PlayerPrefs.GetInt("OrbitBreaker.Muted", 0));
        SessionState.SetBool(Active, true);
        EditorApplication.update -= Tick;
        EditorApplication.update += Tick;
        EditorSceneManager.OpenScene("Assets/01.Scenes/SampleScene.unity");
        EditorApplication.isPlaying = true;
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
        File.AppendAllText("Artifacts/checks.txt", "PASS " + message + "\n");
    }

    private static void Tick()
    {
        if (!EditorApplication.isPlaying || EditorApplication.isCompiling) return;
        try
        {
            GameSession session = GameSession.Instance;
            if (session == null || session.Player == null && _stage < 4) return;
            if (session.IsPaused) session.TogglePause();
            if (_started == 0) _started = EditorApplication.timeSinceStartup;
            double elapsed = EditorApplication.timeSinceStartup - _started;
            if (_stage == 0 && elapsed > 1)
            {
                foreach (EnemySpawner spawner in Object.FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None)) spawner.enabled = false;
                foreach (Enemy enemy in Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None)) Object.Destroy(enemy.gameObject);
                session.Fire.AutoFireMode = false;
                Check(Object.FindFirstObjectByType<ArcadeHud>() != null, "HUD is attached in SampleScene");
                Check(Object.FindFirstObjectByType<Canvas>() != null, "HUD canvas exists");
                Check(session.Bomb.BombPrefab != null, "Bomb prefab reference is valid");
                Check(session.Fire.BulletPrefab != null, "Bullet prefab reference is valid");
                int health = session.Player.Health;
                session.Player.TakeDamage(2);
                session.Player.TakeDamage(2);
                Check(session.Player.Health == health - 2, "Consecutive hits are blocked by invulnerability");
                session.Player.TryApplyItem(Item.ItemType.HealthRecovery);
                Check(session.Player.Health == session.Player.MaxHealth, "Repair is clamped to maximum hull");
                float interval = session.Fire.CoolTime;
                session.Player.TryApplyItem(Item.ItemType.AttackSpeedUp);
                Check(session.Fire.CoolTime < interval, "Attack upgrade shortens shot interval");
                for (int i = 0; i < 100; i++) session.Fire.IncreaseAttackSpeed(0.2f);
                Check(session.Fire.CoolTime >= 0.0999f, "Attack interval respects its lower bound");
                session.Player.GetComponent<Rigidbody2D>().position = new Vector2(8f, 8f);
                _stage = 1;
            }
            else if (_stage == 1 && elapsed > 1.5)
            {
                PlayerMove move = session.Player.GetComponent<PlayerMove>();
                Check(session.Player.transform.position.x <= move.MaxPositionX + 0.01f && session.Player.transform.position.y <= move.MaxPositionY + 0.01f,
                    "Player movement clamps to playfield bounds");
                session.Player.GetComponent<Rigidbody2D>().position = new Vector2(0, -3.3f);
                Enemy prefab = AssetDatabase.LoadAssetAtPath<Enemy>("Assets/03.Prefabs/Enemy/DownwardEnemy.prefab");
                Enemy target = Object.Instantiate(prefab, new Vector3(0, 2, 0), Quaternion.identity);
                int score = session.Score;
                target.TakeDamage(int.MaxValue);
                target.TakeDamage(int.MaxValue);
                Check(session.Score == score + 100, "Duplicate lethal hits score once");
                _scoreBeforeBomb = session.Score;
                _bombTarget = Object.Instantiate(prefab, new Vector3(2f, 0, 0), Quaternion.identity);
                _bombTarget.enabled = false;
                GameObject bomb = Object.Instantiate(session.Bomb.BombPrefab, Vector3.zero, Quaternion.identity);
                bomb.GetComponent<Bomb>().MoveSpeed = 0f;
                _bombSpawnedAt = Time.time;
                _friendlyShot = Object.Instantiate(session.Fire.BulletPrefab, new Vector3(0, -3.3f, 0), Quaternion.identity).GetComponent<Bullet>();
                _friendlyShot.MoveSpeed = 0f;
                _stage = 2;
            }
            else if (_stage == 2 && elapsed > 2.3)
            {
                Check(_friendlyShot != null, "Friendly bullet survives contact with the player");
                Capture("Artifacts/flight.png");
                _stage = 3;
            }
            else if (_stage == 3 && Time.time > _bombSpawnedAt + 3.3f)
            {
                Check(_bombTarget == null, "Expanding bomb reaches a target two world units away");
                Check(session.Score > _scoreBeforeBomb, "Bomb kill awards score");
                Check(Object.FindObjectsByType<Bomb>(FindObjectsSortMode.None).Length == 0, "Bomb expires after its lifetime");
                session.TogglePause();
                Check(session.IsPaused && Time.timeScale == 0, "Pause freezes game time");
                Capture("Artifacts/paused.png");
                session.TogglePause();
                Check(!session.IsPaused && Time.timeScale == 1, "Resume restores game time");
                session.Player.TakeDamage(int.MaxValue);
                Check(session.IsGameOver, "Player death ends the run");
                _endedAt = EditorApplication.timeSinceStartup;
                _stage = 4;
            }
            else if (_stage == 4 && EditorApplication.timeSinceStartup > _endedAt + 1.2)
            {
                Capture("Artifacts/results.png");
                Check(Time.timeScale == 0, "Game over freezes after the final explosion");
                session.Restart();
                _stage = 5;
            }
            else if (_stage == 5 && elapsed > 7 && session.Player != null)
            {
                Check(!session.IsGameOver && session.Score == 0 && Time.timeScale == 1, "Restart resets score, player and game time");
                Check(Object.FindObjectsByType<GameSession>(FindObjectsSortMode.None).Length == 1, "Restart leaves one session");
                File.WriteAllText("Artifacts/result.txt", "PASS");
                Finish(0);
            }
            if (elapsed > 45) throw new TimeoutException("Runtime validation timed out.");
        }
        catch (Exception exception)
        {
            File.WriteAllText("Artifacts/result.txt", exception.ToString());
            Debug.LogException(exception);
            Finish(1);
        }
    }

    private static void Capture(string path)
    {
        Object.FindFirstObjectByType<ArcadeHud>().SendMessage("Refresh", SendMessageOptions.DontRequireReceiver);
        Camera camera = Camera.main;
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        RenderTexture target = new RenderTexture(1280, 720, 24);
        RenderTexture previous = RenderTexture.active;
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = camera;
        canvas.planeDistance = 1;
        camera.targetTexture = target;
        Canvas.ForceUpdateCanvases();
        camera.Render();
        RenderTexture.active = target;
        Texture2D image = new Texture2D(1280, 720, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
        image.Apply();
        File.WriteAllBytes(path, image.EncodeToPNG());
        camera.targetTexture = null;
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        RenderTexture.active = previous;
        Object.Destroy(image);
        Object.Destroy(target);
    }

    private static void Finish(int code)
    {
        SessionState.SetBool(Active, false);
        SessionState.SetBool("ArcadeSmoke.FinishPending", true);
        SessionState.SetInt("ArcadeSmoke.ExitCode", code);
        EditorApplication.update -= Tick;
        EditorApplication.isPlaying = false;
        EditorApplication.delayCall += RestoreAndExit;
    }

    private static void RestoreAndExit()
    {
        if (EditorApplication.isPlaying) { EditorApplication.delayCall += RestoreAndExit; return; }
        SessionState.SetBool("ArcadeSmoke.FinishPending", false);
        PlayerPrefs.SetInt("OrbitBreaker.BestScore", SessionState.GetInt("ArcadeSmoke.Best", 0));
        PlayerPrefs.SetInt("OrbitBreaker.Muted", SessionState.GetInt("ArcadeSmoke.Muted", 0));
        PlayerPrefs.Save();
        EditorApplication.Exit(SessionState.GetInt("ArcadeSmoke.ExitCode", 1));
    }
}

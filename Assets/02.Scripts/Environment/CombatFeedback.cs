using System.Collections.Generic;
using UnityEngine;

public class CombatFeedback : MonoBehaviour
{
    private sealed class Pulse
    {
        public LineRenderer Ring;
        public float Age;
        public float Duration;
        public float Radius;
        public Color Color;
    }

    private static CombatFeedback _instance;
    private readonly List<Pulse> _pulses = new List<Pulse>(48);
    private Material _lineMaterial;
    private Camera _camera;
    private Vector3 _baseCameraPosition;
    private float _shake;
    private AudioSource _audio;
    private AudioClip _shot;
    private AudioClip _impact;
    private AudioClip _explosion;
    private AudioClip _pickup;
    private float _nextImpactSound;
    public static float DamageFlash { get; private set; }

    private void Awake()
    {
        _instance = this;
        DamageFlash = 0f;
        _lineMaterial = new Material(Shader.Find("Sprites/Default"));
        _camera = Camera.main;
        if (_camera != null) _baseCameraPosition = _camera.transform.position;
        _audio = gameObject.AddComponent<AudioSource>();
        _audio.playOnAwake = false;
        _audio.spatialBlend = 0f;
        _audio.volume = 0.35f;
        _shot = Tone("Laser", 0.065f, 1100f, 260f, 0.08f);
        _impact = Tone("Hit", 0.09f, 300f, 90f, 0.4f);
        _explosion = Tone("Pulse", 0.4f, 120f, 28f, 0.75f);
        _pickup = Tone("Upgrade", 0.2f, 600f, 1450f, 0f);
    }

    private static AudioClip Tone(string name, float duration, float start, float end, float noise)
    {
        const int rate = 22050;
        float[] samples = new float[Mathf.CeilToInt(duration * rate)];
        double phase = 0;
        var random = new System.Random(37);
        for (int i = 0; i < samples.Length; i++)
        {
            float t = (float)i / samples.Length;
            phase += Mathf.Lerp(start, end, t) * 2.0 * System.Math.PI / rate;
            float wave = (float)System.Math.Sin(phase);
            float envelope = Mathf.Min(1f, t * 40f) * Mathf.Pow(1f - t, 2f);
            samples[i] = (wave * (1f - noise) + ((float)random.NextDouble() * 2f - 1f) * noise) * envelope * 0.5f;
        }
        AudioClip clip = AudioClip.Create(name, samples.Length, 1, rate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private void Play(AudioClip clip, float volume)
    {
        if (GameSession.Instance != null && GameSession.Instance.IsMuted) return;
        _audio.PlayOneShot(clip, volume);
    }

    public static LineRenderer CreateRing(Transform parent, Color color, float width)
    {
        if (_instance == null) return null;
        var ringObject = new GameObject("Shockwave ring");
        ringObject.transform.SetParent(parent, false);
        LineRenderer ring = ringObject.AddComponent<LineRenderer>();
        ring.sharedMaterial = _instance._lineMaterial;
        ring.useWorldSpace = true;
        ring.loop = true;
        ring.positionCount = 64;
        ring.startWidth = ring.endWidth = width;
        ring.startColor = ring.endColor = color;
        ring.sortingOrder = 15;
        return ring;
    }

    public static void SetRingRadius(LineRenderer ring, float radius)
    {
        if (ring == null) return;
        Vector3 center = ring.transform.position;
        for (int i = 0; i < ring.positionCount; i++)
        {
            float angle = i * Mathf.PI * 2f / ring.positionCount;
            ring.SetPosition(i, center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius);
        }
    }

    private void Emit(Vector3 position, Color color, float radius, float duration, float width)
    {
        if (_pulses.Count >= 48) return;
        LineRenderer ring = CreateRing(transform, color, width);
        ring.transform.position = position;
        SetRingRadius(ring, 0.04f);
        _pulses.Add(new Pulse { Ring = ring, Duration = duration, Radius = radius, Color = color });
    }

    public static void Shoot(Vector3 position)
    {
        if (_instance == null) return;
        _instance.Play(_instance._shot, 0.35f);
        _instance.Emit(position, new Color(0.55f, 1f, 1f), 0.2f, 0.1f, 0.035f);
    }

    public static void Hit(Vector3 position)
    {
        if (_instance == null) return;
        _instance.Emit(position, new Color(0.7f, 1f, 1f), 0.3f, 0.16f, 0.045f);
        if (Time.time < _instance._nextImpactSound) return;
        _instance._nextImpactSound = Time.time + 0.08f;
        _instance.Play(_instance._impact, 0.3f);
    }

    public static void Explosion(Vector3 position)
    {
        if (_instance == null) return;
        _instance.Emit(position, new Color(1f, 0.65f, 0.25f), 0.8f, 0.35f, 0.07f);
        _instance._shake = Mathf.Max(_instance._shake, 0.045f);
    }

    public static void BombPulse(Vector3 position, float radius)
    {
        if (_instance == null) return;
        _instance.Emit(position, new Color(0.4f, 1f, 0.85f), radius + 0.4f, 0.55f, 0.09f);
        _instance._shake = Mathf.Max(_instance._shake, 0.15f);
        _instance.Play(_instance._explosion, 0.9f);
    }

    public static void PlayerHit(Vector3 position)
    {
        if (_instance == null) return;
        DamageFlash = 0.3f;
        _instance._shake = 0.2f;
        _instance.Emit(position, new Color(1f, 0.28f, 0.35f), 1.1f, 0.4f, 0.1f);
        _instance.Play(_instance._explosion, 0.65f);
    }

    public static void Pickup(Vector3 position)
    {
        if (_instance == null) return;
        _instance.Emit(position, new Color(0.5f, 1f, 0.7f), 0.8f, 0.4f, 0.045f);
        _instance.Play(_instance._pickup, 0.7f);
    }

    private void LateUpdate()
    {
        for (int i = _pulses.Count - 1; i >= 0; i--)
        {
            Pulse pulse = _pulses[i];
            pulse.Age += Time.deltaTime;
            if (pulse.Ring == null || pulse.Age >= pulse.Duration)
            {
                if (pulse.Ring != null) Destroy(pulse.Ring.gameObject);
                _pulses.RemoveAt(i);
                continue;
            }
            float t = pulse.Age / pulse.Duration;
            SetRingRadius(pulse.Ring, Mathf.Lerp(0.05f, pulse.Radius, 1f - (1f - t) * (1f - t)));
            Color color = pulse.Color;
            color.a = 1f - t;
            pulse.Ring.startColor = pulse.Ring.endColor = color;
        }
        DamageFlash = Mathf.MoveTowards(DamageFlash, 0f, Time.unscaledDeltaTime);
        if (_camera == null) return;
        _shake = Mathf.MoveTowards(_shake, 0f, Time.unscaledDeltaTime * 0.7f);
        float tNoise = Time.unscaledTime * 45f;
        Vector3 offset = new Vector3(Mathf.PerlinNoise(tNoise, 0f) - 0.5f, Mathf.PerlinNoise(0f, tNoise) - 0.5f) * (_shake * 2f);
        _camera.transform.position = _baseCameraPosition + offset;
    }

    private void OnDestroy()
    {
        if (_camera != null) _camera.transform.position = _baseCameraPosition;
        Destroy(_lineMaterial);
        Destroy(_shot);
        Destroy(_impact);
        Destroy(_explosion);
        Destroy(_pickup);
        if (_instance == this) { _instance = null; DamageFlash = 0f; }
    }
}

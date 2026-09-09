using UnityEngine;

public class SpaceBackdrop : MonoBehaviour
{
    private const int StarCount = 110;
    private readonly Transform[] _stars = new Transform[StarCount];
    private readonly float[] _speeds = new float[StarCount];
    private Sprite _pixel;
    private Camera _camera;
    private float _halfWidth;
    private float _halfHeight;
    private System.Random _random;

    private void Start()
    {
        _camera = Camera.main;
        if (_camera == null) { enabled = false; return; }
        _camera.backgroundColor = new Color(0.018f, 0.028f, 0.065f);
        _camera.clearFlags = CameraClearFlags.SolidColor;
        _halfHeight = _camera.orthographicSize + 1f;
        _halfWidth = _halfHeight * Mathf.Max(_camera.aspect, 2f);
        _pixel = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f, 1f);
        _random = new System.Random(1207);
        CreateRect("Flight corridor", new Vector3(0f, 0f, 5f), new Vector2(8.1f, 30f), new Color(0.025f, 0.055f, 0.09f), -110);
        for (int x = -4; x <= 4; x++)
            CreateRect("Navigation grid", new Vector3(x, 0f, 4f), new Vector2(0.008f, 30f), new Color(0.1f, 0.4f, 0.48f, 0.11f), -105);
        for (int y = -10; y <= 10; y++)
            CreateRect("Navigation grid", new Vector3(0f, y, 4f), new Vector2(8f, 0.008f), new Color(0.1f, 0.4f, 0.48f, 0.09f), -105);
        CreateRect("Port boundary", new Vector3(-4.05f, 0f, 3f), new Vector2(0.018f, 30f), new Color(0.2f, 0.85f, 0.9f, 0.25f), -100);
        CreateRect("Starboard boundary", new Vector3(4.05f, 0f, 3f), new Vector2(0.018f, 30f), new Color(0.2f, 0.85f, 0.9f, 0.25f), -100);
        for (int i = 0; i < StarCount; i++)
        {
            float depth = Range(0.2f, 1f);
            float size = Mathf.Lerp(0.012f, 0.034f, depth);
            _stars[i] = CreateRect("Star", new Vector3(Range(-_halfWidth, _halfWidth), Range(-_halfHeight, _halfHeight), 2f),
                new Vector2(size, size * Mathf.Lerp(1f, 2.8f, depth)), new Color(0.55f, 0.8f, 1f, depth * 0.65f), -90);
            _speeds[i] = Mathf.Lerp(0.12f, 0.85f, depth);
        }
    }

    private Transform CreateRect(string label, Vector3 position, Vector2 size, Color color, int order)
    {
        GameObject obj = new GameObject(label);
        obj.transform.SetParent(transform, false);
        obj.transform.position = position;
        obj.transform.localScale = new Vector3(size.x, size.y, 1f);
        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = _pixel;
        renderer.color = color;
        renderer.sortingOrder = order;
        return obj.transform;
    }

    private float Range(float min, float max) { return Mathf.Lerp(min, max, (float)_random.NextDouble()); }

    private void Update()
    {
        if (_camera == null) return;
        float pace = 1f + Mathf.Min(1f, (GameSession.Instance != null ? GameSession.Instance.Elapsed : 0f) / 180f);
        for (int i = 0; i < _stars.Length; i++)
        {
            if (_stars[i] == null) continue;
            Vector3 position = _stars[i].position;
            position.y -= _speeds[i] * pace * Time.deltaTime;
            if (position.y < -_halfHeight) position = new Vector3(Range(-_halfWidth, _halfWidth), _halfHeight, 2f);
            _stars[i].position = position;
        }
    }

    private void OnDestroy() { if (_pixel != null) Destroy(_pixel); }
}

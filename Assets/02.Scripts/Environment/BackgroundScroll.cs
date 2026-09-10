using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundScroll : MonoBehaviour
{
    private static readonly int MainTextureTransform = Shader.PropertyToID("_MainTex_ST");

    [SerializeField] private float _scrollSpeed = 0.1f;

    private SpriteRenderer _spriteRenderer;
    private MaterialPropertyBlock _propertyBlock;
    private Vector2 _textureScale = Vector2.one;
    private float _offsetY;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _propertyBlock = new MaterialPropertyBlock();

        Material sharedMaterial = _spriteRenderer.sharedMaterial;
        if (sharedMaterial == null || !sharedMaterial.HasProperty(MainTextureTransform)) return;

        Vector4 textureTransform = sharedMaterial.GetVector(MainTextureTransform);
        _textureScale = new Vector2(textureTransform.x, textureTransform.y);
        _offsetY = textureTransform.w;
    }

    private void Update()
    {
        _offsetY = Mathf.Repeat(_offsetY + _scrollSpeed * Time.deltaTime, 1f);

        _spriteRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetVector(
            MainTextureTransform,
            new Vector4(_textureScale.x, _textureScale.y, 0f, _offsetY));
        _spriteRenderer.SetPropertyBlock(_propertyBlock);
    }
}

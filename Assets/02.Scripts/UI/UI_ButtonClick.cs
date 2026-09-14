using UnityEngine;
using UnityEngine.UI;
public class UI_ButtonClick : MonoBehaviour
{   
    
    private Button _button;
    private AudioSource _audioSource; 
   
    [Header("클릭시 애니메이션")]
    [SerializeField] private AnimationCurve _bumpCurve;
    private bool _isBumping =false;
    private float _elapsedTime = 0.0f;
    private const float BumpDuration = 0.6f;
    private const float BunmpScale = 1.2f;
    private void Start()
    {   
        //동적으로 버튼 클릭시 실행할 함수 추가
        _audioSource = GetComponent<AudioSource>();
        _button.onClick.AddListener(PlayAnimation);
        _button.onClick.AddListener(PlaySound);
    }

    public void PlayAnimation()
    {
        _isBumping = true;
        _elapsedTime = 0.0f;
    }
    
    private void Update()
    {
        if (!_isBumping) return;
        
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > BumpDuration)
        {
            transform.localScale = Vector3.one;
            _isBumping = false;
            return;
        }
        
        //2. 누적 시간과 애니메이션 커브에 따른 스케일 변경
        float time=_elapsedTime / BumpDuration;
        float curveValue=_bumpCurve.Evaluate(time);
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * BunmpScale, curveValue);
    }

    public void PlaySound()
    {
        _audioSource.Play();
    }
    
}

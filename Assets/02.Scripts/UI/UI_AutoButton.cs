using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class UI_AutoButton : MonoBehaviour
{
    private Animator _animator;
    
    [Header("on/off스프라이트")]
    [SerializeField] private Sprite _onSprite;
    [SerializeField] private Sprite _offSprite;
    
    private Image _myImage;
    private AudioSource _audioSource;
    
    private bool _autoMode = false;
    private Player _player;
    
    [Header("클릭시 애니메이션")]
    [SerializeField] private AnimationCurve _bumpCurve;
    private float _scale = 1.0f;
    private bool _isBumping =false;
    private float _elapsedTime = 0.0f;
    private const float BumpDuration = 0.6f;
    private const float BunmpScale = 1.2f;
    private void Start()
    
    {   _myImage = GetComponent<Image>();
        _audioSource = GetComponent<AudioSource>();
        _player = GameObject.FindAnyObjectByType<Player>();
        
        AutoToggle();
    }
    
    
    public void AutoToggle()
    {       
        _autoMode = !_autoMode;
        _player.GetComponent<PlayerFire>().SetAuto((_autoMode));
        _player.GetComponent<PlayerMove>().enabled=!_autoMode;
        _player.GetComponent<PlayerAutoMove>().enabled=((_autoMode));
        
        _myImage.sprite = _autoMode ? _onSprite : _offSprite;
        
        
    }

    //todo: 버튼 클릭할때 애니메이션 주기+ 사운드 주기
    // 애니메이션: 코드로 구현 약간 커졌다가 작아지게
    // 사운드: 일레븐랩스에서 버튼 클릭 공영 사운드 만들어서 적용

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

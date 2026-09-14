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
   
    
    private bool _autoMode = false;
    private Player _player;
    
   
    private void Start()
    
    {   _myImage = GetComponent<Image>();
        
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






}

using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // 싱글톤 패턴
    // 1. 전역적으로 접근 가능하다.
    // 2. 인스턴스(생성된 객체)가 하나임을 보장한다.
    private static ScoreManager _instance = null;
    public static ScoreManager Instance => _instance;

    // 관리: 특정 데이터에 대한 무결성과 생성,읽기,수정,삭제 등과 관련된 게임 로직
    private int _bestScore;
    private int _currentScore;
    
    private const string Savekey = "BestScore";

    // UI 책임 추가 (텍스트메시 프로 참조)
    [SerializeField] private TextMeshProUGUI _bestScoreTextUI;
    [SerializeField] private TextMeshProUGUI _currentScoreTextUI;
    
    

    private void Awake()
    {
        // 늦게 태어난 매니저는 나는 늦었네~ 하면서 삭제
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }


    public void AddScore(int score)
    {
        if (score <= 0) return;

        _currentScore += score;
        if (_currentScore > _bestScore)
        {
            _bestScore = _currentScore;
            
            // 저장: PlayerPrefs.Set~ 시리즈를 이용해서 int float string 을 저장 가능하다
            // 내 컴퓨터 어딘가에 저장이 된다
            PlayerPrefs.SetInt(Savekey, _bestScore);
            PlayerPrefs.Save();
        }
        Refresh();
    }

    private void Start()
    {
        _bestScore = PlayerPrefs.GetInt(Savekey, 0);
        Refresh();
    }

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        _bestScoreTextUI.text = $"BestScore: {_bestScore}";
        _currentScoreTextUI.text = $"Score: {_currentScore}";
    }
}

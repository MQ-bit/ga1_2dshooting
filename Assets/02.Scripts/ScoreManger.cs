using System;
using TMPro;
using UnityEngine;

public class ScoreManger : MonoBehaviour
{
    //관리: 특정 데이터에 대한 무결성과 생성,읽기,수정,삭제 등과 관련된 게임 로직

    private int _bestScore;
    private int _currentScore;
    
    //UI 책임추가( 텍스트이서
    [SerializeField] TextMeshProUGUI _bestScoreTextUI;
    [SerializeField] TextMeshProUGUI _currentScoreTextUI;

    

    public void AddScore(int score)
    {
        _currentScore = score;
        if (_currentScore > _bestScore)
        {
            _bestScore = _currentScore;
        }
    }

    private void Update()
    {
        _bestScoreTextUI.text =$"BestScore: {_bestScore}";
        _currentScoreTextUI.text =$"Score: {_currentScore}";
    }
}

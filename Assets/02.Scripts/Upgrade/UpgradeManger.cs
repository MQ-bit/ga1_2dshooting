using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    // 업그레이드 관리자: 업그레이드들에 대한 무결성과 생성,조회,수정,삭제 등과 관련된 게임 로직
    private static UpgradeManager _instance = null;
    public static UpgradeManager Instance => _instance;


    // 업그레이드 도메인 클래스들
    [SerializeField] private Upgrade[] _upgrades;
    public Upgrade[] Upgrades => _upgrades;

    // 업그레이드 UI들
    [SerializeField] private UI_Upgrade[] _uiUpgrades;

    private const string UpgradeSaveDataKey = "UpgradeSaveData";

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

    private void Start()
    {
        Load();

        RefreshUI();
    }

    public void LevelUp(int index)
    {
        // Todo: 묻지말고 시켜라!
        // 골드 매니저에게 돈이 있는지 물어보고 돈이 있다면 차감 후 업그레이드 호출

        Upgrade upgrade = _upgrades[index];

        if (ScoreManager.Instance.Score < upgrade.Cost)
        {
            return;
        }

        ScoreManager.Instance.SpendScore(upgrade.Cost);

        _upgrades[index].LevelUp();

        Save();
        RefreshUI();
    }

    // UI 갱신
    private void RefreshUI()
    {
        foreach (UI_Upgrade uiUpgrade in _uiUpgrades)
        {
            uiUpgrade.Refresh();
        }
    }

    private void Save()
    {
        // 데이터 저장은 유의미한 정보만 저장을 한다.
        // 그래서 레벨만 저장한다.

        UpgradeSaveData saveData = new UpgradeSaveData(_upgrades.Length);
        for (int i = 0; i < _upgrades.Length; i++)
        {
            saveData.Name[i] = _upgrades[i].Name;
            saveData.Level[i] = _upgrades[i].Level;
        }

        // json 포맷으로 문자열 변환으로
        // 키와 밸류 형태로 저장한 형태
        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(UpgradeSaveDataKey, json);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        if (!PlayerPrefs.HasKey((UpgradeSaveDataKey))) return;

        string json = PlayerPrefs.GetString((UpgradeSaveDataKey));
        UpgradeSaveData saveData = JsonUtility.FromJson<UpgradeSaveData>(json);

        for (int i = 0; i < _upgrades.Length; i++)
        {
            Debug.Log($"{_upgrades[i].Name} 로드 완료!");
            _upgrades[i].SetLevel(saveData.Level[i]);
        }
    }
}
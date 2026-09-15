[System.Serializable]
public class UpgradeSaveData
{

    public string[] Name;
    public int[] Level;

    public UpgradeSaveData(int count)
    {
        Name= new string[count];
        Level = new int[count];
    }

}

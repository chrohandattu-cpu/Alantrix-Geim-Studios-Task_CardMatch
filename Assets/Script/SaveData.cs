[System.Serializable]
public class SaveData
{
    public int rows;
    public int columns;

    public int turns;
    public int matches;

    public int[] spriteIds;     // card assignments
    public bool[] matched;      // permanently removed cards
    public bool[] faceUp;       // currently visible cards

    public bool gameRunning;
}
using System;

[Serializable]
public class SaveSlotData
{
    public string[] characterIds = new string[3];
    public string[] jokerIds = new string[5];
    public string clearedAtIso;
    public string memo = "";
}

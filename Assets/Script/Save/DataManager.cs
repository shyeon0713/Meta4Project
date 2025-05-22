using UnityEngine;
using System.IO;

public static class DataManager
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "saves.json");
    public const int MaxSlots = 6;

    public static SaveSlotsData Load()
    {
        if (!File.Exists(SavePath))
            return new SaveSlotsData { slots = new SaveFile[MaxSlots] };

        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<SaveSlotsData>(json);
    }

    public static void Save(SaveSlotsData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }
}

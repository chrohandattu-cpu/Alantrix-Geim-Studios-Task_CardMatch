using System.IO;
using UnityEngine;

public static class SaveManager
{
    private static string GetPath(string levelKey)
    {
        return Path.Combine(
            Application.persistentDataPath,
            $"save_{levelKey}.json");
    }

    public static void Save(string levelKey, SaveData data)
    {
        string path = GetPath(levelKey);

        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(path, json);
        Debug.Log("Saved to: " + path);
    }

    public static SaveData Load(string levelKey)
    {
        string path = GetPath(levelKey);

        if (!File.Exists(path))
            return null;

        string json = File.ReadAllText(path);

        return JsonUtility.FromJson<SaveData>(json);
    }

    public static bool HasSave(string levelKey)
    {
        return File.Exists(GetPath(levelKey));
    }
    public static void DeleteSave(string levelKey)
    {
        string path = GetPath(levelKey);

        if (File.Exists(path))
            File.Delete(path);
    }
}
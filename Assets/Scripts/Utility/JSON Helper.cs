using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class JSONHelper 
{
    public static void SaveStallDataToJson(StallData data)
    {
        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "StallData.json");

        File.WriteAllText(path, json);
        Debug.Log($"Stall data saved to: {path}");
    }

    public static void LoadStallDataFromJson()
    {
        string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "StallData.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            StallData stallData = JsonUtility.FromJson<StallData>(json);
            Debug.Log("Stall data loaded from JSON:");
            Debug.Log($"Title: {stallData.title}, Description: {stallData.description}, Player ID: {stallData.playerId}");
        }
        else
        {
            Debug.LogWarning("Stall data file not found.");
        }
    }
}

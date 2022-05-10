using System;
using System.IO;
using UnityEngine;

[Serializable]
public class Report
{
    public string version = "0.0.1";
    public string datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    public double epoch = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    public string eventKey;
    public string levelID;
    public string userID;

    public string baseHealth;
    public string insuranceTowerRatio;
    public string optimality;

    public Report(string userID, string eventKey, string levelID, string baseHealth, string insuranceTowerRatio, string optimality)
    {
        this.userID = userID;
        this.eventKey = eventKey;
        this.levelID = levelID;
        this.baseHealth = baseHealth;
        this.insuranceTowerRatio = insuranceTowerRatio;
        this.optimality = optimality;
    }
}

public static class WinsuranceAnalytics
{
    /*
    private const string path = "Assets/Logs/userLog.json";
    private static readonly StreamWriter Writer = new(path, true);

    public static void ReportEvent(string userId, string eventKey, string levelID)
    {
        Writer.WriteLine(JsonUtility.ToJson(new Report(userId, eventKey, levelID)));
        Writer.Flush();
    }
    
    public static void Close() => Writer.Close();
    */

    private static string path;
    private static StreamWriter Writer = null;

    public static void ReportEvent(string userId, string eventKey, string levelID, string baseHealthPercentage, string insuranceTowerRatio, string optimality)
    {
        if (!Directory.Exists(Application.streamingAssetsPath + "/UserLogs/")) {
            Directory.CreateDirectory(Application.streamingAssetsPath + "/UserLogs/");
        }
        if (Writer == null) {
            path = Application.streamingAssetsPath + "/UserLogs/userLog" + userId + ".json";
            Writer = new(path, true);
        }

        Writer.WriteLine(JsonUtility.ToJson(new Report(userId, eventKey, levelID, baseHealthPercentage, insuranceTowerRatio, optimality)));
        Writer.Flush();
    }

    public static void Close() => Writer.Close();
}



using UnityEngine;

[System.Serializable]
public struct Channel
{
    public string ChannelName;
    public string GameID => Data.gameId;
    public ChannelData Data;
    public Channel(string title, string dataBlob)
    {
        ChannelName = title;
        Data = JsonUtility.FromJson<ChannelData>(dataBlob);
    }
}

[System.Serializable]
public struct ChannelData
{
    public LichessUser user;
    public int rating;
    public string gameId;
    public string color;
}

[System.Serializable]
public struct LichessUser
{
    public string id;
    public string name;
    public string title;
    public bool patron;
    public string flair;
}
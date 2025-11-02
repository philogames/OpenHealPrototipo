using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoseLandmarkChunkDTO
{

    [JsonProperty(Order = 1)]
    public int match_id { get; set; }
    [SerializeField, JsonProperty(Order = 2)]
    public Dictionary<string, PoseTimestampDTO> timestamps = new Dictionary<string, PoseTimestampDTO>();

   
    float timeOnStart = -1;

    //get current timestamp
    public PoseTimestampDTO GetCurrentTimestamp()
    {
        if(timeOnStart == -1)
        {
           
            timeOnStart = Time.time;
        }

        if (timestamps.Count >= 60)
        {
            ApiHandler.Instance.SendGeneralPoseData(GameDataBubbles.Instance.GetJsonGeneralPoseDataChunk());
            timestamps.Clear();
            
            Debug.Log("Maximum number of timestamps reached (60).");
        
        }


        string timestampKey =  (Time.time - timeOnStart).ToString();
       // Debug.Log($"Current timestamp key: {timestampKey}");
        if (!timestamps.ContainsKey(timestampKey))
        {
            timestamps.Add(timestampKey, new PoseTimestampDTO());
           // timestamps[timestampKey] = new PoseTimestampDTO();
        }
       
        return timestamps[timestampKey];
    }

    public IEnumerator LogTimestamps()
    {
        Debug.Log($"PoseLandmarkChunkDTO match_id: {match_id}, timestamps count: {timestamps.Count}");
        foreach (var kvp in timestamps)
        {
            string timestampKey = kvp.Key;
            PoseTimestampDTO timestamp = kvp.Value;
            Debug.Log($"Timestamp: {timestampKey}");
            foreach (var landmarkKvp in timestamp.landmarks)
            {
                string landmarkName = landmarkKvp.Key;
                PoseLandmarkDataDTO data = landmarkKvp.Value;
                Debug.Log($"  {landmarkName}: x={data.x}, y={data.y}, z={data.z}, presence={data.presence}");
                Debug.Log($"  {landmarkName}: visibility={data.visibility}");
                yield return null; // Yield to avoid blocking
            }
        }
    }

}

[System.Serializable]
public class PoseTimestampDTO
{
    [SerializeField]
    // Dicionário com 13 pontos: nose, leftShoulder, rightShoulder, leftElbow, rightElbow, leftWrist, rightWrist, leftHip, rightHip, leftKnee, rightKnee, leftAnkle, rightAnkle
    public Dictionary<string, PoseLandmarkDataDTO> landmarks { get; set; } = new Dictionary<string, PoseLandmarkDataDTO>();

    //inicializar o dicionario com os 11 pontos
    public PoseTimestampDTO()
    {
        landmarks.Add("nose", new PoseLandmarkDataDTO());
        landmarks.Add("leftShoulder", new PoseLandmarkDataDTO());
        landmarks.Add("rightShoulder", new PoseLandmarkDataDTO());
        landmarks.Add("leftElbow", new PoseLandmarkDataDTO());
        landmarks.Add("rightElbow", new PoseLandmarkDataDTO());
        landmarks.Add("leftWrist", new PoseLandmarkDataDTO());
        landmarks.Add("rightWrist", new PoseLandmarkDataDTO());
        landmarks.Add("leftHip", new PoseLandmarkDataDTO());
        landmarks.Add("rightHip", new PoseLandmarkDataDTO());
        landmarks.Add("leftKnee", new PoseLandmarkDataDTO());
        landmarks.Add("rightKnee", new PoseLandmarkDataDTO());
        landmarks.Add("leftAnkle", new PoseLandmarkDataDTO());
        landmarks.Add("rightAnkle", new PoseLandmarkDataDTO());


    }

    public void SetLandmark(string landmarkName, double x, double y, double z, double presence, double visibility)
    {
        if (landmarks.ContainsKey(landmarkName))
        {
            landmarks[landmarkName].SetLandmarkData(x, y, z, presence, visibility);
        }
    }





}

[System.Serializable]
public class PoseLandmarkDataDTO
{
    public double x { get; set; }
    public double y { get; set; }
    public double z { get; set; }

    public double presence { get; set; }

    public double visibility { get; set; }


    public void SetLandmarkData(double _x, double _y, double _z, double _presence, double _visibility)
    {
        x = _x;
        y = _y;
        z = _z;
        presence = _presence;
        visibility = _visibility;
    }

}

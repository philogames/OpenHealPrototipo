using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GeneralPoseData
{
    public int match_id = 0;
    [SerializeField]
    public List<ChunckedGeneralPoseData> chunc = new List<ChunckedGeneralPoseData>();

    //construtor
    public GeneralPoseData()
    {
        chunc.Add(new ChunckedGeneralPoseData());
        
    }

}
[System.Serializable]
public class ChunckedGeneralPoseData
{
  
    [SerializeField]
    public List<TimestampSkeleton> timestamp = new List<TimestampSkeleton>(500);

    //construtor
    public ChunckedGeneralPoseData()
    {
        for (int i = 0; i < 500; i++)
        {
            timestamp.Add(new TimestampSkeleton());
        }
    }
}

[System.Serializable]
public class SkeletonPoint
{
    public float x;
    public float y;
    public float z;
    public float likelihood;

    public void SetupSkeletonPoint(float _x, float _y, float _z, float _likelihood)
    {
        x = _x;
        y = _y;
        z = _z;
        likelihood = _likelihood;
    }
}

[System.Serializable]
public class TimestampSkeleton
{
    [SerializeField] public SkeletonPoint nose = new SkeletonPoint();
    [SerializeField] public SkeletonPoint leftShoulder = new SkeletonPoint();
    [SerializeField] public SkeletonPoint rightShoulder = new SkeletonPoint();
    [SerializeField] public SkeletonPoint leftElbow = new SkeletonPoint();
    [SerializeField] public SkeletonPoint rightElbow = new SkeletonPoint();
    [SerializeField] public SkeletonPoint leftWrist = new SkeletonPoint();
    [SerializeField] public SkeletonPoint rightWrist = new SkeletonPoint();
    [SerializeField] public SkeletonPoint leftIndex = new SkeletonPoint();
    [SerializeField] public SkeletonPoint rightIndex = new SkeletonPoint();
    [SerializeField] public SkeletonPoint leftIndexTip = new SkeletonPoint();
    [SerializeField] public SkeletonPoint rightIndexTip = new SkeletonPoint();

    public TimestampSkeleton()
    {
        
    }

    /// <summary>
    /// Sets the time stamp points. Returns false if the SkeletonIndex is invalid.
    /// </summary>
    /// <param name="SkeletonIndex"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="likelihood"></param>
    /// <returns></returns>
    public bool SetTimeStampPoints(int SkeletonIndex, float x, float y, float z, float likelihood)
    {
        float screenX = (1.0f - x) * UnityEngine.Screen.currentResolution.width;
        float screenY = (1.0f - y) * UnityEngine.Screen.currentResolution.height;
        z = (1.0f - z);
        switch (SkeletonIndex)
        {
            case 0:
                nose.SetupSkeletonPoint(screenX, screenY, z, likelihood);
                break;
            case 11:
                leftShoulder.SetupSkeletonPoint(screenX, screenY, z, likelihood);
                break;
            case 12:
                rightShoulder.SetupSkeletonPoint(screenX, screenY, z, likelihood);
                break;
            case 13:
                leftElbow.SetupSkeletonPoint(screenX, screenY, z, likelihood);
                break;
            case 14:
                rightElbow.SetupSkeletonPoint(screenX, screenY, z, likelihood);
                break;
            case 15:
                leftWrist.SetupSkeletonPoint(screenX, screenY, z, likelihood);
                break;
            case 16:
                rightWrist.SetupSkeletonPoint(screenX, screenY, z, likelihood);
                break;
            case 19:
                leftIndex.SetupSkeletonPoint(screenX, screenY, z, likelihood);
                break;
            case 20:
                rightIndex.SetupSkeletonPoint(screenX, screenY, z, likelihood);
                break;
            case 21:
                leftIndexTip.SetupSkeletonPoint(screenX, screenY, z, likelihood);
                break;
            case 22:
                rightIndexTip.SetupSkeletonPoint(screenX, screenY, z, likelihood);
                break;
            default:
                return false;
        }

        return true;
    }
}



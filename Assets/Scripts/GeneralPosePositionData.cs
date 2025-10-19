using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class GeneralPosePositionData
{
    [SerializeField]
    public List<ChunckedGeneralPosePositionData> chunc = new List<ChunckedGeneralPosePositionData>();

}
[Serializable]
public class ChunckedGeneralPosePositionData
{
    [SerializeField]
    public List<TimestampSkeleton> timestamp = new List<TimestampSkeleton>(500);
}

[Serializable]
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

[Serializable]
public class TimestampSkeleton
{
    [SerializeField] public SkeletonPoint nose;
    [SerializeField] public SkeletonPoint leftShoulder;
    [SerializeField] public SkeletonPoint rightShoulder;
    [SerializeField] public SkeletonPoint leftElbow;
    [SerializeField] public SkeletonPoint rightElbow;
    [SerializeField] public SkeletonPoint leftWrist;
    [SerializeField] public SkeletonPoint rightWrist;
    [SerializeField] public SkeletonPoint leftIndex;
    [SerializeField] public SkeletonPoint rightIndex;
    [SerializeField] public SkeletonPoint leftIndexTip;
    [SerializeField] public SkeletonPoint rightIndexTip;


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
        switch (SkeletonIndex)
        {
            case 0:
                nose.SetupSkeletonPoint(x, y, z, likelihood);
                break;
            case 11:
                leftShoulder.SetupSkeletonPoint(x, y, z, likelihood);
                break;
            case 12:
                rightShoulder.SetupSkeletonPoint(x, y, z, likelihood);
                break;
            case 13:
                leftElbow.SetupSkeletonPoint(x, y, z, likelihood);
                break;
            case 14:
                rightElbow.SetupSkeletonPoint(x, y, z, likelihood);
                break;
            case 15:
                leftWrist.SetupSkeletonPoint(x, y, z, likelihood);
                break;
            case 16:
                rightWrist.SetupSkeletonPoint(x, y, z, likelihood);
                break;
            case 19:
                leftIndex.SetupSkeletonPoint(x, y, z, likelihood);
                break;
            case 20:
                rightIndex.SetupSkeletonPoint(x, y, z, likelihood);
                break;
            case 21:
                leftIndexTip.SetupSkeletonPoint(x, y, z, likelihood);
                break;
            case 22:
                rightIndexTip.SetupSkeletonPoint(x, y, z, likelihood);
                break;
            default:
                return false;
        }

        return true;
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandPositionData
{
    [SerializeField]
    public List<Vector3> leftHand = new List<Vector3>();
    [SerializeField]
    public List<Vector3> rightHand = new List<Vector3>();
}

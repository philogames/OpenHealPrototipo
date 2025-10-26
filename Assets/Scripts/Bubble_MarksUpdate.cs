using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe;
using Mediapipe.Unity;
using Mediapipe.Unity.PoseTracking;
using UnityEngine.Device;
using MoreMountains.Tools;


public class Bubble_MarksUpdate : MonoBehaviour
{
    List<NormalizedLandmark> listaMaoEsquerda = new List<NormalizedLandmark>(), listaMaoDireita = new List<NormalizedLandmark>();
    public GameObject handColliderDireita;
    public GameObject handColliderEsquerda;
    public PoseTrackingSolution pose;

    [SerializeField]
    public HandPositionData handsData;
 //   [SerializeField]
 //   public GeneralPoseData generalPoseData;

    [SerializeField]
    public PoseLandmarkChunkDTO poseLandmarkChunkDTO;
    [MMReadOnly]
    public bool isCollectingData = false;
    [MMReadOnly]
    public int currentChunck = 0;
    [MMReadOnly]
    public int currentTimestampInChunck = 0;

    public void SetUpHands()
    {
        handsData = new HandPositionData();
     //   generalPoseData = new GeneralPoseData();
        poseLandmarkChunkDTO = new PoseLandmarkChunkDTO();


        pose = GameObject.FindObjectOfType<PoseTrackingSolution>();
        

        if (pose != null)
        {
            pose.OnPoseLandmarksUpdated += UpdateHands;
            pose.OnPoseLandmarksUpdated += UpdateGeneralPose;
        }
    }

    void OnDestroy()
    {
        if (pose != null)
        {
            pose.OnPoseLandmarksUpdated -= UpdateHands;
            pose.OnPoseLandmarksUpdated -= UpdateGeneralPose;
        }
    }

    public void StartCollectSkeletonData()
    {
        isCollectingData = true;
        StartCoroutine(Start_CollectHandData());
    }

    IEnumerator Start_CollectHandData()
    {
        
        Vector3 mD = GetAverageHandPosition(listaMaoDireita);
        Vector3 mE = GetAverageHandPosition(listaMaoEsquerda);
       // Debug.Log("Mao Direita: " + mD + "     " + "Mao Esquerda: " + mE);

        handsData.rightHand.Add(mD);
        handsData.leftHand.Add(mE);
       
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(Start_CollectHandData());
    }

    public HandPositionData Stop_CollectHandData()
    {
        StopAllCoroutines();
        isCollectingData = false;

        return handsData;
    }

    /*
    public GeneralPoseData Stop_CollectGeneralPoseData()
    {
        isCollectingData = false;
        return generalPoseData;
    }
    */

    
    public PoseLandmarkChunkDTO Stop_CollectPoseLandmarkChunkDTO()
    {
        isCollectingData = false;
        return poseLandmarkChunkDTO;

    }
    
    void _update()
    {
      //  pose.graphRunner.OnPoseLandmarksOutput
    }
    public void UpdateHands(NormalizedLandmarkList landmarks)
    {
       // Debug.Log("TENTEU ATUALIZAR POSICAOOOOOO"); 
        //limpar listas de pontos das mãos
        listaMaoEsquerda.Clear();
        listaMaoDireita.Clear();

        //seta os pontos das mãos direita e esquerda conforme definição https://developers.google.com/mediapipe/solutions/vision/pose_landmarker
        //adiciona pontos referentes a mao direita
        listaMaoDireita.Add(landmarks.Landmark[20]);
        listaMaoDireita.Add(landmarks.Landmark[22]);
        listaMaoDireita.Add(landmarks.Landmark[18]);
        //adiciona pontos referentes a mão esquerda
        listaMaoEsquerda.Add(landmarks.Landmark[21]);
        listaMaoEsquerda.Add(landmarks.Landmark[19]);
        listaMaoEsquerda.Add(landmarks.Landmark[17]);

        //atualiza posição dos colisores das mãos
        UpdateHandCollider(listaMaoDireita, handColliderDireita);
        UpdateHandCollider(listaMaoEsquerda, handColliderEsquerda);
    }

    float cooldownUpdateGeneralPose = 0f;
    public void UpdateGeneralPose(NormalizedLandmarkList landmarks)
    {
        if (!isCollectingData)
            return;

        cooldownUpdateGeneralPose += Time.deltaTime;

        if(cooldownUpdateGeneralPose > 0.1f)
        {
            cooldownUpdateGeneralPose = 0f;
        }
        else
        {
            return;
        }



        int indexLandmark = 0;

        
        foreach (var landmark in landmarks.Landmark)
        {
            string landmarkName = "";
            switch (indexLandmark)
            {
                case 0:
                    landmarkName = "nose";
                    break;
                case 11:
                    landmarkName = "leftShoulder";
                    break;
                case 12:
                    landmarkName = "rightShoulder";
                    break;
                case 13:
                    landmarkName = "leftElbow";
                    break;
                case 14:
                    landmarkName = "rightElbow";
                    break;
                case 15:
                    landmarkName = "leftWrist";
                    break;
                case 16:
                    landmarkName = "rightWrist";
                    break;
                case 23:
                    landmarkName = "leftHip";
                    break;
                case 24:
                    landmarkName = "rightHip";
                    break;
                case 25:
                    landmarkName = "leftKnee";
                    break;
                case 26:
                    landmarkName = "rightKnee";
                    break;
                case 27:
                    landmarkName = "leftAnkle";
                    break;
                case 28:
                    landmarkName = "rightAnkle";
                    break;

                default:
                    indexLandmark++;
                    continue;
            }
            poseLandmarkChunkDTO.GetCurrentTimestamp()?.SetLandmark(landmarkName, landmark.X, landmark.Y, landmark.Z, landmark.Presence);

            //mostra no console os valores dos pontos adicionados
           // Debug.Log("Landmark: " + landmarkName + " X: " + landmark.X + " Y: " + landmark.Y + " Z: " + landmark.Z + " Likelihood: " + landmark.Presence);

            indexLandmark++;

        }
        
        
        /*
        foreach (var landmark in landmarks.Landmark)
        {
            generalPoseData.chunc[currentChunck].timestamp[currentTimestampInChunck].SetTimeStampPoints(indexLandmark, landmark.X, landmark.Y, landmark.Z, landmark.Presence);
            indexLandmark++; 
        }
        currentTimestampInChunck++;

        if(currentTimestampInChunck >= generalPoseData.chunc[currentChunck].timestamp.Count)
        {
            currentChunck++;
            generalPoseData.chunc.Add(new ChunckedGeneralPoseData());
            currentTimestampInChunck = 0;
        }
        */
    }


    void UpdateHandCollider(List<NormalizedLandmark> listDedos, GameObject hand)
    {
        Vector3 screenPosition = GetAverageHandPosition(listDedos);

        // Converta a posição da tela para a posição do mundo
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 2));



        hand.transform.position = worldPosition;
    }

    private Vector3 GetAverageHandPosition(List<NormalizedLandmark> landmarks)
    {
        Vector3 sum = Vector3.zero;
        int i = 0;
        foreach (var landmark in landmarks)
        {
            Vector3 landPos = new Vector3(landmark.X, landmark.Y, landmark.Z);
            i++;
            sum += landPos;
        }

        Vector3 average = sum / landmarks.Count;
       
        // Converta a posição normalizada para a posição da tela
        float screenX = (1.0f - average.x) * UnityEngine.Screen.currentResolution.width;
        float screenY = (1.0f - average.y) * UnityEngine.Screen.currentResolution.height;
        float screenZ = (1.0f - average.z);

        return new Vector3((int)screenX, (int) screenY, screenZ);
    }

    public void HideHands()
    {
        handColliderDireita.SetActive(false);
        handColliderEsquerda.SetActive(false);
    }
}

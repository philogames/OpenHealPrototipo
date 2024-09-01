using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe;
using Mediapipe.Unity;
using Mediapipe.Unity.PoseTracking;
using UnityEngine.Device;


public class Bubble_MarksUpdate : MonoBehaviour
{
    List<NormalizedLandmark> listaMaoEsquerda = new List<NormalizedLandmark>(), listaMaoDireita = new List<NormalizedLandmark>();
    public GameObject handColliderDireita;
    public GameObject handColliderEsquerda;
    public PoseTrackingSolution pose;

    [SerializeField]
    public HandPositionData handsData;

    public void SetUpHands()
    {
      handsData = new HandPositionData();
       pose = GameObject.FindObjectOfType<PoseTrackingSolution>();
        

        if (pose != null)
        {
            pose.OnPoseLandmarksUpdated += UpdateHands;
        }
    }

    void OnDestroy()
    {
        if (pose != null)
        {
            pose.OnPoseLandmarksUpdated -= UpdateHands;
        }
    }

    public IEnumerator Start_CollectHandData()
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
        return handsData;
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
        float screenZ = 0;

        return new Vector3((int)screenX, (int) screenY, (int)screenZ);
    }

    public void HideHands()
    {
        handColliderDireita.SetActive(false);
        handColliderEsquerda.SetActive(false);
    }
}

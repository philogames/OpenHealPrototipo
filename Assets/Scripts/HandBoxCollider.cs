using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mediapipe;
using Mediapipe.Unity;
using System;

using Stopwatch = System.Diagnostics.Stopwatch;


public class HandboxCollider : MonoBehaviour
{
    // Configura��es e refer�ncias de UI

    [SerializeField] private TextAsset _configAsset;
    [SerializeField] private RawImage _screen;
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private int _fps;
    [SerializeField] private PoseLandmarkListAnnotationController _poseLandmarkListAnnotationController;

    // Refer�ncias para objetos de colis�o das m�os
    public GameObject handColliderDireita;
    public GameObject handColliderEsquerda;

    // Vari�veis para gerenciamento do gr�fico e recursos
    private CalculatorGraph _graph;
    private StreamingAssetsResourceManager _resourceManager;
    BoolPacket output_H = new BoolPacket(false);
    BoolPacket output_V = new BoolPacket(false);
    IntPacket output_R = new IntPacket(0);

    // Vari�veis para captura e processamento de imagem da webcam
    [HideInInspector]
    public WebCamTexture _webCamTexture;
    private Texture2D _inputTexture;
    private Color32[] _inputPixelData;

    // Refer�ncia para a c�mera principal do jogo
    Camera mainCamera;

    // Listas para armazenar os marcos das m�os
    List<NormalizedLandmark> listaMaoDireita = new List<NormalizedLandmark>();
    List<NormalizedLandmark> listaMaoEsquerda = new List<NormalizedLandmark>();

    // Inicializa��o
    private void Start()
    {

        //StartCoroutine(Inicializar(1));
    }

    public IEnumerator IniciarCamera(int cam)
    {

        LimparDados();

        yield return new WaitForSeconds(0.1f);

        mainCamera = Camera.main; // Obt�m a c�mera principal

        // Inicializa e verifica a disponibilidade da webcam
        if (WebCamTexture.devices.Length == 0)
        {
            Debug.LogError("Nenhuma webcam encontrada.");
            yield break;
        }
        var webCamDevice = WebCamTexture.devices[cam]; //altere esse numero para selecionar sua webcam
        _webCamTexture = new WebCamTexture(webCamDevice.name, _width, _height, _fps);

        if (TestCameraDevice(webCamDevice.name))
        {
            _webCamTexture.Play();


            yield return new WaitUntil(() => _webCamTexture.width > 16);

            // Configura a textura de entrada e o display de imagem
            _screen.rectTransform.sizeDelta = new Vector2(_width, _height);
            _inputTexture = new Texture2D(_width, _height, TextureFormat.RGBA32, false);
            _inputPixelData = new Color32[_width * _height];
            _screen.texture = _webCamTexture;
        }
        
    }

    bool TestCameraDevice(string deviceName)
    {
        WebCamTexture testTexture = new WebCamTexture(deviceName);
        try
        {
            testTexture.Play();
        }
        catch (Exception e)
        {
            return false;
        }

        // Aguarde um curto per�odo para a c�mera iniciar
        System.Threading.Thread.Sleep(100);

        // Verifica se a c�mera est� efetivamente transmitindo com uma resolu��o aceit�vel
        bool isWorking = testTexture.width > 16 && testTexture.height > 16;

        testTexture.Stop();
        return isWorking;
    }
    public IEnumerator Inicializar(int cam)
    {

        StartCoroutine(IniciarCamera(cam));
        yield return new WaitForSeconds(0.5f);
        // Prepara recursos e inicia o gr�fico de c�lculo
        _resourceManager = new StreamingAssetsResourceManager();
        yield return _resourceManager.PrepareAssetAsync("pose_detection.bytes"); //grafico da pose, na pasta packages

        var stopwatch = new Stopwatch(); //iniciar classe para gerir o tempo do grafico

        _graph = new CalculatorGraph(_configAsset.text); //carrega os nodes

        // Configura��o de streams e pacotes
        var multiHandLandmarksStream = new OutputStream<NormalizedLandmarkListPacket, NormalizedLandmarkList>(_graph, "pose_landmarks"); //de acordo com os nodes carregados
        multiHandLandmarksStream.StartPolling().AssertOk();

        //Iniciar sidePackets
        SidePacket sidePacket = new SidePacket();
        sidePacket.Emplace("input_horizontally_flipped", new BoolPacket(false));
        sidePacket.Emplace("input_vertically_flipped", new BoolPacket(true));
        sidePacket.Emplace("input_rotation", new IntPacket(0));
        sidePacket.Emplace("output_horizontally_flipped", output_H);
        sidePacket.Emplace("output_vertically_flipped", output_V);
        sidePacket.Emplace("output_rotation", output_R);

        //iniciar graficocom sidepackages
        _graph.StartRun(sidePacket).AssertOk();

        //iniciar gerenciardor de tempo
        stopwatch.Start();


        //loop principal
        while (true)
        {


            //cria um image frame com o frame atual da webcam
            //  Debug.Log( WebCamTexture.);
            // var temp = _webCamTexture.GetPixels32(_inputPixelData);
            yield return null;
           // Debug.Log("imagem registrada colors" +temp.Length);
            if(_webCamTexture.didUpdateThisFrame)
            {
                bool valido = false;
                try
                {
                    _inputTexture.SetPixels32(_webCamTexture.GetPixels32(_inputPixelData));
                    valido = true;
                }
                catch(Exception e)
                {
                    valido = false;
                }

                if (valido)
                {
                    var imageFrame = new ImageFrame(ImageFormat.Types.Format.Srgba, _width, _height, _width * 4, _inputTexture.GetRawTextureData<byte>());
                    var currentTimestamp = stopwatch.ElapsedTicks / (System.TimeSpan.TicksPerMillisecond / 1000);
                    _graph.AddPacketToInputStream("input_video", new ImageFramePacket(imageFrame, new Timestamp(currentTimestamp))).AssertOk();//adiciona como input de video o frame atual da webcam

                    yield return new WaitForEndOfFrame();

                    if (multiHandLandmarksStream.TryGetNext(out var multiHandLandmarks))
                    {
                        //atualiza os colisores das m�os
                        UpdateHands(multiHandLandmarks);

                        //desenha o esqueleto na view scene
                        _poseLandmarkListAnnotationController.DrawNow(multiHandLandmarks);
                    }
                }



            }
         
        }
    }
    /// <summary>
    /// Calcula a posicao media das m�os e seta os colisores nas respectivas posi��es
    /// </summary>
    /// <param name="landmarks"></param>
    void UpdateHands(NormalizedLandmarkList landmarks)
    {

        //limpar listas de pontos das m�os
        listaMaoEsquerda.Clear();
        listaMaoDireita.Clear();

        //seta os pontos das m�os direita e esquerda conforme defini��o https://developers.google.com/mediapipe/solutions/vision/pose_landmarker
        //adiciona pontos referentes a mao direita
        listaMaoDireita.Add(landmarks.Landmark[20]);
        listaMaoDireita.Add(landmarks.Landmark[22]);
        listaMaoDireita.Add(landmarks.Landmark[18]);
        //adiciona pontos referentes a m�o esquerda
        listaMaoEsquerda.Add(landmarks.Landmark[21]);
        listaMaoEsquerda.Add(landmarks.Landmark[19]);
        listaMaoEsquerda.Add(landmarks.Landmark[17]);
        
        //atualiza posi��o dos colisores das m�os
        UpdateHandCollider(listaMaoDireita, handColliderDireita);
        UpdateHandCollider(listaMaoEsquerda, handColliderEsquerda);
    }

   
    void UpdateHandCollider(List<NormalizedLandmark> listDedos, GameObject hand)
    {
        Vector3 screenPosition = GetAverageHandPosition(listDedos);

        // Converta a posi��o da tela para a posi��o do mundo
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y,2));

       

        hand.transform.position = worldPosition;
    }

    /// <summary>
    /// calcula a posi��o media de acordo com os pontos de uma lista
    /// </summary>
    /// <param name="landmarks"></param>
    /// <returns></returns>
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

        // Converta a posi��o normalizada para a posi��o da tela
        float screenX = (1.0f - average.x) * _screen.rectTransform.sizeDelta.x;
        float screenY = (1.0f - average.y) * _screen.rectTransform.sizeDelta.y;
        float screenZ =0; 

        return new Vector3(screenX, screenY, screenZ);
    }

    //limpar dados ao encerrar
    private void OnDestroy()
    {
        LimparDados();
    }

    void LimparDados()
    {
      
        if (_webCamTexture != null)
        {
            _webCamTexture.Stop();
        }

        if (_graph != null)
        {
            try
            {
                _graph.CloseInputStream("input_video").AssertOk();
                _graph.WaitUntilDone().AssertOk();
            }
            finally
            {

                _graph.Dispose();
                
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MoreMountains.Tools;
using System.IO;
using Mediapipe.Unity;
using Mediapipe.Unity.PoseTracking;
using UnityEngine.Device;
[Serializable]
public class BolaListJsonClass
{
    public List<Bola_Info> bolas = new List<Bola_Info>();
}

/// <summary>
/// Classe Singleton para armazenar informações persistentes entre partidas. 
/// Possui um modo DEBUG para testes, que gera uma lista de bolas aleatoriamente de acordo com os parametros. 
/// </summary>

public class GameDataBubbles : MonoBehaviour
{
    public static GameDataBubbles Instance { get; private set; }
    public GameObject Solution;
    private int idJogador;
    private int ScoreTotal  = 0;
    private float LastMatchScore = 0;
    private int FaseAtual  = 0;

    BolaListJsonClass listaBolas = new BolaListJsonClass();
    /// <summary>
    /// Lista que armazena as bolas da próxima partida, de acordo com dados do Servidor.
    /// Caso DEBUG_MODE esteja ativo, essa lista será preenchida aleatoriamente
    /// </summary>
    public List<Bola_Info> Bolas_ProximaPartida; 

    [Tooltip("Ative para gerar a lista de bolas aleatoriamente, sem acessar o servidor")]
    public bool DEBUG_MODE = false;
    #region DEBUG PARAMETERS
    [MMCondition("DEBUG_MODE", true)]
    public Vector2 qtdBolas_minMax;
    [MMCondition("DEBUG_MODE", true)]
    public Vector2 speed_minMax;
    [MMCondition("DEBUG_MODE", true)]
    public Vector2 size_minMax;
    [MMCondition("DEBUG_MODE", true)]
    public Vector2 launchCoord_min;
    [MMCondition("DEBUG_MODE", true)]
    public Vector2 launchCoord_max;
    [MMCondition("DEBUG_MODE", true)]
    public float minDelayEntreBolas = 0.5f;
    [MMCondition("DEBUG_MODE", true)]
    public bool randomColor = false;
    #endregion

    [SerializeField]
    public bubble_match_data loadedSession;


    private void OnEnable()
    {
        SetupBubbleMatch(ApiHandler.Instance.GetBubbleMatchString());
    }

    public void SetupBubbleMatch(string s)
    {
        loadedSession = JsonUtility.FromJson<bubble_match_data>(s);
    }

    public bool isMatchReady()
    {
        return loadedSession != null; //retorna true se a partida estiver pronta
    }

    public IEnumerator SetupBubbleMatch_IfEmpty()
    {
        
        if (!DEBUG_MODE)
        {

          //  yield return new WaitForSeconds(1);
         //   if (loadedSession.data.user_id == "")
          //      yield return StartCoroutine(ApiHandler.Instance.GetBubbleMatch());
        }
        else
        {
            GerarProximaPartidaAleatoria();
            yield return new WaitForSeconds(1);
        }

    }

    /// <summary>
    /// Adiciona o Score da Partida ao Score Total do jogador
    /// </summary>
    /// <param name="valor">Score ganho na última partida</param>
    public void AddTotalScore(int valor)
    {
        ScoreTotal += valor;
    }

    public int GetScore()
    {
        return ScoreTotal;
    }

    public float GetLastMatchScore()
    {
        return LastMatchScore;
    }

    /// <summary>
    /// Adiciona uma Fase
    /// </summary>
    public void AddFase()
    {
        FaseAtual++;
    }

    public void SetLastMatchScore(float s)
    { 
        LastMatchScore = s; 
    }

   

    public void StopMediaPipe()
    {
        Solution.GetComponent<PoseTrackingSolution>().Pause();
       // Solution.GetComponent<PoseTrackingGraph>().Stop();
        Solution.GetComponent<WebCamSource>().Stop();
        Solution.GetComponent<TextureFramePool>().Destruir();
    }

    public void StartMediaPipe()
    {

        PoseTrackingSolution solution = Solution.GetComponent<PoseTrackingSolution>();
        solution.SetScreen(GameObject.FindObjectOfType<Mediapipe.Unity.Screen>());
        solution._worldAnnotationArea = GameObject.Find("UI_Esqueleto").GetComponent<RectTransform>();
        solution._poseDetectionAnnotationController = GameObject.FindObjectOfType<DetectionAnnotationController>();
        solution._poseLandmarksAnnotationController = GameObject.FindObjectOfType<PoseLandmarkListAnnotationController>();
        solution._poseWorldLandmarksAnnotationController = GameObject.FindObjectOfType<PoseWorldLandmarkListAnnotationController>();
        solution._segmentationMaskAnnotationController = GameObject.FindObjectOfType<MaskAnnotationController>();
        solution._roiFromLandmarksAnnotationController = GameObject.FindObjectOfType<NormalizedRectAnnotationController>();

       
        Solution.GetComponent<WebCamSource>().Play();
        solution.graphRunner.Initialize(solution.runningMode);
        Solution.GetComponent<TextureFramePool>().Iniciar();
        solution.Play();
    }

    private void Awake()
    {
        

        //garante que a classe seja um singleton
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
        {
            Instance = this;
            Solution = GameObject.Find("Solution");
            
          //  DontDestroyOnLoad(this.gameObject);
        }

       


     }


        /// <summary>
        /// Gerar aleatóriamente uma lista de bolas para testar a partida localmente, sem acessar o servidor.
        /// Só funciona caso o DEBUG_MODE esteja ativado.
        /// </summary>
        public void GerarProximaPartidaAleatoria()
    {
        if (DEBUG_MODE)
        {

            //define uma quantidade aleatoria de bolas para serem instanciadas de acordo com o min e max permitido
            int temp_QtdBolas = UnityEngine.Random.Range((int)qtdBolas_minMax.x, (int)qtdBolas_minMax.y);

            //apaga a lista de bolas anterior;
            loadedSession = new bubble_match_data();
            loadedSession.data = new bubble_match();
            loadedSession.data.balls = new List<Bola_Info>();

            //gera as bolas com propriedades aleatórias definidas pelas variaveis de controle do DEBUG_MODE
            for (int i = 0; i < temp_QtdBolas; i++)
            {
                //coordenada inicial aleatoria
                Vector2 _temp_launchCoord = new Vector2(UnityEngine.Random.Range(launchCoord_min.x, launchCoord_max.x), UnityEngine.Random.Range(launchCoord_min.y, launchCoord_max.y));
              
                //velocidade inicial aleatória
                float _temp_speed = UnityEngine.Random.Range(speed_minMax.x, speed_minMax.y);

                //cor branca
                UnityEngine.Color _temp_color = UnityEngine.Color.white;
                if (randomColor)//caso a cor tbm esteja definica como aleatória, então gerar uma cor aleatória
                    _temp_color = new UnityEngine.Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);

                //tamanho aleatório
                float _temp_size = UnityEngine.Random.Range(size_minMax.x, size_minMax.y);

                //direcao aleatória
                float _direction = UnityEngine.Random.Range(0, 360);
              

                float _matureTime = (i* minDelayEntreBolas) + UnityEngine.Random.Range(1, 2.5f);

                float _destroyTime = UnityEngine.Random.Range(.75f, 1.5f) + _matureTime + ( minDelayEntreBolas);

                //cria uma instancia Bola_Info para armazenar os dados da bola
                Bola_Info b = new Bola_Info(i * minDelayEntreBolas, _temp_launchCoord, _temp_speed, _temp_color, _temp_size, _direction, _matureTime, _destroyTime);

                //adiciona os dados da bola recem gerados para a lista que será utilizada pelo Gerenciador de Partida
                loadedSession.data.balls.Add(b);

            }
        }
    }

    public void AddBolaToSendApi(Bola_Info bola)
    {
        listaBolas.bolas.Add(bola);
    }

    /*
    public BolaListJsonClass GetListaBolasJson()
    {
        return listaBolas;
    }
    */
    /* para salvar no desktop, descomente o código abaixo e chame a função GerarJsonHandData() no final da partida
    public void GerarJsonHandData(HandPositionData handsData)
    {
        string json = JsonUtility.ToJson(handsData);

        // Obtem o caminho para o Desktop do usuário atual
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        // Constrói o caminho para a pasta OpenHeal no Desktop
        string openHealPath = Path.Combine(desktopPath, "OpenHeal");

        // Constrói o caminho para a subpasta Bubbles dentro de OpenHeal
        string bubblesPath = Path.Combine(openHealPath, "Bubbles");

        // Verifica se a pasta Bubbles existe, se não, cria
        if (!Directory.Exists(bubblesPath))
        {
            Directory.CreateDirectory(bubblesPath);
        }

        // Constrói o caminho final do arquivo, incluindo o nome do arquivo com a data e hora atuais
        string finalPath = Path.Combine(bubblesPath, "Hands Data " + DateTime.Now.ToString("dd-MM-yy--HH-mm-ss") + ".json");

        File.WriteAllText(finalPath, json);
    }
    */

    public string GetJsonHandData()
    {
        HandPositionData handsData = GameObject.FindObjectOfType<Bubble_MarksUpdate>().Stop_CollectHandData();
        string json = JsonUtility.ToJson(handsData);
        return json;
    }
    public string GetJsonBubblesData()
    {

        bubble_match_data resultadoFinal = loadedSession;
        resultadoFinal.data.balls = listaBolas.bolas;
        string json = JsonUtility.ToJson(resultadoFinal.data);
       
        return json;
    }

    /* para salvar no desktop, descomente o código abaixo e chamne a funcao GerarJsonBubbles() no final da partida
    public void GerarJsonBubbles()
    {
        
        bubble_match_data resultadoFinal = loadedSession;
        resultadoFinal.data.balls = listaBolas.bolas;
        string json = JsonUtility.ToJson(resultadoFinal);


        
         Debug.Log("SALVOUUUUUUUUU" + Application.persistentDataPath);
        // Obtem o caminho para o Desktop do usuário atual
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        // Constrói o caminho para a pasta OpenHeal no Desktop
        string openHealPath = Path.Combine(desktopPath, "OpenHeal");

        // Constrói o caminho para a subpasta Bubbles dentro de OpenHeal
        string bubblesPath = Path.Combine(openHealPath, "Bubbles");

        // Verifica se a pasta Bubbles existe, se não, cria
        if (!Directory.Exists(bubblesPath))
        {
            Directory.CreateDirectory(bubblesPath);
        }

        // Constrói o caminho final do arquivo, incluindo o nome do arquivo com a data e hora atuais
        string finalPath = Path.Combine(bubblesPath, "Bubbles Data " + DateTime.Now.ToString("dd-MM-yy--HH-mm-ss") + ".json");

        File.WriteAllText(finalPath, json);
        
    }
    */


    public void LoadJson(string file)
    {
        //  TextAsset jsonFile = Resources.Load<TextAsset>("bubble_match3");
        //   Debug.Log(jsonFile.text);
        Bolas_ProximaPartida.Clear();
        loadedSession = JsonUtility.FromJson<bubble_match_data>(file);
        // Debug.Log(loadedSession.balls.Count);
        foreach (Bola_Info b in loadedSession.data.balls)
        {
           Bolas_ProximaPartida.Add(b);
        }
        

    }

    public void LoadJson()
    {
        Bolas_ProximaPartida.Clear();
       
        // Debug.Log(loadedSession.balls.Count);
        foreach (Bola_Info b in loadedSession.data.balls)
        {
            Bolas_ProximaPartida.Add(b);
        }
    }
}

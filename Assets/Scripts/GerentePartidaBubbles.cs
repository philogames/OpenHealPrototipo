using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;




/// <summary>
/// Classe que gerencia a partida atual
/// </summary>
/// 
[System.Serializable]
public enum Estado
{
    menuInicial,
    game,
    pause
}
public class GerentePartidaBubbles : MonoBehaviour
{
    
  
    public Estado EstadoGame = Estado.menuInicial;
    

    List<GameObject> bolas;
    
    public GameObject ModeloBola;
    public Camera mainCam;
    float ScoreLocal = 0;
    public float ScorePorcentagem = 0;
    public float TempoInicialPartida = 300;
    public float precisaoScore = 1; //quanto maior, mais pontos ganha ao acertar um bom tempo.
    float tempoAtual = 0;
    //GerenciarUI gerenteUI;
    bool partidaIniciada = false;

    public UnityEvent<float> OnScoreChange;

    public UnityEvent OnIniciarPartida, OnFimPartida;
    // Start is called before the first frame update
    void Start()
    {
     //   gerenteUI = GameObject.FindObjectOfType<GerenciarUI>();

    //    gerenteUI.AbrirUI_MenuInicial();
        bolas = new List<GameObject>();
       
        //mainCam = Camera.main;
        
    }

    private void Update()
    {

        

        if (partidaIniciada)
        {
            tempoAtual += Time.deltaTime;
            TimeSpan time = TimeSpan.FromSeconds(TempoInicialPartida - tempoAtual);
       //     gerenteUI.cronometroText.text = string.Format("{0:00}:{1:00}", time.Minutes, time.Seconds);
        }

    }

    
    IEnumerator IniciarPartida_Coroutine()
    {

        yield return StartCoroutine(GameDataBubbles.Instance.SetupBubbleMatch_IfEmpty()); //caso esteja tentando iniciar o jogo no debug mode, vai gerar uma partida aleatória
            

        OnIniciarPartida?.Invoke();



        EstadoGame = Estado.game;
        InstanciarBolas();
        partidaIniciada = true;
    }

    public void IniciarPartida()
    {

        StartCoroutine(IniciarPartida_Coroutine());
    }


    public void AddScore()
    {
        ScoreLocal++;
        float percentageScore = (ScoreLocal / bolas.Count) * 100;
        ScorePorcentagem = percentageScore;
        OnScoreChange?.Invoke(percentageScore);
   //     gerenteUI.SincronizarScore(ScoreLocal);
    }


    public void InstanciarBolas()
    {
        float tempoDaUltimaBola = 0;
        //para cada bola na lista de bolas recebida da API, instanciar uma bola no jogo, de acordo com as infos recebidas
        foreach(Bola_Info bola_info in GameDataBubbles.Instance.loadedSession.data.balls)
        {
            GameObject temp_bolaObj = Instantiate(ModeloBola, this.transform);
            temp_bolaObj.GetComponent<Bola>().Inicializar(bola_info, mainCam);
            bolas.Add(temp_bolaObj);//adiciona o gameobject da bola a uma lista local para possiveis manipulacoes
            GameDataBubbles.Instance.AddBolaToSendApi(temp_bolaObj.GetComponent<Bola>().GetInfo());//adiciona a bola a lista de bolas para enviar ao servidor
            tempoDaUltimaBola = (float)bola_info.destroy_time;
          
        }
        Invoke("FimDeJogo", tempoDaUltimaBola+1f);
        StartCoroutine(GameObject.FindObjectOfType<Bubble_MarksUpdate>().Start_CollectHandData());//começa a coletar dados da mão

    }

    void FimDeJogo()
    {
        StartCoroutine(FimDeJogo_Coroutine());
    }

    IEnumerator FimDeJogo_Coroutine()
    {
        OnFimPartida?.Invoke();

        GerenciarUIBubbles gerenteTelas = GameObject.FindObjectOfType<GerenciarUIBubbles>();

        gerenteTelas.sendingDataToServer.SetActive(true);

        GameDataBubbles.Instance.loadedSession.data.SetScreenResolution(UnityEngine.Screen.currentResolution.width, UnityEngine.Screen.currentResolution.height);
        GameDataBubbles.Instance.loadedSession.data.hit_percentage = ScorePorcentagem;
        GameDataBubbles.Instance.SetLastMatchScore(ScorePorcentagem);

        if (!GameDataBubbles.Instance.DEBUG_MODE)
        {

         yield return ApiHandler.Instance.SendBubbleMatchData(GameDataBubbles.Instance.GetJsonBubblesData(), GameDataBubbles.Instance.GetJsonHandData());

        }

        
        yield return new WaitForSeconds(0.5f);
        gerenteTelas.sendingDataToServer.SetActive(false);
        gerenteTelas.AbrirUI_FimParabens();
    }
    
}

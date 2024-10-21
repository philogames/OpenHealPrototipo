using MoreMountains.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GerenteTelas : MonoBehaviour
{

    public float fadeTime = .5f;

    [Header("Tela Startup Error")]
    bool startUpLogin = false;//ficara true caso o usuario tenha salvo alguma senha antes
    public FadeCanvaGroup OnStartUpErrorCanvas;
    public Text OnStartUpErrorText; //mostrar erro em caso de login no startup mas por algum motivo não foi possível conectar


    [Header("Telas de Login")]
    public FadeCanvaGroup TelaLogin;
    public FadeCanvaGroup TelaLogin_Login;
    public InputField email_text;
    public InputField password_text;
    public Text error_text;
    public Toggle lembrarSenha_toggle;
    public GameObject loadingIcon;
    public Button botaoConectar;
    bool lembraSenha = false;

    //a TelaLogin_Inicial foi inicialmente projetada para mostrar os botões de login, cadastrar e sair do jogo,
    //mas para alinhar com o layout do site, foi removida.
    //Agora, o jogo inicia direto na tela de login, caso não tenha nenhuma conta salva.
    public FadeCanvaGroup TelaLogin_Inicial;



    [Header("Telas Principal")]
    public FadeCanvaGroup TelaPrincipal;
    public FadeCanvaGroup TelaPrincipal_Principal;
    public FadeCanvaGroup TelaPrincipal_Configuracoes;
    public FadeCanvaGroup TelaPrincipal_Usuario;
    public GameObject PainelSelecaoGames;
    public GameObject PainelLateral;
    public GameObject PainelSelecaoPresets;
    public Text nomeUsuario_text;

    [Header("Tela Seleção de Presets")]
    public GameObject presetPrefab;
    public Transform presetParent;
    public List<LevelPreset> levelPresets;
    public Text presetDescription;


    [Header("Telas de Jogo")]
    public UnityEvent OnStartBubbles;


    [Header("Unity Events")]
    public UnityEvent OnLoginScreenStart;
    public UnityEvent OnConnect, OnConnectError;
    public UnityEvent OnToggleOn, OnToggleOff;

    private void OnEnable()
    {
        TelaLogin.GetComponent<CanvasGroup>().alpha = 0;
        TelaLogin_Inicial.GetComponent<CanvasGroup>().alpha = 0;
        TelaLogin_Login.GetComponent<CanvasGroup>().alpha = 0;

        TelaPrincipal.GetComponent<CanvasGroup>().alpha = 0;
        TelaPrincipal_Principal.GetComponent<CanvasGroup>().alpha = 0;
        TelaPrincipal_Configuracoes.GetComponent<CanvasGroup>().alpha = 0;
        TelaPrincipal_Usuario.GetComponent<CanvasGroup>().alpha = 0;

        //conecta automaticamente caso senha esteja salva
        if (PlayerPrefs.HasKey("Login_email"))
        {
            startUpLogin = true;
            email_text.text = PlayerPrefs.GetString("Login_email");
            password_text.text = PlayerPrefs.GetString("Login_password");
            StartCoroutine(Conectar());
        }
        else
        {

            if (!ApiHandler.Instance.GetFezLogin())
                Invoke("InicializarTelaLogin", .1f);
            else
                Invoke("InicializarTelaPrincipal", .1f);

            OnLoginScreenStart?.Invoke();
        }


    }

    IEnumerator Conectar()
    {
        error_text.text = "";

        yield return StartCoroutine(ApiHandler.Instance.LoginCoroutine(email_text.text, password_text.text));

        if (ApiHandler.Instance.GetFezLogin())
        {
            TelaLogin.FadeOut(fadeTime);
            TelaPrincipal.FadeIn(fadeTime);
            TelaPrincipal_Principal.FadeIn(fadeTime);
            AbrirSelecaoJogos();
            SetupUserName();
            if (lembraSenha)
            {
                PlayerPrefs.SetString("Login_email", email_text.text);
                PlayerPrefs.SetString("Login_password", password_text.text);
            }
            yield return new WaitForEndOfFrame();
            OnConnect?.Invoke();
        }
        else
        {

            //caso o erro seja no startup, mostrar a mensagem na tela de erro
            if (startUpLogin)
            {
                InicializarTelaStartupError();
                OnStartUpErrorText.text = ApiHandler.Instance.GetLoginError();
                startUpLogin = false;
            }
            else//caso o erro seja no login normal, mostrar a mensagem na tela de login
            {
                error_text.text = ApiHandler.Instance.GetLoginError();
            }


            yield return new WaitForEndOfFrame();
            OnConnectError?.Invoke();
        }
        botaoConectar.interactable = true;
        loadingIcon.SetActive(false);
    }

    public void SetToggle_LembrarSenha()
    {

        lembraSenha = !lembraSenha;
        if (lembraSenha)
        {
            OnToggleOn?.Invoke();
        }
        else
        {
            OnToggleOff?.Invoke();
        }
        lembrarSenha_toggle.isOn = lembraSenha;
    }
    public void SetupUserName()
    {
        nomeUsuario_text.text = ApiHandler.Instance.GetPlayerNickName();
    }
    public void BotaoLogin()
    {
       
        TelaLogin_Inicial.FadeOut(fadeTime);
        TelaLogin_Login.FadeIn(fadeTime);
        
    }

    public void BotaoTentarNovamente_ErroStartUp()
    {
        TelaLogin.FadeIn(fadeTime);
        TelaLogin_Login.FadeIn(fadeTime);
        OnStartUpErrorCanvas.FadeOut(fadeTime);
        TelaPrincipal.FadeOut(fadeTime);

       // BotaoLogin();
    }

    public void BotaoVoltarTelaInicial()
    {
        error_text.text = "";
        TelaLogin_Inicial.FadeIn(fadeTime);
        TelaLogin_Login.FadeOut(fadeTime);
    }

    public void BotaoCadastrar()
    {
        Application.OpenURL("https://game.openheal.org/signup");
    }

    public void BotaoSair()
    {
        Application.Quit();
    }

    public void BotaoConectar()
    {
        loadingIcon.SetActive(true);
        botaoConectar.interactable = false;
        StartCoroutine(Conectar());
    }

    public void BotaoDesconectar()
    {
        ApiHandler.Instance.Logout();
        TelaPrincipal.FadeOut(fadeTime);
      //  TelaLogin_Login.FadeOut(fadeTime);
        TelaLogin.FadeIn(fadeTime);
        TelaLogin_Login.FadeIn(fadeTime);
        
    
    }

    public void BotaoBubblesJogar(string level = "")
    {
        StartCoroutine(GetBubblesData(level)); 
    }

    public void BotaoBubblesPreset()
    {
        AbrirSelecaoPresets();
        InicializarTelaSelecaoPresets();

    }

    IEnumerator GetBubblesData(string level)
    {
        OnStartBubbles?.Invoke();
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(ApiHandler.Instance.GetBubbleMatch(level));
        SceneManager.LoadScene("Bubbles");
        
        
    }

    

    


    public void InicializarTelaSelecaoPresets()
    {

        presetDescription.text = ApiHandler.Instance.playerLoginData.data.preset_description ;
        int i = 1;
        foreach(levelInfo preset in ApiHandler.Instance.GetPlayerPresets())
        {
            //instanciar preset no scrollview
            GameObject p = Instantiate(presetPrefab, presetParent);
            PresetPrefab pPrefab = p.GetComponent<PresetPrefab>();

            //setar nome e descrição do preset
            pPrefab.SetPresetName(preset.level);
            pPrefab.SetPresetDescription(preset.description);

            //selecionar preset atual
            if(i == int.Parse(ApiHandler.Instance.playerLoginData.data.current_level))
            {
                pPrefab.SetPresetCurrentLevel();
            }
            i++;
        }

        
    }
   
    public void InicializarTelaLogin()
    {
       
        if(ApiHandler.Instance.GetFezLogin())
        {
            TelaPrincipal.FadeIn(fadeTime);
            TelaPrincipal_Principal.FadeIn(fadeTime);

        }
        else
        {
            TelaLogin.FadeIn(fadeTime);
            TelaLogin_Login.FadeIn(fadeTime);
        }
    }

    public void InicializarTelaPrincipal()
    {
        TelaPrincipal.FadeIn(fadeTime);
        TelaPrincipal_Principal.FadeIn(fadeTime);
    }

    public void  SairTelaSelecaoPresets()
    {
        //destuir presets antigos
        foreach (Transform t in presetParent)
        {
            if (t.gameObject != presetPrefab)
                Destroy(t.gameObject);
        }

        AbrirSelecaoJogos();

    }

    void AbrirSelecaoJogos()
    {
        PainelSelecaoGames.SetActive(true);
        PainelSelecaoPresets.SetActive(false);
        PainelLateral.SetActive(true);
    }

    void AbrirSelecaoPresets()
    {
        PainelSelecaoGames.SetActive(false);
        PainelSelecaoPresets.SetActive(true);
        PainelLateral.SetActive(false);
    }

    void InicializarTelaStartupError()
    {
        OnStartUpErrorCanvas.FadeIn(fadeTime);
    }

    

}

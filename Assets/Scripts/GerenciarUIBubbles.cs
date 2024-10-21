using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Mediapipe.Unity;
using System;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
public class GerenciarUIBubbles : MonoBehaviour
{

    public GerentePartidaBubbles gerentePartida;
    

    [Header("UI Tela Inicial")]
    public GameObject UI_TelaInicial;

    [Header("UI Game Loop")]
    public GameObject UI_GameLoop;
    public Text scoreLocalText;
    public GameObject contagemRegressiva;
    public Text contagemRegressivaText;
    public GameObject sendingDataToServer;
    public UnityEvent Contagem321, ContagemGO;

    [Header("UI Opcoes")]
    public GameObject UI_Opcoes;
    public GameObject ColorPicker;
    public GameObject TexturePicker;
    public Texture2D DefaultTexture;
    public UnityEvent OnUseMaskOn, OnUseMaskOff;

    Color oldColor;

    [Header("UI Fim Parabens")]
    public GameObject UI_FimParabens;
    public Text finalScoreLabel;
    public GameObject botaoJogarNovamente;
    public GameObject botaoSair;
    public UnityEvent OnOpenFimParabens, OnScoreAdding, OnShowButtons;

    [Header("WebCam Renderers")]
    public RawImage MiniaturaWebCam;
    public RawImage TextureWebCam;

    [Header("Mask Manager")]
    public MaskAnnotation maskAnnotation;
    GameObject activeUI;

    [Header("UI Elements")]
    public Toggle maskToggle;
    public Slider thresholdSlider;
    public Dropdown maskTypeDropdown;
    public Slider musicVolume;
    public Slider sfxVolume;
    public Texture2D[] texture2Ds;
    public FlexibleColorPicker colorPicker;



    void OnEnable()
    {
        AbrirUI_TelaInicial();
        UI_GameLoop.GetComponent<CanvasGroup>().alpha = 0;
        UI_FimParabens.GetComponent<CanvasGroup>().alpha = 0;
        UI_Opcoes.GetComponent<CanvasGroup>().alpha = 0;
        
    }
    private void Start()
    {
        StartCoroutine(LoadPlayerPrefs());
    }

    IEnumerator LoadPlayerPrefs()
    {
        // Carregar e definir o volume da música
        float musicVolumeValue = PlayerPrefs.GetFloat("MusicVolume", .3f);
        musicVolume.value = musicVolumeValue;
        
     
        // Carregar e definir o volume do SFX
        float sfxVolumeValue = PlayerPrefs.GetFloat("SfxVolume", 1);
        sfxVolume.value = sfxVolumeValue;

      
        yield return new WaitUntil(() =>  GameObject.FindObjectOfType<WebCamSource>().isPlaying);

        yield return new WaitForSeconds(.1f);

        // Carregar e definir o estado da máscara
        int maskOnOff = PlayerPrefs.GetInt("MaskOnOff", 0);
        maskToggle.isOn = maskOnOff == 1;
        yield return new WaitForSeconds(.01f);
        // Carregar e definir o threshold da máscara
        float maskThreshold = PlayerPrefs.GetFloat("MaskThreshold", 0);
        thresholdSlider.value = maskThreshold;
        yield return new WaitForSeconds(.01f);
        // Carregar e definir o tipo de máscara
        int maskType = PlayerPrefs.GetInt("MaskType", 0);
        maskTypeDropdown.value = maskType;
        SetMaskType(maskType);
        yield return new WaitForSeconds(.01f);
        if (maskType == 0)
        {

            // Carregar e definir a cor da máscara
            float maskColorR = PlayerPrefs.GetFloat("MaskColorR", 1);
            float maskColorG = PlayerPrefs.GetFloat("MaskColorG", 1);
            float maskColorB = PlayerPrefs.GetFloat("MaskColorB", 1);
            float maskColorA = PlayerPrefs.GetFloat("MaskColorA", 1);
            Color maskColor = new Color(maskColorR, maskColorG, maskColorB, maskColorA);
            colorPicker.color = maskColor;
            //OnMaskColorChange(maskColor);
        }
        else
        if (maskType == 1)
        {
            // Carregar e definir a textura da máscara
            string maskTextureName = PlayerPrefs.GetString("MaskTexture", "");
            foreach (Texture2D texture in texture2Ds)
            {
                if (texture.name == maskTextureName)
                {
                    SetMaskTexture(texture);
                    break;
                }
            }
        }

        colorPicker.enabled = true;
    }


    public void AbrirUI_TelaInicial()
    {
        activeUI?.GetComponent<FadeCanvaGroup>().FadeOut(.5f);
        UI_TelaInicial.GetComponent<FadeCanvaGroup>().FadeIn(.5f);
        activeUI = UI_TelaInicial;
        Invoke("SetUpMiniatura", 1);
    }
    
    
    void SetUpMiniatura()
    {
       
        MiniaturaWebCam.texture = TextureWebCam.texture;
        MiniaturaWebCam.color = Color.white;
        maskAnnotation._screen = MiniaturaWebCam;
    }

    void TearDownMiniatura()
    {
        MiniaturaWebCam.color = new Color(0, 0, 0, 0);

        MiniaturaWebCam.texture = null;
        maskAnnotation._screen = TextureWebCam;
    }

    public void AbrirUI_GameLoop()
    {
        activeUI?.GetComponent<FadeCanvaGroup>().FadeOut(1);
        UI_GameLoop.GetComponent<FadeCanvaGroup>().FadeIn(1);
        activeUI = UI_GameLoop;
        TearDownMiniatura();
    }


    public void AbrirUI_FimParabens()
    {
        StartCoroutine(AbrirFimParabens_Coroutine());
    }

    IEnumerator AbrirFimParabens_Coroutine()
    {
        activeUI = UI_FimParabens;

        finalScoreLabel.text = "0";
        int _score = 0;

        botaoJogarNovamente.SetActive(false);
        botaoSair.SetActive(false);

        OnOpenFimParabens?.Invoke();
        activeUI?.GetComponent<FadeCanvaGroup>().FadeOut(1);
        yield return new WaitForSeconds(1);

        UI_FimParabens.GetComponent<FadeCanvaGroup>().FadeIn(1);
        yield return new WaitForSeconds(2.5f);

        while(_score < GameDataBubbles.Instance.GetLastMatchScore())
        {
            _score += 1;
            finalScoreLabel.text = _score.ToString("0");
            OnScoreAdding?.Invoke();
            yield return new WaitForEndOfFrame();
        }

        finalScoreLabel.text = GameDataBubbles.Instance.GetLastMatchScore().ToString("0");

        yield return new WaitForSeconds(1.5f);


        OnShowButtons?.Invoke();
        botaoJogarNovamente.SetActive(true);
        botaoSair.SetActive(true);
        
        
    }

    public void AbrirUI_Opcoes()
    {
        activeUI?.GetComponent<FadeCanvaGroup>().FadeOut(1);
        UI_Opcoes.GetComponent<FadeCanvaGroup>().FadeIn(1);
        activeUI = UI_Opcoes;
        
    }

 

    public void SincronizarScore(float newScore)
   {
        scoreLocalText.text = newScore.ToString("0");
   }

    public void OnMaskColorChange(Color newColor)
    {
        maskAnnotation._color = newColor;
        maskAnnotation.ApplyMaskTexture(maskAnnotation._maskTexture, newColor);
        //salvar cor

        PlayerPrefs.SetFloat("MaskColorR", newColor.r);
        PlayerPrefs.SetFloat("MaskColorG", newColor.g);
        PlayerPrefs.SetFloat("MaskColorB", newColor.b);
        PlayerPrefs.SetFloat("MaskColorA", newColor.a);
    }

    public void SetMaskOnOff(bool v)
    {
        maskAnnotation.SetMaskOnOff(v);

        if(v)
        {
            OnUseMaskOn?.Invoke();
        }
        else
        {
            OnUseMaskOff?.Invoke();
        }
        //salvar estado

        PlayerPrefs.SetInt("MaskOnOff", v ? 1 : 0);
    }

    public void SetThreshold(float v)
    {
        maskAnnotation._threshold = v;
        maskAnnotation.ApplyThreshold(v);
        //salvar threshold

        PlayerPrefs.SetFloat("MaskThreshold", v);
    }

    
    public void SetMaskType(Int32 v)
    {
        if(v == 0)
        {
            ColorPicker.SetActive(true);
            TexturePicker.SetActive(false);

            if(oldColor != null)
            {
                maskAnnotation._color = oldColor;
            }

            maskAnnotation._maskTexture = null;
            maskAnnotation.ApplyMaskTexture(null, maskAnnotation._color);
        }
        else
        {   
            oldColor = maskAnnotation._color;
            ColorPicker.SetActive(false);
            TexturePicker.SetActive(true);
            maskAnnotation._maskTexture = DefaultTexture;
            maskAnnotation.ApplyMaskTexture(DefaultTexture, maskAnnotation._color);
        }

        //salvar ESTADO

        PlayerPrefs.SetInt("MaskType", v);
    }


    public void SetMaskTexture(Texture2D v)
    {
        maskAnnotation._maskTexture = v;
        maskAnnotation.ApplyMaskTexture(v, maskAnnotation._color);

        //salvar textura

        PlayerPrefs.SetString("MaskTexture", v.name);
    }

    
    public void IniciarContagemRegressiva()
    {
        StartCoroutine(ContagemRegressiva());
    }
   
    IEnumerator ContagemRegressiva()
    {
        AbrirUI_GameLoop();
        yield return new WaitForSeconds(.5f);
        contagemRegressiva.SetActive(true);

        Contagem321?.Invoke();
        contagemRegressivaText.text = "3";
        yield return new WaitForSeconds(1);

        Contagem321?.Invoke();
        contagemRegressivaText.text = "2";
        yield return new WaitForSeconds(1);

        Contagem321?.Invoke();    
        contagemRegressivaText.text = "1";
        yield return new WaitForSeconds(1);

        ContagemGO?.Invoke();
        contagemRegressivaText.text = "GO!";
        yield return new WaitForSeconds(1);
        contagemRegressivaText.text = "";
        contagemRegressiva.SetActive(false);

        gerentePartida.IniciarPartida();
    }



    public void FimParabens_JogarNovamente()
    {
        
        StartCoroutine(JogarNovamente_Coroutine());
    }

    IEnumerator JogarNovamente_Coroutine()
    {
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(ApiHandler.Instance.GetBubbleMatch(""));
        SceneManager.LoadScene("Bubbles");
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("Tela1_Bootstrap");
    }

  
}

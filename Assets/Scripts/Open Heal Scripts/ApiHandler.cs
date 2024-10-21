using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using MoreMountains.Tools;
using System.Collections.Generic;



using System;
using UnityEngine.UI;
/// <summary>
/// http://openheal-api-dev.wemoga.com.br/swagger/index.html
/// </summary>

[System.Serializable]
public class ErrorData
{

    public bool isSuccess;
    public List<string> listErrors;
}

[System.Serializable]
public class levelInfo
{
    public string level;
    public string description;
}

[Serializable]
public class PlayerLoginData
{
    public Data data;
    public bool isSuccess;

    [Serializable]
    public class Data
    {
        public string name;
        public string last_name;
        public string nickname;
        public string email;
        //public PlayerProgress player_progress;
        public string current_level;
        public string preset_description;
        public string description_current_level;
        public List<levelInfo> levels;
        public Token token;
    }

    

    [Serializable]
    public class PlayerProgress
    {
        public int? matches_played;
        public string total_hours_played;
        public int? total_score;
        public List<LevelPreset> presets;
    }

    [Serializable]
    public class Token
    {
        public string token;
        public string expiration;
    }
}

[System.Serializable]
public class LoginData
{
    public string email;
    public string password;
}

[System.Serializable]
public class RegisterData
{
    public string email;
    public string password;
    public string passwordConfirmation;
}

[System.Serializable]
public class LevelPreset
{

    public string levelName;
    public string levelDescription;

    public LevelPreset(string levelName, string levelDescription)
    {
        this.levelName = levelName;
        this.levelDescription = levelDescription;
    }
}
[System.Serializable]
public class LevelRequest
{
    public string level;

    public LevelRequest(string level)
    {
        this.level = level;
    }

}
public class ApiHandler : MonoBehaviour
{
    public static ApiHandler Instance { get; private set; }

    bool fezLogin = false;

    [Header("Login Settings")]
    [MMInspectorButton("TryLogin")]
    public bool DEBUG_TentarConectar;

    public string email_login = "user@example.com"; 
    public string password_login = "sua_senha";
    string ErrorMessage = "";


    [Header("Register Settings - DEBUG ONLY")]
    [MMInspectorButton("Register")]
    public bool DEBUG_TentarRegistrar;

    public string email_register = "user@example.com";
    public string password_register = "sua_senha";
    public string passwordConfirm_register = "sua_senha";

    [SerializeField]
    public PlayerLoginData playerLoginData;



    [Header("Bubble Match Settings - DEBUG ONLY")]
    [MMInspectorButton("GetBubbleMatchData")]
    public bool DEBUG_GetBubbleMatchData;

    private string bubbleMatchData;

    private void OnEnable()
    {

        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void Register()
    {
        StartCoroutine(RegisterCoroutine(email_register, password_register, passwordConfirm_register));
    }
    public void TryLogin()
    {
        StartCoroutine(LoginCoroutine(email_login, password_login));
    }

   

    IEnumerator RegisterCoroutine(string email, string password, string passwordConfirm)
    {
        //string url = "http://openheal-api-dev.wemoga.com.br/api/ApplicationUser/register";
        string url = "https://openheal-api.openheal.org/api/ApplicationUser/register"; 

        RegisterData registerData = new RegisterData
        {
            email = email,
            password = password,
            passwordConfirmation = passwordConfirm
        };

        string jsonData = JsonUtility.ToJson(registerData);

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Erro no cadastro: " + www.error + "\nResponse: " + www.downloadHandler.text);
        }
        else
        {
            Debug.Log("Cadastro bem-sucedido: " + www.downloadHandler.text);
            // Aqui você pode processar a resposta, como extrair um token de acesso se necessário
        }
    }

    public void Logout()
    {
        PlayerPrefs.DeleteKey("Login_email");
        PlayerPrefs.DeleteKey("Login_password");
        fezLogin = false;
        playerLoginData = null;
    }

    public IEnumerator LoginCoroutine(string email, string password)
    {

        //antigo endpoint que não retorna os presets
        //string url = "https://openheal-api.openheal.org/api/ApplicationUser/player-login";

        string url = "https://openheal-api.openheal.org/api/ApplicationUser/player-login-with-levels";

        Debug.Log(email + "  " + password);
        LoginData loginData = new LoginData
        {
            email = email,
            password = password
        };

        string jsonData = JsonUtility.ToJson(loginData);

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {

            try
            {
                ErrorData loginError = JsonUtility.FromJson<ErrorData>(www.downloadHandler.text);

                ErrorMessage = loginError.listErrors[0];
                
            }
            catch(Exception e)
            {
                ErrorMessage = www.error;
            }

            Debug.Log(ErrorMessage);
        }
        else
        {
            Debug.Log("Login bem-sucedido: " + www.downloadHandler.text);

            PlayerPrefs.SetString("email", email);
            PlayerPrefs.SetString("passoword", password);
            fezLogin = true;

            playerLoginData = JsonUtility.FromJson<PlayerLoginData>(www.downloadHandler.text);

        }


    }

    
    public void GetBubbleMatchData()
    {
        StartCoroutine(GetBubbleMatch(""));
    }
    
    public IEnumerator GetBubbleMatch(string nivel)
    {
        yield return new WaitForSeconds(1f);

        string url = "https://openheal-api.openheal.org/api/bubble/play-bubble-match";

        LevelRequest levelRequest = new LevelRequest(nivel);
        string jsonData = JsonUtility.ToJson(levelRequest);

        Debug.Log("JSON Data being sent: " + jsonData);

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);//envia o parametro do nivel requisitado
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + playerLoginData.data.token.token); // Adicione o token de acesso aqui
        
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Erro ao obter a partida: " + www.error + "\nResponse: " + www.downloadHandler.text);
        }
        else
        {
            Debug.Log("Partida obtida com sucesso: " + www.downloadHandler.text);
          
          //  GameData.Instance.SetupBubbleMatch(www.downloadHandler.text);
            bubbleMatchData = www.downloadHandler.text ;
        }
    }
    public string GetBubbleMatchString()
    {
       return bubbleMatchData;
    }

    public IEnumerator SendBubbleMatchData(string bubblesData, string handsData)
    {
        // string url = "http://openheal-api-dev.wemoga.com.br/api/bubble/bubble-match-result";

        string url = "https://openheal-api.openheal.org/api/bubble/bubble-match-result"; 


        // Converta as strings para binário
        byte[] bubblesDataBytes = System.Text.Encoding.UTF8.GetBytes(bubblesData);
        byte[] handsDataBytes = System.Text.Encoding.UTF8.GetBytes(handsData);

        // Criação do form
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormFileSection("BubblesData", bubblesDataBytes, "bubblesData.txt", "text/plain"));
        formData.Add(new MultipartFormFileSection("HandsData", handsDataBytes, "handsData.txt", "text/plain"));

        UnityWebRequest www = UnityWebRequest.Post(url, formData);
        
        // Imprima os parâmetros no console
        Debug.Log("BubblesData: " + bubblesData);
        Debug.Log("HandsData: " + handsData);

        // Adicionando o token de acesso no cabeçalho
        www.SetRequestHeader("Authorization", "Bearer " + playerLoginData.data.token.token);

        // Iniciando a Coroutine para enviar a requisição
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Erro ao enviar os dados da partida: " + www.error + "\nResponse: " + www.downloadHandler.text);
            GameObject erro =  GameObject.Find("ERRO BUBBLES SEND");
            if(erro != null)
            {
                erro.GetComponent<Text>().text = www.error + "\nResponse: " + www.downloadHandler.text;
                GameObject score = GameObject.Find("SCORE BUBBLES");
                if(score != null)
                {
                    score.SetActive(false);
                }
            }
        }
        else
        {
            Debug.Log("Dados da partida enviados com sucesso: " + www.downloadHandler.text);
        }
    }

    public List<levelInfo> GetPlayerPresets()
    {
        return playerLoginData.data.levels;
    }

    public bool GetFezLogin()
    {
        return fezLogin;
    }

    public string GetLoginError()
    {
        return ErrorMessage;
    
    }

    public string GetPlayerNickName()
    {
        return playerLoginData.data.nickname;
    }
}

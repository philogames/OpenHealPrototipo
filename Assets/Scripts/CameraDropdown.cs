using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class CameraDropdown : MonoBehaviour
{
    public Dropdown dropdown;
    public HandboxCollider webCamInput;
    
    void Start()
    {
        UpdateWebcamList();
    }

    public void UpdateWebcamList()
    {
        // Verifica se o Dropdown est� configurado
        if (dropdown == null)
        {
            Debug.LogError("Dropdown n�o est� configurado no script CameraDropdown");
            return;
        }

        // Obt�m a lista de dispositivos de c�mera
        var devices = WebCamTexture.devices;

        // Prepara uma lista para as op��es do Dropdown
        List<string> options = new List<string>();

        foreach (var device in devices)
        {
            // Testa se a c�mera suporta alguma das resolu��es comuns
            if (TestCameraDevice(device.name))
            {
                // Adiciona apenas c�meras com uma resolu��o compat�vel
                options.Add(device.name);
            }
        }

        // Limpa op��es existentes e adiciona as novas
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
    }



    public void SetCam(int i)
    {
        var deviceName = WebCamTexture.devices[i].name;

        // Verifica se a c�mera selecionada est� funcionando
        if (TestCameraDevice(deviceName))
        {
            // Se a c�mera est� funcionando, inicia o MediaPipe com essa c�mera
            StartCoroutine(webCamInput.Inicializar(i));
        }
        else
        {
            // Se a c�mera n�o est� funcionando, mostra uma mensagem de erro
            Debug.LogError("A c�mera selecionada n�o est� funcionando corretamente.");
            // Aqui voc� pode implementar qualquer l�gica adicional, como reverter para uma c�mera padr�o
            // ou pedir ao usu�rio para selecionar outra c�mera.
        }
    }



    bool TestCameraDevice(string deviceName)
    {
        WebCamTexture testTexture = new WebCamTexture(deviceName);
        try
        {
            testTexture.Play();
        }
        catch(Exception e)
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


}

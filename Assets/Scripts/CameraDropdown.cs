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
        // Verifica se o Dropdown está configurado
        if (dropdown == null)
        {
            Debug.LogError("Dropdown não está configurado no script CameraDropdown");
            return;
        }

        // Obtém a lista de dispositivos de câmera
        var devices = WebCamTexture.devices;

        // Prepara uma lista para as opções do Dropdown
        List<string> options = new List<string>();

        foreach (var device in devices)
        {
            // Testa se a câmera suporta alguma das resoluções comuns
            if (TestCameraDevice(device.name))
            {
                // Adiciona apenas câmeras com uma resolução compatível
                options.Add(device.name);
            }
        }

        // Limpa opções existentes e adiciona as novas
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
    }



    public void SetCam(int i)
    {
        var deviceName = WebCamTexture.devices[i].name;

        // Verifica se a câmera selecionada está funcionando
        if (TestCameraDevice(deviceName))
        {
            // Se a câmera está funcionando, inicia o MediaPipe com essa câmera
            StartCoroutine(webCamInput.Inicializar(i));
        }
        else
        {
            // Se a câmera não está funcionando, mostra uma mensagem de erro
            Debug.LogError("A câmera selecionada não está funcionando corretamente.");
            // Aqui você pode implementar qualquer lógica adicional, como reverter para uma câmera padrão
            // ou pedir ao usuário para selecionar outra câmera.
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

        // Aguarde um curto período para a câmera iniciar
        System.Threading.Thread.Sleep(100);

        // Verifica se a câmera está efetivamente transmitindo com uma resolução aceitável
        bool isWorking = testTexture.width > 16 && testTexture.height > 16;

        testTexture.Stop();
        return isWorking;
    }


}

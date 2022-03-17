using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class ConnectUI : NetworkBehaviour
{
    [SerializeField] private UnityTransport transport;
    [SerializeField] private NetworkManager manager;
    [SerializeField] private TMP_InputField addressField;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject sceneCam;

    bool isFullscreen = false;

    public void Connect()
    {
        if(addressField.text == "") { addressField.text = "127.0.0.1"; }
        transport.SetConnectionData(addressField.text, 18769);
        manager.StartClient();
    }

    public void Host()
    {
        if (addressField.text == "") { addressField.text = "127.0.0.1"; }
        transport.SetConnectionData(addressField.text, 18769);
        manager.StartHost();
    }

    public void CanvasDisable()
    {
        canvas.SetActive(false);
        sceneCam.SetActive(false);
    }

    public void SetFullscreen()
    {
        if(isFullscreen)
        {
            Screen.fullScreen = false;
            isFullscreen = false;
        }
        else
        {
            Screen.fullScreen = true;
            isFullscreen = true;
        }
    }
}

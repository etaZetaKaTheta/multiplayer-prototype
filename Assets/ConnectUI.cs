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

    public void Connect()
    {
        transport.SetConnectionData(addressField.text, 18769);
        manager.StartClient();
    }

    public void Host()
    {
        transport.SetConnectionData(addressField.text, 18769);
        manager.StartHost();
    }

    public void CanvasDisable()
    {
        canvas.SetActive(false);
        sceneCam.SetActive(false);
    }
}

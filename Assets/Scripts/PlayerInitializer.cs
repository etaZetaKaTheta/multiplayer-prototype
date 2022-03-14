using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerInitializer : NetworkBehaviour
{
    [SerializeField] private Movement movement;
    [SerializeField] private RaycastWeapon weapons;
    [SerializeField] private PlayerStats stats;
    [SerializeField] private GameObject playerCanvas;
    [SerializeField] private GameObject playerCamera;
    private void Start()
    {
        if (IsLocalPlayer) { return; }
        movement.enabled = false;
        weapons.enabled = false;
        stats.enabled = false;
        playerCanvas.SetActive(false);
        playerCamera.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RaycastWeapon : NetworkBehaviour
{
    [Header("Weapon Stats")]
    [SerializeField] private int damage;
    [SerializeField] private float distance = 1000.0f;
    [SerializeField] private float timeBetweenShots = 0.5f;

    bool isReadyToShoot = true;
    

    [SerializeField] private Camera cam;
    
    [SerializeField] private LayerMask layersToHit;
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && isReadyToShoot)
        {
            isReadyToShoot = false;
            Invoke("ResetTimeBetweeenShots", timeBetweenShots);
            Fire();
        }
    }

    private void Fire()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));

        if (Physics.Raycast(ray, out RaycastHit hit, distance, layersToHit))
        {
            hit.collider.TryGetComponent(out PlayerStats health);
            
            DealDamageServerRpc(health, damage);

            Debug.Log("HIT");
        }
    }

    [ServerRpc]
    private void DealDamageServerRpc(NetworkBehaviourReference health, int damage)
    {
        if(health.TryGet(out PlayerStats healthComponent))
        {
            healthComponent.health.Value -= damage;
        }
    }

    private void ResetTimeBetweeenShots()
    {
        isReadyToShoot = true;
    }
}

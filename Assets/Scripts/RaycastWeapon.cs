using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class RaycastWeapon : NetworkBehaviour
{
    public NetworkVariable<int> kills = new NetworkVariable<int>(NetworkVariableReadPermission.Everyone, 0);
    [Header("Weapon Stats")]
    [SerializeField] private int damage;
    [SerializeField] private float distance = 1000.0f;
    [SerializeField] private float timeBetweenShots = 0.5f;

    bool isReadyToShoot = true;
    

    [SerializeField] private Camera cam;
    
    [SerializeField] private LayerMask layersToHit;

    [Header("Animation and Sounds")]
    [SerializeField] private Animator anim;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private TMP_Text killCount;

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && isReadyToShoot)
        {
            isReadyToShoot = false;
            Invoke("ResetTimeBetweeenShots", timeBetweenShots);
            Fire();
        }
    }

    private void OnEnable()
    {
        kills.OnValueChanged += SetKillCount;
    }
    private void OnDisable()
    {
        kills.OnValueChanged -= SetKillCount;
    }

    private void SetKillCount(int prevValue, int newValue)
    {
        killCount.text = "Kills: " + newValue.ToString();
    }

    private void Fire()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));

        anim.SetBool("HasShot", true);
        audioSource.Play();

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
        if (health.TryGet(out PlayerStats healthComponent))
        {
            if ((healthComponent.health.Value - damage) <= 0.0f)
            {
                kills.Value++;
            }
            healthComponent.health.Value -= damage;
        }
    }

    private void ResetTimeBetweeenShots()
    {
        isReadyToShoot = true;
        anim.SetBool("HasShot", false);
    }
}

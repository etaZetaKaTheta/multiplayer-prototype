using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class PlayerStats : NetworkBehaviour
{
    public NetworkVariable<float> health = new NetworkVariable<float>(NetworkVariableReadPermission.Everyone, 150.0f);

    [SerializeField] private float maxHealth = 150.0f;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private MeshRenderer mr;
    [SerializeField] private Movement movement;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private NetworkObject nObject;
    [SerializeField] private Vector2 range;
    [SerializeField] private Transform middlePoint;

    private void OnEnable()
    {
        health.OnValueChanged += HandleHealthChange;
        healthText.text = "Health: " +  health.Value.ToString();
    }

    private void OnDisable()
    {
        health.OnValueChanged -= HandleHealthChange;
    }

    private void HandleHealthChange(float prevValue, float newValue)
    {
        healthText.text = "Health: " + newValue.ToString();
        if(newValue <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        //movement.enabled = false;
        Respawn();
    }

    private void Respawn()
    {
        SetPlayerHealthServerRpc(maxHealth);
        transform.position = new Vector3(middlePoint.position.x + Random.Range(range.x, range.y), 4.0f, middlePoint.position.y + Random.Range(range.x, range.y));
        //movement.enabled = true;
    }

    [ServerRpc]
    private void SetPlayerHealthServerRpc(float amount)
    {
        health.Value = amount;
    }
}

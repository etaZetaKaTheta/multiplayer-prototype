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

    private void OnEnable()
    {
        health.OnValueChanged += HandleHealthChange;
        healthText.text = health.Value.ToString();
    }

    private void OnDisable()
    {
        health.OnValueChanged -= HandleHealthChange;
    }

    private void HandleHealthChange(float prevValue, float newValue)
    {
        healthText.text = newValue.ToString();
        if(newValue <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        mr.enabled = false;
        //movement.enabled = false;
        Invoke("Respawn", 2.0f);
    }

    private void Respawn()
    {
        SetPlayerHealthServerRpc(maxHealth, nObject.OwnerClientId);
        rb.MovePosition(new Vector3(0.0f, 0.0f, 0.0f));
        mr.enabled = true;
        //movement.enabled = true;
    }

    [ServerRpc]
    private void SetPlayerHealthServerRpc(float amount, ulong clientId)
    {
        health.Value = amount;

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        };

        SetPlayerVisibilityClientRpc(clientRpcParams);

    }

    [ClientRpc]
    private void SetPlayerVisibilityClientRpc(ClientRpcParams clientRpcParams = default)
    {
        mr.enabled = false;
    }
}

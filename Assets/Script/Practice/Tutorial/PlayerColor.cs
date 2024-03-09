using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;

public class PlayerColor : NetworkBehaviour
{
    public MeshRenderer MeshRenderer;

    [Networked(OnChanged = nameof(NetworkColorChanged))]
    public Color NetworkedColor { get; set; }

    private void Update()
    {
        if(HasStateAuthority == false)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            NetworkedColor = new Color(Random.Range(0f,1f), Random.Range(0f,1f),Random.Range(0f,1f),1f);
            //�� ���¿��� �ٷ� ������ �ٲ�� ���� �ƴ� Get�� �����͸� Set���Ѿ� �� 
        }
    }

    private static void NetworkColorChanged(Changed<PlayerColor> changed)
    {
        //Changed<PlayerColor>�� ��Ʈ��ũ �Ӽ��� ������ �ƴ� NetworkBehaviour�� Ÿ���Դϴ�.
        changed.Behaviour.MeshRenderer.material.color = changed.Behaviour.NetworkedColor;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void ColorChangedRpc()
    {
        NetworkedColor = Color.red;
    }
}

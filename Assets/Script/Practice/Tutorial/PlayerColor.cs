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
            //이 상태에서 바로 색상이 바뀌는 것은 아님 Get한 데이터를 Set시켜야 함 
        }
    }

    private static void NetworkColorChanged(Changed<PlayerColor> changed)
    {
        //Changed<PlayerColor>는 네트워크 속성의 유형이 아닌 NetworkBehaviour의 타입입니다.
        changed.Behaviour.MeshRenderer.material.color = changed.Behaviour.NetworkedColor;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void ColorChangedRpc()
    {
        NetworkedColor = Color.red;
    }
}

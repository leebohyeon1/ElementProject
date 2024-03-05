using HeoWeb.Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FusionMaker : MonoBehaviour
{
    public GameObject FusionManagerPrefab;

    private void Start()
    {
        // FusionConnection 스크립트를 찾습니다.
        FusionConnection fusionConnection = FindObjectOfType<FusionConnection>();

        // FusionConnection이 없다면 FusionManager를 생성합니다.
        if (fusionConnection == null)
        {
            CreateFusionManager();
        }
    }

    // FusionManager를 생성하는 함수
    private void CreateFusionManager()
    {
        Debug.Log("퓨전 매니저 생성");
        // FusionManagerPrefab을 이용하여 FusionManager 오브젝트를 생성합니다.
        GameObject fusionManager = Instantiate(FusionManagerPrefab);
    }
}

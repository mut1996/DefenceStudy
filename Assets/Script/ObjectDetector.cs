using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectDetector : MonoBehaviour
{
    [SerializeField]
    private TowerSpawner towerSpawner;
    [SerializeField]
    private TowerDataViewer towerDataViewer;

    private Camera mainCamera;
    private Ray ray;
    private RaycastHit hit;
    private Transform hitTransform = null;  // 마우스 픽킹으로 선택한 오브젝트 임시저장


    private void Awake()
    {
        // "MainCamera" 의 태그
        mainCamera = Camera.main;
    }


    private void Update()
    {
        // 마우스가 UI 에 머물러 있을 떄는 아래 코드가 실행 되지 않도록 함 
        if (EventSystem.current.IsPointerOverGameObject() == true) 
        {
            return;
        }


        if (Input.GetMouseButtonDown(0)) 
        {
            // 카메라 위치에서 ㅎ퐈면의 마우스 위치를 관통하는 광선 생성
            // ray.origin : 광선의 시작 위치(= 카메라 위치)
            // ray.direction : 광선의 진행 방향 
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // 광선에 부딪히는 오브젝트를 검출해서 hit 에 저장 

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                hitTransform = hit.transform;

                // 광선에 부딪히는 오브젝트 태그가 "Tile" 이면
                // hit. 의 위치에 스폰 
                if (hit.transform.CompareTag("Tile"))
                {
                    towerSpawner.SpawnTower(hit.transform);
                }
                // 타워 정보창 on
                else if (hit.transform.CompareTag("Tower"))
                {
                    towerDataViewer.OnPanel(hit.transform);
                }
            }

        }
        else if (Input.GetMouseButtonUp(0))
        {
            // 마우스를 눌럿을 떄 선택한 오브젝트가 없거나 선택한 오브젝트가 타워가 아니면
            if (hitTransform == null || hitTransform.CompareTag("Tower") == false)
            {
                // 타워 정보 패널을 피활성화 한다
                towerDataViewer.OffPanel();
            }

            hitTransform = null;
        }
    }

}

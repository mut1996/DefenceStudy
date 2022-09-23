using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDetector : MonoBehaviour
{
    [SerializeField]
    private TowerSpawner towerSpawner;
    [SerializeField]
    private TowerDataViewer towerDataViewer;

    private Camera mainCamera;
    private Ray ray;
    private RaycastHit hit;


    private void Awake()
    {
        // "MainCamera" �� �±�
        mainCamera = Camera.main;
    }


    private void Update()
    {
        if (Input.GetMouseButton(0)) 
        {
            // ī�޶� ��ġ���� ��ǡ���� ���콺 ��ġ�� �����ϴ� ���� ����
            // ray.origin : ������ ���� ��ġ(= ī�޶� ��ġ)
            // ray.direction : ������ ���� ���� 
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // ������ �ε����� ������Ʈ�� �����ؼ� hit �� ���� 

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                // ������ �ε����� ������Ʈ �±װ� "Tile" �̸�
                // hit. �� ��ġ�� ���� 
                if (hit.transform.CompareTag("Tile"))
                {
                    towerSpawner.SpawnTower(hit.transform);
                }
                // Ÿ�� ����â on
                else if (hit.transform.CompareTag("Tower")) 
                {
                    towerDataViewer.OnPanel(hit.transform);
                }
            }
            


        }
    }

}

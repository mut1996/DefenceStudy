using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WaveSystem : MonoBehaviour
{
    [SerializeField]
    private Wave[] waves;               // 현재 스테이지의 모든 웨이브 정보 
    [SerializeField]
    private EnemySpawner enemySpawner;
    private int currentWaveIndex = -1;  // 현재 웨이브 인덱스 

    // 웨이브 정보 출력을 위한 Get 프로퍼티(현재웨이브, 총 웨이브)
    public int CurrentWave => currentWaveIndex + 1; // 시작이 0이기 때문에 +1 
    public int MaxWave => waves.Length;

    public void StartWave() 
    {
        // 현재 맴에 적이없고 wave 가 남아있으면
        if (enemySpawner.EnemyList.Count == 0 && currentWaveIndex < waves.Length - 1) 
        {
            // 인덱스의 시작이 -1 이기 때문에 웨이브 이넫ㄱ스 중가를 제일 먼저함 
            currentWaveIndex++;

            //EnemySpawner 의 StartWave() 함수 호출. 현재 웨이브 정보 제공
            enemySpawner.StartWave(waves[currentWaveIndex]);
        }
    }

}



/// <summary>
/// [System.Serializable]
/// 구조체, 클래스를 직렬화 하는 명령
/// 메모리 상에 존재하는 오브젝트 정보를 string, 또는 byte데이터 형태로 변형 하는것 
/// (드라이브 저장, 네트워크를 통한 데이터 전송 가능)
/// </summary>

[System.Serializable]
public struct Wave 
{
    public float spawnTime;
    public int maxEnemyCount;
    public GameObject[] enemyPrefabs;
}

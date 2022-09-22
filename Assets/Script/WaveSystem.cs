using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class WaveSystem : MonoBehaviour
{
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

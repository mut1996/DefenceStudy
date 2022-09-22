using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class WaveSystem : MonoBehaviour
{
}

/// <summary>
/// [System.Serializable]
/// ����ü, Ŭ������ ����ȭ �ϴ� ���
/// �޸� �� �����ϴ� ������Ʈ ������ string, �Ǵ� byte������ ���·� ���� �ϴ°� 
/// (����̺� ����, ��Ʈ��ũ�� ���� ������ ���� ����)
/// </summary>

[System.Serializable]
public struct Wave 
{
    public float spawnTime;
    public int maxEnemyCount;
    public GameObject[] enemyPrefabs;
}

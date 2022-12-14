using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TowerTemplate : ScriptableObject
{
    public GameObject towerPrefab;      // 타워 생성을 위한 프리팹 
    public GameObject followTowerPrefab; // 임시 타워 프리팹
    public Weapon[] weapon;             // 레벨 별 무기 정보

    [System.Serializable]
    public struct Weapon 
    {
        public Sprite sprite;
        public float damage;
        public float slow;      // 감속 퍼센트(0.2 = 20%)
        public float buff;      // 공격력 중가율 (0.2 = 20%)
        public float rate;
        public float range;
        public int cost;
        public int sell;
    }
}

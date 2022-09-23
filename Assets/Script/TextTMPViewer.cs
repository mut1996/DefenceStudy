using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextTMPViewer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textPlayerHP;   // Text - TextMeshPro UI [�÷��̾��� ü��}
    [SerializeField]
    private TextMeshProUGUI textPlayerGold; // Text - TextMeshPro UI [�÷��̾��� ���]
    [SerializeField]
    private TextMeshProUGUI textWave;       // Text - TextMesPro UI [���� ���̺�/ �� ���̺�]
    [SerializeField]
    private TextMeshProUGUI textEnemyCount; // Text - TextMeshPro UI [���� ������/ �ִ� �� ����]
    [SerializeField]
    private PlayerHP playerHP;
    [SerializeField]
    private PlayerGold playerGold;
    [SerializeField]
    private WaveSystem waveSystem;
    [SerializeField]
    private EnemySpawner enemySpawner;

    private void Update()
    {
        textPlayerHP.text = playerHP.CurrentHP + "/" + playerHP.MaxHP;
        textPlayerGold.text = playerGold.CurrentGold.ToString();
        textWave.text = waveSystem.CurrentWave + "/" + waveSystem.MaxWave;
        textEnemyCount.text = enemySpawner.CurrentEnemyCount + "/" + enemySpawner.MaxEnemyCount;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EnemyAreaSpawner : MonoBehaviour
{
    public List<OpenWorldEnemyController> enemies;
    public float intervalSpawnEnemy;
    private void Start()
    {

    }


    public void SpawnEnemy()
    {

        List<OpenWorldEnemyController> inActiveEnemy = enemies.FindAll(x => x.gameObject.activeSelf == false);

        if(inActiveEnemy.Count != 0)
        {
            Debug.Log("Spawning Enemy Area");

            int index = Random.Range(0, inActiveEnemy.Count);
            inActiveEnemy[index].level = Random.Range(1, 4);
            inActiveEnemy[index].enemyCount = Random.Range(1, 5);

            inActiveEnemy[index].gameObject.SetActive(true);
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SaveFileController.instance.SendNotification("Memasuki Wilayah Berbahaya");
            Debug.Log("Player Entering Enemy Area");
            InvokeRepeating("SpawnEnemy", intervalSpawnEnemy, intervalSpawnEnemy);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Leaving Enemy Area");

            CancelInvoke();
            enemies.ForEach(x => x.gameObject.SetActive(false));

        }
    }

}

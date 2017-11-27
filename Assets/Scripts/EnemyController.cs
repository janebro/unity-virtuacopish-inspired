using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public Soldier[] enemies;
	
	// Update is called once per frame
	void Update ()
    {
        if (GameController.Instance.IsGameOver)
            return;

        foreach (Soldier enemy in enemies)
        {
            if (enemy.IsActive)
                return;
        }	

        int soldier = Random.Range(0, enemies.Length);
        enemies[soldier].Activate();
	}
}

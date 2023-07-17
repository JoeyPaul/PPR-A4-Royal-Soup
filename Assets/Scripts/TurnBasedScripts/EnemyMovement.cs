using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public static void MoveEnemies()
    {
        foreach(EnemyMovement enemy in FindObjectsOfType<EnemyMovement>())
        {
            enemy.Move();
        }
    }

    public void Move()
    {
        int rand = Random.Range(0, 4);
        switch (rand)
        {
            case 0:
                transform.position += new Vector3(1, 0, 0);
                break;
            case 1:
                transform.position += new Vector3(-1, 0, 0);
                break;
            case 2:
                transform.position += new Vector3(0, 0, 1);
                break;
            case 3:
                transform.position += new Vector3(0, 0, -1);
                break;
        }
    }
}

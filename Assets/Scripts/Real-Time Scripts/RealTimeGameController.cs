using UnityEngine;

public class RealTimeGameController : MonoBehaviour
{
    [SerializeField]
    TurnBasedPlayerMovement player;

    void Update()
    {
        if (player.arrivedAtKing)
        {
            SceneManagerScript.ChangeScene("ResultsScreenWin");
        }
    }
}

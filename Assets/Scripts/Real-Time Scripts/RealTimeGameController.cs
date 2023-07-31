using UnityEngine;

public class RealTimeGameController : MonoBehaviour
{
    [SerializeField]
    RealTimeMovement player;

    void Update()
    {
        if (player.arrivedAtKing)
        {
            print("You reached the king!");
        }
    }
}

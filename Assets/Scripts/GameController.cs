using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    TurnBasedPlayerMovement player;
    [SerializeField]
    //GameObject freeLookCamera;
    CinemachineFreeLook freeLookCamera;
    // Update is called once per frame
    void Update()
    {
        if (player.arrivedAtKing) 
        {
            print("You reached the king!");
        }
        if (Input.GetMouseButtonDown(0)) // Mouse 1 is pressed
        {
            freeLookCamera.m_XAxis.m_MaxSpeed = 300;
            freeLookCamera.m_YAxis.m_MaxSpeed = 4;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            freeLookCamera.m_XAxis.m_MaxSpeed = 0;
            freeLookCamera.m_YAxis.m_MaxSpeed = 0;
        }
    }
}

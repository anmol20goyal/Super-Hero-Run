using System;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public static bool once = true;
    public static bool onlyOnce = true;
    private GameObject hitParent;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "End Path")
        {
            GameHandler.InstanceGH.EndGame();
        }
    }
    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Obstacle") || hit.collider.CompareTag("ObstacleParent"))
        {
            if (GameHandler.InstanceGH.currentPlayerName == "man_yeti")
            {
                if (once)
                {
                    once = false;
                    if (hit.collider.name != "Obstacle3(Clone)")
                    {
                        hitParent = hit.transform.parent.gameObject;
                    }
                    // Debug.Log(hitParent.name);
                    // break the obstacle
                    StartCoroutine(GameHandler.InstanceGH.breakTheObstacle(hitParent));
                }
            }
            else
            {
                if (GameHandler.InstanceGH.currentPlayerName != "man_superhero")
                {
                    // Debug.Log("VAR453");
                    if (onlyOnce)
                    {
                        // dead
                        // GameHandler.InstanceGH.canChange = false;
                        StartCoroutine(GameHandler.InstanceGH.playerDead());
                        onlyOnce = false;
                    }
                }
            }
        }
    }
}
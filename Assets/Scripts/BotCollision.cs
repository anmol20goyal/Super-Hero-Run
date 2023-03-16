using UnityEngine;

public class BotCollision : MonoBehaviour
{
    public static bool once = true;
    public static bool onceAgain = true;
    public static bool Again = true;
    private GameObject hitParent;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "End Path")
        {
            BotManager.InstanceBM.EndGame();
            // BotManager2.InstanceBM2.EndGame();
        }
        else if (other.name == "End Path2")
        {
            BotManager2.InstanceBM2.EndGame();
        }
        else if (other.name == "End Path3")
        {
            BotManager3.InstanceBM3.EndGame();
        }
    }
    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Obstacle") || hit.collider.CompareTag("ObstacleParent"))
        {
            if (gameObject.transform.parent.name == "Bot1")
            {
                if (BotManager.InstanceBM.currentBotname == "bot_yeti")
                {
                    if (once)
                    {
                        once = false;
                        if (hit.collider.name != "Obstacle3(Clone)")
                        {
                            hitParent = hit.transform.parent.gameObject;
                        }
                   
                        // break the obstacle
                        StartCoroutine(BotManager.InstanceBM.breaking(hitParent));
                    }
                }
            }

            else if (gameObject.transform.parent.name == "Bot2")
            {
                if (BotManager2.InstanceBM2.currentBotname == "bot_yeti")
                {
                    if (onceAgain)
                    {
                        onceAgain = false;
                        if (hit.collider.name != "Obstacle3(Clone)")
                        {
                            hitParent = hit.transform.parent.gameObject;
                        }
                    
                        // break the obstacle
                        StartCoroutine(BotManager2.InstanceBM2.breaking(hitParent));
                    }
                }
            }
            
            else
            {
                if (BotManager3.InstanceBM3.currentBotname == "bot_yeti")
                {
                    if (Again)
                    {
                        Again = false;
                        if (hit.collider.name != "Obstacle3(Clone)")
                        {
                            hitParent = hit.transform.parent.gameObject;
                        }
                    
                        // break the obstacle
                        StartCoroutine(BotManager3.InstanceBM3.breaking(hitParent));
                    }
                }
            }
        }
    }
}

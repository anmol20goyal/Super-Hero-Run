using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player;
    private Vector3 offset;
    
    // Start is called before the first frame update
    private void Start()
    {
        player = GameHandler.InstanceGH.players.transform;
        offset = transform.position - player.position;
    }

    // Update is called once per frame
    private void Update()
    {
        player = GameHandler.InstanceGH.players.transform;
        transform.position = player.position + offset;
    }
}
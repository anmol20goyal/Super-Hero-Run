using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleSpawner : MonoBehaviour
{
	#region GameObjects

	[SerializeField] private GameObject[] obstacles;
	[SerializeField] private Transform[] EndGame;

	#endregion
    
    #region Variables

    private float zPlatValue;
    private float[] Xpos;

    #endregion

    private void Awake()
    {
	    Xpos = new float[EndGame.Length];
    }

    private void Start()
    {
	    for (int i = 0; i < EndGame.Length; i++)
	    {
		    Xpos[i] = EndGame[i].position.x;
	    }
	    zPlatValue = Random.Range(10,30);
    }

    private void Update()
    {
	    if (zPlatValue + 15 < EndGame[0].transform.position.z)
	    {
		    if (GameHandler.InstanceGH.startGame)
		    {
			    int randNo = Random.Range(0, obstacles.Length);
			    if (randNo != 2)
			    {
				    foreach (float xpo in Xpos)
				    {
					    Instantiate(obstacles[randNo], new Vector3(xpo, 1.7959f, zPlatValue), Quaternion.identity);
				    }
			    }
			    else
			    {
				    foreach (float xpo in Xpos)
				    {
					    GameObject obst = Instantiate(obstacles[randNo], new Vector3(xpo, 9f, zPlatValue), Quaternion.identity);
					    Quaternion rotate = Quaternion.Euler(new Vector3(90,0,90));
					    obst.transform.rotation = rotate;
				    }
			    }
			    int RandPos = Random.Range(30,40);
			    zPlatValue += RandPos;
		    }
	    }
    }
}
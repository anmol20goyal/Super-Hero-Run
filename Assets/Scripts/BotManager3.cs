using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BotManager3 : MonoBehaviour
{
	#region GameObjects

	private Animator botAnim;
	private CharacterController botCharControl;
	[SerializeField] private GameObject botsPlayers;
	[SerializeField] private GameObject restartBtn;

	#endregion

	#region Variables

	private bool startMovingBot;

	private bool startGame;
	[NonSerialized] public string currentBotname;
	[NonSerialized] public bool gameEnded;
	private bool once = true;
	private float gravity = -20f;
	private Vector3 direc;
	private float movingSpeed;
	[NonSerialized] public bool bot3Lost;
	[NonSerialized] public bool bot3Won;

	#endregion

	#region InstanceBM

	private static BotManager3 instance;

	public static BotManager3 InstanceBM3
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<BotManager3>();
			}

			return instance;
		}
	}

	#endregion

	#region constants

	private const string Texting = "isTexting";
	private const string Waving = "isWaving";
	private const string Running = "isRunning";
	private const string Walking = "isWalking";
	private const string Generic = "isGeneric";
	private const string Dead = "isDead";
	private const string kick = "isKicking";
	private const string punch_left = "isPunching_Left";
	private const string punch_right = "isPunching_Right";
	private const string hardDay = "isSad";

	#endregion
	
	public void StartBot()
	{
		startMovingBot = true;
		botsPlayers.SetActive(true);
		currentBotname = botsPlayers.name;
		botAnim = botsPlayers.GetComponent<Animator>();
		once = true;
		botCharControl = botsPlayers.GetComponent<CharacterController>();
		movementBotAnims();
	}

	// Update is called once per frame
	public void FixedUpdate()
	{
		if (startMovingBot)
		{
			botCharControl.Move(direc * Time.fixedDeltaTime);
		}
	}

	private void movementBotAnims()
	{
		botAnim.SetBool(Waving, false);
		botAnim.SetBool(Generic, false);
		botAnim.SetBool(Texting, false);
		botAnim.SetBool(Walking, false);
		botAnim.SetBool(Running, true);
		botAnim.SetBool(Dead, false);
	}

	private void Update()
	{
		if (startMovingBot && !gameEnded)
		{
			// downward gravity
			direc.y = gravity;

			checkForCurrentBot();
			// rayCasting();
			direc.z = movingSpeed;
		}

		destroyObstacles();
	}

	private void checkForCurrentBot()
	{
		if (currentBotname == "bot_yeti")
		{
			// strength player
			botsPlayers.layer = 9;
			currentBotname = "bot_yeti";
			movingSpeed = GameHandler.InstanceGH.speed;
			changeAnimations();
		}
	}

	private void changeAnimations()
	{
		if (currentBotname == "bot_yeti")
		{
			// yeti
			botAnim.SetBool(Texting, false);
			botAnim.SetBool(Waving, false);
			botAnim.SetBool(Generic, false);
			botAnim.SetBool(Running, false);
			botAnim.SetBool(Walking, true);
		}
	}

	public IEnumerator breaking(GameObject obstacleParentHitInfo)
	{
		botAnim.SetBool(Running, false);
		int RandNo = Random.Range(0, 3);

		switch (RandNo)
		{
			case 0:
				botAnim.SetBool(punch_left, true);
				yield return new WaitForSeconds(0.7f);
				break;
			case 1:
				botAnim.SetBool(punch_right, true);
				yield return new WaitForSeconds(0.35f);
				break;
			case 2:
				botAnim.SetBool(kick, true);
				yield return new WaitForSeconds(1.05f);
				break;
		}

		var rbObstacles = obstacleParentHitInfo.GetComponentsInChildren<Rigidbody>();
		foreach (Rigidbody rb in rbObstacles)
		{
			rb.AddForce(new Vector3(Random.Range(-15, 15), Random.Range(5, 10), Random.Range(15, 20)),
				ForceMode.Impulse);
			rb.AddTorque(new Vector3(Random.Range(-5, 5), Random.Range(5, 10), 0), ForceMode.Impulse);
			rb.gameObject.tag = "Untagged";

			rb.GetComponent<Collider>().isTrigger = true;
			Destroy(rb.gameObject, 6);
		}

		Destroy(obstacleParentHitInfo, 6);

		botAnim.SetBool(punch_left, false);
		botAnim.SetBool(punch_right, false);
		botAnim.SetBool(kick, false);
		botAnim.SetBool(Running,false);
		botAnim.SetBool(Walking, true);

		BotCollision.Again = true;
	}

	public void EndGame()
	{
		gameEnded = true;
		bot3Won = true;
		botAnim.SetBool(Running, false);
		botAnim.SetBool(Generic, false);
		botAnim.SetBool(Walking, false);
		if (bot3Lost)
		{
			botAnim.SetBool(hardDay, true);
			restartBtn.SetActive(true);
		}

		direc = Vector3.zero;
	}

	private void destroyObstacles()
	{
		GameObject[] obst = GameObject.FindGameObjectsWithTag("ObstacleParent");
		if (obst != null)
		{
			foreach (GameObject ob in obst)
			{
				if (ob.transform.localPosition.z + 5 < botsPlayers.transform.localPosition.z &&
				    ob.transform.localPosition.x <= -15f)
				{
					Destroy(ob);
				}
			}
		}
	}
}
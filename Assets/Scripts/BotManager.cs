using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BotManager : MonoBehaviour
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
	private Vector3 currentBotPos;
	[NonSerialized] public bool gameEnded;
	private bool once = true;
	private RaycastHit hit, hit1, hit2;
	private bool obstacleRemoved;
	private float gravity = -20f;
	private Vector3 direc;
	private float movingSpeed;
	private bool forOnce = true;
	private bool for2 = true;
	private bool for3 = true;
	[SerializeField] private LayerMask myLayerMask;
	[NonSerialized] public bool botLost;
	[NonSerialized] public bool botWon;

	#endregion

	#region InstanceBM

	private static BotManager instance;

	public static BotManager InstanceBM
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<BotManager>();
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
			rayCasting();
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
			movingSpeed = GameHandler.InstanceGH.speed * 0.75f;
			changeAnimations();
		}
		else if (currentBotname == "bot_ninja")
		{
			// ninja player
			botsPlayers.layer = 9;
			currentBotname = "bot_ninja";
			movingSpeed = GameHandler.InstanceGH.speed;
			changeAnimations();
		}
	}

	private void rayCasting()
	{
		Vector3 pos = new Vector3(botsPlayers.transform.localPosition.x, 3, botsPlayers.transform.localPosition.z);
		Vector3 pos2 = new Vector3(botsPlayers.transform.localPosition.x, 9, botsPlayers.transform.localPosition.z);
		Vector3 pos3 = new Vector3(botsPlayers.transform.localPosition.x, 6, botsPlayers.transform.localPosition.z);
		Debug.DrawRay(pos2, Vector3.forward * 6, Color.yellow);
		Debug.DrawRay(pos3, Vector3.forward * 7, Color.blue);
		Debug.DrawRay(pos, Vector3.forward * 8, Color.black);

		if (!Physics.Raycast(pos, Vector3.forward, out hit, 8, myLayerMask))
		{
			forOnce = true;
		}

		if (!Physics.Raycast(pos2, Vector3.forward, out hit1, 7, myLayerMask))
		{
			for2 = true;
		}

		if (!Physics.Raycast(pos3, Vector3.forward, out hit2, 6, myLayerMask))
		{
			for3 = true;
		}

		if (Physics.Raycast(pos, Vector3.forward, out hit, 8, myLayerMask) && hit.collider.gameObject.layer == 10 &&
		    forOnce &&
		    currentBotname != "bot_superhero")
		{
			// Debug.Log("flying");
			changeToYetiBot();

			forOnce = false;
		}
		else if (Physics.Raycast(pos2, Vector3.forward, out hit1, 7, myLayerMask) && currentBotname != "bot_ninja" &&
		         for2)
		{
			// Debug.Log("ninja");
			changeToNinjaBot();
			for2 = false;
		}
		else if (Physics.Raycast(pos3, Vector3.forward, out hit2, 6, myLayerMask) &&
		         (hit2.collider.CompareTag("Obstacle") || hit2.collider.CompareTag("ObstacleParent")) && for3)
		{
			// Debug.Log("yeti bot");
			changeToYetiBot();
			for3 = false;
		}
	}

	private void changeToYetiBot()
	{
		// Debug.Log("yeti");
		botsPlayers.transform.localScale = Vector3.one * 3;
		currentBotname = "bot_yeti";
		changeAnimations();
	}

	private void changeToNinjaBot()
	{
		// Debug.Log("ninja Bot");
		botsPlayers.transform.localScale = Vector3.one * 2;
		currentBotname = "bot_ninja";
		changeAnimations();
	}
	
	private void changeAnimations()
	{
		if (currentBotname == "bot_ninja")
		{
			// ninja
			botAnim.SetBool(Texting, false);
			botAnim.SetBool(Waving, false);
			botAnim.SetBool(Generic, false);
			botAnim.SetBool(Running, true);
			botAnim.SetBool(Walking, false);
		}

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
		botAnim.SetBool(Running, true);

		BotCollision.once = true;
		yield return new WaitForSeconds(1f);
		changeToNinjaBot();
	}

	public void EndGame()
	{
		gameEnded = true;
		botWon = true;
		botAnim.SetBool(Running, false);
		botAnim.SetBool(Generic, false);
		botAnim.SetBool(Walking, false);
		if (botLost)
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
					ob.transform.localPosition.x < -5f && ob.transform.localPosition.x > -10f)
				{
					Destroy(ob);
				}
			}
		}
	}
}
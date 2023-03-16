using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameHandler : MonoBehaviour
{
	#region GameObjects

	private Transform PlayerTrans;
	private Animator CharAnim;
	[SerializeField] private Button StartBtn;
	[SerializeField] public GameObject players;
	private CharacterController charControl;
	private GameObject playerMatChange;
	[SerializeField] private Material fadeCharMat;
	private Material opaqueSuperMat;
	[SerializeField] private GameObject restartBtn;

	#endregion

	#region constants

	private const float transitionTime = 0.3f;

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

	#region InstanceGH

	private static GameHandler instance;

	public static GameHandler InstanceGH
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<GameHandler>();
			}

			return instance;
		}
	}

	#endregion

	#region Variables


	[NonSerialized] public string currentPlayerName;
	private bool allowMove;
	private string stateName;
	[NonSerialized] public bool startGame;
	private bool endGame;
	private int change;
	private Vector3 direc;
	public float speed;
	private float forwardSpeed;
	public bool canChange;
	private Vector3 currentPlayerPos;
	private string transitionTO;
	private float gravity = -20f;
	private float height;
	private float rotateAngle = 70f;
	[SerializeField] private LayerMask layers;
	[NonSerialized] public bool playerlost;
	private bool once = true;
	[NonSerialized] public bool won;

	#endregion

	private void Awake()
	{
		opaqueSuperMat = players.transform.GetChild(0).GetComponentsInChildren<Transform>()[1]
			.GetComponent<Renderer>().material;
		
		restartBtn.SetActive(false);
	}

	private void Start()
	{
		currentPlayerName = players.name;
		PlayerTrans = players.transform;
		
		Quaternion rotateAtStart = Quaternion.Euler(PlayerTrans.rotation.x,
			180 + Camera.main.transform.rotation.eulerAngles.y, PlayerTrans.rotation.z);
		PlayerTrans.rotation = rotateAtStart;
		change = 1;
		canChange = true;
		forwardSpeed = speed;

		StartBtn.gameObject.SetActive(true);
		charControl = players.GetComponent<CharacterController>();
		
		CharAnim = players.GetComponent<Animator>();
	}

	private void Update()
	{
		// downward gravity 
		direc.y = gravity;
		
		destroyObstacles();

		if (!endGame)
		{
			if (!startGame)
			{
				// play animation at start i.e. idle_waving and idle_texting
				if (change == 1)
				{
					StartCoroutine(playAnimation());
				}
				else if (change == 2)
				{
					StartCoroutine(playAnim2());
				}
			}
			else
			{
				ChangeAnimationState();
				var rotateAtStart = Quaternion.Euler(Vector3.zero);
				PlayerTrans.rotation = rotateAtStart;
				direc.z = forwardSpeed;
			}
		}

		// check for the current player of the 3 players
		checkForCurrentPlayer();
	}

	private void destroyObstacles()
	{
		GameObject[] obst = GameObject.FindGameObjectsWithTag("ObstacleParent");
		if (obst != null)
		{
			foreach (GameObject ob in obst)
			{
				if (ob.transform.localPosition.z + 5 < Camera.main.transform.localPosition.z &&
				    Mathf.Abs(ob.transform.localPosition.x - Camera.main.transform.localPosition.x) < 4)
				{
					Destroy(ob);
				}
			}
		}
	}

	private void checkForCurrentPlayer()
	{
		if (startGame)
		{
			if (!endGame)
			{
				rayCastingForNoChange();

				if (currentPlayerName == "man_ninja")
				{
					players.layer = 9;
					currentPlayerName = "man_ninja"; // ninja
					forwardSpeed = speed;
					allowMove = false;
				}

				if (currentPlayerName == "man_superhero")
				{
					players.layer = 11;
					currentPlayerName = "man_superhero"; // flying
					forwardSpeed = speed;

					raycasting();
					rayCastingForPlayerDead();
					players.transform.localPosition = new Vector3(0, height, players.transform.localPosition.z);
					if (allowMove)
					{
						players.transform.localRotation = Quaternion.Euler(rotateAngle, 0, 0);
						UpDownMoveFly();
					}
				}

				if (currentPlayerName == "man_yeti")
				{
					players.layer = 9;
					currentPlayerName = "man_yeti"; // strength
					forwardSpeed = 0.75f * speed;
					allowMove = false;
				}
			}
		}
	}

	private void rayCastingForPlayerDead()
	{
		var playerpos = players.transform.localPosition;
		Vector3 pos = new Vector3(playerpos.x, 8f, playerpos.z);
		Debug.DrawRay(pos, Vector3.forward * 5, Color.red);
		if (Physics.Raycast(pos, Vector3.forward, out RaycastHit hit, 5, layers))
		{
			if (hit.collider.CompareTag("Obstacle") || hit.collider.CompareTag("ObstacleParent"))
			{
				if (once)
				{
					once = false;
					StartCoroutine(playerDead());
				}
			}
		}
	}

	private void rayCastingForNoChange()
	{
		var playerPos = players.transform.localPosition;
		Vector3 pos = new Vector3(playerPos.x, 3, playerPos.z);
		Debug.DrawRay(pos, Vector3.forward * 4, Color.magenta);
		if (Physics.Raycast(pos, Vector3.forward, out RaycastHit hit, 4, layers))
		{
			canChange = false;
		}
		else
		{
			canChange = true;
		}
	}

	private void raycasting()
	{
		Vector3 pos = new Vector3(players.transform.localPosition.x, 4, players.transform.localPosition.z);
		Debug.DrawRay(pos, Vector3.forward * 6, Color.cyan);
		if (Physics.Raycast(pos, Vector3.forward, out RaycastHit hit, 6))
		{
			if (hit.collider.name == "End Path")
			{
				charControl.skinWidth = 0.08f;
			}
		}
	}

	private void FixedUpdate()
	{
		Movement();
	}

	private void Movement()
	{
		if (currentPlayerName != "man_superhero")
		{
			charControl.Move(direc * Time.fixedDeltaTime);
		}
		else
		{
			if (allowMove)
			{
				charControl.Move(direc * Time.fixedDeltaTime);
			}
		}
	}

	private void UpDownMoveFly()
	{
		if (!endGame)
		{
			Vector3 diffPos = new Vector3(0, 1.0f, 0);
			Vector3 position1 = players.transform.localPosition + diffPos;
			Vector3 position2 = players.transform.localPosition - diffPos;

			players.transform.localPosition =
				Vector3.Lerp(position1, position2, (Mathf.Sin(Time.time * 3) + 1) / 2);
		}
	}

	private IEnumerator playAnimation()
	{
		CharAnim.SetBool(Texting, true);
		CharAnim.SetBool(Waving, false);
		while (CharAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
		{
			yield return null;
		}

		change = 2;
	}

	private IEnumerator playAnim2()
	{
		CharAnim.SetBool(Waving, true);
		CharAnim.SetBool(Texting, false);
		while (CharAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
		{
			yield return null;
		}

		change = 1;
	}

	
	private IEnumerator jumpForTransition()
	{
		var playerMat = players.transform.GetChild(0).GetComponentsInChildren<Transform>()[1].gameObject;
		SetMaterialFade(playerMat);
		iTween.FadeTo(playerMat, 0.5f, transitionTime);
		if (currentPlayerName == "man_superhero")
		{
			rotateLanding();
			if (transitionTO == "strength")
			{
				StartCoroutine(increaseScale(players.transform.localScale, 2.5f, transitionTime));
			}
		}
		else if (currentPlayerName == "man_ninja" && transitionTO == "strength")
		{
			StartCoroutine(increaseScale(players.transform.localScale, 2.5f, transitionTime));
		}
		else if (currentPlayerName == "man_yeti")
		{
			StartCoroutine(decreaseScale(players.transform.localScale, 2.5f, transitionTime));
		}

		yield return new WaitForSeconds(transitionTime);
		
		switch (transitionTO)
		{
			case "ninja":
				currentPlayerName = "man_ninja";
				StartCoroutine(decreaseScale(players.transform.localScale, 2, transitionTime));
				
				// change the material from opaque to fade
				playerMatChange = players.transform.GetChild(0).GetComponentsInChildren<Transform>()[1]
					.gameObject;
				SetMaterialFade(playerMatChange);
				Color color = playerMatChange.GetComponent<Renderer>().material.color;
				color.a = 0.5f;
				playerMatChange.GetComponent<Renderer>().material.color = color;

				iTween.FadeTo(playerMatChange, 1, transitionTime);
				yield return new WaitForSeconds(transitionTime);
				SetMaterialOpaque(playerMatChange);
				break;
			case "flying":
				rotateAngle = 70;
				StartCoroutine(rotateFlying());
				/*players[1].transform.localPosition = currentPlayerPos;
				currentPlayer.SetActive(false);
				currentPlayer = players[1];*/
				currentPlayerName = "man_superhero";
				StartCoroutine(decreaseScale(players.transform.localScale, 2, transitionTime));
				// change the material from opaque to fade
				playerMatChange = players.transform.GetChild(0).GetComponentsInChildren<Transform>()[1]
					.gameObject;
				SetMaterialFade(playerMatChange);
				Color color1 = playerMatChange.GetComponent<Renderer>().material.color;
				color1.a = 0.5f;
				playerMatChange.GetComponent<Renderer>().material.color = color1;

				// currentPlayer.SetActive(true);
				iTween.FadeTo(playerMatChange, 1, transitionTime);
				yield return new WaitForSeconds(transitionTime);
				SetMaterialOpaque(playerMatChange);
				// currentPlayer.transform.localPosition = new Vector3(0, currentPlayerPos.y, currentPlayerPos.z);
				break;
			case "strength":
				currentPlayerName = "man_yeti";
				StartCoroutine(increaseScale(players.transform.localScale, 3, transitionTime));
				// change the material from opaque to fade
				playerMatChange = players.transform.GetChild(0).GetComponentsInChildren<Transform>()[1]
					.gameObject;
				SetMaterialFade(playerMatChange);
				Color color2 = playerMatChange.GetComponent<Renderer>().material.color;
				color2.a = 0.5f;
				playerMatChange.GetComponent<Renderer>().material.color = color2;

				iTween.FadeTo(playerMatChange, 1, transitionTime);
				yield return new WaitForSeconds(transitionTime);
				SetMaterialOpaque(playerMatChange);
				break;
		}
	}

	private IEnumerator increaseScale(Vector3 s_start, float s_end, float duration)
	{
		var playerScale = s_start;
		float elapsed = 0.0f;
		while (elapsed < duration)
		{
			playerScale.x = Mathf.Lerp(s_start.x, s_end, elapsed / duration);
			playerScale.y = Mathf.Lerp(s_start.y, s_end, elapsed / duration);
			playerScale.z = Mathf.Lerp(s_start.z, s_end, elapsed / duration);
			players.transform.localScale = playerScale;
			elapsed += Time.deltaTime;
			yield return null;
		}

		playerScale.y = s_end;
	}

	private IEnumerator decreaseScale(Vector3 s_start, float s_end, float duration)
	{
		var playerScale = s_start;
		float elapsed = 0.0f;
		while (elapsed < duration)
		{
			playerScale.x = Mathf.Lerp(s_start.x, s_end, elapsed / duration);
			playerScale.y = Mathf.Lerp(s_start.y, s_end, elapsed / duration);
			playerScale.z = Mathf.Lerp(s_start.z, s_end, elapsed / duration);
			players.transform.localScale = playerScale;
			elapsed += Time.deltaTime;
			yield return null;
		}

		playerScale.y = s_end;
	}

	private void SetMaterialFade(GameObject playerMat)
	{
		Color color = fadeCharMat.color;
		color.a = 1;
		fadeCharMat.color = color;

		playerMat.GetComponent<Renderer>().material = fadeCharMat;
	}

	private void SetMaterialOpaque(GameObject playerMat)
	{
		playerMat.GetComponent<Renderer>().material = opaqueSuperMat;
	}

	private void ChangeAnimationState()
	{
		if (currentPlayerName == "man_ninja")
		{
			CharAnim.SetBool(Texting, false);
			CharAnim.SetBool(Waving, false);
			CharAnim.SetBool(Running, true);
			CharAnim.SetBool(Generic, false);
			CharAnim.SetBool(Walking, false);
		}

		if (currentPlayerName == "man_superhero")
		{
			CharAnim.SetBool(Texting, false);
			CharAnim.SetBool(Waving, false);
			CharAnim.SetBool(Generic, true);
			CharAnim.SetBool(Running, false);
			CharAnim.SetBool(Walking, false);
		}

		if (currentPlayerName == "man_yeti")
		{
			CharAnim.SetBool(Texting, false);
			CharAnim.SetBool(Waving, false);
			CharAnim.SetBool(Running, false);
			CharAnim.SetBool(Generic, false);
			CharAnim.SetBool(Walking, true);
		}
	}

	public void startBtn()
	{
		startGame = true;
		StartBtn.gameObject.SetActive(false);

		StartCoroutine(rotateFlying());

		// for bots
		BotManager.InstanceBM.StartBot();
		BotManager2.InstanceBM2.StartBot();
		BotManager3.InstanceBM3.StartBot();
	}


	private IEnumerator rotateFlying()
	{
		StartCoroutine(changeHeight(players.transform.localPosition.y, 8f, transitionTime));
		iTween.RotateTo(players,
			iTween.Hash("x", rotateAngle, "time", transitionTime, "islocal", true, "easetype", "linear"));
		yield return new WaitForSeconds(transitionTime);
		allowMove = true;
	}

	private void rotateLanding()
	{
		StartCoroutine(changeHeight(height, 0.5f, transitionTime));
		StartCoroutine(changeAngleRotation(players.transform.localRotation.x, 0, transitionTime));
	}

	private IEnumerator changeAngleRotation(float a_start, float a_end, float duration)
	{
		var angleRotation = players.transform.localRotation;
		var elapsed = 0.0f;
		while (elapsed < duration)
		{
			angleRotation.x = Mathf.Lerp(a_start, a_end, elapsed / duration);
			players.transform.localRotation = angleRotation;
			elapsed += Time.deltaTime;
			yield return null;
		}

		rotateAngle = 0;
	}
	
	public void EndGame()
	{
		Debug.Log("game ended");
		startGame = false;
		won = true;
		CharAnim.SetBool(Running, false);
		CharAnim.SetBool(Generic, false);
		CharAnim.SetBool(Walking, false);
		if (playerlost)
		{
			CharAnim.SetBool(hardDay, true);
			restartBtn.SetActive(true);
		}

		PlayerTrans = players.transform;
		Quaternion rotateAtStart = Quaternion.Euler(PlayerTrans.rotation.x,
			180 + Camera.main.transform.rotation.eulerAngles.y, PlayerTrans.rotation.z);
		PlayerTrans.rotation = rotateAtStart;

		endGame = true;
		canChange = false;
		direc = Vector3.zero;

		charControl.skinWidth = 0.08f;
	}

	public IEnumerator breakTheObstacle(GameObject obstacleParent)
	{
		CharAnim.SetBool(Running, false);
		CharAnim.SetBool(Walking, false);
		var RandNo = Random.Range(0, 3);
		switch (RandNo)
		{
			case 0:
				CharAnim.SetBool(punch_left, true);
				yield return new WaitForSeconds(0.7f);
				break;
			case 1:
				CharAnim.SetBool(punch_right, true);
				yield return new WaitForSeconds(0.35f);
				break;
			case 2:
				CharAnim.SetBool(kick, true);
				yield return new WaitForSeconds(1.05f);
				break;
		}

		var rbObstacles = obstacleParent.GetComponentsInChildren<Rigidbody>();
		foreach (Rigidbody rb in rbObstacles)
		{
			rb.AddForce(new Vector3(Random.Range(-15, 15), Random.Range(5, 10), Random.Range(15, 20)),
				ForceMode.Impulse);
			rb.AddTorque(new Vector3(Random.Range(-5, 5), Random.Range(5, 10), 0), ForceMode.Impulse);
			rb.gameObject.tag = "Untagged";

			rb.GetComponent<Collider>().isTrigger = true;
			Destroy(rb.gameObject, 6);
		}

		Destroy(obstacleParent, 6);

		CharAnim.SetBool(punch_left, false);
		CharAnim.SetBool(punch_right, false);
		CharAnim.SetBool(kick, false);
		CharAnim.SetBool(Running, false);
		CharAnim.SetBool(Walking, true);
		PlayerCollision.once = true;
	}

	public IEnumerator playerDead()
	{
		CharAnim.SetBool(Dead, true);
		CharAnim.SetBool(Waving, false);
		CharAnim.SetBool(Texting, false);
		CharAnim.SetBool(Walking, false);
		CharAnim.SetBool(Running, false);
		CharAnim.SetBool(Generic, false);

		direc = Vector3.zero;
		
		if (currentPlayerName == "man_superhero")
		{
			StartCoroutine(changeHeight(height, 0.5f, 1f));
		}
		
		rotateAngle = 0;
		allowMove = false;
		if (currentPlayerName == "man_superhero")
		{
			charControl.skinWidth = 0.08f;
		}

		canChange = false;

		yield return new WaitForSeconds(1);
		endGame = true;

		yield return new WaitForSeconds(1.5f);
		restartGame();
	}

	private IEnumerator changeHeight(float h_start, float h_end, float duration)
	{
		float elapsed = 0.0f;
		while (elapsed < duration)
		{
			height = Mathf.Lerp(h_start, h_end, elapsed / duration);
			elapsed += Time.deltaTime;
			yield return null;
		}

		height = h_end;
	}

	private void restartGame()
	{
		PlayerCollision.onlyOnce = true;
		startGame = true;
		endGame = false;
		rotateAngle = 70;
		allowMove = true;
		if (currentPlayerName == "man_superhero")
		{
			StartCoroutine(changeHeight(players.transform.localPosition.y, 8, 0.5f));
		}

		canChange = true;
		rayCastObstacle();
		CharAnim.SetBool(Dead, false);
	}

	private void rayCastObstacle()
	{
		RaycastHit hitObst;
		Vector3 pos = new Vector3(players.transform.localPosition.x, 9.5f,
			players.transform.localPosition.z);
		Vector3 pos2 = new Vector3(players.transform.localPosition.x, 3f,
			players.transform.localPosition.z);

		if (Physics.Raycast(pos, Vector3.forward, out hitObst, 10, layers))
		{
			Destroy(hitObst.collider.gameObject);
		}

		if (Physics.Raycast(pos2, Vector3.forward, out hitObst, 10, layers))
		{
			var parent = hitObst.collider.transform.parent.gameObject;
			Destroy(parent);
		}

		once = true;
	}

	public void superHeroBtn()
	{
		if (canChange)
		{
			if (currentPlayerName != "man_superhero")
			{
				transitionTO = "flying"; // player[1]
				StartCoroutine(jumpForTransition());
				//change to flying
			}
		}
	}

	public void ninjaBtn()
	{
		if (canChange)
		{
			if (currentPlayerName != "man_ninja")
			{
				transitionTO = "ninja"; // player[0]
				StartCoroutine(jumpForTransition());
				// change to ninja
			}
		}
	}

	public void yetiBtn()
	{
		if (canChange)
		{
			if (currentPlayerName != "man_yeti")
			{
				transitionTO = "strength"; // player[2]
				StartCoroutine(jumpForTransition());
				//change to strength
			}
		}
	}
}
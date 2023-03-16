using UnityEngine;

public class GameManager : MonoBehaviour
{
	#region Variables

	private bool once = true;

	[SerializeField] private GameObject playerWonTxt;
	[SerializeField] private GameObject playerLostTxt;

	#endregion
	
	#region InstanceGM

	private static GameManager instance;
	public static GameManager InstanceGM
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<GameManager>();
			}

			return instance;
		}
	}

	#endregion
	
	private void Update()
	{
		if (once)
		{
			if (GameHandler.InstanceGH.won)
			{
				wonGame();
				once = false;
			}
			else if (BotManager.InstanceBM.botWon)
			{
				lostGame1();
				once = false;
			}
			else if (BotManager2.InstanceBM2.bot2Won)
			{
				lostGame2();
				once = false;
			}
			else if (BotManager3.InstanceBM3.bot3Won)
			{
				lostGame3();
				once = false;
			}
		}
	}

	private void wonGame()
	{
		playerWonTxt.SetActive(true);
		GameHandler.InstanceGH.playerlost = false;
		BotManager.InstanceBM.botLost = true;
		BotManager2.InstanceBM2.bot2Lost = true;
		BotManager3.InstanceBM3.bot3Lost = true;
	}

	private void lostGame1()
	{
		GameHandler.InstanceGH.playerlost = true;
		BotManager.InstanceBM.botLost = false;
		BotManager2.InstanceBM2.bot2Lost = true;
		BotManager3.InstanceBM3.bot3Lost = true;
		playerLostTxt.SetActive(true);
	}

	private void lostGame2()
	{
		GameHandler.InstanceGH.playerlost = true;
		BotManager.InstanceBM.botLost = true;
		BotManager2.InstanceBM2.bot2Lost = false;
		BotManager3.InstanceBM3.bot3Lost = true;
		playerLostTxt.SetActive(true);
	}
	
	private void lostGame3()
	{
		GameHandler.InstanceGH.playerlost = true;
		BotManager.InstanceBM.botLost = true;
		BotManager2.InstanceBM2.bot2Lost = true;
		BotManager3.InstanceBM3.bot3Lost = false;
		playerLostTxt.SetActive(true);
	}
}
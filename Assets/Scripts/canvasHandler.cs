using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class canvasHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] btns;

    private void Update()
    {
        if (!GameHandler.InstanceGH.startGame)
        {
            foreach (GameObject btn in btns)
            {
                btn.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject btn in btns)
            {
                btn.SetActive(true);
            }
        }
    }

    public void RestartBtn()
    {
        SceneManager.LoadScene("Level1");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameButtons : MonoBehaviour
{
    public void ButtonRestart()
    {
        GameManager.Instance.RestartGame();
    }
    public void ButtonExitToMenu()
    {
        GameManager.Instance.ExitToMenu();
    }
    public void ButtonSwitchSides()
    {
        PlayerManager.Instance.SwitchMark();
        GameManager.Instance.RestartGame();
    }
}

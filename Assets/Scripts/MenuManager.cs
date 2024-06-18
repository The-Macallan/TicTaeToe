using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Menu[] menus;
    [SerializeField] private ToggleGroup FieldSizeOptions;
    [SerializeField] private ToggleGroup TurnOptions;
    
    public void ButtonOnePlayer()
    {
        GameManager.Instance.GameMode = GameMode.PVE;
        OpenMenu("SettingMenu");
    }
    public void ButtonTwoPlayers()
    {
        GameManager.Instance.GameMode = GameMode.PVP;
        OpenMenu("SettingMenu");    
    }
    public void ButtonStart()
    {
        var fieldSizeOption  = FieldSizeOptions.ActiveToggles().FirstOrDefault();
        var turnOption       = TurnOptions.ActiveToggles().FirstOrDefault();
          
        GameManager.Instance.FieldSize                  = (fieldSizeOption.name == "FieldSize3x3") ? FieldSize._3x3 : FieldSize._5x5;
        GameManager.Instance.PlayerNumberWhoGoesFirst   = (turnOption.name == "Player1") ? 1 : 2;

        GameManager.Instance.StartGame();
    }
    public void ButtonExitToMainMenu()
    {
        OpenMenu("MainMenu");
    }
    public void ButtonExitGame()
    {
        Application.Quit();
    }

    private void OpenMenu(string menuName)
    {
        foreach (var menu in menus)
        {
            if (menu.Name == menuName)
                menu.Open();
            else
                menu.Close();
        }

        SetUpCustomVisibility();
    }
    private void SetUpCustomVisibility()
    {
        var label = GameObject.Find("SettingMenu/TurnOptions/Player2/Label");
        if (label != null)
            label.GetComponent<Text>().text = (GameManager.Instance.GameMode == GameMode.PVE) ? "computer" : "player2";
    }
}

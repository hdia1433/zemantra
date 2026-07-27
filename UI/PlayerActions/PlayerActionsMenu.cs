using Godot;
using System.Collections.Generic;

public partial class PlayerActionsMenu : Control
{
    private BattlePlayer player;
    private VBoxContainer buttonParent;
    private Dictionary<BattlePlayerAction, Button> actionMap;

    public BattlePlayer Player
    {set => player = value;}

    public override void _Ready()
    {
        buttonParent = GetNode<VBoxContainer>("%Buttons");

        actionMap = new();

        foreach(BattlePlayerAction action in player.Actions)
        {
            Button actionButton = new();

            switch(action)
            {
                case BattlePlayerAction.Move:
                    actionButton.Text = "Move";
                    actionButton.ButtonUp += player.StartMoving;
                    break;
                case BattlePlayerAction.EndTurn:
                    actionButton.Text = "End Turn";
                    actionButton.ButtonUp += player.EndCurrentTurn;
                    break;
            }

            actionButton.Theme = GD.Load<Theme>("res://UI/Themes/PlayerActionsMenu.tres");

            if(actionButton is null)
            {
                GD.Print("What?!");
            }

            buttonParent.AddChild(actionButton);

            if(actionButton is null)
            {
                GD.Print("What?! 2");
            }

            actionMap.Add(action, actionButton);
        }
    }

    ///<summary>
    ///Disables the button for the associated action in the action menu for the player.
    ///</summary>
    ///<param name="action">The action to disable</param>
    public void DisableAction(BattlePlayerAction action)
    {
        actionMap[action].Disabled = true;
    }

    public void EnableAllActions()
    {
        foreach(Button button in actionMap.Values)
        {
            if(button.Disabled)
            {
                button.Disabled = false;
            }
        }
    }

    public void NewTurn()
    {
        foreach (Button button in actionMap.Values)
        {
            button.Disabled = false;
        }
    }
}

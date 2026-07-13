using Godot;

using System.Collections.Generic;

public enum PartyMember
{
    Test = 0
}

public partial class PartyManager : Node
{
    public static PartyManager Inst {get; private set;}

    public List<PartyMember> Members {get; private set;}
    public PlayerManager[] Managers {get; private set;}

    public override void _Ready()
    {
        Inst = this;
        Members = new();

        Members.Add(PartyMember.Test);

        Managers = new PlayerManager[1];
        for(int i = 0; i < Managers.Length; i++)
        {
            Managers[i] = (PartyMember)i switch
            {
                PartyMember.Test => new(100, 5, 2),
                _ => new(0, 0, 0)
            };
        }
    }
}

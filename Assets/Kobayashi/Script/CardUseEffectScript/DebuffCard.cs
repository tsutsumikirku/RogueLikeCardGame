using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectTargetMouseClick : IChoiseTarget
{
    public bool _effect { get => true; }
    public bool ChoiseTarget(CharacterBase usecharacter,out CharacterBase target)
    {
        Debug.Log("“G‚ð‘I‚×");
        target = BattleManager.Instance.GetMousePositionObject<CharacterBase>();
        return Input.GetMouseButton(0) && target != null;
    }
}
public class UseCharacterTarget : IChoiseTarget
{
    public bool _effect { get => false; }
    public bool ChoiseTarget(CharacterBase useCharacter, out CharacterBase target)
    {
        target = useCharacter;
        return true;
    }
}
public class PlayerTarGet : IChoiseTarget
{
    public bool _effect { get => false; }
    public bool ChoiseTarget(CharacterBase useCharacter, out CharacterBase target)
    {
        target = BattleManager.Instance._player;
        return true;
    }
}

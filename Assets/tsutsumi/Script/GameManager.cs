using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance;
    GameManagerState _state;
    GameManagerMove _move;
    public GameManagerState CurrentState
    {
        get { return _state; }
        set
        {
            if (_state != value)
            {
                _state = value;
                OnStateChanged();
            }
        }
    }
    private void OnStateChanged()
    {

    }

    GameManagerMove RandomSet()
    {
        
        return;
    }
}
public enum GameManagerState
{
    Surch,
    Move,
    Shop,
    Deck,
    UserManual
}
public enum GameManagerMove
{
    Battle,
    TresureBox
}

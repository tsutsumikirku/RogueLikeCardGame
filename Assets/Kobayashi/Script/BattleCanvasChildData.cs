using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCanvasChildData : MonoBehaviour
{
    private BattleCanvasChildData instans;
    public Transform _deck;
    public Transform _handCard;
    public Transform _waitingCardListParent;
    public Transform _trashParent;
    public BattleCardManager _cardManager;
    public ButtleTimeLineManager _timeLineManager;
    public Text _battleText;
    [Tooltip("���̓��X�g�̍ő�l���G�l�~�[�̍ő�l�ł�")]
    public List<RectTransform> _enemysAnchor;
    public RectTransform _buttonAnchar;
}

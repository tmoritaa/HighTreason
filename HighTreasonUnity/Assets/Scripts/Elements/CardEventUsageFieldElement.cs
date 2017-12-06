using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CardEventUsageFieldElement : CardUsageFieldElement 
{
    private GameState.GameStateType usableState;

    private int eventIdx;

    private Card card;

    private Func<Game, bool> isClickable;

    public void Init(Card _card, GameState.GameStateType _usableState, int _eventIdx, CardInfo.EffectInfo ei, Func<Game, bool> _isClickable)
    {
        card = _card;
        usableState = _usableState;
        eventIdx = _eventIdx;

        this.GetComponentInChildren<Text>().text = ei.Text;

        Color color = Color.black;
        switch (ei.Type)
        {
            case CardInfo.EffectInfo.EffectType.Prosecution:
                color = ViewManager.Instance.ProsecutionColor;
                break;
            case CardInfo.EffectInfo.EffectType.Defense:
                color = ViewManager.Instance.DefenseColor;
                break;
            case CardInfo.EffectInfo.EffectType.Neutral:
                color = ViewManager.Instance.NeutralColor;
                break;
            case CardInfo.EffectInfo.EffectType.JurySelect:
                color = ViewManager.Instance.JurySelectColor;
                break;
        }

        this.GetComponent<Image>().color = color;

        isClickable = _isClickable;
        this.GetComponent<Graphic>().raycastTarget = isClickable(GameManager.Instance.Game);
    }

    protected override void onValidClick()
    {
        Debug.Log("Event Choice complete");
        ChoiceHandlerDelegator.Instance.ChoiceMade(Player.PlayerActionParams.UsageType.Event, card, eventIdx);
    }

    protected override bool canUse()
    {
        return
            isClickable(GameManager.Instance.Game)
            && ChoiceHandlerDelegator.Instance.CurChoiceType == UnityChoiceHandler.ChoiceType.CardAndUsage
            && GameManager.Instance.Game.CurState.StateType == usableState
            && card.CanBePlayed;
    }
}

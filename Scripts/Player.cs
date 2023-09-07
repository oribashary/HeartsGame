using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] protected PlayingCard _originalCard;
    [SerializeField] protected SceneController controller;
    [SerializeField] protected TextMesh scoreText;

    #endregion

    #region Public Properties

    public bool has2C = false;
    public bool hasQoS = false;

    #endregion

    #region Private Fields

    protected List<PlayingCard> cards = new List<PlayingCard>();
    protected List<PlayingCard> clubs;
    protected List<PlayingCard> diamonds;
    protected List<PlayingCard> spades;
    protected List<PlayingCard> hearts;
    protected List<PlayingCard> displayOrder;
    protected string defaultScoreText;
    protected int score;

    private int _selectedCards = 0;
    private int _cardNum = 0;

    #endregion
    public Vector3 startPos;

    #region Public Properties

    public int cardNum
    {
        get { return _cardNum; }
        set { _cardNum = value; }
    }

    public int selectedCards
    {
        get { return _selectedCards; }
        set { _selectedCards = value; }
    }

    #endregion

    #region Constants

    public const float OffsetX = 1f;
    public const float OffsetY = 2.5f;
    private const float DefaultFallTime = 5f;
    private const float FallSpeed = 2.0f;
    private const float InitialPercentage = 0.1f;
    private const float FallHeight = -5f;
    private const float TargetPositionZ = -1;

    #endregion

    #region Public Properties

    public PlayingCard originalCard
    {
        get { return _originalCard; }
    }

    #endregion

    #region Unity Lifecycle

    void Start()
    {
        defaultScoreText = scoreText.text;
    }

    #endregion

    #region Public Methods

    public virtual void GameStart()
    {
        startPos = originalCard.transform.position;
        DisplayCards();
        Destroy(_originalCard.gameObject);
    }

    public virtual void AddToHand(PlayingCard c)
    {
        cards.Add(c);
        c.SetUser(this, false);

        if (c.displayVal == 1)
            has2C = true;

        if (c.displayVal == 110000)
            hasQoS = true;

        _cardNum++;
    }

    public void RemoveFromHand(PlayingCard c)
    {
        cards.Remove(c);
    }

    public virtual void AddToHand(List<PlayingCard> cardsToAdd)
    {
        foreach (PlayingCard c in cardsToAdd)
        {
            cards.Add(c);
            c.SetUser(this, false);

            if (c.displayVal == 1)
                has2C = true;

            if (c.displayVal == 110000)
                hasQoS = true;

            _cardNum++;
        }
    }

    public void DisplayCards()
    {
        displayOrder = (from card in cards orderby card.displayVal select card).ToList();
        float offset = (13 - displayOrder.Count()) * (OffsetX / 2);
        int i = 0;

        foreach (PlayingCard c in displayOrder)
        {
            float posX = offset + (OffsetX * i) + startPos.x;
            float posY = startPos.y;
            c.SetDefaultPos(new Vector3(posX, posY, startPos.z - 1));
            i++;
        }
        selectedCards = 0;
    }

    public void PassCards()
    {
        List<PlayingCard> cardsToPass = new List<PlayingCard>();
        foreach (PlayingCard c in cards)
        {
            if (!c.selected)
                continue;

            cardsToPass.Add(c);

            if (c.displayVal == 1)
                has2C = false;

            if (c.displayVal == 110000)
                hasQoS = false;

            _cardNum--;
        }
        controller.SubmitCardsToPass(this, cardsToPass);
    }

    public void SelectCard()
	{
		foreach (PlayingCard c in cards) 
				c.Playable ();

		if (controller.gameRound == 0) 
        {
			foreach (PlayingCard c in cards) 
            {
				if (c.displayVal == 110000)
					c.Unplayable ();
				else if (c.suit.Equals ("h")) 
					c.Unplayable ();
			}
		}
		if (!controller.suit.Equals ("n")) 
        {
			Debug.Log ("not selecting");
			if ((from card in cards where card.suit == controller.suit select card).Count () > 0) {
				foreach (PlayingCard c in cards) 
                {
					if (!c.suit.Equals (controller.suit)) 
						c.Unplayable ();
				}
			}
		}
		else if (has2C) {
			foreach (PlayingCard c in cards) 
            {
				if (!(c.displayVal == 1)) 
					c.Unplayable ();
			}
		} else if (!controller.heartsBroken) 
        {
			Debug.Log ("hearts not broken");
			foreach (PlayingCard c in cards) 
            {
				if (c.suit.Equals ("h")) 
					c.Unplayable ();
			}
		}
			
	}

	public virtual void PlayCard(ref List<PlayingCard> cardsInRound)
	{
		foreach (PlayingCard c in cards) 
        {
			if (c.selected) 
            {
				c.MoveToCenter ();
				cardsInRound.Add (c);
                
				if (c.displayVal == 1) 
					has2C = false;

				if (!controller.heartsBroken && c.suit.Equals ("h")) 
					controller.heartsBroken = true;
			} 
            else 
            {
				c.Unplayable ();
			}
		}
	}

    public void AddToScore(int val)
    {
        score += val;
        scoreText.text = score.ToString();
    }

    #endregion
}
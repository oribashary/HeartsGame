using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerController : Player 
{
    #region Fields and Initialization
    public PlayingCard toPlay;

    void Start () 
    {
        startPos = originalCard.transform.position;
        defaultScoreText = scoreText.text;
    }
    #endregion

    #region Game Initialization
    public override void GameStart()
    {
        startPos = originalCard.transform.position;
        PassCards();
        Destroy (_originalCard.gameObject);
    }
    #endregion

    #region Card Presentation
    public void PresentCard()
    {
        foreach (PlayingCard c in displayOrder) 
            if (c.selected) 
                c.MoveToCenter ();
    }
    #endregion

    #region Hand Management
    public override void AddToHand(PlayingCard c)
    {
        cards.Add (c);
        c.SetUser (this, true);

        if (c.displayVal == 1) 
            has2C = true;

        if (c.displayVal == 110000) 
            hasQoS = true;
    }

    public override void AddToHand(List<PlayingCard> cardsToAdd)
    {
        string s = "";
        foreach (PlayingCard c in cardsToAdd)
        {
            cards.Add(c);
            c.SetUser (this, true);
            s += c.displayVal + ",";

            if (c.displayVal == 1) 
                has2C = true;

            if (c.displayVal == 110000) 
                hasQoS = true;
        }
        Debug.Log (s);
    }
    #endregion

    #region Card Passing
    private void PassCards()
    {
        List<PlayingCard> cardsToPass = new List<PlayingCard>();
        List<List<PlayingCard>> suitLists = new List<List<PlayingCard>>
        {
            cards.Where(card => card.suit == "c").OrderByDescending(card => card.val).ToList(),
            cards.Where(card => card.suit == "d").OrderByDescending(card => card.val).ToList(),
            cards.Where(card => card.suit == "s").OrderByDescending(card => card.val).ToList(),
            cards.Where(card => card.suit == "h").OrderByDescending(card => card.val).ToList()
        };

        while (cardsToPass.Count() < 3)
        {
            int highestIndex = -1;
            PlayingCard highestCard = null;

            for (int i = 0; i < suitLists.Count; i++)
            {
                List<PlayingCard> suitList = suitLists[i];
                if (suitList.Count > 0)
                {
                    PlayingCard topCard = suitList[0];
                    if (highestCard == null || topCard.val > highestCard.val)
                    {
                        highestCard = topCard;
                        highestIndex = i;
                    }
                }
            }

            if (highestIndex == -1)
            {
                break;
            }

            List<PlayingCard> selectedSuitList = suitLists[highestIndex];
            cardsToPass.Add(removeHighest(ref selectedSuitList));
        }

        foreach (PlayingCard goal in cardsToPass)
        {
            int i = 0;
            foreach (PlayingCard c in cards)
            {
                if (goal.displayVal == c.displayVal)
                {
                    c.selected = true;
                    break;
                }
                i++;
            }
        }
        controller.SubmitCardsToPass(this, cardsToPass);
    }

    private PlayingCard removeHighest(ref List<PlayingCard> suit)
    {
        PlayingCard highest = suit[0];

        if (highest.displayVal == 1)
            has2C = false;

        if (highest.displayVal == 110000)
            hasQoS = false;

        suit.RemoveAt(0);
        return highest;
    }
    #endregion

    #region Card Playing
    public void PlayCard()
    {
    }

    public void StartRound(ref List<PlayingCard> cardsInRound)
    {
        if (has2C)
        {
            foreach (PlayingCard c in cards)
            {
                if (c.displayVal == 1)
                {
                    HandleCardMoveToCenter(c, cardsInRound);
                    return;
                }
            }
        }

        clubs = GetSortedSuitList("c");
        diamonds = GetSortedSuitList("d");
        spades = GetSortedSuitList("s");
        hearts = GetSortedSuitList("h");

        List<List<PlayingCard>> suits = new List<List<PlayingCard>> { clubs, diamonds, spades, hearts };
        suits = suits.Where(suit => suit.Count() > 0).OrderBy(suit => suit.Count()).ToList();

        int count = 0;
        List<PlayingCard> fewestSuit = suits[0];

        if (fewestSuit == spades && fewestSuit[0].displayVal >= 11 && suits.Count() > count + 1)
        {
            count++;
            fewestSuit = suits[count];
        }

        if (fewestSuit == hearts && !controller.heartsBroken && suits.Count() > count + 1)
        {
            count++;
            fewestSuit = suits[count];
        }

        PlayingCard prev = fewestSuit[0];

        foreach (PlayingCard c in fewestSuit)
        {
            if (c.val == prev.val)
                continue;

            if (c.val > prev.val + 1)
                break;

            prev = c;
        }

        HandleCardMoveToCenter(prev, cardsInRound);
    }

    private List<PlayingCard> GetSortedSuitList(string suit)
    {
        return cards
            .Where(card => card.suit == suit)
            .OrderByDescending(card => card.val)
            .ToList();
    }

    private void HandleCardMoveToCenter(PlayingCard card, List<PlayingCard> cardsInRound)
    {
        card.MoveToCenter();
        cardsInRound.Add(card);
        controller.suit = card.suit;
        card.selected = true;
    }

    public override void PlayCard(ref List<PlayingCard> cardsInRound)
    {
        bool firstRound = controller.gameRound == 0;
        toPlay = null;

        List<PlayingCard> suit = GetSortedSuitList(controller.suit);
        List<PlayingCard> cardsOfSuit = GetSortedCardsOfSuit(cardsInRound, controller.suit);
        PlayingCard highestInRound = cardsOfSuit.FirstOrDefault();

        if (suit.Count > 0)
        {
            toPlay = ChooseCardInSuit(suit, highestInRound) ?? suit.FirstOrDefault();

            if (ShouldSortSuitDescending(cardsInRound))
            {
                suit = suit.OrderByDescending(c => c.val).ToList();
                toPlay = suit.FirstOrDefault();
            }
        }
        else
        {
            if (hasQoS && !firstRound)
                toPlay = GetTenOfSpades();
            else
            {
                if (cards.Any(c => c.val > 8))
                {
                    if (controller.queenSpadesPlayed)
                        HandleSpadesSelection();
                    else
                        SuitSelect(ref toPlay);
                }
            }
        }

        HandleSpecialCases(cardsInRound);
    }

    private PlayingCard ChooseCardInSuit(List<PlayingCard> suit, PlayingCard highestInRound)
    {
        return suit.FirstOrDefault(c => c.val > highestInRound.val);
    }

    private bool ShouldSortSuitDescending(List<PlayingCard> cardsInRound)
    {
        return cardsInRound.Count == 3 && !cardsInRound.Any(c => c.suit == "h" || c.displayVal == 110000);
    }

    private PlayingCard GetTenOfSpades()
    {
        return spades.FirstOrDefault(c => c.val == 10);
    }

    private void HandleSpadesSelection()
    {
        hearts = GetSortedSuitList("h");
        if (hearts.Count > 0)
        {
            if (controller.heartsBroken && cards.Count < 5)
                toPlay = hearts.FirstOrDefault();
            else
                SuitSelect(ref toPlay);
        }
        else
        {
            spades = GetSortedSuitList("s");
            if (spades.FirstOrDefault()?.val > 10 && !(controller.gameRound == 0 && spades.FirstOrDefault()?.val == 11))
                toPlay = spades.FirstOrDefault();
            else
                SuitSelect(ref toPlay);
        }
    }

    private void HandleSpecialCases(List<PlayingCard> cardsInRound)
    {
        if (toPlay == null)
            SuitSelect(ref toPlay);

        if (toPlay.suit == "h" && !controller.heartsBroken)
            controller.heartsBroken = true;

        if (toPlay.displayVal == 110000)
            hasQoS = false;

        toPlay.selected = true;
        toPlay.MoveToCenter();
        cardsInRound.Add(toPlay);
    }

    private List<PlayingCard> GetSortedCardsOfSuit(List<PlayingCard> cardsInRound, string suit)
    {
        return cardsInRound.Where(card => card.suit.Equals(suit)).OrderByDescending(card => card.val).ToList();
    }

    private void SuitSelect(ref PlayingCard toPlay)
    {
        clubs = GetSortedSuitList("c");
        diamonds = GetSortedSuitList("d");
        spades = GetSortedSuitList("s");
        hearts = GetSortedSuitList("h");

        List<List<PlayingCard>> suits = new List<List<PlayingCard>> { clubs, diamonds, spades, hearts };
        suits = suits.Where(suit => suit.Count() > 0).OrderBy(suit => suit.Count()).ToList();

        if (hearts.Count() > 0 && controller.gameRound != 0)
        {
            if (!(hearts[0].val < 8))
            {
                toPlay = hearts[0];
                return;
            }
        }

        int count = 1;
        List<PlayingCard> fewest = suits[0];
        while ((fewest == hearts && controller.gameRound == 0 && suits.Count > count) || (fewest == spades && hasQoS && suits.Count > count))
        {
            fewest = suits[count];
            count++;
        }

        toPlay = fewest[0];

        if (toPlay.displayVal == 110000 && controller.gameRound == 0)
        {
            if (fewest.Count > 1)
            {
                toPlay = fewest[2];
            }
            else if (suits.Count > count)
            {
                fewest = suits[count];
                toPlay = fewest[0];
            }
        }

        toPlay.selected = true;
    }
    #endregion
}
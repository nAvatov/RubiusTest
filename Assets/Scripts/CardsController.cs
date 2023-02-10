using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsController : MonoBehaviour
{
    private const string urlPattern = "http://picsum.photos/200";
    [SerializeField] private Button loadButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMPro.TMP_Dropdown dropdownList;
    [SerializeField] private List<Card> cards;
    private bool cardsAreHidden = true;
    private int amountOfUsedCards = 0;
    public bool CardsAreHidden {
        get {
            return cardsAreHidden;
        }
        set {
            cardsAreHidden = value;

            HandleControlButtons(true, false);
            amountOfUsedCards = 0;
        }
    }

    #region Public Methods

    public void Load() {
        StopAllCoroutines();
        StartCoroutine(ShowCardsInSpecificType());
    }

    public void Cancel() {
        StopAllCoroutines();
        StartCoroutine(HideCards());
    }
        
    #endregion

    #region Private Methods

    /// <summary>
    /// Coroutine for hiding cards and executing specific view type based on dropdownList current variant.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowCardsInSpecificType() {
        StartCoroutine(HideCards());

        yield return new WaitUntil(() => CardsAreHidden == true);

        HandleControlButtons(false, true);

        switch(dropdownList.captionText.text.ToLower()) {
            case "onebyone" : { 
                StartCoroutine(OneByOne.ShowCards(cards, urlPattern, () => { CardsAreHidden = false; }));
                break;
            }
            case "allatonce" : { 
                StartCoroutine(AllAtOnce.ShowCards(cards, urlPattern, () => { CardsAreHidden = false; }));
                break;
            }
            case "eachasready" : {
                StartCoroutine(EachAsReady.ShowCards(cards, urlPattern, () => {
                    if (++amountOfUsedCards == cards.Count) {
                        CardsAreHidden = false;
                    }
                }));
                break;
            }
        }
    }


    /// <summary>
    /// Coroutine for hiding cards based on CardsAreHidden flag and *calculatedAmount value (*How much cards are shown at the time of this function call).
    /// </summary>
    private IEnumerator HideCards() {
        if (!CardsAreHidden) { // Condition becomes true when all cards are shown
            HandleControlButtons(false, false);
            
            foreach(Card card in cards) {
                card.Flip(() => {
                    amountOfUsedCards++;
                });
            }

            yield return new WaitUntil(() => amountOfUsedCards == cards.Count);
            CardsAreHidden = true;
        }
        else { // When (part of || none of) cards are shown
            int  calculatedAmount = GetShownCardsAmount();
            
            if (calculatedAmount != 0) {
                int actualAmount = 0;
                foreach(Card card in cards) {
                    
                    if (card.IsShown) {
                        card.Flip(() => {
                            actualAmount++;
                        });
                    }
                }

                yield return new WaitUntil(() => actualAmount == calculatedAmount);
                CardsAreHidden = true;
            } else {
                HandleControlButtons(true, false);
            }
        }
    }

    /// <summary>
    /// This method calculates and retunrns amount of shown cards whose states are repaired.
    /// </summary>
    /// <returns></returns>
    private int GetShownCardsAmount() {
        int a = 0;

        foreach (Card card in cards) {
            card.RepairState();
            
            if (card.IsShown) {
                a++;
            }
        }

        return a;
    }

    private void HandleControlButtons(bool loadButtonState, bool cancelButtonState) {
        loadButton.interactable = loadButtonState;
        cancelButton.interactable = cancelButtonState;
    }

    #endregion
}

public interface ICardsDisplayer {
    public static IEnumerator ShowCards(List<Card> cards, string url, System.Action callback) {
        yield break;
    }
}



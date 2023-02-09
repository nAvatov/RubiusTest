using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsController : MonoBehaviour
{
    const string urlPattern = "http://picsum.photos/200";
    const float tweenFlipDelay = 0.5f;
    [SerializeField] Button loadButton;
    [SerializeField] Button cancelButton;
    [SerializeField] TMPro.TMP_Dropdown dropdownList;
    [SerializeField] List<Card> cards;

    bool cardsAreHidden = true;
    int amountOfUsedCards = 0;

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
        StartCoroutine(ShowCardsInSpecificType());
    }

    public void Cancel() {
        StopAllCoroutines();

        StartCoroutine(HideCards());
    }
        
    #endregion

    #region Private Methods

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

    private IEnumerator HideCards(bool beggining = false) {
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
                        Debug.Log("isShown");
                        card.Flip(() => {
                            actualAmount++;
                        });
                    }
                }

                yield return new WaitUntil(() => actualAmount == calculatedAmount);
                CardsAreHidden = true;
            }
        }
    }

    private int GetShownCardsAmount() {
        int a = 0;

        foreach (Card card in cards) {
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

    public static void Interrupt() {

    }
}



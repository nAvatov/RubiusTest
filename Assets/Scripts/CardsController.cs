using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface ICardsDisplayer {
    public static IEnumerator ShowCards(List<Card> cards, string url, System.Action callback) {
        yield break;
    }
}

public class CardsController : MonoBehaviour {
    private const string URL = "http://picsum.photos/200";
    [SerializeField] private Button _loadButton;
    [SerializeField] private Button _interruptButton;
    [SerializeField] private TMPro.TMP_Dropdown _dropdownList;
    [SerializeField] private List<Card> _cards;
    private bool _cardsAreHidden = true;
    private int _amountOfUsedCards = 0;
    public bool CardsAreHidden {
        get {
            return _cardsAreHidden;
        }
        set {
            _cardsAreHidden = value;

            HandleControlButtons(true, false);
            _amountOfUsedCards = 0;
        }
    }

    public void Load() {
        StopAllCoroutines();
        StartCoroutine(ShowCardsInSpecificType());
    }

    public void Cancel() {
        StopAllCoroutines();
        StartCoroutine(HideCards());
    }

    // Coroutine for hiding cards and executing specific view type based on dropdownList current variant.
    private IEnumerator ShowCardsInSpecificType() {
        if (!CardsAreHidden) {
            yield return HideCards();
        }


        HandleControlButtons(false, true);

        switch(_dropdownList.captionText.text.ToLower()) {
            case "onebyone" : { 
                StartCoroutine(OneByOne.ShowCards(_cards, URL, () => { CardsAreHidden = false; }));
                break;
            }
            case "allatonce" : { 
                StartCoroutine(AllAtOnce.ShowCards(_cards, URL, () => { CardsAreHidden = false; }));
                break;
            }
            case "eachasready" : {
                StartCoroutine(EachAsReady.ShowCards(_cards, URL, () => {
                    if (++_amountOfUsedCards == _cards.Count) {
                        CardsAreHidden = false;
                    }
                }));
                break;
            }
        }
    }


    // Coroutine for hiding cards based on CardsAreHidden flag and *calculatedAmount value (*How much cards are shown at the time of this function call).
    private IEnumerator HideCards() {
        // Condition becomes true when all cards are shown
        if (!CardsAreHidden) {
            HandleControlButtons(false, false);
            
            foreach(Card card in _cards) {
                card.Flip(() => {
                    _amountOfUsedCards++;
                });
            }

            yield return new WaitUntil(() => _amountOfUsedCards == _cards.Count);
            CardsAreHidden = true;
        }
        // When (part of || none of) cards are shown
        else {
            int calculatedAmount = GetShownCardsAmount();
            
            if (calculatedAmount != 0) {
                int actualAmount = 0;
                foreach(Card card in _cards) {
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

    // This method calculates and retunrns amount of shown cards whose states are repaired.
    private int GetShownCardsAmount() {
        int a = 0;

        foreach (Card card in _cards) {
            card.RepairState();
            
            if (card.IsShown) {
                a++;
            }
        }

        return a;
    }

    private void HandleControlButtons(bool loadButtonState, bool cancelButtonState) {
        _loadButton.interactable = loadButtonState;
        _interruptButton.interactable = cancelButtonState;
    }
}
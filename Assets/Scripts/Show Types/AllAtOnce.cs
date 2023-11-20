using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllAtOnce : ICardsDisplayer
{
    // This coroutine implements all at once cards view by starting sprite load coroutines for each card sequentially. When all cards are ready - they will be flipped.
    public static IEnumerator ShowCards(List<Card> cards, string url, System.Action callback) {
        int amountOfFlippedCards = 0;
        
        foreach(Card card in cards) {
            card.LoadSprite(url);

            yield return new WaitUntil(() => card.CharImageSprite != null);
        }

        foreach(Card card in cards) {
            card.Flip(() => {
                amountOfFlippedCards++;
            });
        }

        yield return new WaitUntil(() => amountOfFlippedCards == cards.Count);
        callback();
    }
}

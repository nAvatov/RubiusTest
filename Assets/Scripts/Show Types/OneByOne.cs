using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneByOne : ICardsDisplayer
{
    public static IEnumerator ShowCards(List<Card> cards, string url, System.Action callback) {
        int amountOfFlippedCards = 0;
        
        foreach(Card card in cards) {
            card.LoadSprite(url);

            yield return new WaitUntil(() => card.CharImageSprite != null);
            card.Flip(() => {
                amountOfFlippedCards++;
            });
        }

        yield return new WaitUntil(() => amountOfFlippedCards == cards.Count);
        callback();
    }
}

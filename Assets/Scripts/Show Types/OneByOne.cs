using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneByOne : ICardsDisplayer
{   
    /// <summary>
    /// This coroutine implements one by one cards view by sequential enumeration thru cards list and flipping each card as it's ready to.
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="url"> request destination adress </param>
    /// <param name="callback"></param>
    /// <returns></returns>
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

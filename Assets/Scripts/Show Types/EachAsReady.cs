using System.Collections;
using System.Collections.Generic;

public class EachAsReady : ICardsDisplayer
{
    // This coroutine implements each as ready cards view by starting sprite load coroutine for each card sequentially. By using callbacks - each card flips when it's ready.
    public static IEnumerator ShowCards(List<Card> cards, string url, System.Action callback) {
        foreach(Card card in cards) {
            card.LoadSprite(url, () => { 
                card.Flip(() => {
                    callback(); 
                }); 
            });
        }

        yield break;
    }
}

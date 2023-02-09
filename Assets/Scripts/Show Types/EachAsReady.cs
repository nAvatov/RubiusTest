using System.Collections;
using System.Collections.Generic;

public class EachAsReady 
{
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

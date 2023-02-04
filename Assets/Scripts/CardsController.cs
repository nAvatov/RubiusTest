using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class CardsController : MonoBehaviour
{
    [SerializeField] List<Card> cards;
    [SerializeField] Sprite defaultSprite; // Incase something wrong happened with download
    private string urlPattern = "http://picsum.photos/200";
    private bool cardsAreShown = false;

    #region Public Methods

    public void Call() {
        StopAllCoroutines();
        StartCoroutine(ShowOneByOne());
    }
        
    #endregion

    #region Private Methods

    private IEnumerator ShowOneByOne() {
        HideAllCards();

        yield return new WaitUntil(() => cardsAreShown == false);

        StartCoroutine(ShowCards(null, (Card loadedCard) => {
            loadedCard.Flip();
        }));
    }

    private IEnumerator ShowCards(System.Action AllAtOnce = null, System.Action<Card> OneByOne = null) {
        foreach(Card card in cards) {
            StartCoroutine(SendRequest(urlPattern, (UnityWebRequest response) => {
                card.CharImageSprite = CreateSpriteFrom(GenerateTextureFrom(response.downloadHandler.data));
            }));
            
            yield return new WaitUntil( () => card.CharImageSprite != null);
            if (OneByOne != null) OneByOne(card);
        }
        cardsAreShown = true;

        if (AllAtOnce != null) AllAtOnce();
        yield break;
    }

    private static IEnumerator SendRequest(string url, System.Action<UnityWebRequest> callback = null) {
        UnityWebRequest request = UnityWebRequest.Get(url);
        
        yield return request.SendWebRequest();

        callback(request);
    }

    private Sprite CreateSpriteFrom(Texture2D texture) {
        if (texture != null) {
            try {
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),new Vector2(0,0));
            }
            catch (System.Exception e) {
                Debug.LogError("Texture is broken. Detailed: " + e);
                return defaultSprite;
            }
        }
        return defaultSprite;
    }

    private Texture2D GenerateTextureFrom(byte[] bytes) {
        if (bytes.Length > 0)  {
            Texture2D newTexture = new Texture2D(2,2);
        
            try {
                if (newTexture.LoadImage(bytes)) {
                    return newTexture;
                }
            }
            catch (System.Exception e) {
                Debug.LogError("Bytes are crashed. Detailed: " + e);
                return null;
            }
        }
              
        return null;
    }

    private bool EachCardHaveSprite() {
        foreach(Card card in cards) {
            if (card.CharImageSprite == null) {
                return false;
            }
        }

        return true;
    }

    private void HideAllCards() {
        if (cardsAreShown) {
            foreach(Card card in cards) {
                card.Flip();
            }
            cardsAreShown = false;
        }
    }
        
    #endregion
}



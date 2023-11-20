using UnityEngine;

public class ResourceCreator
{
    /// <summary>
    /// Method for generating Texture2D object by passing raw byte[] array (representing picture by default).
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static Texture2D GenerateTextureFrom(byte[] bytes) {
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

    /// <summary>
    /// Method for creating sprite by given Texture2D.
    /// </summary>
    /// <param name="texture"></param>
    /// <returns></returns>
    public static Sprite CreateSpriteFrom(Texture2D texture) {
        if (texture != null) {
            try {
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),new Vector2(0,0));
            }
            catch (System.Exception e) {
                Debug.LogError("Texture is broken. Detailed: " + e);
                return null;
            }
        }
        return null;
    }
}

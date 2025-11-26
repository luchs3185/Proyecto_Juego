using UnityEngine;
using UnityEngine.UI;

public class LifeBarController : MonoBehaviour
{
    [Header("Sprites de la barra de vida")]
    public Sprite[] lifeSprites; // Tus 5 imágenes

    private Image lifeImage;

    private void Awake()
    {
        lifeImage = GetComponent<Image>();
    }

    // Cambia el sprite según la vida restante
    public void UpdateLife(int currentLife)
    {
        currentLife = Mathf.Clamp(currentLife, 0, lifeSprites.Length - 1);
        lifeImage.sprite = lifeSprites[currentLife];
    }
}

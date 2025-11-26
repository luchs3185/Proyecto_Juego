using UnityEngine;

public class Cheese : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null)
        {   
             player.life ++;  //subir vida
            // Evitar superar la vida máxima
            if (player.life > player.maxLife)
                player.life = player.maxLife;

            //actualizar barra de vida
            if (player.lifeBar != null)
                player.lifeBar.UpdateLife(player.life);

            // Actualizar UI 
             GetComponent<Collider>().enabled = false;

            if (GetComponent<MeshRenderer>() != null)
                GetComponent<MeshRenderer>().enabled = false;
            Destroy(gameObject);  // Se destruye al recogerlo
        }
    }
}

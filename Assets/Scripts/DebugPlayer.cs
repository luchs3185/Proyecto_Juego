using UnityEngine;
using UnityEngine.UI;

public class DebugPlayer : MonoBehaviour
{
    public GameObject player;
    private Rigidbody playerRG;
    public Text speedText;
    public Text groundText;
    void Start()
    {
        playerRG = player.GetComponent<Rigidbody>();
    }

    void Update()
    {
        speedText.text ="Speed: X(" + playerRG.linearVelocity.x.ToString("F3") + ") Y(" + playerRG.linearVelocity.y.ToString("F3")+")"; 
        groundText.text = "Ground: " + Player.inGround;
        
    }
}

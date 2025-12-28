using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Range(0f, 1f)]
    public float parallaxFactor = 0.5f;

    [Range(0f, 1f)]
    public float verticalFactor = 0.3f;

    private Transform cam;
    private Vector3 lastCamPos;

    void Start()
    {
        cam = Camera.main.transform;
        lastCamPos = cam.position;
    }

    void LateUpdate()
    {
        Vector3 delta = cam.position - lastCamPos;

        Vector3 move = new Vector3(
            delta.x * parallaxFactor,
            delta.y * parallaxFactor * verticalFactor,
            0f
        );

        transform.position += move;
        lastCamPos = cam.position;
    }
}

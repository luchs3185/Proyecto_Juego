using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public Transform target;
    public float verticalOffset=10f;
    public float horizontalOffset=0f;

    void Start()
    {
    }

    void LateUpdate()
    {
        gameObject.transform.position = new Vector3(target.position.x +horizontalOffset , target.position.y + verticalOffset, gameObject.transform.position.z);
    }
    
}
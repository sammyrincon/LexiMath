using UnityEngine;

public class ForwardOnlyCamera : MonoBehaviour
{
    public Transform player;

    void LateUpdate()

    {
        if(player != null)
        {
            if(player.position.x > transform.position.x)
            {
                transform.position = new Vector3(player.position.x, transform.position.y, transform.position.z);
            }
        }
    } 



}

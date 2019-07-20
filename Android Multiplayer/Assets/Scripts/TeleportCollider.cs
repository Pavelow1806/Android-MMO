using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCollider : MonoBehaviour
{
    private Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        player.Parent_Colliding = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        player.Parent_Colliding = false;
    }
}

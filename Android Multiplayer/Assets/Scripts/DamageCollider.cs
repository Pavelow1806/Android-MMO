using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    public Player player;

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Projectile":
                player.TakeDamage(DamageType.PROJECTILE);
                break;
            case "Laser":
                player.TakeDamage(DamageType.LASER);
                break;
            case "Poison":
                player.TakeDamage(DamageType.POISON);
                break;
            default:
                break;
        }
    }
}

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
                player.TakeDamage(SpellType.Fire);
                break;
            case "Laser":
                player.TakeDamage(SpellType.Arcane);
                break;
            case "Poison":
                player.TakeDamage(SpellType.Poison);
                break;
            default:
                break;
        }
    }
}

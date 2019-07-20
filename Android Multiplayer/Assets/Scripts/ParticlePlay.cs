using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePlay : MonoBehaviour
{
    public List<ParticleSystem> particleSystems;
    
    public void PlayAll()
    {
        for (int i = 0; i < particleSystems.Count; i++)
        {
            particleSystems[i].Play();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveParticles : MonoBehaviour
{
    public int particleCount;
    ParticleSystem particleSystem;
    ParticleSystem.Particle[] particles;
    Vector3[] startPos;
    Transform destination;

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[particleCount];
        startPos = new Vector3[particleCount];
    }

    public void StartMoving(Transform destination)
    {
        this.destination = destination;
        particleSystem.Emit(particles.Length);
        particleSystem.GetParticles(particles);
        for (int i = 0; i < particles.Length; i++)
        {
            startPos[i] = particles[i].position;
        }
    }

    private void Update()
    {
        int size = particleSystem.GetParticles(particles);
        if (size > 0)
        {
            for (int i = 0; i < size; i++)
            {
                particles[i].position = Vector3.Lerp(destination.position, startPos[i], particles[i].remainingLifetime / particles[i].startLifetime);
            }
            particleSystem.SetParticles(particles, size);
        }
    }
}
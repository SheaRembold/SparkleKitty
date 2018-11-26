using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveParticles : MonoBehaviour
{
    public int particleCount = 50;
    public float moveLength = 2f;
    ParticleSystem particleSystem;
    ParticleSystem.Particle[] particles;
    Vector3[] startPos;
    Transform destination;
    float moveTime;
    bool isPlaying;

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
        moveTime = 0f;
        particleSystem.Pause();
    }

    public void StartPlaying()
    {
        particleSystem.Play();
        isPlaying = true;
    }

    private void Update()
    {
        moveTime += Time.deltaTime;
        int size = particleSystem.GetParticles(particles);
        if (size > 0)
        {
            for (int i = 0; i < size; i++)
            {
                particles[i].position = Vector3.Lerp(startPos[i], destination.position, 
                    (particles[i].startLifetime - particleSystem.main.startLifetime.constantMin) / (particleSystem.main.startLifetime.constantMax - particleSystem.main.startLifetime.constantMin) * Mathf.Clamp01(moveTime / moveLength));// particles[i].remainingLifetime / particles[i].startLifetime);
            }
            particleSystem.SetParticles(particles, size);
        }

        if (isPlaying && size == 0)
        {
            Destroy(gameObject);
        }
    }
}
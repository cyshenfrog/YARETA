using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticalWrapper : MonoBehaviour
{
    private ParticleSystem TargetParticleSystem;
    private ParticleSystem.Particle[] particles;
    private float radius;
    private Vector3 direction;
    private Vector3 pos;
    private Vector3 center;

    // Start is called before the first frame update：
    private void Start()
    {
        TargetParticleSystem = GetComponent<ParticleSystem>();
        radius = TargetParticleSystem.shape.radius;
        particles = new ParticleSystem.Particle[TargetParticleSystem.main.maxParticles];
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (TargetParticleSystem == null)
            return;
        int numParticlesAlive = TargetParticleSystem.GetParticles(particles);
        center = transform.position;
        for (int i = 0; i < numParticlesAlive; i++)
        {
            pos = particles[i].position;
            pos.y = center.y;
            if (Vector3.Distance(pos, center) > radius)
            {
                direction = center - pos;
                particles[i].position += 2 * direction;
            }
        }
        TargetParticleSystem.SetParticles(particles, numParticlesAlive);
    }
}
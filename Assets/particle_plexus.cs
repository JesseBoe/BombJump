using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class particle_plexus : MonoBehaviour
{
    public float maxDistance = 1.0f;

    new ParticleSystem _particleSystem;
    ParticleSystem.Particle[] particles;

    ParticleSystem.MainModule particleSystemMainModule;

    public LineRenderer lineRendererTemplate;

    List<LineRenderer> lineRenderers = new List<LineRenderer>();

    Transform _transform;
	// Use this for initialization
	void Start ()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        particleSystemMainModule = _particleSystem.main;
        _transform = transform;
    }
	
	// Update is called once per frame
	void LateUpdate ()
    {
        int maxparticles = particleSystemMainModule.maxParticles;

        if (particles == null || particles.Length < maxparticles)
        {
            particles = new ParticleSystem.Particle[maxparticles];
        }

        _particleSystem.GetParticles(particles);
        int particleCount = _particleSystem.particleCount;

        float maxDistanceSqr = maxDistance * maxDistance;

        int lrIndex = 0;
        int lineRendererCount = lineRenderers.Count;

        for (int i = 0; i < particleCount; i++)
        {
            Vector3 p1_position = particles[i].position;
            for (int j = i + 1; j < particleCount; i++)
            {
                Vector3 p2_position = particles[j].position;
                float distanceSqr = Vector3.SqrMagnitude(p1_position - p2_position);

                if (distanceSqr <= maxDistanceSqr)
                {
                    LineRenderer lr;

                    //Line below only activates at 0 = 0. Fucking genius
                    if (lrIndex == lineRendererCount)
                    {
                        lr = Instantiate(lineRendererTemplate, _transform, false);
                        lineRenderers.Add(lr);
                        lineRendererCount++;
                    }
                    lr = lineRenderers[lrIndex];
                    lr.enabled = true;
                    lr.SetPosition(0, p1_position);
                    lr.SetPosition(1, p2_position);

                    lrIndex++;

                }
            }
        }

        for (int i = lrIndex; i < lineRendererCount; i++)
        {
            lineRenderers[i].enabled = false;
        }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsController : Singleton<EffectsController>
{
    [SerializeField] ParticleSystem Smoke;

    public ParticleSystem GetParticles { get { return Smoke; } }

	protected override void AwakeSingleton()
	{
	}
}

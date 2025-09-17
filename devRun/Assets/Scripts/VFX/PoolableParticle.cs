using Az7.Utils.Pool;
using UnityEngine;

namespace Branches
{
    [RequireComponent(typeof(ParticleSystem))]
    public class PoolableParticle : PoolableObject
    {
        private ParticleSystem _particleSystem;

        private void Update()
        {
            if (ReturnRequested) return;

            if (_particleSystem.isStopped)
            {
                Return();
            }
        }

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }
    }
}

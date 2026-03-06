using Az7.Extensions;
using Az7.Utils.Pool;
using UnityEngine;
using UniRx;
using System.Collections.Generic;

namespace DevRun
{
    public class CollectableFX : MonoBehaviour
    {
        [SerializeField] private Pool _fxPool;

        private List<ParticleSystem> _activeSystems = new List<ParticleSystem>(10);
        private List<ParticleSystem> _inactiveSystems = new List<ParticleSystem>(10);

        private void FixedUpdate()
        {
            foreach (var system in _inactiveSystems)
            {
                _activeSystems.Remove(system);
                system.GetComponent<PoolableObject>().Return();
            }

            _inactiveSystems.Clear();

            foreach (var system in _activeSystems)
            {
                if (!system.isPlaying)
                {
                    _inactiveSystems.Add(system);
                }
            }
        }

        private void Awake()
        {
            Collectable.OnCollected.Subscribe(collectable =>
            {
                var fx = _fxPool.Take().GetComponent<ParticleSystem>();
                var main = fx.main;

                main.startColor = ColorProvider.Instance.Get(collectable.ColorIndex);

                fx.transform.position = collectable.transform.position;
                fx.gameObject.SetActive(true);
                _activeSystems.Add(fx);
                fx.Play();
            }).AddTo(this);
        }
    }
}

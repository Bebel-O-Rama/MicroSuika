using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiSuika.Ball
{
    public class BallVisualEffects : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _vfxBallFusedClear;
        [SerializeField] private ParticleSystem _vfxBallFusedContact;
        [SerializeField] private ParticleSystem _vfxContainerBallDamage;

        public void PlayBallFusedClear()
        {
            PlayAndDestroy(_vfxBallFusedClear);
        }

        public void PlayBallFusedContact(Vector3 position)
        {
            PlayAndDestroy(_vfxBallFusedContact, position);
        }
        
        public void PlayContainerBallDamage()
        {
            PlayAndDestroy(_vfxContainerBallDamage);
        }
        
        // Let's detach the particle system for every usage FOR NOW
        private void PlayAndDestroy(ParticleSystem particles)
        {
            if (!particles)
                return;
            particles.transform.SetParent(null);
            particles.gameObject.SetActive(true);
            particles.Play();
            Destroy(particles.gameObject, particles.main.duration);
        }
        
        private void PlayAndDestroy(ParticleSystem particles, Vector3 position)
        {
            if (!particles)
                return;
            particles.transform.SetParent(null);
            particles.transform.position = position;
            particles.gameObject.SetActive(true);
            particles.Play();
            Destroy(particles.gameObject, particles.main.duration);
        }
    }
}

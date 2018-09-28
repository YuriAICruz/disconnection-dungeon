using Graphene.Acting;
using UnityEngine;

namespace Graphene.DisconnectionDungeon
{
    public abstract class Weapon : MonoBehaviour
    {
        public int Damage;
        private bool _enabled;

        private GameObject _hitParticle;

        public float Height;
        public Vector3 Offset;
        private IDamageble _owner;

        private void Awake()
        {
            _hitParticle = Resources.Load<GameObject>("Particle/Hit0");
        }

        public void SetOwner(IDamageble owner)
        {
            _owner = owner;
        }

        public void SetEnabled(bool enable)
        {
            _enabled = enable;
        }

        private void Update()
        {
            if (!_enabled) return;

            var hits = UnityEngine.Physics.RaycastAll(transform.TransformPoint(Offset), transform.forward, Height);

            foreach (var hit in hits)
            {
                var dmg = hit.transform.GetComponent<IDamageble>();

                if (dmg == null || (_owner != null && _owner == dmg)) continue;

                Instantiate(_hitParticle, hit.point, Quaternion.identity);
                dmg.DoDamage(Damage);
            }
        }
    }
}
using System.Collections.Generic;
using Graphene.Acting.Platformer;
using Graphene.BehaviourTree;
using Graphene.BehaviourTree.Actions;
using Graphene.BehaviourTree.Composites;
using Graphene.BehaviourTree.Conditions;
using UnityEngine;
using Behaviour = Graphene.BehaviourTree.Behaviour;

namespace Graphene.DisconnectionDungeon.Enemies
{
    public class Dummy : PlatformerActor
    {
        enum BlackboardIds
        {
            MoveTo = 1,
            MoveAway = 2,
            Scan = 3,
            HpIsLow = 4,
            CurrentTarget = 5,
            HasTarget = 6,
            DummyAttack = 7,
            MoveStray = 8
        }

        public int DangerHp;

        private float _lastScan;

        public float VisionRadius;
        public float AttackDistance;

        private Behaviour _tree;
        private Blackboard _blackboard;

        protected override void OnAwake()
        {
            base.OnAwake();

            _blackboard = new Blackboard();
            _tree = new Behaviour();

            CreateBehaviour();
        }

        private void CreateBehaviour()
        {
            SetupDelegates();

            _tree.root = new Priority(new List<Node>
            {
                new Sequence(new List<Node>
                {
                    new CallSystemActionMemory((int) BlackboardIds.Scan),
                    new Priority(new List<Node>
                    {
                        new Sequence(new List<Node>
                        {
                            new CallSystemActionMemory((int) BlackboardIds.HpIsLow),
                            new CallSystemAction((int) BlackboardIds.MoveAway)
                        }),
                        new Priority(new List<Node>
                        {
                            new Sequence(new List<Node>
                            {
                                new CheckDistance(AttackDistance, (int) BlackboardIds.CurrentTarget),
                                new CallSystemAction((int) BlackboardIds.DummyAttack)
                            }),
                            new Sequence(new List<Node>
                            {
                                new CallSystemAction((int) BlackboardIds.MoveTo)
                            }),
                        }),
                    }),
                }),
                new Sequence(new List<Node>
                {
                    new CallSystemAction((int) BlackboardIds.MoveStray)
                })
            });
        }

        public override void DoDamage(int damage, Vector3 from)
        {
            if (from != null)
            {
                var dir = transform.position - from;
                _physics.Push(dir);
            }
            
            base.DoDamage(damage, from);
        }

        protected override void OnDie()
        {
            base.OnDie();

            _physics.EnableRagdool();
        }

        private void Update()
        {
            if (Life.Hp <= 0) return;

            _tree.Tick(this.gameObject, _blackboard);
        }

        private void SetupDelegates()
        {
            _blackboard.Set((int) BlackboardIds.CurrentTarget, null, _tree.id);

            _blackboard.Set((int) BlackboardIds.HpIsLow, new Behaviour.NodeResponseAction(HpIsLow), _tree.id);
            _blackboard.Set((int) BlackboardIds.Scan, new Behaviour.NodeResponseAction(Scan), _tree.id);
            _blackboard.Set((int) BlackboardIds.HasTarget, new Behaviour.NodeResponseAction(HasTarget), _tree.id);
            _blackboard.Set((int) BlackboardIds.MoveTo, new System.Action(MoveTo), _tree.id);
            _blackboard.Set((int) BlackboardIds.MoveAway, new System.Action(MoveAway), _tree.id);
            _blackboard.Set((int) BlackboardIds.MoveStray, new System.Action(MoveStray), _tree.id);
            _blackboard.Set((int) BlackboardIds.DummyAttack, new System.Action(DummyAttack), _tree.id);
        }

        private void DummyAttack()
        {
            Debug.Log("I'm Atacking, Yay");
        }

        private NodeStates HasTarget()
        {
            return _blackboard.Get((int) BlackboardIds.CurrentTarget, _tree.id).value == null ? NodeStates.Failure : NodeStates.Success;
        }

        private NodeStates HpIsLow()
        {
            return Life.Hp < DangerHp ? NodeStates.Success : NodeStates.Failure;
        }

        private NodeStates Scan()
        {
            Debug.DrawRay(transform.position + Vector3.up, transform.forward * VisionRadius, Color.magenta);
            Debug.DrawRay(transform.position + Vector3.up, -transform.forward * VisionRadius, Color.magenta);
            Debug.DrawRay(transform.position + Vector3.up, transform.right * VisionRadius, Color.magenta);
            Debug.DrawRay(transform.position + Vector3.up, -transform.right * VisionRadius, Color.magenta);
            
            if (_lastScan + 0.2f > Time.time) return HasTarget();

            _lastScan = Time.time;
            var hits = UnityEngine.Physics.SphereCastAll(transform.position, VisionRadius, transform.forward, VisionRadius);
            
            foreach (var hit in hits)
            {
                var player = hit.transform.GetComponent<Player>();

                if (player == null) continue;

                _blackboard.Set((int) BlackboardIds.CurrentTarget, player.transform, _tree.id);
                return NodeStates.Success;
            }

            _blackboard.Set((int) BlackboardIds.CurrentTarget, null, _tree.id);
            return NodeStates.Failure;
        }

        private void MoveAway()
        {
            var tgt = (Transform) _blackboard.Get((int) BlackboardIds.CurrentTarget, _tree.id).value;
            if (tgt == null) return;

            var dir = tgt.position - transform.position;
            dir.y = dir.z;
            _physics.Move(-dir.normalized, Speed, false);
            
            Look(dir);
        }

        private void MoveTo()
        {
            var tgt = (Transform) _blackboard.Get((int) BlackboardIds.CurrentTarget, _tree.id).value;
            if (tgt == null) return;

            var dir = tgt.position - transform.position;
            dir.y = dir.z;
            _physics.Move(dir.normalized, Speed, false);

            Look(dir);
        }

        private void MoveStray()
        {
            Debug.Log("I'll not move :(");
        }
    }
}
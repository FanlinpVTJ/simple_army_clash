using System;
using System.Collections.Generic;
using Scellecs.Morpeh;
using SimpleArmyClash.Domain;
using UnityEngine;

namespace SimpleArmyClash.Ecs
{
    public sealed class UnitWorld : IDisposable
    {
        private readonly List<Entity> _entities;

        public World World { get; }
        public Stash<UnitComponent> Units { get; }
        public Stash<PositionComponent> Positions { get; }
        public Stash<HealthComponent> Health { get; }
        public Stash<MovementComponent> Movement { get; }
        public Stash<AttackComponent> Attacks { get; }
        public Stash<TargetComponent> Targets { get; }
        public Stash<AliveComponent> Alive { get; }
        public Stash<SelectionComponent> Selection { get; }
        public Filter LivingUnits { get; }
        public int UnitCount => _entities.Count;

        public UnitWorld(int capacity)
        {
            World = World.Create();

            if (World == null)
            {
                throw new InvalidOperationException("Morpeh world capacity has been exhausted.");
            }

            World.UpdateByUnity = false;
            World.DoNotDisableSystemOnException = true;
            _entities = new List<Entity>(capacity);
            Units = World.GetStash<UnitComponent>();
            Positions = World.GetStash<PositionComponent>();
            Health = World.GetStash<HealthComponent>();
            Movement = World.GetStash<MovementComponent>();
            Attacks = World.GetStash<AttackComponent>();
            Targets = World.GetStash<TargetComponent>();
            Alive = World.GetStash<AliveComponent>();
            Selection = World.GetStash<SelectionComponent>();
            LivingUnits = World.Filter.With<UnitComponent>().With<PositionComponent>()
                .With<HealthComponent>().With<AliveComponent>().Build();
        }

        public Entity Spawn(int armyIndex, UnitDefinition definition, Vector3 position, int attackPriority)
        {
            if (armyIndex < 0 || armyIndex > 1 || definition.Statistics.MaximumHealth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(armyIndex), "A unit requires a valid army and positive health.");
            }

            Entity entity = World.CreateEntity();
            ref UnitComponent unit = ref Units.Add(entity);
            unit.Identifier = _entities.Count;
            unit.ArmyIndex = armyIndex;
            unit.Definition = definition;
            ref PositionComponent location = ref Positions.Add(entity);
            location.Value = position;
            location.Radius = definition.Radius;
            ref HealthComponent health = ref Health.Add(entity);
            health.Current = definition.Statistics.MaximumHealth;
            health.Maximum = health.Current;
            ref MovementComponent movement = ref Movement.Add(entity);
            movement.Speed = definition.Statistics.MovementSpeed;
            movement.NextPosition = position;
            ref AttackComponent attack = ref Attacks.Add(entity);
            attack.Damage = definition.Statistics.AttackDamage;
            attack.Interval = definition.Statistics.AttackInterval;
            attack.Priority = attackPriority;
            Targets.Add(entity);
            Alive.Add(entity);
            Selection.Add(entity);
            _entities.Add(entity);
            return entity;
        }

        public Entity GetEntity(int identifier)
        {
            return _entities[identifier];
        }

        public bool Contains(int identifier)
        {
            return !World.IsDisposed && identifier >= 0 && identifier < _entities.Count && World.Has(_entities[identifier]);
        }

        public bool IsAlive(Entity entity)
        {
            return !World.IsDisposed && World.Has(entity) && Alive.Has(entity) && Health.Get(entity).Current > 0;
        }

        public UnitState GetSnapshot(Entity entity)
        {
            ref UnitComponent unit = ref Units.Get(entity);
            UnitState snapshot = new UnitState(unit.Identifier, unit.ArmyIndex, unit.Definition,
                Positions.Get(entity).Value, Health.Get(entity).Current);
            return snapshot;
        }

        public void Dispose()
        {
            World.Dispose();
            _entities.Clear();
        }
    }
}

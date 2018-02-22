﻿using System;
using UnityEngine;



namespace ECS
{
    /// <summary>
    /// This attribute will disable instantiating ComponentWrapper on relaese build
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EditorOnlyAttribute : Attribute { }



    [RequireComponent(typeof(GameObjectEntity))]
    public abstract class ComponentWrapper : MonoBehaviour
    {
        public abstract IComponent Component { get; set; }
        public abstract Type ComponentType { get; }
        public abstract void AddComponentToEntity(Entity entity, EntityManager entityManager);
        public abstract void Initialize(Entity entity, EntityManager entityManager);
        public abstract void SetComponentToEntityManager();
    }



    [DisallowMultipleComponent]
    public class ComponentWrapper<TComponent> : ComponentWrapper, IComponentChangedOfEntityEventListener<TComponent> where TComponent : class, IComponent, ICloneable, new()
    {
        [SerializeField] TComponent _component = new TComponent();
        public TComponent TypedComponent { get { return _component; } }

        public override IComponent Component
        {
            get { return _component; }
            set { _component = (TComponent)value; }
        }

        public override Type ComponentType { get { return typeof(TComponent); } }

        public override void AddComponentToEntity(Entity entity, EntityManager entityManager)
        {
            if (enabled) entityManager.AddComponent(entity, (TComponent)_component.Clone());
        }

        public override void SetComponentToEntityManager() { }


        public override void Initialize(Entity entity, EntityManager entityManager)
        {
            entityManager.SubscribeOnComponentChangedOfEntity(entity, this);
            if (enabled)
            {
                if (entityManager.HasComponent<TComponent>(entity)) _component = entityManager.GetComponent<TComponent>(entity);
                else entityManager.AddComponent(entity, _component);
            }
        }

        private void OnEnable()
        {
            var gameObjectEntity = GetComponent<GameObjectEntity>();
            if (gameObjectEntity.IsInitialized)
            {
                gameObjectEntity.EntityManager.SubscribeOnComponentChangedOfEntity(gameObjectEntity.Entity, this);
                if (gameObjectEntity.EntityManager.HasComponent<TComponent>(gameObjectEntity.Entity))
                    _component = gameObjectEntity.GetComponentFromEntityManager<TComponent>();
                else
                    gameObjectEntity.EntityManager.AddComponent(gameObjectEntity.Entity, _component);
            }
        }

        private void OnDisable()
        {
            var gameObjectEntity = GetComponent<GameObjectEntity>();
            if (gameObjectEntity.IsInitialized)
            {
                gameObjectEntity.EntityManager.UnsbscribeOnComponentChangedOfEntity(gameObjectEntity.Entity, this);

                if (gameObjectEntity.EntityManager.HasComponent<TComponent>(gameObjectEntity.Entity))
                    gameObjectEntity.EntityManager.RemoveComponent<TComponent>(gameObjectEntity.Entity);
            }
        }

        public void OnComponentChanged(object sender, Entity entity, TComponent component)
        {
            _component = component;
        }

    }



    [DisallowMultipleComponent]
    public class ComponentDataWrapper<TComponent> : ComponentWrapper, IComponentChangedOfEntityEventListener<TComponent> where TComponent : struct, IComponent
    {
        [SerializeField] TComponent _component;

        public TComponent TypedComponent { get { return _component; } }

        public override IComponent Component
        {
            get { return _component; }
            set { _component = (TComponent)value; }
        }

        public override Type ComponentType { get { return typeof(TComponent); } }

        public override void Initialize(Entity entity, EntityManager entityManager)
        {
            entityManager.SubscribeOnComponentChangedOfEntity(entity, this);
            if (entityManager.HasComponent<TComponent>(entity)) _component = entityManager.GetComponent<TComponent>(entity);
            else entityManager.AddComponent(entity, _component);
        }

        public override void AddComponentToEntity(Entity entity, EntityManager entityManager)
        {
            entityManager.AddComponent(entity, _component);
        }

        public void OnComponentChanged(object sender, Entity entity, TComponent component)
        {
            _component = component;
        }

        public override void SetComponentToEntityManager()
        {
            GetComponent<GameObjectEntity>().SetComponentToEntityManager(_component);
        }
        private void OnEnable()
        {
            var gameObjectEntity = GetComponent<GameObjectEntity>();
            if (enabled && gameObjectEntity.IsInitialized)
            {
                gameObjectEntity.EntityManager.SubscribeOnComponentChangedOfEntity(gameObjectEntity.Entity, this);
                if (gameObjectEntity.EntityManager.HasComponent<TComponent>(gameObjectEntity.Entity))
                    _component = gameObjectEntity.GetComponentFromEntityManager<TComponent>();
                else
                    gameObjectEntity.EntityManager.AddComponent(gameObjectEntity.Entity, _component);
            }
        }

        private void OnDisable()
        {
            var gameObjectEntity = GetComponent<GameObjectEntity>();
            if (gameObjectEntity.IsInitialized)
            {
                gameObjectEntity.EntityManager.UnsbscribeOnComponentChangedOfEntity(gameObjectEntity.Entity, this);
                if (gameObjectEntity.EntityManager.HasComponent<TComponent>(gameObjectEntity.Entity))
                    gameObjectEntity.EntityManager.RemoveComponent<TComponent>(gameObjectEntity.Entity);
            }
        }
    }
}

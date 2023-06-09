using Askeladd.Scripts.Characters.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Askeladd.Scripts.Characters.Enemies
{
    public class EnemyBase : MonoBehaviour, IDamageAble
    {
        [field: Header("Enemy States")]
        [SerializeField]
        protected float _health;
        [SerializeField]
        protected float _maxHealth;

        protected virtual void Awake()
        {
            _health = _maxHealth;
        }

        public virtual void TakeDamage(float damage) { }        
    }
}

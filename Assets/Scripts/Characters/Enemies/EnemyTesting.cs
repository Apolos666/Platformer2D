using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Askeladd.Scripts.Characters.Enemies
{
    public class EnemyTesting : EnemyBase
    {
        public override void TakeDamage(float damage)
        {
            _health -= damage;

            if (_health < 0) Debug.Log("Enemy Die");
        }
    }
}

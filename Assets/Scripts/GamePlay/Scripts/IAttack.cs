using UnityEngine;

namespace Gameplay.Scripts
{
    public interface IAttack
    {
        void Attack(GameObject target);
        float GetMagicCost();
        bool IsMagicAttack();
    }
}
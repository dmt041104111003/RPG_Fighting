using UnityEngine;
using System.Collections;

namespace Gameplay.Scripts
{
    using Core.Scripts;

    public class EnemyAI : MonoBehaviour
    {
        public GameObject[] attackPrefabs; // 0: Melee, 1: Range
        private FighterStats enemyStats;
        private GameObject hero;
        private Vector3 originalPosition;

        public float meleeDistance = 1.5f;
        public float attackCooldown = 3f;

        void Start()
        {
            enemyStats = GetComponent<FighterStats>();
            hero = GameObject.FindGameObjectWithTag("Hero");
            originalPosition = transform.position;
            StartCoroutine(AttackRoutine());
        }

        IEnumerator AttackRoutine()
        {
            while (true)
            {
                PerformGreedyAttack();
                yield return new WaitForSeconds(attackCooldown);
            }
        }

        void PerformGreedyAttack()
        {
            if (attackPrefabs.Length < 1 || hero == null || enemyStats.GetDead()) return;

            float distanceToHero = Vector3.Distance(transform.position, hero.transform.position);
            FighterStats heroStats = hero.GetComponent<FighterStats>();

            int bestAttackIndex = -1;
            float bestScore = -1f;

            for (int i = 0; i < attackPrefabs.Length; i++)
            {
                if (attackPrefabs[i] == null) continue;

                AttackScript attackScript = attackPrefabs[i].GetComponent<AttackScript>();
                if (attackScript.magicCost > enemyStats.magic) continue;

                float score = CalculateAttackScore(attackScript, distanceToHero, heroStats);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestAttackIndex = i;
                }
            }

            if (bestAttackIndex >= 0)
            {
                ExecuteAttack(bestAttackIndex, hero);
            }
        }

        float CalculateAttackScore(AttackScript attack, float distance, FighterStats targetStats)
        {
            float avgAttackMultiplier = (attack.minAttackMultiplier + attack.maxAttackMultiplier) / 2f;
            float avgDefenseMultiplier = (attack.minDefenseMultiplier + attack.maxDefenseMultiplier) / 2f;

            float baseDamage = attack.magicAttack ? avgAttackMultiplier * enemyStats.magicRange : avgAttackMultiplier * enemyStats.melee;
            float estimatedDamage = Mathf.Max(0, baseDamage - (avgDefenseMultiplier * targetStats.defense));

            float distanceFactor = attack.magicAttack ? 1f : (distance <= meleeDistance ? 1f : 0.1f);
            float magicCostPenalty = attack.magicCost > 0 ? attack.magicCost / enemyStats.magic : 0f;

            return estimatedDamage * distanceFactor * (1f - magicCostPenalty);
        }

        void ExecuteAttack(int attackIndex, GameObject target)
        {
            AttackScript attackScript = attackPrefabs[attackIndex].GetComponent<AttackScript>();
            if (attackIndex == 0)
                StartCoroutine(MeleeAttackSequence(attackScript, target));
            else
                attackScript.Attack(target);
        }

        private IEnumerator MeleeAttackSequence(AttackScript attackScript, GameObject target)
        {
            yield return CombatUtils.MoveToMeleePosition(transform, target.transform, meleeDistance);
            attackScript.Attack(target);
            yield return new WaitForSeconds(1f);
        }
    }
}
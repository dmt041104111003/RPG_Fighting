using UnityEngine;

namespace Gameplay.Scripts
{
    using Core.Scripts;

    public class AttackScript : MonoBehaviour
    {
        public GameObject owner;

        [SerializeField] 
        private string animationName;
        [SerializeField] 
        public bool magicAttack;
        [SerializeField] 
        public float magicCost;
        [SerializeField] 
        public float minAttackMultiplier;
        [SerializeField] 
        public float maxAttackMultiplier;
        [SerializeField] 
        public float minDefenseMultiplier;
        [SerializeField] 
        public float maxDefenseMultiplier;
        [SerializeField] 
        private RectTransform MagicFill;

        private FighterStats attackerStats;
        private FighterStats targetStats;
        private float damage = 0.0f;
        private Vector2 magicScale;

        void Start()
        {
            if (MagicFill == null)
            {
                Debug.LogWarning($"{nameof(MagicFill)} is not assigned in {gameObject.name}. Magic bar won't update properly.");
            }
            else
            {
                magicScale = MagicFill.localScale;
            }

            if (owner == null)
            {
                Debug.LogError($"{nameof(owner)} is not assigned in {gameObject.name}. Attack will not function.");
            }
        }

        /// <summary>
        /// Executes an attack on the specified victim, calculates damage, and plays the attack animation.
        /// </summary>
        /// <param name="victim">The target GameObject to receive damage.</param>
        public void Attack(GameObject victim)
        {
            attackerStats = owner?.GetComponent<FighterStats>();
            targetStats = victim?.GetComponent<FighterStats>();

            if (attackerStats == null || targetStats == null)
            {
                Debug.LogError($"Missing FighterStats component on owner ({owner?.name}) or victim ({victim?.name})");
                return;
            }

            if (attackerStats.magic >= magicCost)
            {
                attackerStats.UpdateMagicFill(magicCost);

                float multiplier = Random.Range(minAttackMultiplier, maxAttackMultiplier);
                damage = multiplier * (magicAttack ? attackerStats.magicRange : attackerStats.melee);

                float defenseMultiplier = Random.Range(minDefenseMultiplier, maxDefenseMultiplier);
                damage = Mathf.Max(0, damage - (defenseMultiplier * targetStats.defense));

                Animator animator = owner.GetComponent<Animator>();
                if (animator != null && !string.IsNullOrEmpty(animationName))
                {
                    animator.Play(animationName);
                }
                else
                {
                    Debug.LogWarning($"Animator or animationName missing on {owner.name}");
                }

                targetStats.ReceiveDamage(damage);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private FighterStats attackerStats;
    private FighterStats targetStats;

    private float damage = 0.0f;
    private float xMagicNewScale;

    private Vector2 magicScale;

    void Start()
    {
        magicScale = GameObject.Find("HeroMagicFill").GetComponent<RectTransform>().localScale;
    }

    private static int attackCallCount = 0;
    public void Attack(GameObject victim)
    {
        attackCallCount++;
        attackerStats = owner.GetComponent<FighterStats>();
        targetStats = victim.GetComponent<FighterStats>();

        if (attackerStats.magic >= magicCost)
        {

            attackerStats.UpdateMagicFill(magicCost);


            float multiplier = Random.Range(minAttackMultiplier, maxAttackMultiplier);

            damage = multiplier * attackerStats.melee;

            if (magicAttack)
            {
                damage = multiplier * attackerStats.magicRange;
            }

            float defenseMultiplier = Random.Range(minDefenseMultiplier, maxDefenseMultiplier);
            damage = Mathf.Max(0, damage - (defenseMultiplier * targetStats.defense));
            owner.GetComponent<Animator>().Play(animationName);
            targetStats.ReceiveDamage(damage);
        }
    }
}

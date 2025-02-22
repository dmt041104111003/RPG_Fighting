using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public GameObject[] attackPrefabs;
    private FighterAction fighterAction;
    private float attackCooldown = 3f;
    private float attackCooldownTimer = 0f;
    private int attackTypeIndex = 0; 
    private GameObject hero;
    private Vector3 originalPosition;
    public float meleeDistance = 1.5f;

    void Start()
    {
        fighterAction = GetComponent<FighterAction>();
        hero = GameObject.FindGameObjectWithTag("Hero");
        originalPosition = transform.position; 
        attackCooldownTimer = attackCooldown;
    }

    void Update()
    {
        attackCooldownTimer += Time.deltaTime;

        if (attackCooldownTimer >= attackCooldown)
        {
            PerformAttack();
            attackCooldownTimer = 0f;
        }
    }

    void PerformAttack()
    {
        if (attackPrefabs.Length > 0)
        {
            GameObject attackPrefab = attackPrefabs[attackTypeIndex];
            if (attackPrefab != null)
            {
                AttackScript attackScript = attackPrefab.GetComponent<AttackScript>();
                float magicCost = attackScript.magicCost;
                bool magicAttack = attackScript.magicAttack;
                FighterStats grantEnemyStats = GetComponent<FighterStats>();

                if (grantEnemyStats.magic >= magicCost)
                {
                    if (attackTypeIndex == 0)
                    {
                        StartCoroutine(MeleeAttackSequence(attackScript, hero));
                    }
                    else if (attackTypeIndex == 1) 
                    {
                        attackScript.Attack(hero);
                    }
                    attackTypeIndex = (attackTypeIndex + 1) % 2;
                }
                else
                {
                    attackTypeIndex = (attackTypeIndex + 1) % 2;
                }
            }
        }
    }

    private IEnumerator MeleeAttackSequence(AttackScript attackScript, GameObject target)
    {

        Vector3 targetPosition = target.transform.position;
        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 meleePosition = targetPosition - direction * meleeDistance;
        transform.position = meleePosition; 

        attackScript.Attack(target);
        yield return new WaitForSeconds(1f);

        transform.position = originalPosition;
    }
}
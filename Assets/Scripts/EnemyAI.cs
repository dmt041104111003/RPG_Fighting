using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public GameObject[] attackPrefabs;
    private FighterAction fighterAction;
    private float attackCooldown = 3f; 
    private float attackCooldownTimer = 0f;
    private bool canAttack = false;
    private int attackTypeIndex = 0; // 0: Melee, 1: Range
    private GameObject hero;
    private Vector3 originalPosition;
    public float moveSpeed = 5f;
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
                    if (attackTypeIndex == 0) // Melee attack
                    {
                        StartCoroutine(MeleeAttackSequence(attackScript, hero));
                    }
                    else if (attackTypeIndex == 1) // Range attack
                    {
                        attackScript.Attack(hero);
                    }
                    //grantEnemyStats.updateMagicFill(magicCost);

                    attackTypeIndex = (attackTypeIndex + 1) % 2;
                }
                else
                {
                    attackTypeIndex = (attackTypeIndex + 1) % 2; 
                }
            }
            else
            {
        
            }
        }
    }

    private IEnumerator MeleeAttackSequence(AttackScript attackScript, GameObject target)
    {

        Vector3 targetPosition = target.transform.position;
        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 meleePosition = targetPosition - direction * meleeDistance; 

        while (Vector3.Distance(transform.position, meleePosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, meleePosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        attackScript.Attack(target);
        yield return new WaitForSeconds(1f);

        while (Vector3.Distance(transform.position, originalPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public GameObject[] attackPrefabs;
    private FighterStats fighterStats;
    private float attackCooldown = 3f; 
    private float attackCooldownTimer = 0f;
    private int attackTypeIndex;
    private GameObject hero;
    private Vector3 originalPosition;
    public float moveSpeed = 5f;
    public float meleeDistance = 1.5f;
    private bool isAttacking;
    void Start()
    {
        fighterStats = GetComponent<FighterStats>();
        hero = GameObject.FindGameObjectWithTag("Hero");
        originalPosition = transform.position;
        attackCooldownTimer = attackCooldown;
        attackTypeIndex = 0;
        isAttacking = false;
    }

    void Update()
    {



        attackCooldownTimer += Time.deltaTime;
 
        if (attackCooldownTimer >= attackCooldown)
        {

            PerformAttack();
            attackCooldownTimer = 0f;
   isAttacking = false;
        }
    }

    void PerformAttack()
    {

        if (attackPrefabs.Length == 0 || isAttacking) return;

        isAttacking = true;
        float distanceToHero = Vector3.Distance(transform.position, hero.transform.position);
        GameObject attackPrefab = attackPrefabs[attackTypeIndex];
        AttackScript attackScript = attackPrefab.GetComponent<AttackScript>();

        if (distanceToHero <= 5f && fighterStats.magic >= attackPrefabs[0].GetComponent<AttackScript>().magicCost)
        {
            attackPrefab = attackPrefabs[0];
            attackTypeIndex = 0;
        }
        else if (fighterStats.magic >= attackPrefabs[1].GetComponent<AttackScript>().magicCost)
        {
            attackPrefab = attackPrefabs[1];
            attackTypeIndex = 1;
        }
        else
        {
            isAttacking = false;
            return; 
        }

        if (attackTypeIndex == 0)
        {
            StartCoroutine(MeleeAttackSequence(attackScript, hero));
        }
        else
        {
            attackScript.Attack(hero);
            isAttacking = false;
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
        isAttacking = false;
    }

}

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

    void Start()
    {
        fighterAction = GetComponent<FighterAction>();
        hero = GameObject.FindGameObjectWithTag("Hero");
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
                Debug.Log("<color=blue>EnemyAI - PerformAttack - Attack Type: " + (attackTypeIndex == 0 ? "Melee" : "Range") + ", Magic Cost: " + magicCost + ", magicAttack: " + magicAttack + "</color>");
                Debug.Log("GrantEnemy Magic BEFORE attack: " + grantEnemyStats.magic);
                Debug.Log("Attack Type: " + (attackTypeIndex == 0 ? "Melee" : "Range") + ", Magic Cost: " + magicCost);

                if (grantEnemyStats.magic >= magicCost)
                {
                    if (attackTypeIndex == 0)
                    {
                        attackScript.Attack(hero);
        
                    }
                    else if (attackTypeIndex == 1)
                    {
                        attackScript.Attack(hero);
                  
                    }
                    grantEnemyStats.updateMagicFill(magicCost);

                    Debug.Log("GrantEnemy Magic AFTER attack: " + grantEnemyStats.magic);

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

}

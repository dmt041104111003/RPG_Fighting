using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FighterAction : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject hero;
    private GameObject enemy;
    [SerializeField]
    private GameObject meleePrefab;

    [SerializeField]
    private GameObject rangePrefab;

    [SerializeField]
    private Sprite faceIcon;

    private GameObject currentAttack;
    private Vector3 originalPosition;
    public float moveSpeed = 5f;
    public float meleeDistance = 1.5f;
    public float jumpHeight = 3f;
    public void Awake()
    {
        hero = GameObject.FindGameObjectWithTag("Hero");
        enemy = GameObject.FindGameObjectWithTag("Enemy");
        originalPosition = hero.transform.position;
    }

    public void SelectAttack(string btn)
    {
        GameObject victim = (tag == "Hero") ? enemy : hero;
        if (tag == "Hero")
        {
            victim = enemy;
        }

        if (btn.CompareTo("melee") == 0)
        {
            StartCoroutine(MeleeAttackSequence(meleePrefab.GetComponent<AttackScript>(), victim));
        }
        else if (btn.CompareTo("range") == 0)
        {
            rangePrefab.GetComponent<AttackScript>().Attack(victim);
        }
        else
        {
            Debug.Log("Run");
        }
    }

    private IEnumerator MeleeAttackSequence(AttackScript attackScript, GameObject target)
    {
        FighterStats heroStats = hero.GetComponent<FighterStats>();
        if (heroStats.magic < attackScript.magicCost)
        {
            Debug.Log("Not enough magic!");
            yield break;
        }
        Vector3 jumpPosition = originalPosition + Vector3.up * jumpHeight;
        hero.transform.position = jumpPosition;
        yield return new WaitForSeconds(0.5f);

        Vector3 targetPosition = target.transform.position;
        Vector3 direction = (targetPosition - hero.transform.position).normalized;
        Vector3 meleePosition = targetPosition - direction * meleeDistance;
        hero.transform.position = meleePosition;

        attackScript.Attack(target);
        yield return new WaitForSeconds(1f); 

        hero.transform.position = originalPosition;
    }
}

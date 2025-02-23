using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FighterAction : MonoBehaviour
{
    private GameObject hero;
    private GameObject enemy;
    [SerializeField]
    private GameObject meleePrefab;

    [SerializeField]
    private GameObject rangePrefab;

    [SerializeField]
    private Sprite faceIcon;
    [SerializeField]
    private Text notificationText;
    private Vector2 initialTextPosition;
    private Coroutine activeNotificationCoroutine;
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
    public void Start()
    {
        if (notificationText != null)
        {
            initialTextPosition = notificationText.GetComponent<RectTransform>().anchoredPosition;
            notificationText.gameObject.SetActive(false);
        }
    }
    public void SelectAttack(string btn)
    {
        GameObject victim = (tag == "Hero") ? enemy : hero;

        AttackScript attackScript = null;
        if (btn.CompareTo("melee") == 0)
        {
            attackScript = meleePrefab?.GetComponent<AttackScript>();
        }
        else if (btn.CompareTo("range") == 0)
        {
            attackScript = rangePrefab?.GetComponent<AttackScript>();
        }
        else if (btn.CompareTo("run") == 0)
        {
            attackScript = null;
        }

        FighterStats heroStats = hero.GetComponent<FighterStats>();
        if (!CanUseSkill(attackScript, heroStats))
        {
            ShowNotification("Not Enough Magic!");
            return;
        }

        if (btn.CompareTo("melee") == 0)
        {
            StartCoroutine(MeleeAttackSequence(attackScript, victim));
        }
        else if (btn.CompareTo("range") == 0)
        {
            attackScript.Attack(victim);
        }
        else if (btn.CompareTo("run") == 0)
        {
            StartCoroutine(RunSequence());
        }
    }
    private void ShowNotification(string message)
    {
        if (notificationText != null)
        {

            if (activeNotificationCoroutine != null)
            {
                StopCoroutine(activeNotificationCoroutine);
                notificationText.gameObject.SetActive(false);
            }

            RectTransform textTransform = notificationText.GetComponent<RectTransform>();
            textTransform.anchoredPosition = initialTextPosition;
            notificationText.text = message;
            notificationText.color = new Color(notificationText.color.r, notificationText.color.g, notificationText.color.b, 1f);
            notificationText.gameObject.SetActive(true);

            activeNotificationCoroutine = StartCoroutine(FloatingFadeNotification(2f));
        }
    }
    private IEnumerator FloatingFadeNotification(float duration)
    {
        RectTransform textTransform = notificationText.GetComponent<RectTransform>();
        Color originalColor = notificationText.color;
        Vector3 startPosition = textTransform.anchoredPosition;
        float elapsedTime = 0f;
        float floatDistance = 50f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            float newY = startPosition.y + floatDistance * progress;
            textTransform.anchoredPosition = new Vector2(startPosition.x, newY);

            notificationText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f - progress);

            yield return null;
        }

        notificationText.color = originalColor;
        textTransform.anchoredPosition = startPosition;
        notificationText.gameObject.SetActive(false);
    }
    private IEnumerator MeleeAttackSequence(AttackScript attackScript, GameObject target)
    {
        FighterStats heroStats = hero.GetComponent<FighterStats>();
        if (!CanUseSkill(attackScript, heroStats))
        {
            Debug.Log("Not enough magic!");
            yield break;
        }
        Vector3 targetPosition = target.transform.position;
        float distanceToTarget = Vector3.Distance(hero.transform.position, targetPosition);

        Vector3 direction = (targetPosition - hero.transform.position).normalized;
        Vector3 meleePosition = new Vector3(
            targetPosition.x - direction.x * meleeDistance,
            hero.transform.position.y,
            targetPosition.z - direction.z * meleeDistance
        );

        hero.transform.position = meleePosition;

        attackScript.Attack(target);
        yield return new WaitForSeconds(1f);

    }
    private bool CanUseSkill(AttackScript attackScript, FighterStats heroStats)
    {
        if (attackScript == null) return true;

        float currentMagic = heroStats.magic;
        float requiredMagic = attackScript.magicCost;

        return currentMagic >= requiredMagic;
    }
    private IEnumerator RunSequence()
    {
        FighterStats heroStats = hero.GetComponent<FighterStats>();
        AttackScript attackScript = null;
        if (!CanUseSkill(attackScript, heroStats))
        {
            ShowNotification("Not Enough Magic!");
            yield break;
        }

        Vector3 runPosition = originalPosition + Vector3.back * 5f;
        while (Vector3.Distance(hero.transform.position, runPosition) > 0.1f)
        {
            hero.transform.position = Vector3.MoveTowards(hero.transform.position, runPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
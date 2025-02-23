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
        if (tag == "Hero")
        {
            victim = enemy;
        }

        if (btn.CompareTo("melee") == 0)
        {
            AttackScript attackScript = rangePrefab.GetComponent<AttackScript>();
            FighterStats heroStats = hero.GetComponent<FighterStats>();
            if (heroStats.magic < attackScript.magicCost)
            {
                ShowNotification("Not Enough Magic!");
                return;
            }            
            StartCoroutine(MeleeAttackSequence(meleePrefab.GetComponent<AttackScript>(), victim));

        }
        else if (btn.CompareTo("range") == 0)
        {
            AttackScript attackScript = rangePrefab.GetComponent<AttackScript>();
            FighterStats heroStats = hero.GetComponent<FighterStats>();
            
            if (heroStats.magic < attackScript.magicCost)
            {
                ShowNotification("Not Enough Magic!");
                return;
            }
            rangePrefab.GetComponent<AttackScript>().Attack(victim);
        }
        else
        {
            Debug.Log("Run");
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

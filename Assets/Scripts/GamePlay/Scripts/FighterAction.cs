using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Gameplay.Scripts
{
    using Core.Scripts;

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
        private Vector3 originalPosition;
        public float moveSpeed = 5f;
        public float meleeDistance = 1.5f;

        // Object Pooling
        private Queue<GameObject> meleePool = new Queue<GameObject>();
        private Queue<GameObject> rangePool = new Queue<GameObject>();
        private const int INITIAL_POOL_SIZE = 5;

        public void Awake()
        {
            hero = GameObject.FindGameObjectWithTag("Hero");
            enemy = GameObject.FindGameObjectWithTag("Enemy");
            originalPosition = hero.transform.position;

            if (hero == null || enemy == null)
            {
                Debug.LogError("Hero or Enemy not found in the scene!");
            }

            InitializeObjectPool();
        }

        private void InitializeObjectPool()
        {
            for (int i = 0; i < INITIAL_POOL_SIZE; i++)
            {
                if (meleePrefab != null)
                {
                    GameObject melee = Instantiate(meleePrefab);
                    melee.SetActive(false);
                    meleePool.Enqueue(melee);
                }
                if (rangePrefab != null)
                {
                    GameObject range = Instantiate(rangePrefab);
                    range.SetActive(false);
                    rangePool.Enqueue(range);
                }
            }
        }

        public void Start()
        {
            if (notificationText != null)
            {
                initialTextPosition = notificationText.GetComponent<RectTransform>().anchoredPosition;
                notificationText.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Selects and executes an attack or action based on the button pressed.
        /// </summary>
        /// <param name="btn">The action type: "melee", "range", or "run".</param>
        public void SelectAttack(string btn)
        {
            GameObject victim = (tag == "Hero") ? enemy : hero;
            FighterStats heroStats = hero?.GetComponent<FighterStats>();
            if (heroStats == null)
            {
                Debug.LogError("FighterStats missing on Hero!");
                return;
            }

            AttackScript attackScript = null;
            if (btn == "melee")
            {
                attackScript = GetPooledAttack(meleePool, meleePrefab)?.GetComponent<AttackScript>();
            }
            else if (btn == "range")
            {
                attackScript = GetPooledAttack(rangePool, rangePrefab)?.GetComponent<AttackScript>();
            }

            if (attackScript != null && !heroStats.CanUseSkill(attackScript.magicCost))
            {
                ShowNotification("Not Enough Magic!");
                return;
            }

            if (btn == "melee")
            {
                StartCoroutine(MeleeAttackSequence(attackScript, victim));
            }
            else if (btn == "range")
            {
                attackScript.Attack(victim);
            }
            else if (btn == "run")
            {
                StartCoroutine(RunSequence());
            }
        }

        private GameObject GetPooledAttack(Queue<GameObject> pool, GameObject prefab)
        {
            if (pool.Count == 0 && prefab != null)
            {
                GameObject newAttack = Instantiate(prefab);
                pool.Enqueue(newAttack);
            }

            GameObject attack = pool.Dequeue();
            attack.SetActive(true);
            attack.transform.position = transform.position; 
            StartCoroutine(ReturnToPool(attack, pool, 2f)); 
            return attack;
        }

        private IEnumerator ReturnToPool(GameObject obj, Queue<GameObject> pool, float delay)
        {
            yield return new WaitForSeconds(delay);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }

        /// <summary>
        /// Displays a floating notification with fade effect.
        /// </summary>
        /// <param name="message">The message to display.</param>
        private void ShowNotification(string message)
        {
            if (notificationText == null) return;

            if (activeNotificationCoroutine != null)
            {
                StopCoroutine(activeNotificationCoroutine);
                notificationText.gameObject.SetActive(false);
            }

            RectTransform textTransform = notificationText.GetComponent<RectTransform>();
            textTransform.anchoredPosition = initialTextPosition;
            notificationText.text = message;
            notificationText.color = Color.white;
            notificationText.gameObject.SetActive(true);

            activeNotificationCoroutine = StartCoroutine(FloatingFadeNotification(2f));
        }

        private IEnumerator FloatingFadeNotification(float duration)
        {
            RectTransform textTransform = notificationText.GetComponent<RectTransform>();
            Vector3 startPosition = textTransform.anchoredPosition;
            float elapsedTime = 0f;
            float floatDistance = 50f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / duration;
                textTransform.anchoredPosition = new Vector2(startPosition.x, startPosition.y + floatDistance * progress);
                notificationText.color = new Color(1f, 1f, 1f, 1f - progress);
                yield return null;
            }

            notificationText.gameObject.SetActive(false);
        }

        private IEnumerator MeleeAttackSequence(AttackScript attackScript, GameObject target)
        {
            yield return CombatUtils.MoveToMeleePosition(hero.transform, target.transform, meleeDistance);
            attackScript.Attack(target);
            yield return new WaitForSeconds(1f);
        }

        private IEnumerator RunSequence()
        {
            Vector3 runPosition = originalPosition + Vector3.back * 5f;
            while (Vector3.Distance(hero.transform.position, runPosition) > 0.1f)
            {
                hero.transform.position = Vector3.MoveTowards(hero.transform.position, runPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }

    public static class CombatUtils
    {
        public static IEnumerator MoveToMeleePosition(Transform attacker, Transform target, float meleeDistance)
        {
            Vector3 direction = (target.position - attacker.position).normalized;
            Vector3 meleePosition = target.position - direction * meleeDistance;
            attacker.position = new Vector3(meleePosition.x, attacker.position.y, meleePosition.z);
            yield return null;
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.UI;

public class FighterStats : MonoBehaviour, IComparable
{
    [SerializeField] 
    private Animator animator;
    [SerializeField] 
    private GameObject healthFill;
    [SerializeField] 
    private GameObject magicFill;
    [SerializeField] 
    private Text healthText; 
    [SerializeField] 
    private Text magicText;

    [Header("Stats")]
    private bool dead = false;
    public float health;
    public float magicRange;
    public float melee;
    public float magic;
    public float defense;
    public float range;
    public float speed;
    public float experience;

    private float startHealth;
    private float startMagic;
    [HideInInspector] public int nextActTurn;


    private Transform healthTransform;
    private Transform magicTransform;
    private Vector2 healthScale;
    private Vector2 magicScale;
    private float xNewHealthScale;
    private float xNewMagicScale;

    private void Start()
    {
        healthTransform = healthFill.GetComponent<RectTransform>();
        healthScale = healthFill.transform.localScale;
        magicTransform = magicFill.GetComponent<RectTransform>();
        magicScale = magicFill.transform.localScale;
        startHealth = health;
        startMagic = magic;
        UpdateHealth_MagicUI();
    }

    public void ReceiveDamage(float damage)
    {
        health -= damage;
        animator.Play("Damage");

        if (health <= 0)
        {
            dead = true;
            gameObject.tag = "Dead";
            Destroy(healthFill);
            Destroy(gameObject);
        }
        else
        {
            xNewHealthScale = healthScale.x * (health / startHealth);
            healthFill.transform.localScale = new Vector2(xNewHealthScale, healthScale.y);
            UpdateHealth_MagicUI();
        }
    }

    public void CalculateNextTurn(int currentTurn)
    {
        nextActTurn = currentTurn + Mathf.CeilToInt(100f / speed);
    }

    public void UpdateMagicFill(float cost)
    {
        magic -= cost;
        xNewMagicScale = magicScale.x * (magic / startMagic);
        magicFill.transform.localScale = new Vector2(xNewMagicScale, magicScale.y);
        UpdateHealth_MagicUI();
    }

    private void UpdateHealth_MagicUI()
    {
        if (healthText != null)
        {
            healthText.text = $"{health:F0}/{startHealth:F0}";
        }
        if (magicText != null)
        {
            magicText.text = $"{magic:F0}/{startMagic:F0}";
        }
    }

    public bool GetDead()
    {
        return dead;
    }

    public int CompareTo(object otherStats)
    {
        return nextActTurn.CompareTo(((FighterStats)otherStats).nextActTurn);
    }
}
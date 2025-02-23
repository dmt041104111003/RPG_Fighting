using System;
using UnityEngine;
using UnityEngine.UI;

public class FighterStats : MonoBehaviour, IComparable
{
    [SerializeField]
    protected Animator animator;
    [SerializeField]
    protected GameObject healthFill;
    [SerializeField]
    protected GameObject magicFill;
    [SerializeField]
    protected Text healthText;
    [SerializeField]
    protected Text magicText;

    [Header("Stats")]
    protected bool dead = false;
    public float health;
    public float magicRange;
    public float melee;
    public float magic;
    public float defense;
    public float range;
    public float speed;
    public float experience;

    protected float startHealth;
    protected float startMagic;
    [HideInInspector] public int nextActTurn;

    protected Transform healthTransform;
    protected Transform magicTransform;
    protected Vector2 healthScale;
    protected Vector2 magicScale;
    protected float xNewHealthScale;
    protected float xNewMagicScale;

    protected virtual void Start()
    {
        healthTransform = healthFill.GetComponent<RectTransform>();
        healthScale = healthFill.transform.localScale;
        magicTransform = magicFill.GetComponent<RectTransform>();
        magicScale = magicFill.transform.localScale;
        startHealth = health;
        startMagic = magic;
        UpdateUI();
    }

    public virtual void ReceiveDamage(float damage)
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
            UpdateUI();
        }
    }

    public void CalculateNextTurn(int currentTurn)
    {
        nextActTurn = currentTurn + Mathf.CeilToInt(100f / speed);
    }

    public virtual void UpdateMagicFill(float cost)
    {
        magic -= cost;
        xNewMagicScale = magicScale.x * (magic / startMagic);
        magicFill.transform.localScale = new Vector2(xNewMagicScale, magicScale.y);
        UpdateUI();
    }

    protected virtual void UpdateUI()
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
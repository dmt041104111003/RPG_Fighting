using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using Core.Scripts;
namespace Characters.Scripts
{
    
    public class WizardHeroStats : FighterStats
    {
        [SerializeField] 
        private Image healthFillImage;

        private float flashTimer;
        private bool isFlashing;
        private const float FLASH_SPEED = 6f; 
        private const float FLASH_ALPHA = 0.7f;

        protected override void Start()
        {
            base.Start();
        }

        private void FixedUpdate() 
        {
            if (!dead && healthFillImage != null)
            {
                float healthPercentage = health / startHealth;
                if (healthPercentage <= 0.9f)
                {
                    isFlashing = true;
                    UpdateHealthFlash();
                }
                else if (isFlashing)
                {
                    isFlashing = false;
                    ResetHealthColor();
                }
            }
        }

        public override void ReceiveDamage(float damage)
        {
            base.ReceiveDamage(damage);
        }

        public override void UpdateMagicFill(float cost)
        {
            base.UpdateMagicFill(cost);
        }

        private void UpdateHealthFlash()
        {
            if (healthFillImage != null)
            {
                flashTimer += Time.fixedDeltaTime;
                float alpha = Mathf.Abs(Mathf.Sin(flashTimer * FLASH_SPEED));
                Color flashColor = new Color(1f, 0f, 0f, alpha * FLASH_ALPHA);
                healthFillImage.color = Color.Lerp(healthFillImage.color, flashColor, Time.fixedDeltaTime * 20f); 
            }
        }

        private void ResetHealthColor()
        {
            if (healthFillImage != null)
            {
                healthFillImage.color = Color.white;
            }
        }
    }
}

using Core.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Characters.Scripts
{
    public class WizardHeroStats : FighterStats
    {
        [SerializeField]
        private Image healthFillImage;

        private float flashTimer;
        private bool isFlashing;
        private const float HEALTH_FLASH_SPEED = 6f;
        private const float HEALTH_FLASH_ALPHA = 0.7f;
        private const float HEALTH_FLASH_THRESHOLD = 0.9f;

        protected override void Start()
        {
            base.Start();
            if (healthFillImage == null)
            {
                Debug.LogError($"{nameof(healthFillImage)} is not assigned in the Inspector for {gameObject.name}");
            }
        }

        private void FixedUpdate()
        {
            if (!dead && healthFillImage != null)
            {
                float healthPercentage = health / startHealth;
                if (healthPercentage <= HEALTH_FLASH_THRESHOLD)
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

        /// <summary>
        /// Updates the health bar flashing effect when health drops below the threshold.
        /// </summary>
        private void UpdateHealthFlash()
        {
            if (healthFillImage != null)
            {
                flashTimer += Time.fixedDeltaTime;
                float alpha = Mathf.Abs(Mathf.Sin(flashTimer * HEALTH_FLASH_SPEED));
                Color flashColor = new Color(1f, 0f, 0f, alpha * HEALTH_FLASH_ALPHA);
                healthFillImage.color = Color.Lerp(healthFillImage.color, flashColor, Time.fixedDeltaTime * 20f);
            }
        }

        /// <summary>
        /// Resets the health bar color to white when health is above the flashing threshold.
        /// </summary>
        private void ResetHealthColor()
        {
            if (healthFillImage != null)
            {
                healthFillImage.color = Color.white;
            }
        }
    }
}
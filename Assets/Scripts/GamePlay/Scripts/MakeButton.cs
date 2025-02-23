using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Scripts
{
    public class MakeButton : MonoBehaviour
    {
        [SerializeField] 
        private bool physical;
        private GameObject hero;

        private enum ButtonType { Melee, Range, Run }

        void Start()
        {
            hero = GameObject.FindGameObjectWithTag("Hero");
            string buttonName = gameObject.name;
            gameObject.GetComponent<Button>().onClick.AddListener(() => AttachCallback(buttonName));
        }

        private void AttachCallback(string btn)
        {
            ButtonType type = btn switch
            {
                "MeleeBtn" => ButtonType.Melee,
                "RangeBtn" => ButtonType.Range,
                _ => ButtonType.Run
            };
            hero.GetComponent<FighterAction>().SelectAttack(type.ToString().ToLower());
        }
    }
}
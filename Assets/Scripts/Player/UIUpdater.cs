using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class UIUpdater : MonoBehaviour
    {
        [SerializeField] Text moneyText;
        [SerializeField] Text woodText;
        [SerializeField] Text stoneText;
        [SerializeField] Image healthBar;

        void Start()
        {
            moneyText.text = "" + PlayerData.money;
            woodText.text = "" + PlayerData.wood;
        }

        void Update()
        {
            moneyText.text = "" + PlayerData.money;
            woodText.text = "" + PlayerData.wood;
            healthBar.fillAmount = PlayerData.hp / (float) PlayerData.MaxHp;
        }
    }
}

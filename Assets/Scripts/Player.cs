using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    public string className;

    public int health;
    public int maxHealth;

    public TMP_Text healthText;
    public Slider healthBar;
    public int attackStat;
    public int defenceStat;

    public string skillName;
    public int skillValue;
    public int skillRoll;

    public string infoText;

    public enum SkillTypes
    {
        TrueDamage,
        LifeSteal,
        HugeHeal,
        HalfHealth,
        
    }

    public SkillTypes currentSkill;

    private void Start()
    {
        health = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = health;
        healthText.text = health + " / " + maxHealth;
    }

    public void UpdateUI()
    {
        healthText.text = (health + " / " + maxHealth);
        healthBar.value = health;
    }

    
}

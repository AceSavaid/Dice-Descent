using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public TMP_Text healthText;
    public Slider healthBar;
    public int health;
    public int maxHealth = 100;
    public int attack = 0;
    public int defense = 0;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = health;
        healthText.text = health + " / " + maxHealth;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHealthBar()
    {
        healthBar.value = health;
        healthText.text = health + " / " + maxHealth;
    }

}

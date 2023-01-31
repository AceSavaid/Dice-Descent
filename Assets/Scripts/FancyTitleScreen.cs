using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FancyTitleScreen : MonoBehaviour
{
    Dice dice;
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        dice = FindObjectOfType<Dice>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 2)
        {
            dice.RollDice();
            timer = 0;
        }
    }
}

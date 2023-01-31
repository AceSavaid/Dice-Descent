using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    int diceNum;
    public Image dice;
    public List<Sprite> diceImages = new List<Sprite>();
    public AudioClip diceRollSound;
    
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int RollDice()
    {
        PlaySoundEffect(diceRollSound);
        diceNum =Random.Range(1, 7);
        UpdateDice();
        return diceNum;
    }

    void UpdateDice()
    {
        dice.sprite = diceImages[diceNum - 1];
    }

    void PlaySoundEffect(AudioClip audioClip)
    {
        if (audioClip)
        {
            AudioSource.PlayClipAtPoint(audioClip, new Vector3(0, 0, 0));
        }
    }
}

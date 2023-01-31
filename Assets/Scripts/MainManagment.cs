using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainManagment : MonoBehaviour
{
    Dice _die;

    int floor = 1;
    int stage = 0;

    int maxStage;

    [Header("Buttons")]
    public Button fightButton;
    public Button skillButton;
    public Button itemButton;

    [Header("Player")]
    bool playerTurn = true;
    bool enemyTurn = false;

    public Transform playerPosition;
    GameObject player;
    public List<GameObject> playerClasses = new List<GameObject>();

    [Header("Enemy")]
    public Transform enemyTransform;
    GameObject currentEnemy;
    public List<GameObject> enemies = new List<GameObject>();

    [Header("Item")]
    public List<GameObject> itemTypes = new List<GameObject>();

    [Header("Floors")]
    public GameObject currentFloorType;
    public List<GameObject> floorTypes = new List<GameObject>();

    [Header("Level Text")]
    public TMP_Text infoText;
    public TMP_Text floorProgressionText;
    public TMP_Text stageProgressionText;

    [Header("Game Over")]
    public GameObject gameOverScreen;

    [Header("Audio")]
    public AudioClip attackButtonPress;
    public AudioClip itemButtonPress;
    public AudioClip playerAttackSound;
    public AudioClip enemyAttckSound;
    public AudioClip nextStageSound;
    public AudioClip nextFloorSound;
    public AudioClip GameOverSound;
    public AudioClip buttonDeclined;

    // Start is called before the first frame update
    void Start()
    {
        _die = FindObjectOfType<Dice>();
        fightButton.onClick.AddListener(Fight);
        skillButton.onClick.AddListener(Skill);
        itemButton.onClick.AddListener(Item);

        int choice = _die.RollDice() - 1;
        GameObject pastFloor = currentFloorType;
        currentFloorType = floorTypes[choice];
        pastFloor.gameObject.SetActive(false);
        currentFloorType.gameObject.SetActive(true);


        SelectClass();
        maxStage = 4 + _die.RollDice();

        //pregress stage to first stage
        stage++;
        playerTurn = true;
        int newEnemy = _die.RollDice() - 1;
        currentEnemy = enemies[newEnemy];
        currentEnemy = Instantiate(currentEnemy, enemyTransform);
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerTurn && enemyTurn)
        {
            EnemyAttack();
        }
        UpdateUi();
    }

    void SelectClass()
    {
        player = playerClasses[_die.RollDice() - 1];
        player = Instantiate(player, playerPosition);
        StatusWindow("Player's class this run is " + player.GetComponent<Player>().className + "\n Atk:" + player.GetComponent<Player>().attackStat + " Def: " + player.GetComponent<Player>().defenceStat + ". \n" + player.GetComponent<Player>().infoText);
    }

    void UpdateUi()
    {
        floorProgressionText.text = "Floor: " + floor;
        stageProgressionText.text = "Stage: " + stage + " / " + maxStage;

        player.GetComponent<Player>().UpdateUI();
        currentEnemy.GetComponent<Enemy>().UpdateHealthBar();
    }

    void Fight()
    {
        if (playerTurn)
        {
            int damage = _die.RollDice();
            int actualdamage = (int)(damage + player.GetComponent<Player>().attackStat - currentEnemy.GetComponent<Enemy>().defense);
            if (actualdamage < 0)
            {
                actualdamage = 0;
            }

            currentEnemy.GetComponent<Enemy>().health -= actualdamage;
            StatusWindow("Player attacked " + currentEnemy.GetComponent<Enemy>().enemyName + " for " + actualdamage + " damage.");

            PlaySoundEffect(playerAttackSound);
            StartCoroutine(HurtEffect(currentEnemy));


            if (currentEnemy.GetComponent<Enemy>().health <= 0)
            {
                StatusWindow(currentEnemy.GetComponent<Enemy>().enemyName + " defeated.");
                StartCoroutine(PauseForEffect());
                ProgressStage();
            }
            else
            {
                playerTurn = false;

                StartCoroutine(PauseForEnemy());
            }


        }

    }

    void Skill()
    {
        if (playerTurn)
        {
            int roll = _die.RollDice();
            switch (player.GetComponent<Player>().currentSkill)
            {
                case Player.SkillTypes.TrueDamage:

                    if (roll >= player.GetComponent<Player>().skillRoll)
                    {
                        
                        currentEnemy.GetComponent<Enemy>().health -= player.GetComponent<Player>().skillValue;
                        StatusWindow("Player succesfully used " + player.GetComponent<Player>().skillName + ", \n" + currentEnemy.GetComponent<Enemy>().enemyName + " took " + player.GetComponent<Player>().skillValue + " damage.");
                        StartCoroutine(HurtEffect(currentEnemy));

                        if (currentEnemy.GetComponent<Enemy>().health <= 0)
                        {
                            StatusWindow(currentEnemy.GetComponent<Enemy>().enemyName + " defeated.");
                            StartCoroutine(PauseForEffect());
                            ProgressStage();
                        }
                        else
                        {
                            playerTurn = false;

                            StartCoroutine(PauseForEnemy());
                        }
                    }
                    else
                    {
                        StatusWindow("Player did not roll " + player.GetComponent<Player>().skillRoll + " or higher. Skill unsuccessful.");

                        playerTurn = false;
                        StartCoroutine(PauseForEnemy());
                    }
                    break;

                case Player.SkillTypes.LifeSteal:
                    if (roll >= player.GetComponent<Player>().skillRoll)
                    {
                        StartCoroutine(HurtEffect(currentEnemy));
                        currentEnemy.GetComponent<Enemy>().health -= player.GetComponent<Player>().skillValue;

                        
                        player.GetComponent<Player>().health += player.GetComponent<Player>().skillValue;
                        if (player.GetComponent<Player>().health > player.GetComponent<Player>().maxHealth)
                        {
                            player.GetComponent<Player>().health = player.GetComponent<Player>().maxHealth;
                        }

                        StatusWindow("Player succesfully used " + player.GetComponent<Player>().skillName + ", \n" + currentEnemy.GetComponent<Enemy>().enemyName + " took " + player.GetComponent<Player>().skillValue + " damage.  Player healed " + player.GetComponent<Player>().skillValue + " health.");
                        StartCoroutine(HealEffect(player));

                        if (currentEnemy.GetComponent<Enemy>().health <= 0)
                        {
                            StatusWindow(currentEnemy.GetComponent<Enemy>().enemyName + " defeated.");
                            StartCoroutine(PauseForEffect());
                            ProgressStage();
                        }
                        else
                        {
                            playerTurn = false;

                            StartCoroutine(PauseForEnemy());
                        }
                    }
                    else
                    {
                        StatusWindow("Player did not roll " + player.GetComponent<Player>().skillRoll + " or higher. Skill unsuccessful.");

                        playerTurn = false;
                        StartCoroutine(PauseForEnemy());
                    }
                    break;

                case Player.SkillTypes.HugeHeal:
                    if (roll >= player.GetComponent<Player>().skillRoll)
                    {

                        player.GetComponent<Player>().health += player.GetComponent<Player>().skillValue;

                        if (player.GetComponent<Player>().health > player.GetComponent<Player>().maxHealth)
                        {
                            player.GetComponent<Player>().health = player.GetComponent<Player>().maxHealth;
                        }

                        PlaySoundEffect(itemButtonPress);
                        StatusWindow("Player used " + player.GetComponent<Player>().skillName);
                        StartCoroutine(HealEffect(player));
                        playerTurn = false;

                        StartCoroutine(PauseForEnemy());
                    }
                    else
                    {
                        StatusWindow("Player did not roll higher than " + player.GetComponent<Player>().skillRoll + ". Skill unsuccessful.");

                        playerTurn = false;
                        StartCoroutine(PauseForEnemy());
                    }

                    break;

                case Player.SkillTypes.HalfHealth:
                    if (roll >= player.GetComponent<Player>().skillRoll)
                    {
                        currentEnemy.GetComponent<Enemy>().health -= (int)(currentEnemy.GetComponent<Enemy>().health / 2);
                        StatusWindow("Player succesfully used " + player.GetComponent<Player>().skillName + ", \n" + currentEnemy.GetComponent<Enemy>().enemyName + " took " + (int)(currentEnemy.GetComponent<Enemy>().health) + " damage.");

                        if (currentEnemy.GetComponent<Enemy>().health <= 0)
                        {
                            StatusWindow(currentEnemy.GetComponent<Enemy>().enemyName + " defeated.");
                            StartCoroutine(PauseForEffect());
                            ProgressStage();
                        }
                        else
                        {
                            playerTurn = false;

                            StartCoroutine(PauseForEnemy());
                        }
                    }
                    else
                    {
                        StatusWindow("Player did not roll " + player.GetComponent<Player>().skillRoll + " or higher. Skill unsuccessful.");

                        playerTurn = false;
                        StartCoroutine(PauseForEnemy());
                    }
                    break;

                default:
                    if (roll >= player.GetComponent<Player>().skillRoll)
                    {
                        currentEnemy.GetComponent<Enemy>().health -= player.GetComponent<Player>().skillValue;
                        StatusWindow("Player succesfully used " + player.GetComponent<Player>().skillName + ", \n" + currentEnemy.GetComponent<Enemy>().enemyName + " took " + player.GetComponent<Player>().skillValue + " damage.");

                        if (currentEnemy.GetComponent<Enemy>().health <= 0)
                        {
                            StatusWindow(currentEnemy.GetComponent<Enemy>().enemyName + " defeated.");
                            StartCoroutine(PauseForEffect());
                            ProgressStage();
                        }
                        else
                        {
                            playerTurn = false;

                            StartCoroutine(PauseForEnemy());
                        }
                    }
                    else
                    {
                        StatusWindow("Player did not roll higher than " + player.GetComponent<Player>().skillRoll + ". Skill unsuccessful.");

                        playerTurn = false;
                        StartCoroutine(PauseForEnemy());
                    }
                    break;
            }

        }
    }

    void Item()
    {
        if (playerTurn)
        {
            int itemtype = _die.RollDice() - 1;
            int itemhealth = itemTypes[itemtype].GetComponent<ItemBase>().UseItem();
            player.GetComponent<Player>().health += itemhealth;

            if (player.GetComponent<Player>().health > player.GetComponent<Player>().maxHealth)
            {
                player.GetComponent<Player>().health = player.GetComponent<Player>().maxHealth;
            }

            PlaySoundEffect(itemButtonPress);
            StatusWindow("Player used item, healing " + itemhealth + " health.");
            StartCoroutine(HealEffect(player));
            playerTurn = false;

            StartCoroutine(PauseForEnemy());
        }

    }

    void EnemyAttack()
    {
        int damage = _die.RollDice();
        int actualdamage = (int)(damage + currentEnemy.GetComponent<Enemy>().attack - player.GetComponent<Player>().defenceStat);
        if (actualdamage < 0)
        {
            actualdamage = 0;
        }
        player.GetComponent<Player>().health -= actualdamage;
        PlaySoundEffect(enemyAttckSound);
        StatusWindow(currentEnemy.GetComponent<Enemy>().enemyName + " attacked player for " + actualdamage + " damage.");
        StartCoroutine(HurtEffect(player));

        if (player.GetComponent<Player>().health <= 0)
        {
            player.GetComponent<Player>().health = 0;
            StatusWindow("Player died.");
            GameOver();
        }
        enemyTurn = false;
        playerTurn = true;
    }

    void ProgressStage()
    {
        stage++;
        playerTurn = true;
        enemyTurn = false;
        if (stage <= maxStage)
        {
            PlaySoundEffect(nextStageSound);
            GameObject oldEnemy = currentEnemy;
            int newEnemy = _die.RollDice() - 1;
            currentEnemy = enemies[newEnemy];
            currentEnemy = Instantiate(currentEnemy, enemyTransform);
            Destroy(oldEnemy);
            StatusWindow(currentEnemy.GetComponent<Enemy>().enemyName + " appeared. \n Scanning... \n Atk: " + currentEnemy.GetComponent<Enemy>().attack + " Def: " + currentEnemy.GetComponent<Enemy>().defense + " .");

        }
        else
        {
            NewFloor();
        }
    }

    void NewFloor()
    {
        int choice = _die.RollDice() - 1;
        GameObject pastFloor = currentFloorType;
        currentFloorType = floorTypes[choice];
        pastFloor.gameObject.SetActive(false);
        currentFloorType.gameObject.SetActive(true);


        floor++;
        StatusWindow("Player decendeed to next floor.");
        stage = 0;
        PlaySoundEffect(nextFloorSound);

        StartCoroutine(PauseForEffect());

        ProgressStage();
    }

    void GameOver()
    {
        playerTurn = false;
        enemyTurn = false;
        gameOverScreen.SetActive(true);
    }

    IEnumerator HurtEffect(GameObject entity)
    {
        entity.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(.25f);
        entity.GetComponent<SpriteRenderer>().color = Color.white;

    }

    IEnumerator HealEffect(GameObject entity)
    {
        entity.GetComponent<SpriteRenderer>().color = Color.green;
        yield return new WaitForSeconds(.25f);
        entity.GetComponent<SpriteRenderer>().color = Color.white;

    }

    void StatusWindow(string text)
    {
        infoText.text = text;
    }

    void PlaySoundEffect(AudioClip audioClip)
    {
        if (audioClip)
        {
            AudioSource.PlayClipAtPoint(audioClip, new Vector3(0, 0, 0));
        }
    }

    IEnumerator PauseForEnemy()
    {
        Debug.Log("Pause Start");
        yield return new WaitForSeconds(1.25f);
        enemyTurn = true;
        Debug.Log("Pause Stop");
    }

    IEnumerator PauseForEffect()
    {
        yield return new WaitForSeconds(1);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Game_Controller : MonoBehaviour
{
    [Header("Game References")]
    private AudioSource gameAudioSource;
    public GameObject playerObject;
    public Attack_Controller playerScript;
    public Animator playerModelAnim;
    public GameObject playerModelMesh;
    public GameObject enemyObject;
    private Enemy_Attack_Controller enemyScript;
    public Enemy_Cosmetic_Controller enemyCosmeticScript;
    public Animator enemyModelAnim;
    public GameObject enemyModelMesh;
    public List<string> enemyAdjectiveList, enemyNameList;
    [HideInInspector] public List<string> _enemyNormalNameList, _enemySadNameList, _enemyStrongNameList, _enemyCrazyNameList;

    [Header("Camera Refereneces")]
    private Camera cam;
    private Animator canim;

    [Header("Dolly References")]
    public Dolly_Controller dollyScript;

    [Header("Fog Controls")]
    public List<Color32> fogColorList;

    [Header("UI References")]
    public Slider playerHealthSlider;
    public Slider enemyHealthSlider;
    public TextMeshProUGUI enemyName;
    public Slider playerCooldownSlider, playerStunSlider;
    public Button lightButton, heavyButton, counterBlockButton, specialButton;
    public GameObject buttonEffectRipple;
    public TextMeshProUGUI playerHealthNumber;
    public Animator playerHealthNumberAnim;
    public TextMeshProUGUI playerHealthNumberRegen;
    public Animator playerHealthNumberRegenAnim;

    [Header("Menu References")]
    public GameObject gameUIGroup;
    public Animator gameUIGroupAnimator;
    public GameObject tutorialUIGroup;
    public GameObject battleIntroUIGroup;
    private Animator battleIntroUIAnimator;
    public GameObject winScreenUIGroup, loseScreenUIGroup, upgradeUIGroup;
    public Upgrade_Screen_Controller upgradeScript;
    public TextMeshProUGUI battleIntroUIName, loseScreenDescription, battleRoundNumber, battleRoundNumberShadow;
    public Button menuStartButton;

    [Header("Model References")]
    public GameObject model_wireframeGroup;
    public Wireframe_Scroll wireScript;
    public GameObject model_ground;
    public GameObject model_playerGroup, model_enemyGroup;
    public GameObject battleCirclePlayer, battleCircleEnemy;
    public GameObject xpugLogoObject;
    public List<GameObject> binaryLineObjectList;
    public List<GameObject> _tempBinaryLineObjectList;

    [Header("Gameplay Modifiers")]
    public float playerControlRoundStartDelay;
    public bool playerInputBeingDelayed;

    [Header("Game Controls")]
    public bool startGame;
    public int enemysDefeated, coinsCollected, playerMaxHealth, playerMaxExpandHealthBase, enemysDefeatedNeededToUpgrade;
    public bool playerMaxUpgraded;
    private int prevEnemysDefeated;

    [Header("Audio References")]
    public AudioClip battleIntroSound;
    public List<int> normalList = new List<int>();
    public List<int> sadList = new List<int>();
    public List<int> strongList = new List<int>();
    public List<int> crazyList = new List<int>();

    private void Start()
    {
        playerScript = playerObject.GetComponent<Attack_Controller>();
        enemyScript = enemyObject.GetComponent<Enemy_Attack_Controller>();

        battleIntroUIAnimator = battleIntroUIGroup.GetComponent<Animator>();

        // Update Sliders to be accurate
        playerHealthSlider.maxValue = playerMaxHealth;
        playerHealthSlider.value = playerMaxHealth;
        playerHealthNumber.text = playerMaxHealth + "";

        enemyHealthSlider.maxValue = enemyScript.health;
        enemyHealthSlider.value = enemyHealthSlider.maxValue;

        // Get camera
        cam = Camera.main;
        canim = cam.GetComponent<Animator>();

        // Get audio source
        gameAudioSource = GetComponent<AudioSource>();

        // Hide enemy
        ToggleEnemyMesh(false);

        // Get enemy names
        _enemySadNameList = enemyScript.sadNameList;
        _enemyStrongNameList = enemyScript.strongNameList;
        _enemyCrazyNameList = enemyScript.crazyNameList;
        GenerateNormalNameList();

        // Set player health
        playerScript.health = playerMaxHealth;

        wireScript.scrollSpeedModifier = 1.0f;
        ChangeFogColor(0);

        _tempBinaryLineObjectList = binaryLineObjectList;
    }

    private void GenerateNormalNameList()
    {
        foreach(string name in enemyAdjectiveList)
        {
            Debug.Log("NAME IS: " + name);
            if ((!_enemySadNameList.Contains(name) && (!_enemyStrongNameList.Contains(name)) && (!_enemyCrazyNameList.Contains(name))))
            {
                Debug.Log("NAME: " + name + " ACCEPTED!");
                _enemyNormalNameList.Add(name);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GenerateBattleIntroName();
            //enemyScript.DoAttackInput(attackTypes.Heavy, playerScript);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            string genName = "Cute Makayla";
            battleIntroUIName.text = genName;
        }

        // Battle keys
        if (Input.GetKeyDown(KeyCode.Alpha1) || (Input.GetKeyDown(KeyCode.Keypad1)))
        {
            Player_LightAttack(lightButton);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || (Input.GetKeyDown(KeyCode.Keypad2)))
        {
            Player_HeavyAttack(heavyButton);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || (Input.GetKeyDown(KeyCode.Keypad3)))
        {
            Player_CounterBlock(counterBlockButton);
        }
    }

    public void Player_LightAttack(Button thisButton)
    {
        if (thisButton.interactable && !playerInputBeingDelayed)
        {
            ; playerScript.DoAttackInput(attackTypes.Light, enemyScript);
        }
    }

    public void Player_HeavyAttack(Button thisButton)
    {
        if (thisButton.interactable && !playerInputBeingDelayed)
        {
            playerScript.DoAttackInput(attackTypes.Heavy, enemyScript);
        }
    }

    public void Player_CounterBlock(Button thisButton)
    {
        if (thisButton.interactable && !playerInputBeingDelayed)
        {
            playerScript.DoAttackInput(attackTypes.Counter, enemyScript);
        }
    }

    public void Enemy_LightAttack()
    {
        enemyScript.DoAttackInput(attackTypes.Light, playerScript);
    }

    public void Enemy_HeavyAttack()
    {
        enemyScript.DoAttackInput(attackTypes.Heavy, playerScript);
    }

    public void Enemy_CounterBlock()
    {
        enemyScript.DoAttackInput(attackTypes.Counter, playerScript);
    }

    public void UpdateHealthSlider(entityType type, float amountToUpdate, float resetAllHealth = 0)
    {
        if (type == entityType.Player)
        {
            if (resetAllHealth == 0)
            {
                playerHealthSlider.value -= amountToUpdate;
                if (playerHealthSlider.value <= 0)
                {
                    playerHealthSlider.fillRect.GetComponent<Image>().gameObject.SetActive(false);
                }
            }
            else
            {
                playerHealthSlider.value = resetAllHealth;
                playerHealthSlider.fillRect.GetComponent<Image>().gameObject.SetActive(true);
            }

            // sub health
            playerHealthNumber.text = playerHealthSlider.value + "";
            playerHealthNumberAnim.SetTrigger("Damage");
            playerHealthNumberRegen.text = "- " + amountToUpdate;
            playerHealthNumberRegenAnim.SetTrigger("subtractNum");
        }
        else
        {
            if (resetAllHealth == 0)
            {
                enemyHealthSlider.value -= amountToUpdate;
                if (enemyHealthSlider.value <= 0)
                {
                    enemyHealthSlider.fillRect.GetComponent<Image>().gameObject.SetActive(false);
                }
            }
            else
            {
                // Set max health here, player does it differently
                enemyHealthSlider.maxValue = resetAllHealth;

                enemyHealthSlider.value = resetAllHealth;
                enemyHealthSlider.fillRect.GetComponent<Image>().gameObject.SetActive(true);
            }
        }
    }

    public IEnumerator CountDownTimer(float time)
    {
        playerCooldownSlider.maxValue = time;
        playerCooldownSlider.gameObject.SetActive(true);
        while (time >= 0)
        {
            playerCooldownSlider.value = time;
            time -= Time.deltaTime;

            yield return null;
        }
        playerCooldownSlider.gameObject.SetActive(false);
    }

    public IEnumerator StunTimer(float time)
    {
        playerStunSlider.maxValue = time;
        playerStunSlider.gameObject.SetActive(true);
        while (time >= 0)
        {
            playerStunSlider.value = time;
            time -= Time.deltaTime;

            yield return null;
        }
        playerStunSlider.gameObject.SetActive(false);
    }

    public void HaltCountDownTimer()
    {
        StopCoroutine("CountDownTimer");
        playerCooldownSlider.gameObject.SetActive(false);
    }

    public void PlaySound(AudioClip clip)
    {
        gameAudioSource.PlayOneShot(clip);
    }

    public void ToggleButton(attackTypes type, bool toggle)
    {
        switch (type)
        {
            case attackTypes.Light:
                lightButton.interactable = toggle;
                break;
            case attackTypes.Heavy:
                heavyButton.interactable = toggle;
                break;
            case attackTypes.Counter:
                counterBlockButton.interactable = toggle;
                break;
            case attackTypes.Special:
                specialButton.interactable = toggle;
                break;
        }
    }

    public void ToggleAllButtons(bool toggle, bool enableCounter)
    {
        lightButton.interactable = toggle;
        heavyButton.interactable = toggle;
        counterBlockButton.interactable = enableCounter;
        specialButton.interactable = toggle;
    }

    public void ToggleModelExceptWireframe(bool toggle, bool toggleUI = true)
    {
        model_playerGroup.SetActive(!toggle);
        model_enemyGroup.SetActive(!toggle);
        gameUIGroup.SetActive(!toggleUI);
        battleCircleEnemy.SetActive(!toggle);
        battleCirclePlayer.SetActive(!toggle);
    }

    public void ChangeFogColor(int fogInt)
    {
        bool showLogo = false;

        RenderSettings.fogColor = fogColorList[fogInt];
        if (fogInt == 2)
        {
            showLogo = true;
        }

        // For bosses
        DisplayCenterLogo(showLogo);
    }

    public void DisplayCenterLogo(bool toggle)
    {
        xpugLogoObject.SetActive(toggle);
    }

    public void StartIntroEnemy()
    {
        StartCoroutine("WaitForIntroEnemy");
    }

    private IEnumerator WaitForIntroEnemy()
    {
        if (tutorialUIGroup.activeSelf)
        {
            tutorialUIGroup.SetActive(false);
        }

        // disable possible player input
        playerInputBeingDelayed = true;

        // Disable dolly (moved here)
        StopCoroutine(RandomDollyCameraStart());
        dollyScript.ToggleDollyCamera(false);
        dollyScript.CancelInvoke("QueuedDolly");
        dollyScript.currentCameraNumber = 0;
        dollyScript.dollyAnimator.SetInteger("cameraNumber", 0);

        ToggleEnemyMesh(true);
        TogglePlayerMesh(true);
        battleCirclePlayer.SetActive(true);
        battleCircleEnemy.SetActive(true);
        enemyScript.isDead = true;

        gameUIGroup.SetActive(false);
        battleIntroUIGroup.SetActive(true);

        //enemyCosmeticScript.ChooseRandomCosmetics();
        // above is now done in below function (in NameEnemy() in enemy class)
        GenerateBattleIntroName();
        battleRoundNumber.text = "#" + (enemysDefeated + 1);
        battleRoundNumberShadow.text = battleRoundNumber.text;

        // Do intro camera based on type of enemy
        if (!enemyScript.Cosmetic_CheckName_Short())
        {
            if (!enemyScript.Cosmetic_CheckName_Giant())
            {
                if (enemyScript.CheckName_Crazy())
                {
                    enemyModelAnim.SetTrigger("crazyAnim1");
                }
                canim.SetTrigger("introEnemy");
            }
            else
            {
                canim.SetTrigger("introEnemyGiant");
            }
        }
        else
        {
            canim.SetTrigger("introEnemyShort");
        }

        // Play sound
        PlaySound(battleIntroSound);

        yield return new WaitForSeconds(4.0f); // length of cam animation

        // check for player tapping screen now

        enemyScript.isDead = false;

        // Start dolly
        StartCoroutine(RandomDollyCameraStart());

        // Start the battle
        StartGame();
    }

    private void WriteLosingDescription()
    {
        string defeatText = "";

        switch (enemysDefeated)
        {
            case 0:
                defeatText = "-You did not defeat a single opponent-";
                break;
            case 1:
                defeatText = "-You defeated 1 opponent. At least you tried?-";
                break;
            default:
                defeatText = "-You defeated " + enemysDefeated + " opponents-";
                break;
        }
        loseScreenDescription.text = defeatText;
    }

    public void ToggleHeavyButtonHighlight()
    {
        buttonEffectRipple.SetActive(true);
        buttonEffectRipple.GetComponent<Animator>().SetTrigger("show");
    }

    private void GenerateBattleIntroName()
    {
        string genName = "Placeholder";

        // Generate name here from list of names based on how many enemies the player has slain
        genName = ConsiderNameDifficulties();

        battleIntroUIName.text = genName; // fucking love satellite
        enemyName.text = genName;        
        enemyScript.NameEnemy(genName); // amen 7/8/22 4:02am | amen 7/18/22 4:21am | amen 8/18/22 5:12pm | amen 9/1/22 4:27am "How u doin future Brad?"
    }

    private string ConsiderNameDifficulties()
    {
        // rewrite to (after 5):
        // every 5 rounds spawn strong
        // every 10 rounds spawn crazy
        // go here for rounds!

        string chosenName = "";

        if ((enemysDefeated + 1) % 11 == 0)
        {
            chosenName = ChooseNameDifficulty(10, 90, 0, 0);
        }
        else if ((enemysDefeated + 1) % 10 == 0)
        {
            chosenName = ChooseNameDifficulty(0, 0, 0, 100);
        }
        else if ((enemysDefeated + 1) % 5 == 0)
        {
            chosenName = ChooseNameDifficulty(0, 0, 100, 0);
        }
        else
        {
            if (enemysDefeated == 0)
            {
                chosenName = ChooseNameDifficulty(10, 90, 0, 0);
            }
            else if (enemysDefeated == 1)
            {
                chosenName = ChooseNameDifficulty(80, 20, 0, 0);
            }
            else if (enemysDefeated == 2)
            {
                chosenName = ChooseNameDifficulty(90, 10, 0, 0);
            }
            else if (enemysDefeated == 3)
            {
                chosenName = ChooseNameDifficulty(0, 100, 0, 0);
            }
            else
            {
                chosenName = ChooseNameDifficulty(80, 20, 0, 0);
            }
        }

        return chosenName;
    }

    private string ChooseNameDifficulty(int percentageNormal, int percentageSad = 0, int percentageStrong = 0, int percentageCrazy = 0)
    {

        // PROBLEM - The right type of pug is chosen depending on the function, but it will always choose the last
        // example. ChooseNameDifficulty(90, 10, 0, 0) will always choose the 10 ChooseNameDifficulty(25, 25, 0, 25) will always choose the last 25
        // SOLUTION - Make smarter way of choosing one of 4 percentages and return a string depending on percentage chosen
        //----
        string name = "";
        float random = Random.Range(0,100);

        if (random < percentageNormal)
        {
            name = _enemyNormalNameList[Random.Range(0, _enemyNormalNameList.Count)] + " " + enemyNameList[Random.Range(0, enemyNameList.Count)];
            
            // Set fog color
            ChangeFogColor(0);
        }
        else if (random < percentageNormal + percentageSad)
        {
            name = _enemySadNameList[Random.Range(0, _enemySadNameList.Count)] + " " + enemyNameList[Random.Range(0, enemyNameList.Count)];

            // Set fog color
            ChangeFogColor(1);
        }
        else if (random < percentageNormal + percentageSad + percentageStrong)
        {
            name = _enemyStrongNameList[Random.Range(0, _enemyStrongNameList.Count)] + " " + enemyNameList[Random.Range(0, enemyNameList.Count)];

            // Set fog color
            ChangeFogColor(2);
        }
        else
        {
            name = _enemyCrazyNameList[Random.Range(0, _enemyCrazyNameList.Count)] + " " + enemyNameList[Random.Range(0, enemyNameList.Count)];

            // Set fog color
            ChangeFogColor(2);
        }


        return name;
    }

    public void StartGame()
    {
        tutorialUIGroup.SetActive(false);
        //battleIntroUIGroup.SetActive(false);

        battleIntroUIAnimator.SetTrigger("CloseIntro");

        // Delay on player input
        StartCoroutine("DelayPlayerInput");
        startGame = true;
        gameUIGroup.SetActive(true);
        enemyScript.InitiateFight();
    }

    public IEnumerator DelayPlayerInput()
    {
        yield return new WaitForSeconds(playerControlRoundStartDelay);
        playerInputBeingDelayed = false;
    }

    public void ResetRound()
    {
        enemyScript.ResetGameAttributes();
        playerScript.pauseAttack = false;
        playerScript.heavyDamage = 5.0f;
        playerScript.justAttacked = false;

        dollyScript.ToggleDollyCamera(false);
        dollyScript.CancelInvoke("QueuedDolly");
        dollyScript.repeatingCheckInvoked = false;

        StartIntroEnemy();

        // Chance of revealing binary decor
        if (Random.Range(0.0f, 1.0f) <= 0.6f)
        {
            RandomDisplayListObjects();
        }
    }

    public void RandomDisplayListObjects()
    {
        if (_tempBinaryLineObjectList.Count >= 1)
        {
            int tempNum = Random.Range(0, _tempBinaryLineObjectList.Count - 1);
            GameObject chosenObject = _tempBinaryLineObjectList[tempNum];

            if (_tempBinaryLineObjectList.Contains(chosenObject))
            {
                chosenObject.SetActive(true);
                _tempBinaryLineObjectList.Remove(chosenObject);
            }
        }
        else
        {
            Debug.Log("All binary line objects displayed!");
        }
    }

    public void DisplayUpgradeToolkit()
    {
        // if player has upgrades left
        if (!playerScript.CheckUpgradeTotal()) // return if total upgrades equal 9 (3 + 3 + 3)
        {
            upgradeScript.TidyUp(); // recenter everything back to normal
            upgradeUIGroup.SetActive(true);
            ToggleModelExceptWireframe(true);
        }
        else
        {
            // player has no more upgrades to unlock
            playerMaxUpgraded = true;
            winScreenUIGroup.GetComponent<Win_Screen_Controller>().NextRound(); //MAYBE
        }
    }

    public void CloseTutorialUI()
    {
        tutorialUIGroup.GetComponent<Animator>().SetTrigger("closeTutorial");
        menuStartButton.interactable = true;
    }

    public void OpenTutorialUI()
    {
        tutorialUIGroup.GetComponent<Animator>().SetTrigger("openTutorial");
        menuStartButton.interactable = false;
    }

    private void RandomPulsateDirection()
    {
        if (Random.Range(0.0f, 1.0f) <= 0.5f)
        {
            gameUIGroupAnimator.SetTrigger("pulsate");
        }
        else
        {
            gameUIGroupAnimator.SetTrigger("pulsate2");
        }
    }

    public void CameraMovement(attackTypes atype, bool react)
    {
        // Animation handler for the camera and player's pug model
        switch (atype)
        {
            case attackTypes.Light:
                if (react)
                {
                    canim.SetTrigger("reactLight");
                }
                else
                {
                    canim.SetTrigger("attackLight");
                }
                break;
            case attackTypes.Heavy:
                if (react)
                {
                    canim.SetTrigger("reactHeavy");
                    RandomPulsateDirection();

                    playerModelAnim.SetTrigger("hitAnim1");
                    playerScript.StartCoroutine("TimeAnimation", 0.41f);
                    // if something:
                    if (playerScript.inputtedHeavyAttack)
                    {
                        playerScript.StartCoroutine("ShouldBeBoostedStart", 0.41f);
                    }
                }
                else
                {
                    canim.SetTrigger("attackHeavy");
                }
                break;
            case attackTypes.Counter:
                if (react)
                {
                    canim.SetTrigger("reactCounter");
                    RandomPulsateDirection();

                    playerModelAnim.SetTrigger("hitAnim1");
                    playerScript.StartCoroutine("TimeAnimation", 0.41f);
                }
                else
                {
                    canim.SetTrigger("attackCounter");
                }
                break;
            default:
                Debug.Log("No animation for camera");
                break;
        }
    }

    public void DoEnemyMeshHitAnimation(attackTypes atype)
    {
        switch (atype)
        {
            case attackTypes.Heavy:
                enemyModelAnim.SetTrigger("hitAnim1");
                break;
            case attackTypes.Counter:
                enemyModelAnim.SetTrigger("hitAnim1");
                break;
            default:
                Debug.Log("Do not have enemy anim for this reaction!");
                enemyModelAnim.SetTrigger("hitAnim1");
                break;
        }
    }

    public void ToggleEnemyMesh(bool toggle)
    {
        enemyModelMesh.SetActive(toggle);
    }

    public void TogglePlayerMesh(bool toggle)
    {
        playerModelMesh.SetActive(toggle);
    }

    public void IncreaseSpiralSpeed()
    {
        if (enemysDefeated >= 1)
        {
            float tempSpeed = 1 + (enemysDefeated * wireScript.scrollSpeedIncrement);
            wireScript.ModifyScrollSpeedModifier(tempSpeed);
        }
        else
        {
            wireScript.ModifyScrollSpeedModifier(1.05f);
        }
    }

    public void AddToDefeatedEnemies()
    {
        enemysDefeated++;
    }

    public void DisplayWinScreen()
    {
        gameUIGroup.SetActive(false);
        winScreenUIGroup.SetActive(true);
    }

    public IEnumerator DisplayWinScreenDelay(float delayTime)
    {
        gameUIGroup.SetActive(false);
        yield return new WaitForSeconds(delayTime);
        winScreenUIGroup.SetActive(true);
        Debug.Log("DONESSSo");
    }

    public void RestartEntireGame()
    {
        loseScreenUIGroup.SetActive(false);
        gameUIGroup.SetActive(false);
        tutorialUIGroup.SetActive(true);
        enemyScript.ResetGameAttributes();
        ChangeFogColor(0);
        ToggleEnemyMesh(false);
        TogglePlayerMesh(false);
        playerScript.health = playerMaxHealth;
        UpdateHealthSlider(entityType.Player, 0, playerMaxHealth);
        playerHealthSlider.maxValue = playerMaxHealth;
        playerScript.isDead = false;
        prevEnemysDefeated = enemysDefeated;
        enemysDefeated = 0;
        playerScript.successfulCounters = 0;
        playerScript.counterMultiplier = 2.0f;
        enemyScript.counterMultiplier = 2.0f;
        playerScript.heavyDamage = 5.0f;
        playerScript.successfulAttackBoosts = 0;
        playerScript.localCounterMultiplierModifier = 1.0f;
        playerScript.lowHealthIndicatorObject.SetActive(false);

        playerScript.upgradeAttackLevel = 0;
        playerScript.upgradeShieldLevel = 0;
        playerScript.upgradeHealthLevel = 0;
        playerScript.UP_AttackValue = 1.0f;
        playerScript.UP_ShieldValue = 1.0f;
        playerScript.UP_HealthValue = 0.0f;
        playerScript.UP_EnragedChanceValue = 0.2f;
        playerScript.UP_CounterHealValue = 0.0f;
        playerScript.maxExpandHealth = playerMaxExpandHealthBase;
        playerMaxUpgraded = false;
        foreach (Button butt in upgradeScript.buttonObjects)
        {
            if (!butt.interactable)
            {
                butt.interactable = true;
            }
        }

        wireScript.scrollSpeedModifier = 1.0f;
        playerScript.healthUpgradeBonus = 0.0f;
        playerScript.currentEnragedDamageValue = 1.0f;

        upgradeScript.buttonTexts[0].GetComponent<TextMeshProUGUI>().text = "ATTACK";
        upgradeScript.buttonTexts[1].GetComponent<TextMeshProUGUI>().text = "SHIELD";
        upgradeScript.buttonTexts[2].GetComponent<TextMeshProUGUI>().text = "HEALTH";

        // Hide battle circles and reset speed
        battleCirclePlayer.SetActive(false);
        battleCircleEnemy.SetActive(false);
        playerScript.battleCircleScript.UpdateRotateSpeed(100);
        enemyScript.battleCircleScript.UpdateRotateSpeed(100);

        _tempBinaryLineObjectList = binaryLineObjectList;
        foreach (GameObject binaryObject in _tempBinaryLineObjectList)
        {
            binaryObject.SetActive(false);
        }

        dollyScript.ToggleDollyCamera(false);
        dollyScript.CancelInvoke("QueuedDolly");
        dollyScript.repeatingCheckInvoked = false;

        playerScript.justAttacked = false;
    }

    public void DisplayLoseScreen()
    {
        WriteLosingDescription();
        loseScreenUIGroup.SetActive(true);
        enemyScript.canAttack = false;
        enemyScript.isDead = true;
    }

    public void TogglePlayerInvincibility(bool toggle)
    {
        if (toggle)
        {
            playerScript.ActivateInvincibility();
        }
        else
        {
            playerScript.DisableInvincibility();
        }
    }

    public void ReturnToScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public IEnumerator RandomDollyCameraStart()
    {
        dollyScript.dollyStartDelay = Random.Range(dollyScript.dollyStartDelayMin, dollyScript.dollyStartDelayMax);
        int randomStartingCam = dollyScript.GetRandomCam();
        yield return new WaitForSeconds(dollyScript.dollyStartDelay);

        // If game is not over (find better check)
        if (!playerScript.pauseAttack && !enemyScript.isDead)
        {
            dollyScript.ToggleDollyCamera(true, randomStartingCam);
        }
    }
}

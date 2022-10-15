using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Attack_Controller : Attack_Controller
{
    [Header("Enemy Specific Variables")]
    public float actionWaitTime;
    public float maxWaitTime, minWaitTime, waitToStartMin, waitToStartMax;
    public Attack_Controller playerScript;
    public bool isAttacking;
    public bool canLight, canHeavy;
    public int lightAttackCounter;
    public bool respondToCounter;
    public enemyAttackPattern attackPattern, overrideAttackPattern;
    public enum enemyAttackPattern { RandomAttacks, LightAttacks, HeavyAttacks, CounterAttacks };
    public bool attackPatternOverride;
    public int turnsWithoutPlayerInput;
    public float lastHealth;
    public string myName;
    public List<string> sadNameList, strongNameList, crazyNameList;
    public List<string> shortNameList_Cosmetic, giantNameList_Cosmetic;
    public Transform _enemyModelMesh;
    public Vector3 _prevEnemyModelMeshScale;
    public bool usingCountersThisRound;
    public Enemy_Cosmetic_Controller enemyCosmeticControllerScript;
    public enum enemySize { Small, Normal, Big, Giant };
    public enemySize enemyCurrentSize;
    public GameObject emojiObject;
    public Vector3 emojiObjectPosition, emojiObjectBigPosition, emojiObjectGiantPosition;

    [Header("Enemy Body Texture Controls")]
    public Material enemyPugBodyMat;
    public Texture body_Default, body_Happy, body_HappyTongue, body_Closed;
    public Texture body_Bruised;
    public Texture body_Dazed;
    public Texture body_Ugly;
    public Texture body_Emo, body_Emo_Closed;

    [Header("Enemy Blinking Controls")]
    public bool canBlink;
    public float blinkingDuration, blinkingChance;

    [Header("Enemy Expression Controls")]
    public emojiType lastEmojiType;
    public enum emojiType { Sad, Mad };
    public Animator emojiAnimator;

    [Header("Enemy Particle Controls")]
    public bool isParticle;
    public GameObject tearSystem, droolSystem;

    private void Start()
    {
        _prevEnemyModelMeshScale = _enemyModelMesh.transform.localScale;

        // Start blinking timer
        InvokeRepeating("TryToBlink", 0.0f, 1.0f);
    }

    private void Update()
    {
        if (!attackPatternOverride)
        {
            // Decide what attack behavior to use
            if (playerScript.successfulCounters < 2 && successfulCounters < 2 && health > 75 && turnsWithoutPlayerInput >= 12)
            {
                if (attackPattern != enemyAttackPattern.LightAttacks)
                {
                    attackPattern = enemyAttackPattern.LightAttacks;
                    Debug.Log("BEHAVIOR 0");
                }
            }
            else if (playerScript.successfulCounters < 2 && successfulCounters < 2 && health <= 50)
            {
                if (attackPattern != enemyAttackPattern.HeavyAttacks)
                {
                    attackPattern = enemyAttackPattern.HeavyAttacks;
                    Debug.Log("BEHAVIOR 1");
                }
            }
            else if (playerScript.successfulCounters < 2 && successfulCounters >= 2 && health <= 50 && health > 30)
            {
                if (attackPattern != enemyAttackPattern.CounterAttacks)
                {
                    attackPattern = enemyAttackPattern.CounterAttacks;
                    Debug.Log("BEHAVIOR 2");
                }
            }
            else if (playerScript.successfulCounters < 2 && health <= 30)
            {
                if (attackPattern != enemyAttackPattern.HeavyAttacks)
                {
                    attackPattern = enemyAttackPattern.HeavyAttacks;
                    Debug.Log("BEHAVIOR 3");
                }
            }
            else if (successfulCounters >= 2 && health <= 30)
            {
                if (attackPattern != enemyAttackPattern.RandomAttacks)
                {
                    attackPattern = enemyAttackPattern.RandomAttacks;
                    Debug.Log("BEHAVIOR 4");
                }
            }
            else if (health <= 30)
            {
                if (attackPattern != enemyAttackPattern.CounterAttacks)
                {
                    attackPattern = enemyAttackPattern.CounterAttacks;
                    Debug.Log("BEHAVIOR 5");
                }
            }

            if (!isAttacking && canAttack && !respondToCounter)
            {
                RandomWaitTime();
                switch (attackPattern)
                {
                    case enemyAttackPattern.RandomAttacks:
                        StartCoroutine("ChooseRandomAttack");
                        break;
                    case enemyAttackPattern.LightAttacks:
                        StartCoroutine("ChooseLightAttack");
                        break;
                    case enemyAttackPattern.HeavyAttacks:
                        StartCoroutine("ChooseHeavyAttack");
                        break;
                    case enemyAttackPattern.CounterAttacks:
                        StartCoroutine("ChooseCounterAttack");
                        break;
                }
            }

            if (respondToCounter)
            {
                // This makes the enemy throw out a counter after the player hits a counter
                //actionWaitTime = minWaitTime;
                StartCoroutine("RandomDelayToRetaliationCounter");
                respondToCounter = false;
            }
        }
        else
        {
            DoAttackPatternOverride();

            if (!isAttacking && canAttack && !respondToCounter)
            {
                RandomWaitTime();
                switch (overrideAttackPattern)
                {
                    case enemyAttackPattern.RandomAttacks:
                        StartCoroutine("ChooseRandomAttack");
                        break;
                    case enemyAttackPattern.LightAttacks:
                        StartCoroutine("ChooseLightAttack");
                        break;
                    case enemyAttackPattern.HeavyAttacks:
                        StartCoroutine("ChooseHeavyAttack");
                        break;
                    case enemyAttackPattern.CounterAttacks:
                        StartCoroutine("ChooseCounterAttack");
                        break;
                }
            }
        }
    }

    public void NameEnemy(string enemyName)
    {
        myName = enemyName;
        CosmeticName tempHatOverride = CosmeticName.Empty;
        CosmeticName tempFaceOverride = CosmeticName.Empty;

        // Un-Shrink enemy
        Debug.Log("Un-Shrink ray activate!");
        _enemyModelMesh.transform.localScale = new Vector3(_prevEnemyModelMeshScale.x, _prevEnemyModelMeshScale.y, _prevEnemyModelMeshScale.z);
        enemyCurrentSize = enemySize.Normal;

        // Set texture back to normal if it was modified
        ChangeBodyTexture(body_Default);
        if (isParticle)
        {
            tearSystem.SetActive(false);
            droolSystem.SetActive(false);
            isParticle = false;
        }

        // Name specific variables----- Will cycle through sad first then > strong > etc.
        if (CheckNameList(sadNameList, myName))
        {
            SetAttackPattern(enemyAttackPattern.LightAttacks);
            lightDamage = 1.0f;
            heavyDamage = 2.0f;
            counterMultiplier = 1.0f;

            health = 7.0f;

            // special condition
            if (myName.Contains("Small") || myName.Contains("Tiny") || myName.Contains("Shrimp") || myName.Contains("Baby") || myName.Contains("Lil'") || (myName.Contains("Short") && !myName.Contains("Shorty")))
            {
                Debug.Log("Shrink ray activate!");
                _enemyModelMesh.localScale = new Vector3(_prevEnemyModelMeshScale.x * 0.75f, _prevEnemyModelMeshScale.x * 0.75f, _prevEnemyModelMeshScale.x * 0.75f);
                enemyCurrentSize = enemySize.Small;

                // Do expression timing for camera (makes face when camera points down at it)
                // This way is manually timing it and hard coding the value here (bad)
                canBlink = false;
                StartCoroutine(ChangeBodyTextureDelay(body_HappyTongue, 2.55f));
            }
            else if (myName.Contains("Flat"))
            {
                float randomFloatScale = Random.Range(0.1f, 0.4f);
                _enemyModelMesh.localScale = new Vector3(_prevEnemyModelMeshScale.x, _prevEnemyModelMeshScale.x * randomFloatScale, _prevEnemyModelMeshScale.x);
                enemyCurrentSize = enemySize.Small;
                lightDamage = 0.5f;
                heavyDamage = 1.0f;
                counterMultiplier = 0.5f;

                health = 5.0f;
            }
            else if (myName.Contains("Skinny") || myName.Contains("Slim"))
            {
                float randomFloatScale = Random.Range(0.5f, 0.8f);
                _enemyModelMesh.localScale = new Vector3(_prevEnemyModelMeshScale.x * randomFloatScale, _prevEnemyModelMeshScale.x, _prevEnemyModelMeshScale.x);
                enemyCurrentSize = enemySize.Normal;

                health = 5.0f;
            }
            else if (myName.Contains("Hieroglyphic"))
            {
                // Using ancient egyptian energy, this pug is able to channel the power of the gods to protect their thin body!
                SetAttackPattern(enemyAttackPattern.CounterAttacks);

                _enemyModelMesh.localScale = new Vector3(_prevEnemyModelMeshScale.x * 0.197f, _prevEnemyModelMeshScale.x, _prevEnemyModelMeshScale.x);
                enemyCurrentSize = enemySize.Normal;
                lightDamage = 0.5f;
                heavyDamage = 1.0f;
                counterMultiplier = 5.0f;

                health = 5.0f;
            }
            else if (myName.Contains("Emo") || myName.Contains("Goth"))
            {
                ChangeBodyTexture(body_Emo);
                tempFaceOverride = CosmeticName.NoseRing;
                enemyCosmeticControllerScript.permitFaceItemsOverride = true; // Need this to override equipping face items under round 10
                enemyCurrentSize = enemySize.Normal;
            }
            else if (myName.Contains("Sad") || myName.Contains("Sobbing") || myName.Contains("Crying") || myName.Contains("Pleading"))
            {
                ChangeBodyTexture(body_Closed);
                tearSystem.SetActive(true);
                isParticle = true;
                enemyCurrentSize = enemySize.Normal;
            }
        }
        else if (CheckNameList(strongNameList, myName))
        {
            SetAttackPattern(enemyAttackPattern.HeavyAttacks);
            lightDamage = 2.0f;
            heavyDamage = 5.0f;
            counterMultiplier = 3.0f;

            enemyCurrentSize = enemySize.Normal;

            //gameControllerScript.playerScript.ModifyCounterMultiplier(1.0f);
            targetObject.localCounterMultiplierModifier = 0.5f;

            health = 15.0f;

            // special condition
            if ((myName.Contains("Big") && !myName.Contains("Bigby")) || myName.Contains("Fat") || myName.Contains("Bigboy"))
            {
                Debug.Log("Grow ray activate!");
                _enemyModelMesh.localScale = new Vector3(_prevEnemyModelMeshScale.x * 1.2f, _prevEnemyModelMeshScale.x * 1.2f, _prevEnemyModelMeshScale.x * 1.2f);
                enemyCurrentSize = enemySize.Big;
                heavyDamage = 6.0f;
            }
            else if (myName.Contains("Giant") || (myName.Contains("Nephilim")) || (myName.Contains("Colossal")) || (myName.Contains("Son of Pug")))
            {
                _enemyModelMesh.localScale = new Vector3(_prevEnemyModelMeshScale.x * 1.5f, _prevEnemyModelMeshScale.x * 1.5f, _prevEnemyModelMeshScale.x * 1.5f);
                enemyCurrentSize = enemySize.Giant;
                heavyDamage = 7.0f;
            }
        }
        else if (CheckNameList(crazyNameList, myName))
        {
            SetAttackPattern(enemyAttackPattern.RandomAttacks);
            lightDamage = 3.0f;
            heavyDamage = 6.0f;
            counterMultiplier = 4.0f;

            lightCooldown = 1.0f;
            heavyCooldown = 2.0f;
            counterCooldown = 1.5f;

            enemyCurrentSize = enemySize.Normal;

            //gameControllerScript.playerScript.ModifyCounterMultiplier(0.0f); make this a modifier~~~~~
            targetObject.localCounterMultiplierModifier = 0.0f;

            health = 20.0f;

            // Name specific expression
            ChangeBodyTexture(body_Bruised);

            // Choose random face item
            float randomNum = Random.Range(0.0f, 1.0f);
            if (randomNum <= 0.6f)
            {
                tempFaceOverride = CosmeticName.Knife;
            }
            else
            {
                tempFaceOverride = CosmeticName.Hammer;
            }
            enemyCosmeticControllerScript.permitFaceItemsOverride = true;
        }
        else
        {
            // If enemy name was not a special difficulty, but should still have a unique feature (stupid = drooling face)
            if (myName.Contains("Stupid") || myName.Contains("Dazed") || myName.Contains("Dumb") || myName.Contains("Dummy") || myName.Contains("Daft"))
            {
                ChangeBodyTexture(body_Dazed);
                enemyCurrentSize = enemySize.Normal;
                tempHatOverride = CosmeticName.Welt;

                droolSystem.SetActive(true);
                isParticle = true;
            }
            else if (myName.Contains("Ugly"))
            {
                ChangeBodyTexture(body_Ugly);
                enemyCurrentSize = enemySize.Normal;
            }
        }

        // Set health back to full
        gameControllerScript.UpdateHealthSlider(entityType.Enemy, 0, health);

        // Get cosmetic to assign
        enemyCosmeticControllerScript.ChooseRandomCosmetics(tempHatOverride, tempFaceOverride);

    }

    public bool CheckNameList(List<string> nameList, string whatName)
    {
        bool nameHit = false;

        foreach(string name in nameList)
        {
            if (whatName.Contains(name) && !whatName.Contains("Bigby") && !whatName.Contains("Shorty"))
            {
                nameHit = true;
            }
        }
        
        return nameHit;
    }

    public bool Cosmetic_CheckName_Short()
    {
        bool decision = false;

        foreach (string checkName in shortNameList_Cosmetic)
        {
            if (myName.Contains(checkName) && (!myName.Contains("Shorty")))
            {
                decision = true;
            }
        }

        return decision;
    }

    public bool Cosmetic_CheckName_Giant()
    {
        bool decision = false;

        foreach (string checkName in giantNameList_Cosmetic)
        {
            if (myName.Contains(checkName) && (!myName.Contains("Bigby")))
            {
                decision = true;
            }
        }

        return decision;
    }

    public bool CheckName_Crazy()
    {
        bool decision = false;

        foreach (string checkName in crazyNameList)
        {
            if (myName.Contains(checkName))
            {
                decision = true;
            }
        }

        return decision;
    }

    public void ChangeBodyTexture(Texture newTexture)
    {
        enemyPugBodyMat.mainTexture = newTexture;

        // Other textures than default (dazed, bruised, etc) cant blink unless I make more expressions in future
        if (newTexture != body_Default)
        {
            canBlink = false;
        }
        else
        {
            canBlink = true;
        }
    }

    public IEnumerator ChangeBodyTextureDelay(Texture newTexture, float secondsDelay)
    {
        yield return new WaitForSeconds(secondsDelay);
        enemyPugBodyMat.mainTexture = newTexture;
    }

    public void TryToBlink()
    {
        if (canBlink)
        {
            if (Random.Range(0.0f, 1.0f) <= blinkingChance)
            {
                if (enemyPugBodyMat.mainTexture == body_Default)
                {
                    StartCoroutine(BlinkBodyTexture());
                }
                else
                {
                    Debug.Log("Could not blink, expression is already in use!");
                }
            }
        }
    }

    public IEnumerator BlinkBodyTexture()
    {
        ChangeBodyTexture(body_Closed);
        yield return new WaitForSeconds(blinkingDuration);
        ChangeBodyTexture(body_Default);
    }

    public void SetAttackPattern(enemyAttackPattern pattern)
    {
        overrideAttackPattern = pattern;
        attackPatternOverride = true;
    }

    public void DoAttackPatternOverride()
    {
        if (attackPattern != overrideAttackPattern)
        {
            attackPattern = overrideAttackPattern;
            Debug.Log("Overriding attack pattern for specially named enemy: " + myName);
        }
    }

    private IEnumerator RandomDelayToRetaliationCounter()
    {
        float randomTime = Random.Range(0.2f, 1.4f);

        yield return new WaitForSeconds(randomTime);

        if (canAttack)
        {
            StartCoroutine("ChooseAttack", attackTypes.Counter);
        }
    }

    public void OnPlayerCounter()
    {
        respondToCounter = true;
    }

    public void InitiateFight()
    {
        // make player no longer invincible
        playerScript.isInvincible = false;
        // After start is pressed in menu and goes to fighting scene
        StartCoroutine("StartAttacking");
    }

    private IEnumerator StartAttacking()
    {
        canAttack = false;
        yield return new WaitForSeconds(Random.Range(waitToStartMin, waitToStartMax));
        canAttack = true;
    }

    private IEnumerator ChooseAttack(attackTypes attackName)
    {
        DoAttackInput(attackName, playerScript);
        StartCoroutine("DoCooldown", attackName);

        yield return new WaitForSeconds(actionWaitTime);
        isAttacking = false;
    }

    private IEnumerator ChooseCounterAttack()
    {
        Debug.Log("Doing counter attack behavior");
        attackTypes chosenAttackType = attackTypes.None;
        float randomNum = Random.Range(1, 11);
        if (randomNum <= 6 && canCounter && gameControllerScript.enemysDefeated >= 10 && usingCountersThisRound)
        {
            chosenAttackType = attackTypes.Counter;
        }
        else if (randomNum == 7 && canHeavy)
        {
            chosenAttackType = attackTypes.Heavy;
        }
        else if (randomNum >= 8 && canLight)
        {
            chosenAttackType = attackTypes.Light;
        }
        else
        {
            chosenAttackType = attackTypes.None;
        }

        DoAttackInput(chosenAttackType, playerScript);
        StartCoroutine("DoCooldown", chosenAttackType);
        isAttacking = true;
        yield return new WaitForSeconds(actionWaitTime);
        isAttacking = false;
    }

    public bool RandomlyDecideUsingCounters()
    {
        bool decision = false;
        if (gameControllerScript.enemysDefeated < 20)
        {
            float randomNumber = Random.Range(0, 10);
            if (randomNumber > 5)
            {
                decision = true;
            }
        }
        else
        {
            Debug.Log("Counters will always be used for now on");
            decision = true;
        }
        Debug.Log("USING COUNTERS: " + decision);
        return decision;
    }

    private IEnumerator ChooseLightAttack()
    {
        Debug.Log("Doing light attack behavior");
        attackTypes chosenAttackType = attackTypes.None;
        float randomNum = Random.Range(1, 11);
        if (randomNum <= 6 && canLight)
        {
            chosenAttackType = attackTypes.Light;
        }
        else if (randomNum == 7 && canHeavy)
        {
            chosenAttackType = attackTypes.Heavy;
        }
        else if (randomNum >= 8 && canCounter && gameControllerScript.enemysDefeated >= 10 && usingCountersThisRound)
        {
            chosenAttackType = attackTypes.Counter;
        }
        else
        {
            chosenAttackType = attackTypes.None;
        }

        DoAttackInput(chosenAttackType, playerScript);
        StartCoroutine("DoCooldown", chosenAttackType);
        isAttacking = true;
        yield return new WaitForSeconds(actionWaitTime);
        isAttacking = false;
    }

    private IEnumerator ChooseHeavyAttack()
    {
        Debug.Log("Doing heavy attack behavior");
        attackTypes chosenAttackType = attackTypes.None;
        float randomNum = Random.Range(1, 11);
        if (randomNum <= 6 && canHeavy)
        {
            chosenAttackType = attackTypes.Heavy;
        }
        else if (randomNum == 7 && canLight)
        {
            chosenAttackType = attackTypes.Light;
        }
        else if (randomNum >= 8 && canCounter && gameControllerScript.enemysDefeated >= 10 && usingCountersThisRound)
        {
            chosenAttackType = attackTypes.Counter;
        }
        else
        {
            chosenAttackType = attackTypes.None;
        }

        DoAttackInput(chosenAttackType, playerScript);
        StartCoroutine("DoCooldown", chosenAttackType);
        isAttacking = true;
        yield return new WaitForSeconds(actionWaitTime);
        isAttacking = false;
    }

    private IEnumerator ChooseRandomAttack()
    {
        Debug.Log("Doing random attack behavior");
        attackTypes chosenAttackType;
        float randomNum = Random.Range(1, 4);
        if (randomNum <= 1 && canLight && lightAttackCounter < 2)
        {
            chosenAttackType = attackTypes.Light;
            lightAttackCounter++;
        }
        else if (randomNum == 2 && canHeavy)
        {
            chosenAttackType = attackTypes.Heavy;
            lightAttackCounter = 0;
        }
        else if (randomNum == 3 && canCounter && lastAttack != attackTypes.Counter && gameControllerScript.enemysDefeated >= 10)
        {
            chosenAttackType = attackTypes.Counter;
            lightAttackCounter = 0;
        }
        else
        {
            chosenAttackType = attackTypes.None;
        }
        

        DoAttackInput(chosenAttackType, playerScript);
        StartCoroutine("DoCooldown", chosenAttackType);
        isAttacking = true;
        yield return new WaitForSeconds(actionWaitTime);
        isAttacking = false;
    }

    private void RandomWaitTime()
    {
        actionWaitTime = Random.Range(minWaitTime, maxWaitTime);
    }

    public void PlayEmoji(emojiType emojiToPlay) // make enum for emoji type
    {
        Vector3 emojiPos = emojiObject.transform.position;

        // Adjust spacing for giant enemies
        if (enemyCurrentSize == enemySize.Giant)
        {
            emojiPos = emojiObjectGiantPosition;
            Debug.Log("Moving emoji pos for giant!");
        }
        else if (enemyCurrentSize == enemySize.Big)
        {
            emojiPos = emojiObjectBigPosition;
            Debug.Log("Moving emoji pos for big!");
        }
        else
        {
            emojiPos = emojiObjectPosition;
        }

        emojiObject.transform.position = emojiPos;
        
        switch (emojiToPlay)
        {
            case emojiType.Sad:
                emojiAnimator.SetTrigger("triggerSadEmoji");
                break;
            default:
                emojiAnimator.SetTrigger("triggerSadEmoji");
                break;
        }

        lastEmojiType = emojiToPlay;
    }

    public void ResetGameAttributes()
    {
        canAttack = false;
        canCounter = true;
        lastAttack = attackTypes.None;
        //isDead = false;

        // change attack dmg/etc depending on round

        health = 10;
        isCountering = false;
        isStunned = false;
        isInvincible = false;
        isAnimating = false;

        successfulCounters = 0;

        isAttacking = false;
        canLight = true;
        canHeavy = true;
        lightAttackCounter = 0;
        respondToCounter = false;
        attackPattern = enemyAttackPattern.RandomAttacks;
        turnsWithoutPlayerInput = 0;
        lastHealth = 0;

        counterMultiplier = 2.0f;
        lightDamage = 2.0f;
        heavyDamage = 5.0f;
        lightCooldown = 1.25f;
        heavyCooldown = 5.0f;
        specialCooldown = 10.0f;
        counterCooldown = 3.0f;
        counterMultiplier = 2.0f;
        targetObject.localCounterMultiplierModifier = 1.0f; // Modified by Savage pugs
        attackPatternOverride = false;
        targetObject.currentEnragedDamageValue = 1.0f;
        gameControllerScript.playerHealthSlider.maxValue = targetObject.maxExpandHealth;

        usingCountersThisRound = RandomlyDecideUsingCounters();

        // DEPRECATED------------------------------------------- This is now done in Attack_Controller
        // Add health to player every x rounds
        /*if (gameControllerScript.enemysDefeated % 10 == 0)
        {
            Debug.Log("Adding courtesy health because of round being a multiple of 10! Health was: " + targetObject.health + " and now will be: " + (targetObject.health + 5.0f));
            ModifyHealthValueUpdateSlider(targetObject, 4.0f, false, false);
        }
        else if (gameControllerScript.enemysDefeated % 5 == 0)
        {
            Debug.Log("Adding courtesy health because of round being a multiple of 5! Health was: " + targetObject.health + " and now will be: " + (targetObject.health + 2.5f));
            ModifyHealthValueUpdateSlider(targetObject, 2.0f, false, false);
        }
        */
        gameControllerScript.UpdateHealthSlider(entityType.Enemy, 0, 10);
    }

    private IEnumerator DoCooldown(attackTypes atype)
    {
        switch (atype)
        {
            case attackTypes.Light:
                canLight = false;
                break;
            case attackTypes.Heavy:
                canHeavy = false;
                break;
            default:
                break;
        }

        // See if player did an attack (Part 1)
        lastHealth = health;
        
        //waitingForCooldown = true;

        yield return new WaitForSeconds(currentCooldownAmount);

        //waitingForCooldown = false;

        switch (atype)
        {
            case attackTypes.Light:
                canLight = true;
                break;
            case attackTypes.Heavy:
                canHeavy = true;
                break;
            default:
                break;
        }

        // See if player did an attack (Part 2)
        if (health == lastHealth)
        {
            turnsWithoutPlayerInput++;
        }
        else
        {
            turnsWithoutPlayerInput = 0;
        }
    }
}

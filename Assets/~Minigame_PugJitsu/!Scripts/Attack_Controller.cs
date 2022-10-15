using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Attack_Controller : MonoBehaviour
{
    [Header("Game References")]
    public Game_Controller gameControllerScript;
    public GameObject objectMeshModel, objectTailMeshModel;
    public Animator entityAnim;
    public Attack_Controller targetObject;
    public Enemy_Attack_Controller targetObjectEnemyScript;
    public GameObject barrierObject;
    public GameObject lowHealthIndicatorObject;
    public GameObject battleCircleObject;
    public Animator battleCircleAnim;

    [Header("Damage Numbers HP References")]
    public Animator _localDamageNumberAnimator;
    public TextMeshProUGUI _localDamageNumberText;

    [Header("Attack Variables")]
    public entityType type;
    public bool canAttack, canCounter, isDead, pauseAttack;
    public float lightDamage, heavyDamage, boostedHeavyDamage, specialDamage, counterDamage, blockDamage;
    public float attackBufferTime, counterTime, counterMultiplier, prevCounterMultiplier, localCounterMultiplierModifier, heavyDamageBoostTime;
    public float counterStunTime;
    public int maxCountersForReducedStun;
    public float stunReductionPerCounter, counterReductionAmount;

    [Header("Upgrade Variables")]
    public int upgradeAttackLevel;
    public int upgradeShieldLevel;
    public int upgradeHealthLevel;
    public float UP_AttackValue, UP_ShieldValue, UP_HealthValue, UP_EnragedChanceValue, UP_CounterHealValue;
    public float healthUpgradeBonus;
    public TextMeshProUGUI button1Text, button2Text, button3Text;

    [Header("Enraged Variables")]
    public bool isHeavyHitEnraged; // if this and reactingHeavyAnim true, do heavyenragedattack
    public float currentEnragedDamageValue;
    public float enragedDamageValueMax;
    public bool shouldBeBoosted;
    public bool inputtedHeavyAttack;
    public Texture texture_Enraged, texture_PreEnraged;

    [Header("Battle Circle Variables")]
    public Battle_Circle_Controller battleCircleScript;

    [Header("Cooldown Variables")]
    public float lightCooldown;
    public float heavyCooldown, specialCooldown, counterCooldown, blockCooldown;

    [HideInInspector] public float currentDamageAmount, currentCooldownAmount;
    public attackTypes lastAttack;

    [Header("Status Variables")]
    public float health;
    public float healthRegeneratedCap, maxExpandHealth;
    public bool isCountering, isStunned, isInvincible, isAnimating, isAttackBoosted, isReducingIncomingDamage, isLowHealth, justAttacked;
    public float lowHealthIndicatorAmount;

    [Header("Stats")]
    public int successfulCounters;
    public int successfulAttackBoosts;

    public void DoAttackInput(attackTypes atype, Attack_Controller targetObjectScript)
    {
        if (!isAnimating && !isDead && !pauseAttack)
        {
            if (targetObject == null && targetObject != targetObjectScript)
            {
                targetObject = targetObjectScript;
            }

            if (canAttack && !isCountering && !isStunned)
            {
                switch (atype)
                {
                    case attackTypes.Light:
                        currentDamageAmount = lightDamage;
                        currentCooldownAmount = lightCooldown;
                        entityAnim.SetTrigger("attackLight");

                        lastAttack = atype;
                        StartCoroutine("AttackWithCooldown", targetObject);
                        break;
                    case attackTypes.Heavy:
                        inputtedHeavyAttack = true;
                        StartCoroutine(TurnOffInputtedHeavyAttack(0.5f));

                        currentDamageAmount = heavyDamage;
                        currentCooldownAmount = heavyCooldown;
                        entityAnim.SetTrigger("attackHeavy");

                        // Track stat
                        if (isAttackBoosted)
                        {
                            successfulAttackBoosts++;
                        }
                        if (isAnimating)
                        {
                            shouldBeBoosted = true;
                        }

                        lastAttack = atype;
                        StartCoroutine("AttackWithCooldown", targetObject);
                        break;
                    case attackTypes.Special:
                        currentDamageAmount = specialDamage;
                        currentCooldownAmount = specialCooldown;

                        lastAttack = atype;
                        break;
                    case attackTypes.Counter:
                        currentDamageAmount = counterDamage;
                        currentCooldownAmount = counterCooldown;

                        lastAttack = atype;
                        StartCoroutine("CounterWithCooldown", targetObject);
                        break;
                    case attackTypes.Block:
                        currentDamageAmount = blockDamage;
                        currentCooldownAmount = blockCooldown;

                        lastAttack = atype;
                        break;
                    default:
                        // For none
                        currentDamageAmount = 0.0f;
                        currentCooldownAmount = 0.0f;

                        lastAttack = atype;
                        break;
                }
                // Do camera stuff
                // Do not play counter attack anim here, because it does not have one
                // The one that it does have should only come out during a successful counter
                if (type == entityType.Player)
                {
                    if (atype != attackTypes.Counter)
                    {
                        gameControllerScript.CameraMovement(atype, false);
                    }

                    // Disable dolly
                    StopCoroutine(gameControllerScript.RandomDollyCameraStart());
                    gameControllerScript.dollyScript.ToggleDollyCamera(false);
                    gameControllerScript.dollyScript.CancelInvoke("QueuedDolly");
                    gameControllerScript.dollyScript.currentCameraNumber = 0;
                    gameControllerScript.dollyScript.dollyAnimator.SetInteger("cameraNumber", 0);
                }

                StartCoroutine("AttackSpamTime", 0.0f);
            }
            else
            {
                Debug.Log(gameObject.name + " can not attack now!");
            }
        }
        else
        {
            Debug.Log("Currently in an animation or dead, can not act!");
        }
    }

    public IEnumerator CounterActive()
    {
        isCountering = true;
        barrierObject.SetActive(true);

        yield return new WaitForSeconds(counterTime);

        isCountering = false;
        barrierObject.SetActive(false);
    }

    public IEnumerator HeavyDamageIncrease()
    {
        Debug.Log("Starting heavy damage increase for: " + gameObject.name);
        float prevHeavyDamage = heavyDamage;
        heavyDamage = boostedHeavyDamage;

        // Track how many times player does this
        isAttackBoosted = true;
        gameControllerScript.ToggleHeavyButtonHighlight();

        yield return new WaitForSeconds(heavyDamageBoostTime);

        Debug.Log("Ending heavy damage increase for: " + gameObject.name);
        heavyDamage = prevHeavyDamage;

        isAttackBoosted = false;
    }

    public void BattleCircleHit_Light()
    {
        battleCircleAnim.SetTrigger("hitLight");
    }

    public void BattleCircleHit_Heavy()
    {
        battleCircleAnim.SetTrigger("hitHeavy");
    }

    public void UpgradeAttack(Button attackButton)
    {
        if (upgradeAttackLevel < 2)
        {
            upgradeAttackLevel += 1;
        }
        else
        {
            upgradeAttackLevel += 1;
            attackButton.interactable = false;
            button1Text.text = "MAX\nLEVEL";
        }

        // modifies damage target takes
        switch (upgradeAttackLevel)
        {
            case 0:
                UP_AttackValue = 1.0f;
                UP_EnragedChanceValue = 0.2f;
                break;
            case 1:
                UP_AttackValue = 1.2f;
                UP_EnragedChanceValue = 0.4f;
                break;
            case 2:
                UP_AttackValue = 1.3f;
                UP_EnragedChanceValue = 0.6f;
                break;
            case 3:
                UP_AttackValue = 1.5f;
                UP_EnragedChanceValue = 0.8f;
                break;
            default:
                UP_AttackValue = 1.0f;
                UP_EnragedChanceValue = 0.2f;
                break;
        }
    }

    public void UpgradeShield(Button shieldButton)
    {
        if (upgradeShieldLevel < 2)
        {
            upgradeShieldLevel += 1;
        }
        else
        {
            upgradeShieldLevel += 1;
            shieldButton.interactable = false;
            button2Text.text = "MAX\nLEVEL";
        }

        // modifies counter damage
        switch (upgradeShieldLevel)
        {
            case 0:
                UP_ShieldValue = 1.0f;
                UP_CounterHealValue = 0.0f;
                break;
            case 1:
                UP_ShieldValue = 1.1f;
                break;
            case 2:
                UP_ShieldValue = 1.2f;
                break;
            case 3:
                UP_ShieldValue = 1.4f;
                UP_CounterHealValue = 1.0f;
                break;
            default:
                UP_ShieldValue = 1.0f;
                UP_CounterHealValue = 0.0f;
                break;
        }
    }

    public void UpgradeHealth(Button healthButton)
    {
        if (upgradeHealthLevel < 2)
        {
            upgradeHealthLevel += 1;

            // Give health when upgrading
            Debug.Log("Health upgrade bonus health distributed! Was: " + health + " and now is: " + (health + 1.0f) + "!");
            ModifyHealthValueUpdateSlider(this, 1.0f, false);
        }
        else
        {
            upgradeHealthLevel += 1;
            healthButton.interactable = false;
            button3Text.text = "MAX\nLEVEL";

            // Give health when upgrading
            Debug.Log("Health upgrade bonus health distributed! Was: " + health + " and now is: " + (health + 1.0f) + "!");
            ModifyHealthValueUpdateSlider(this, 1.0f, false);
        }

        // modifies max health & gives extra health regenerated per battle
        switch (upgradeHealthLevel)
        {
            case 0:
                UP_HealthValue = 0.0f;
                healthUpgradeBonus = 0.0f;
                break;
            case 1:
                UP_HealthValue = 2.0f;
                break;
            case 2:
                UP_HealthValue = 4.0f;
                healthUpgradeBonus = 1.0f;
                break;
            case 3:
                UP_HealthValue = 6.0f;
                healthUpgradeBonus = 2.0f;
                break;
            default:
                UP_HealthValue = 0.0f;
                healthUpgradeBonus = 0.0f;
                break;
        }

        maxExpandHealth = gameControllerScript.playerMaxExpandHealthBase + UP_HealthValue;
    }

    public bool CheckUpgradeTotal()
    {
        return upgradeShieldLevel + upgradeAttackLevel + upgradeHealthLevel == 9;
    }

    public IEnumerator CounterWithCooldown()
    {
        // Set temp variables to current attack variables which change upon using another attack
        attackTypes attack = lastAttack;

        if (canCounter)
        {
            StartCoroutine("CounterActive");
            canCounter = false;

            if (type == entityType.Player)
            {
                gameControllerScript.ToggleButton(attack, false);

                // Do pressing animation
                gameControllerScript.gameUIGroupAnimator.SetTrigger("counter");
            }
            yield return new WaitForSeconds(counterCooldown);
            canCounter = true;

            if (type == entityType.Player)
            {
                gameControllerScript.ToggleButton(attack, true);
            }
        }
    }

    public IEnumerator AttackWithCooldown()
    {
        // Set temp variables to current attack variables which change upon using another attack
        float cooldown = 0.0f;
        cooldown += currentCooldownAmount;
        attackTypes attack = lastAttack;

        // If player is the one attacking, set trigger in buttons to do pressing animation / disable button
        if (type == entityType.Player)
        {
            string triggerToSet = "";

            switch (attack)
            {
                case attackTypes.Light:
                    triggerToSet = "light";
                    break;
                case attackTypes.Heavy:
                    triggerToSet = "heavy";
                    break;
            }
            gameControllerScript.gameUIGroupAnimator.SetTrigger(triggerToSet);

            gameControllerScript.ToggleButton(attack, false);
        }
        yield return new WaitForSeconds(cooldown);
        if (type == entityType.Player)
        {
            gameControllerScript.ToggleButton(attack, true);
        }
    }

    public void DoDamageToTarget()
    {
        if (!targetObject.isInvincible)
        {
            if (!targetObject.isCountering)
            {
                if (targetObject.type == entityType.Player)
                {
                    gameControllerScript.CameraMovement(lastAttack, true);
                }
                if (shouldBeBoosted && lastAttack == attackTypes.Heavy) // if player is doing heavy anim do enraged
                {
                    targetObject.TakeDamage(currentDamageAmount * UP_AttackValue * currentEnragedDamageValue);
                    Debug.Log(type + " DID ENRAGED DAMAGE OF " + (currentDamageAmount * UP_AttackValue * currentEnragedDamageValue) + " TO: " + targetObject);
                }
                else
                {
                    targetObject.TakeDamage(currentDamageAmount * UP_AttackValue);
                }
            }
            else
            {
                // if player hits target countering, they will be hit with player damage amount * player attack upgrade value * target counter * target shield upgrade
                // if target hits player countering, they will be hit with target damage amount * target attack upgrade value (1) * player counter * player shield upgrade
                targetObject.entityAnim.SetTrigger("attackCounter");
                if (shouldBeBoosted)
                {
                    TakeDamage((currentDamageAmount * UP_AttackValue * (targetObject.counterMultiplier * targetObject.localCounterMultiplierModifier) * targetObject.UP_ShieldValue) * currentEnragedDamageValue);
                    Debug.Log("COUNTERED! FOR DAMAGE: " + currentDamageAmount * UP_AttackValue + " and mulitplier: " + (targetObject.counterMultiplier * targetObject.localCounterMultiplierModifier) + " counterMultiplier: " + targetObject.counterMultiplier + " localCounterMultiplierModifier: " + targetObject.localCounterMultiplierModifier + " for a total damage of: " + ((currentDamageAmount * UP_AttackValue * (targetObject.counterMultiplier * targetObject.localCounterMultiplierModifier) * targetObject.UP_ShieldValue) * currentEnragedDamageValue) + " CURRENT ENRAGED: " + currentEnragedDamageValue);
                }
                else
                {
                    TakeDamage(currentDamageAmount * UP_AttackValue * (targetObject.counterMultiplier * targetObject.localCounterMultiplierModifier) * targetObject.UP_ShieldValue);
                    Debug.Log("COUNTERED! FOR DAMAGE: " + currentDamageAmount * UP_AttackValue + " and mulitplier: " + (targetObject.counterMultiplier * targetObject.localCounterMultiplierModifier) + " counterMultiplier: " + targetObject.counterMultiplier + " localCounterMultiplierModifier: " + targetObject.localCounterMultiplierModifier + " for a total damage of: " + ((currentDamageAmount * UP_AttackValue * (targetObject.counterMultiplier * targetObject.localCounterMultiplierModifier) * targetObject.UP_ShieldValue) * currentEnragedDamageValue) + " CURRENT ENRAGED: " + currentEnragedDamageValue);
                }
                AddToSuccessfulCounters(targetObject);

                // Heal player if upgrade owned
                if (targetObject.upgradeShieldLevel >= 2)
                {
                    ModifyHealthValueUpdateSlider(targetObject, targetObject.UP_CounterHealValue, false); // LEFT OFF
                }

                // Make player able to act out of a successful counter, enabling all their cooldowns to go away
                targetObject.StopCoroutine("AttackSpamTime");
                targetObject.isCountering = false;
                targetObject.canAttack = true;
                targetObject.barrierObject.SetActive(false);

                if (targetObject.type == entityType.Player)
                {
                    //enemy gets hit by player counter
                    targetObject.gameControllerScript.HaltCountDownTimer();
                    GetComponent<Enemy_Attack_Controller>().OnPlayerCounter();
                    gameControllerScript.CameraMovement(attackTypes.Counter, false);

                    // Give player increased heavy attack damage as an incentive to counter into an attack
                    if (lastAttack == attackTypes.Heavy && health != 0 && !isDead && gameControllerScript.heavyButton.IsInteractable())
                    {
                        targetObject.StartCoroutine("HeavyDamageIncrease");
                    }

                    // Play emoji for enemy
                    //GetComponent<Enemy_Attack_Controller>().PlayEmoji(Enemy_Attack_Controller.emojiType.Sad);
                }
                else
                {
                    //player gets hit by enemys counter
                    Debug.Log("PLAYER HIT BY ENEMY COUNTER");
                    gameControllerScript.CameraMovement(attackTypes.Counter, true);
                    //targetObject.GetComponent<Enemy_Attack_Controller>().SuccessfulCounterHit();
                }

                // Whatever gets hit by the counter will have a stun period where they cant attack
                StunFromCounter(counterStunTime);

                // Reset everything manually
                /*if (targetObject.type == entityType.Player)
                {
                    gameControllerScript.ToggleAllButtons(true, false);
                }*/
                //targetObject.barrierObject.SetActive(false);
            }
        }  
    }

    public void StunFromCounter(float stunTime)
    {
        if (type == entityType.Player)
        {
            gameControllerScript.StopCoroutine("CountDownTimer");
            gameControllerScript.playerCooldownSlider.gameObject.SetActive(false);
            //gameControllerScript.StopCoroutine("StunTimer");
        }
        StopCoroutine("AttackSpamTime");
        isStunned = true;
        StartCoroutine("AttackSpamTime", stunTime);
    }

    public void AddToSuccessfulCounters(Attack_Controller targetScript)
    {
        targetScript.successfulCounters++;

        // Reduce counter multiplier every time its used successfully
        if (targetObject.counterMultiplier > 1.0f)
        {
            targetObject.prevCounterMultiplier = targetObject.counterMultiplier;
            targetObject.counterMultiplier -= counterReductionAmount;
        }
    }

    public IEnumerator AttackSpamTime(float addedTime = 0.0f)
    {
        // Player will be able to throw out other attacks when last attack is still in cooldown.
        // Attacks will have cooldown, and the player will have a cooldown of throwing out attacks which is unrelated
        float waitTime;
        ToggleAttack(false);

        // Tell when just attacked
        justAttacked = true;

        if (isStunned)
        {
            // Stun time is affected by how many successful counters the entity has
            // 
            waitTime = GetStunTime(addedTime);
            if (type == entityType.Player)
            {
                gameControllerScript.StartCoroutine("StunTimer", waitTime);
            }
        }
        else
        {
            waitTime = attackBufferTime;
            if (type == entityType.Player)
            {
                gameControllerScript.StartCoroutine("CountDownTimer", waitTime);
            }
        }

        yield return new WaitForSeconds(waitTime);
        if (isStunned)
        {
            isStunned = false;
        }
        ToggleAttack(true);

        justAttacked = false;
    }

    public float GetStunTime(float originalStunAmount)
    {
        float newTime = originalStunAmount;

        // Is minus 1 because first hit should be full duration
        // When it equals 2, it will be applying the first debuff
        float stunModifier = 1.0f;
        if (lastAttack == attackTypes.Light)
        {
            stunModifier = 0.5f;
            Debug.Log("STUN MODIFIER DEPLOYED!");
        }
        
        if ((targetObject.successfulCounters - 1) < maxCountersForReducedStun)
        {
            newTime = originalStunAmount - (targetObject.successfulCounters - 1) * stunReductionPerCounter;
            newTime *= stunModifier;
            if (stunModifier == 0.5f)
            {
                Debug.Log(targetObject.type + " MODIFIED STUN AMOUNT IS: " + newTime);
            }
            else
            {
                Debug.Log(targetObject.type + " REDUCED STUN AMOUNT IS: " + newTime);
            }
        }
        else
        {
            newTime = 0.5f;
            newTime *= stunModifier;
            if (stunModifier == 0.5f)
            {
                Debug.Log(targetObject.type + " IS AT MAX MODIFIED STUN OF: " + newTime);
            }
            else
            {
                Debug.Log(targetObject.type + " IS AT MAX REDUCED STUN OF: " + newTime);
            }
        }

        return newTime;
    }

    private void ToggleAttack(bool toggle)
    {
        canAttack = toggle;
    }

    public void ActivateInvincibility()
    {
        isInvincible = true;
    }

    public void DisableInvincibility()
    {
        isInvincible = false;
    }

    public void RedactedActivateReducedDamage()
    {
        isReducingIncomingDamage = true;
    }

    public void RedactedDisableReducedDamage()
    {
        isReducingIncomingDamage = false;
    }

    public void ModifyCounterMultiplier(float newValue)
    {
        if (counterMultiplier != newValue)
        {
            counterMultiplier = newValue;
        }
    }

    public void HeavyEnragedAttack()
    {
        currentEnragedDamageValue = enragedDamageValueMax;
        isHeavyHitEnraged = true;
    }

    public IEnumerator TimeAnimation(float duration)
    {
        isAnimating = true;

        yield return new WaitForSeconds(duration);

        isAnimating = false;
    }

    public IEnumerator TurnOffInputtedHeavyAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        inputtedHeavyAttack = false;
    }

    public bool RollBoostedChance()
    {
        bool decision = false;

        if (Random.Range(0.0f, 1.0f) <= UP_EnragedChanceValue)
        {
            Debug.Log(type + ": IS ENRAGING DAMAGE!");
            decision = true;

            // Show enraged indicator
            texture_PreEnraged = objectMeshModel.GetComponent<Renderer>().material.mainTexture;
            objectMeshModel.GetComponent<Renderer>().material.mainTexture = texture_Enraged;
            objectTailMeshModel.GetComponent<Renderer>().material.mainTexture = texture_Enraged;
        }
        else
        {
            Debug.Log(type + ": IS NOT ENRAGING DAMAGE!");
        }

        return decision;
    }

    public IEnumerator ShouldBeBoostedStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        shouldBeBoosted = RollBoostedChance();
        if (shouldBeBoosted)
        {
            HeavyEnragedAttack();
            StartCoroutine(TurnOfShouldBeBoosted(0.5f));
        }
    }

    public IEnumerator TurnOfShouldBeBoosted(float delay)
    {
        yield return new WaitForSeconds(delay);
        shouldBeBoosted = false;
        inputtedHeavyAttack = false;
        isHeavyHitEnraged = false;
        currentEnragedDamageValue = 1.0f;

        // Hide enraged indicator
        objectMeshModel.GetComponent<Renderer>().material.mainTexture = texture_PreEnraged;
        objectTailMeshModel.GetComponent<Renderer>().material.mainTexture = texture_PreEnraged;
    }

    public void ActivateAnimation()
    {
        isAnimating = true;
    }

    public void DisableAnimation()
    {
        isAnimating = false;
    }

    public void TakeDamage(float damageAmount)
    {
        if (!isInvincible)
        {
            Debug.Log(gameObject.name + " TOOK DAMAGE: " + damageAmount);

            // Tilt battle circle according to damage
            if (damageAmount >= 5.0f)
            {
                BattleCircleHit_Heavy();
            }
            else
            {
                BattleCircleHit_Light();
            }

            // Do enemy hit anim here
            if (type == entityType.Enemy && damageAmount > 0.0f && targetObject)
            {
                gameControllerScript.DoEnemyMeshHitAnimation(targetObject.lastAttack);
            }

            if (health - damageAmount > 0)
            {
                if (isReducingIncomingDamage)
                {
                    Debug.Log(gameObject.name + " IS REDUCING INCOMING DAMAGE! Took: " + damageAmount * .90f + " damage instead of: " + damageAmount + "!");
                    health -= damageAmount * .90f;
                }
                else
                {
                    health -= damageAmount;
                    if (type == entityType.Player)
                    {
                        float rotateSpeed = 0.0f;
                        float _maxHealth = gameControllerScript.playerMaxHealth;
                        if (health <= _maxHealth * (3/4))
                        {
                            if (health <= _maxHealth * (1/2))
                            {
                                rotateSpeed = 20.0f;
                            }
                            else
                            {
                                rotateSpeed = 50.0f;
                            }
                        }
                        else
                        {
                            rotateSpeed = 100.0f;
                        }

                        battleCircleScript.UpdateRotateSpeed(rotateSpeed);
                    }
                }
            }
            else
            {
                OnDeath(type);
            }

            // Display damage numbers for enemy
            if (type == entityType.Enemy)
            {
                DisplayDamageNumber(damageAmount);
            }
            else
            {
                // Show low health if players health is low
                if (health <= lowHealthIndicatorAmount)
                {
                    isLowHealth = true;
                    lowHealthIndicatorObject.SetActive(true);
                }
            }

            // Update health slider in Game_Controller
            gameControllerScript.UpdateHealthSlider(type, damageAmount, 0);
        }
        else if (isInvincible && !isReducingIncomingDamage)
        {
            Debug.Log(gameObject.name + " IS INVINCIBLE!");
        }
    }

    public void ModifyHealthValueUpdateSlider(Attack_Controller objectScript, float amountToModify, bool subtractHealth, bool expandHealth = true)
    {
        int negativeModifier = -1;

        if (!subtractHealth)
        {
            negativeModifier = 1;
        }

        if (objectScript.health < objectScript.maxExpandHealth && objectScript.health + amountToModify <= objectScript.maxExpandHealth)
        {
            Debug.Log("ADDING HEALTH OF: " + amountToModify + " [HEALTH BONUS INCLUDED: " + (amountToModify + objectScript.healthUpgradeBonus) + "]" + " TO GAMEOBJECT: " + objectScript.name);

            objectScript.health += (Mathf.Abs(amountToModify) * negativeModifier);

            float tempBonus = objectScript.healthUpgradeBonus;
            if (tempBonus != 0 && objectScript.health + tempBonus <= objectScript.maxExpandHealth)
            {
                objectScript.health += objectScript.healthUpgradeBonus;
            }
            else if (tempBonus != 0 && objectScript.health + tempBonus > objectScript.maxExpandHealth)
            {
                objectScript.health = objectScript.maxExpandHealth;
            }

            // Hide low health if players health is up
            if (objectScript.health > 2.0f && objectScript.type == entityType.Player)
            {
                isLowHealth = false;
                objectScript.lowHealthIndicatorObject.SetActive(false);
            }

            // Increase the max health if player has upgrade unlocked
            if (expandHealth)
            {
                if (gameControllerScript.playerHealthSlider.maxValue < objectScript.health)
                {
                    float valueToAssign = objectScript.health;
                    if (gameControllerScript.playerHealthSlider.maxValue > objectScript.maxExpandHealth)
                    {
                        valueToAssign = targetObject.maxExpandHealth;
                    }
                    gameControllerScript.playerHealthSlider.maxValue = valueToAssign;
                }
            }
        }
        else
        {
            objectScript.health = objectScript.maxExpandHealth;
        }

        gameControllerScript.playerHealthSlider.value = objectScript.health;

        if (negativeModifier == 1)
        {
            // add health
            gameControllerScript.playerHealthNumberAnim.SetTrigger("Heal");
            gameControllerScript.playerHealthNumberRegen.text = "+ " + (Mathf.Abs(amountToModify + objectScript.healthUpgradeBonus) * negativeModifier);
            gameControllerScript.playerHealthNumberRegenAnim.SetTrigger("addNum");
        }
        else
        {
            // sub health
            // might be unused??
            gameControllerScript.playerHealthNumberAnim.SetTrigger("Damage");
            gameControllerScript.playerHealthNumberRegen.text = "- " + amountToModify;
            gameControllerScript.playerHealthNumberRegenAnim.SetTrigger("subtractNum");
        }
        gameControllerScript.playerHealthNumber.text = objectScript.health + "";
    }

    public void DisplayDamageNumber(float value, bool subtractHealth = true)
    {
        string operatorSymbol = "-";

        if (!subtractHealth)
        {
            operatorSymbol = "+";
        }

        if (value != 0)
        {
            _localDamageNumberText.text = operatorSymbol + string.Format("{0:#.00}", value);
            //_localDamageNumberText.text = operatorSymbol + value;
        }
        else
        {
            _localDamageNumberText.text = operatorSymbol + value;
        }

        _localDamageNumberAnimator.SetTrigger("showHP");
    }

    public void OnDeath(entityType etype)
    {
        Debug.Log(gameObject.name + " DIED!");
        health = 0.0f;
        isDead = true;

        if (etype == entityType.Enemy)
        {
            // make player invincible so cant die on victory screen
            gameControllerScript.TogglePlayerInvincibility(true);

            gameControllerScript.AddToDefeatedEnemies();
            //gameControllerScript.IncreaseSpiralSpeed();

            float tempRegenHealth = 0.0f;
            if (gameControllerScript.enemysDefeated % 5 == 0)
            {
                GetComponent<Enemy_Attack_Controller>().PlayEmoji(Enemy_Attack_Controller.emojiType.Sad);
                gameControllerScript.StartCoroutine("DisplayWinScreenDelay", 0.75f);
                tempRegenHealth = 3.0f;

                if (gameControllerScript.enemysDefeated % 10 == 0)
                {
                    tempRegenHealth = 4.0f;
                }
            }
            else
            {
                gameControllerScript.DisplayWinScreen();
                tempRegenHealth = 2.0f;
            }

            // Regen health for player when enemy dies
            // targetObject is player in this case
            if (targetObject)
            {
                ModifyHealthValueUpdateSlider(targetObject, tempRegenHealth, false);
            }

            // Disable player input
            targetObject.pauseAttack = true;

            // Disable dolly
            /*StopCoroutine(gameControllerScript.RandomDollyCameraStart());
            gameControllerScript.dollyScript.ToggleDollyCamera(false);
            gameControllerScript.dollyScript.CancelInvoke("QueuedDolly");
            gameControllerScript.dollyScript.currentCameraNumber = 0;
            gameControllerScript.dollyScript.dollyAnimator.SetInteger("cameraNumber", 0);*/

            // Enable dolly
            gameControllerScript.dollyScript.ToggleDollyCamera(true, 2);
        }
        else
        {
            //player death here
            gameControllerScript.DisplayLoseScreen();

            // make enemy invincible so player cant win after they lose
            targetObject.isInvincible = true;
        }
    }
}

[SerializeField]
public enum entityType { Player, Enemy };

[SerializeField]
public enum attackTypes { Light, Heavy, Special, Counter, Block, None };

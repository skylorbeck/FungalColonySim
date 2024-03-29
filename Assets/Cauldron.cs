using System;
using System.Collections;
using System.Collections.Generic;
using DamageNumbersPro;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Cauldron : MonoBehaviour, IPointerClickHandler
{
    public SpriteRenderer cauldronSprite;
    public SpriteRenderer progressSprite;
    public SpriteRenderer[] fire;
    public SpriteRenderer[] wood;
    public Button fuelButton;
    public Button onButton;
    public Button gatherButton;
    public SpriteRenderer potionSprite;
    public SpriteMask progressMask;
    public float progressSpeed = 1f;
    public float maskOffset = 2.25f;
    public float rotationSpeed = 20f;
    public float rotationDegree = 2f;
    public float punchSize = 0.5f;
    public float baseBrewTime = 30f;
    public float additionalBrewRatio = 0.25f;
    public int neededIngredients = 100;
    public int neededIngredientsBase = 100;
    public int desiredPotions = 0;
    public TMP_InputField desiredPotionsText;
    public Button decreaseDesiredPotionsButton;
    public Button increaseDesiredPotionsButton;
    public GameObject percentButtons;
    public GameObject evenPotionButtons;

    public float cauldronClickLimit = 0.1f;
    public float cauldronClickTimer = 0f;

    public SpriteRenderer ingredientSprite;
    public Image ingredientPreviewImage;
    public TMP_InputField ingredientAmountText;
    public TextMeshProUGUI ingredientNameText;
    public Button addIngredientButton;
    public Button nextIngredientButton;
    public Button previousIngredientButton;
    public Button removeIngredientsButton;
    public MushroomBlock.MushroomType currentIngredient;
    public List<CauldronIngredient> ingredientPreviews;
    public RatioBar ingredientBar;
    public TextMeshProUGUI timeLeftText;

    public AudioClip addIngredientSound;
    public AudioClip removeIngredientSound;
    public AudioClip brewSound;
    public AudioClip gatherSound;
    public AudioClip finishSound;

    public DamageNumberMesh damageNumberMesh;
    public DamageNumberGUI xpNumberGUI;
    public RectTransform xpPopSpawnPoint;

    public TextMeshProUGUI curLevelText;
    public TextMeshProUGUI cauldronCapacityText;
    public Image xpBar;
    public Image fakeXpBar;
    public float xpBarFillSpeed = 2f;
    public uint potionBaseXp = 2;
    public float potionCompleteMulti = 5f;
    public ObjectPool<SpriteRenderer> ingredientPool;
    public bool canClick => cauldronClickTimer < cauldronClickLimit;

    private CauldronSave cauldronSave => SaveSystem.save.cauldronSave;
    private uint[] potions => SaveSystem.save.marketSave.potionsCount;

    IEnumerator Start()
    {
        GameMaster.instance.ModeMaster.OnModeChange += ChangeScreen;

        ingredientPool = new ObjectPool<SpriteRenderer>(() =>
        {
            var ingredient = Instantiate(ingredientSprite, ingredientSprite.transform.parent);
            ingredient.transform.localPosition = ingredientSprite.transform.localPosition;
            ingredient.transform.localScale = Vector3.zero;
            ingredient.gameObject.SetActive(true);
            return ingredient;
        }, ingredient =>
        {
            ingredient.transform.localPosition = ingredientSprite.transform.localPosition;
            ingredient.transform.localScale = Vector3.zero;
            ingredient.sprite = MushroomBlock.GetMushroomSprite(currentIngredient);
            ingredient.gameObject.SetActive(true);
        }, ingredient => { ingredient.gameObject.SetActive(false); });

        yield return new WaitUntil(() => SaveSystem.instance != null);
        yield return new WaitUntil(() => SaveSystem.instance.loaded);

        foreach (CauldronIngredient preview in ingredientPreviews)
        {
            preview.Disable();
        }

        foreach (MushroomBlock.MushroomType saveIngredient in cauldronSave.ingredients)
        {
            ingredientPreviews[(int)saveIngredient].Enable();
            ingredientPreviews[(int)saveIngredient].SetIngredient(saveIngredient);
        }

        foreach (var renderer in fire)
        {
            renderer.enabled = cauldronSave.isOn && !cauldronSave.isDone;
        }

        foreach (var renderer in wood)
        {
            renderer.enabled = cauldronSave.hasFuel;
        }

        ingredientBar.SetAmounts(cauldronSave.ingredientAmounts, cauldronSave.ingredients);
        UpdateIngredient();
        UpdateNeededIngredients();
        UpdateButtons();
        fuelButton.gameObject.SetActive(!SaveSystem.save.cauldronSave.upgrades.autoWood);
        cauldronSave.hasFuel = cauldronSave.hasFuel || SaveSystem.save.cauldronSave.upgrades.autoWood;
        percentButtons.SetActive(SaveSystem.save.cauldronSave.upgrades.percentButtons);
        evenPotionButtons.SetActive(SaveSystem.save.cauldronSave.upgrades.evenAmount);
        if (cauldronSave.GetTotalIngredients() > GetMaxIngredients())
        {
            RemoveAllIngredients();
            cauldronSave.progress = 0;
            TurnOff();
        }
    }

    void Update()
    {
    }


    void FixedUpdate()
    {
        if (GameMaster.instance.ModeMaster.IsMode(ModeMaster.Gamemode.Potions))
        {
            curLevelText.text = "Lv. " + cauldronSave.level;
            cauldronCapacityText.text = GetMaxIngredients().ToString("N0");
            xpBar.fillAmount = cauldronSave.GetLevelProgress();
            fakeXpBar.fillAmount =
                Mathf.Lerp(fakeXpBar.fillAmount, xpBar.fillAmount, Time.fixedDeltaTime * xpBarFillSpeed);
        }

        ProgressState();
        progressMask.transform.localPosition =
            new Vector3(0, maskOffset * (cauldronSave.progress / cauldronSave.progressMax), 0);
        if (cauldronClickTimer >= cauldronClickLimit)
        {
            cauldronClickTimer -= Time.fixedDeltaTime;
        }
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (cauldronSave.isOn && !cauldronSave.isDone && canClick)
        {
            cauldronClickTimer = cauldronClickLimit;
            ClickCauldron();
        }
        else if (cauldronSave.isDone)
        {
            TakePotion();
        }
        else if (!cauldronSave.hasFuel)
        {
            AddFuel();
        }
    }

    private void ClickCauldron()
    {
        cauldronSave.progress += 1f + cauldronSave.upgrades.clickPower; //TODO good place to upgrade
        damageNumberMesh.Spawn(cauldronSprite.transform.position, 1 + cauldronSave.upgrades.clickPower);
        cauldronSprite.transform.DOComplete();
        cauldronSprite.transform.DOPunchScale(Vector3.one * punchSize, 0.1f, 1, 0.5f);
        uint xp = (uint)(1 + cauldronSave.upgrades.clickXpBonus);
        AddExperience(xp);
        SFXMaster.instance.PlayCauldronPop();
    }

    private void AddExperience(uint xp)
    {
        cauldronSave.xp += xp;
        xpNumberGUI.Spawn(xpPopSpawnPoint.position, xp).SetAnchoredPosition(xpPopSpawnPoint, Vector2.zero);
        if (cauldronSave.CheckLevelUp())
        {
            cauldronCapacityText.rectTransform.DOComplete();
            curLevelText.rectTransform.DOComplete();
            cauldronCapacityText.rectTransform.DOShakePosition(0.5f, new Vector3(0, 10, 0));
            curLevelText.rectTransform.DOPunchScale(Vector3.one * punchSize, 0.5f, 1, 0.5f);
        }
    }

    public void AddFuel()
    {
        cauldronSave.hasFuel = true;
        SFXMaster.instance.PlayWood();
        foreach (var renderer in wood)
        {
            renderer.enabled = true;
            renderer.transform.DOPunchScale(Vector3.one * punchSize, 0.5f, 1, 0.5f);
        }

        UpdateButtons();
    }

    private void CalculateIngredientTotal()
    {
        cauldronSave.ingredientTotal = 0;
        foreach (int amount in cauldronSave.ingredientAmounts)
        {
            cauldronSave.ingredientTotal += amount;
        }
    }

    public void TurnOn()
    {
        if (!cauldronSave.hasFuel) return;
        cauldronSave.progress = 0;
        SFXMaster.instance.PlayOneShot(brewSound);
        cauldronSave.isOn = true;
        foreach (var renderer in fire)
        {
            renderer.enabled = true;
            renderer.transform.DOPunchScale(Vector3.one * punchSize, 0.5f, 1, 0.5f);
        }

        foreach (CauldronIngredient preview in ingredientPreviews)
        {
            preview.hopping = true;
        }

        foreach (MushroomBlock.MushroomType mushroomType in cauldronSave.ingredients)
        {
            SaveSystem.save.stats.mushrooms[(int)mushroomType] -=
                (uint)cauldronSave.ingredientAmounts[cauldronSave.ingredients.IndexOf(mushroomType)];
            ScoreMaster.instance.UpdateMushroomText();
        }

        UpdateButtons();
        SaveSystem.SaveS();
    }


    public void TurnOff()
    {
        cauldronSave.isOn = false;
        foreach (var renderer in fire)
        {
            renderer.enabled = false;
        }

        cauldronSprite.transform.DOKill();
        cauldronSprite.transform.DORotate(Vector3.zero, 0.5f);
    }

    public void RemoveFuel()
    {
        if (SaveSystem.save.cauldronSave.upgrades.autoWood) return;
        cauldronSave.hasFuel = false;
        foreach (var renderer in wood)
        {
            renderer.enabled = false;
        }
    }

    public void AddIngredient()
    {
        ValidateValue();
        if (cauldronSave.GetTotalIngredients() >= GetMaxIngredients())
        {
            return;
        }

        AddIngredient(currentIngredient, int.Parse(ingredientAmountText.text));
        ValidateValue();
    }

    public int GetMaxIngredients()
    {
        return cauldronSave.GetMaxBrewAmount() * neededIngredients;
    }

    public void AddIngredient(MushroomBlock.MushroomType ingredient, int amount = 1)
    {
        if (amount <= 0)
        {
            return;
        }

        if (cauldronSave.ingredients.Contains(ingredient))
        {
            cauldronSave.ingredientAmounts[cauldronSave.ingredients.IndexOf(ingredient)] += amount;
        }
        else
        {
            cauldronSave.ingredients.Add(ingredient);
            cauldronSave.ingredientAmounts.Add(amount);
        }

        var ingredientSprite = ingredientPool.Get();
        ingredientSprite.sprite = MushroomBlock.GetMushroomSprite(ingredient);
        Transform ingredientSpriteTransform;
        (ingredientSpriteTransform = ingredientSprite.transform).DOKill();
        SFXMaster.instance.PlayMenuClick();
        ingredientSpriteTransform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutElastic).onComplete += () =>
        {
            ingredientSpriteTransform.DOLocalMoveY(-2f, 0.5f).SetEase(Ease.InBack).onComplete += () =>
            {
                SFXMaster.instance.PlayOneShot(addIngredientSound);
                cauldronSprite.transform.DOComplete();
                cauldronSprite.transform.DOPunchPosition(Vector3.down, 0.5f, 1, 0.5f);
                ingredientPreviews[cauldronSave.ingredients.IndexOf(ingredient)].Enable();
                ingredientPreviews[cauldronSave.ingredients.IndexOf(ingredient)].SetIngredient(ingredient);
            };
            ingredientSpriteTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).onComplete += () =>
            {
                ingredientPool.Release(ingredientSprite);
            };
        };

        CalculateIngredientTotal();
        UpdateNeededIngredients();
        cauldronSave.progressMax = baseBrewTime + (baseBrewTime *
                                                   ((cauldronSave.ingredientTotal - neededIngredients) /
                                                    neededIngredients) *
                                                   additionalBrewRatio); //TODO good place for upgrades
        if (SaveSystem.save.cauldronSave.upgrades.spoonEnchanted)
        {
            int total = 0;
            float percent = 0;
            for (var i = 0; i < cauldronSave.ingredients.Count; i++)
            {
                total += cauldronSave.ingredientAmounts[i];
            }

            for (var i = 0; i < cauldronSave.ingredients.Count; i++)
            {
                percent += MathF.Min((float)cauldronSave.ingredientAmounts[i] / total, .1f);
            }

            cauldronSave.progressMax -= cauldronSave.progressMax * percent;
        }

        ingredientBar.SetAmounts(cauldronSave.ingredientAmounts, cauldronSave.ingredients);
        int hours = (int)(cauldronSave.progressMax) / 3600;
        int minutes = (int)((cauldronSave.progressMax) % 3600) / 60;
        int seconds = (int)(cauldronSave.progressMax) % 60;

        timeLeftText.text = hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
        // timeLeftText.text =(cauldronSave.progressMax).ToString("F");
    }

    public void UpdateNeededIngredients()
    {
        neededIngredients = neededIngredientsBase -
                            (SaveSystem.save.cauldronSave.upgrades.potentShrooms
                                ? Mathf.FloorToInt(neededIngredientsBase * .25f)
                                : 0); //TODO good place for upgrades //TODO finish this
        ingredientBar.ratio = neededIngredients;
    }

    public void RemoveIngredient(MushroomBlock.MushroomType ingredient, int amount = 1)
    {
        if (cauldronSave.ingredients.Contains(ingredient))
        {
            cauldronSave.ingredientAmounts[cauldronSave.ingredients.IndexOf(ingredient)] -= amount;
            if (cauldronSave.ingredientAmounts[cauldronSave.ingredients.IndexOf(ingredient)] <= 0)
            {
                cauldronSave.ingredients.Remove(ingredient);
                cauldronSave.ingredientAmounts.Remove(
                    cauldronSave.ingredientAmounts[cauldronSave.ingredients.IndexOf(ingredient)]);
            }
        }
    }

    public void ChangeScreen()
    {
        if (cauldronSave.isOn) return;
        if (GameMaster.instance.ModeMaster.lastMode != ModeMaster.Gamemode.Potions) return;
        RemoveAllIngredients();
        UpdateButtons();
    }

    public void RemoveAllIngredients()
    {
        if (GameMaster.instance.ModeMaster.IsMode(ModeMaster.Gamemode.Potions))
        {
            SFXMaster.instance.PlayOneShot(removeIngredientSound);
        }

        cauldronSave.ingredients.Clear();
        cauldronSave.ingredientAmounts.Clear();
        ingredientBar.SetAmounts(cauldronSave.ingredientAmounts, cauldronSave.ingredients);
        timeLeftText.text = "";
        foreach (CauldronIngredient preview in ingredientPreviews)
        {
            preview.Disable();
        }

        UpdateButtons();
    }

    private void ProgressState()
    {
        if (cauldronSave.hasFuel && cauldronSave.isOn && !cauldronSave.isDone)
        {
            float progress = Time.fixedDeltaTime * progressSpeed;
            progress += SaveSystem.save.cauldronSave.upgrades.betterCauldron
                ? Time.fixedDeltaTime * progressSpeed * .25f
                : 0; //TODO good place for upgrades
            cauldronSave.progress += progress;
            //convert to hours, minutes and seconds left
            int hours = (int)(cauldronSave.progressMax - cauldronSave.progress) / 3600;
            int minutes = (int)((cauldronSave.progressMax - cauldronSave.progress) % 3600) / 60;
            int seconds = (int)(cauldronSave.progressMax - cauldronSave.progress) % 60;

            timeLeftText.text = hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
            // timeLeftText.text =(cauldronSave.progressMax - cauldronSave.progress).ToString("F");
            cauldronSprite.transform.rotation = Quaternion.Euler(0, 0,
                Mathf.Sin(Time.fixedTime * rotationSpeed) * (rotationDegree) *
                (cauldronSave.progress / cauldronSave.progressMax));
            if (cauldronSave.progress >= cauldronSave.progressMax)
            {
                SFXMaster.instance.PlayOneShot(finishSound);
                cauldronSave.isDone = true;
                timeLeftText.text = "!";
                // TurnOff();
                foreach (SpriteRenderer renderer in fire)
                {
                    renderer.enabled = false;
                }

                UpdateButtons();
                foreach (CauldronIngredient preview in ingredientPreviews)
                {
                    preview.hopping = false;
                }
            }
        }
        else if (cauldronSave.isDone)
        {
            timeLeftText.text = "!";
            cauldronSprite.transform.rotation = Quaternion.Euler(0, 0,
                Mathf.Sin(Time.fixedTime * rotationSpeed * 0.25f) * (rotationDegree * 2));
        }
    }

    public void TakePotion()
    {
        potionSprite.enabled = true;

        int random = Random.Range(0, cauldronSave.ingredientTotal);
        MushroomBlock.MushroomType potionType = MushroomBlock.MushroomType.Brown;
        int current = 0;
        for (int i = 0; i < cauldronSave.ingredients.Count; i++)
        {
            current += cauldronSave.ingredientAmounts[i];
            if (random < current)
            {
                potionType = cauldronSave.ingredients[i];
                potionSprite.sprite = GetPotionSprite(cauldronSave.ingredients[i]);
                break;
            }
        }

        int potionAmount =
            Mathf.FloorToInt(cauldronSave.ingredientTotal / (float)neededIngredients); //TODO good place for upgrades

        potions[(int)potionType] += (uint)potionAmount;

        AddExperience((uint)potionAmount *
                      (uint)((potionBaseXp + cauldronSave.upgrades.potionExperienceBonus) * potionCompleteMulti));

        potionSprite.transform.DOComplete();
        timeLeftText.text = "+" + potionAmount.ToString();
        SFXMaster.instance.PlayOneShot(gatherSound);
        potionSprite.transform.DOScale(Vector3.one * 1.5f, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            potionSprite.transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InBack).OnComplete(() =>
            {
                potionSprite.enabled = false;
                timeLeftText.text = "";
            });
        });
        potionSprite.transform.DOLocalJump(potionSprite.transform.localPosition, 3f, 1, 1.5f).SetEase(Ease.OutBack);
        TurnOff();
        RemoveFuel();
        cauldronSprite.transform.DOPunchScale(Vector3.one * punchSize, 0.5f, 1, 0.5f);
        cauldronSave.isDone = false;
        cauldronSave.progress = 0f;
        cauldronSave.ingredients.Clear();
        cauldronSave.ingredientAmounts.Clear();
        UpdateButtons();
        foreach (var ingredientPreview in ingredientPreviews)
        {
            ingredientPreview.Disable();
        }

        ingredientBar.SetAmounts(cauldronSave.ingredientAmounts, cauldronSave.ingredients);
    }

    public void UpdateButtons()
    {
        CalculateIngredientTotal();
        ProcessDesiredPotions();
        fuelButton.interactable = !cauldronSave.hasFuel && !cauldronSave.isOn;
        onButton.interactable = cauldronSave.ingredients.Count > 0 && cauldronSave.hasFuel &&
                                cauldronSave.ingredientTotal >= neededIngredients && !cauldronSave.isOn;
        gatherButton.interactable = cauldronSave.isDone;
        removeIngredientsButton.interactable = cauldronSave.ingredients.Count > 0 && !cauldronSave.isOn;
        addIngredientButton.interactable = !cauldronSave.isOn && !cauldronSave.isDone &&
                                           int.TryParse(ingredientAmountText.text, out int amount) && amount > 0;
    }

    public void NextIngredient()
    {
        currentIngredient++;
        if (currentIngredient > MushroomBlock.MushroomType.Blue)
        {
            currentIngredient = 0;
        }

        SFXMaster.instance.PlayMenuClick();
        UpdateIngredient();
        ValidateValue();
    }

    public void PreviousIngredient()
    {
        currentIngredient--;
        if (currentIngredient < 0)
        {
            currentIngredient = MushroomBlock.MushroomType.Blue;
        }

        SFXMaster.instance.PlayMenuClick();
        UpdateIngredient();
        ValidateValue();
    }

    private void UpdateIngredient()
    {
        ingredientPreviewImage.sprite = MushroomBlock.GetMushroomSprite(currentIngredient);
        ingredientNameText.text = currentIngredient.ToString();
    }

    public void ValidateValue()
    {
        if (int.TryParse(ingredientAmountText.text, out var value))
        {
            if (cauldronSave.GetTotalIngredients() + value >= GetMaxIngredients())
            {
                value = GetMaxIngredients() - cauldronSave.GetTotalIngredients();
            }

            if (value > 0)
            {
                int alreadyHave = 0;
                if (cauldronSave.ingredients.Contains(currentIngredient))
                {
                    alreadyHave = cauldronSave.ingredientAmounts[cauldronSave.ingredients.IndexOf(currentIngredient)];
                }

                if (value > SaveSystem.save.stats.mushrooms[(int)currentIngredient] - alreadyHave)
                {
                    value = Mathf.FloorToInt(SaveSystem.save.stats.mushrooms[(int)currentIngredient] - alreadyHave);
                }

                ingredientAmountText.text = value.ToString();

                UpdateButtons();
                return;
            }
        }

        ingredientAmountText.text = "0";
        UpdateButtons();
    }

    public void SetPercent(float percent)
    {
        int alreadyHave = 0;
        if (cauldronSave.ingredients.Contains(currentIngredient))
        {
            alreadyHave = cauldronSave.ingredientAmounts[cauldronSave.ingredients.IndexOf(currentIngredient)];
        }

        ingredientAmountText.text = Mathf.RoundToInt(GetMaxIngredients() * percent - alreadyHave)
            .ToString();
        ValidateValue();
        SFXMaster.instance.PlayMenuClick();
    }

    public void IncreasePotionAmount()
    {
        desiredPotions++;
        // ProcessDesiredPotions();
        ValidateValue();
        SFXMaster.instance.PlayMenuClick();
    }

    public void DecreasePotionAmount()
    {
        desiredPotions--;
        // ProcessDesiredPotions();
        ValidateValue();
        SFXMaster.instance.PlayMenuClick();
    }

    public void ValidateDesiredPotionAmount()
    {
        if (int.TryParse(desiredPotionsText.text, out var value))
        {
            if (value > 0)
            {
                desiredPotions = value;
            }
        }
        else
        {
            desiredPotions = 1;
        }

        if (desiredPotions > SaveSystem.save.stats.mushrooms[(int)currentIngredient] /
            neededIngredients)
        {
            desiredPotions =
                Mathf.FloorToInt(SaveSystem.save.stats.mushrooms[(int)currentIngredient] /
                                 (float)neededIngredients);
        }

        if (desiredPotions < 0)
        {
            desiredPotions = 0;
        }

        ingredientAmountText.text = neededIngredients * desiredPotions + "";

        ProcessDesiredPotions();
    }

    public void ProcessDesiredPotions()
    {
        increaseDesiredPotionsButton.interactable = desiredPotions <
                                                    SaveSystem.save.stats
                                                        .mushrooms[(int)currentIngredient] / neededIngredients;
        decreaseDesiredPotionsButton.interactable = desiredPotions > 1;
        desiredPotionsText.text = desiredPotions + "";
    }

    public static Sprite GetPotionSprite(MushroomBlock.MushroomType type)
    {
        return Resources.Load<Sprite>("Sprites/Potions/" + type + "Potion");
    }
}

[Serializable]
public class CauldronSave
{
    public bool isUnlocked = false;
    public CauldronUpgrades upgrades = new CauldronUpgrades();
    public float progressMax = 30f;
    public float progress = 0f;
    public bool hasFuel = false;
    public bool isOn = false;
    public bool isDone = false;
    public List<MushroomBlock.MushroomType> ingredients = new List<MushroomBlock.MushroomType>();
    public List<int> ingredientAmounts = new List<int>();
    public int ingredientTotal = 0;
    public uint xp = 0;
    public uint level = 1;
    private float experienceBaseMultiplier = 2.1f; //y
    private float experienceMultiplier = .1f; //x

    private float experienceToNextLevel => Mathf.Pow(level / experienceMultiplier, experienceBaseMultiplier);
    private float levelProgress => xp / experienceToNextLevel;

    public bool CheckLevelUp()
    {
        if (xp >= experienceToNextLevel)
        {
            xp -= (uint)experienceToNextLevel;
            level++;
            return true;
            // SFXMaster.instance.PlayLevelUp();//TODO add this
        }

        return false;
    }

    public float GetLevelProgress()
    {
        return levelProgress;
    }

    public int GetTotalIngredients()
    {
        int total = 0;
        foreach (int i in ingredientAmounts)
        {
            total += i;
        }

        return total;
    }

    public int GetMaxBrewAmount()
    {
        return (int)(level * 10 + upgrades.capacityBooster);
    }
}

[Serializable]
public class CauldronUpgrades
{
    public bool spoonEnchanted = false;
    public bool potentShrooms = false;
    public bool betterCauldron = false;
    public bool autoWood = false;
    public bool percentButtons = false;
    public bool evenAmount = false;

    [FormerlySerializedAs("cauldronClickPower")]
    public uint clickPower = 0;

    public uint capacityBooster = 0; //TODO: Implement
    public uint clickXpBonus = 0; //TODO: Implement
    public uint potionExperienceBonus = 0; //TODO: Implement
}
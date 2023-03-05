using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;



public class Cauldron : MonoBehaviour
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
    public float rotationSpeed = 5f;
    public float rotationDegree = 35f;
    public float punchSize = 1.5f;
    public float baseBrewTime = 30f;
    public float additionalBrewRatio = 0.25f;

    public SpriteRenderer ingredientSprite;
    public Image ingredientPreviewImage;
    public ObjectPool<SpriteRenderer> ingredientPool;
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

    private CauldronSave cauldronSave => SaveSystem.instance.GetSaveFile().cauldronSave;
    private uint[] potions => SaveSystem.instance.GetSaveFile().potionsCount;

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

        yield return new WaitUntil(()=> SaveSystem.instance != null);
        yield return new WaitUntil(()=> SaveSystem.instance.loaded);
        
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
            renderer.enabled = cauldronSave.isOn;
        }

        foreach (var renderer in wood)
        {
            renderer.enabled = cauldronSave.hasFuel;
        }
        ingredientBar.SetAmounts(cauldronSave.ingredientAmounts, cauldronSave.ingredients);
        UpdateIngredient();
        UpdateButtons();
    }

    void Update()
    {

    }

    public void AddFuel()
    {
        cauldronSave.hasFuel = true;
        foreach (var renderer in wood)
        {
            renderer.enabled = true;
            renderer.transform.DOPunchScale(Vector3.one * punchSize, 0.5f, 1, 0.5f);
        }
        UpdateButtons();
    }

    private void CheckFueledAndReady()
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
            SaveSystem.instance.GetSaveFile().mushrooms[(int)mushroomType] -= (uint)cauldronSave.ingredientAmounts[cauldronSave.ingredients.IndexOf(mushroomType)];
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
        cauldronSave.hasFuel = false;
        foreach (var renderer in wood)
        {
            renderer.enabled = false;
        }
    }

    public void AddIngredient()
    {
        ValidateValue();
        AddIngredient(currentIngredient, int.Parse(ingredientAmountText.text));
        ValidateValue();
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
        ingredientSpriteTransform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutElastic).onComplete += () =>
        {
            ingredientSpriteTransform.DOLocalMoveY(-2f, 0.5f).SetEase(Ease.InBack).onComplete += () =>
            {
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

        CheckFueledAndReady();
        cauldronSave.progressMax = baseBrewTime + (baseBrewTime * (cauldronSave.ingredientTotal-100) * 0.01f*additionalBrewRatio);//TODO good place for upgrades
        ingredientBar.SetAmounts(cauldronSave.ingredientAmounts, cauldronSave.ingredients);
        timeLeftText.text =(cauldronSave.progressMax).ToString("F");
    }

    public void RemoveIngredient(MushroomBlock.MushroomType ingredient, int amount = 1)
    {
        if (cauldronSave.ingredients.Contains(ingredient))
        {
            cauldronSave.ingredientAmounts[cauldronSave.ingredients.IndexOf(ingredient)] -= amount;
            if (cauldronSave.ingredientAmounts[cauldronSave.ingredients.IndexOf(ingredient)] <= 0)
            {
                cauldronSave.ingredients.Remove(ingredient);
                cauldronSave.ingredientAmounts.Remove(cauldronSave.ingredientAmounts[cauldronSave.ingredients.IndexOf(ingredient)]);
            }
        }
    }

    public void ChangeScreen()
    {
        if (cauldronSave.isOn) return;
        RemoveAllIngredients();
        UpdateButtons();
    }
    
    public void RemoveAllIngredients()
    {
        cauldronSave.ingredients.Clear();
        cauldronSave.ingredientAmounts.Clear();
        ingredientBar.SetAmounts(cauldronSave.ingredientAmounts, cauldronSave.ingredients);
        timeLeftText.text = "";
        foreach (CauldronIngredient preview in ingredientPreviews)
        {
            preview.Disable();
        }
        CheckFueledAndReady();
    }


    void FixedUpdate()
    {
        ProgressState();
        progressMask.transform.localPosition = new Vector3(0, maskOffset * (cauldronSave.progress / cauldronSave.progressMax), 0);
    }

    private void ProgressState()
    {
        if (cauldronSave.hasFuel && cauldronSave.isOn && !cauldronSave.isDone)
        {
            cauldronSave.progress += Time.fixedDeltaTime * progressSpeed; //TODO good place for upgrades
            timeLeftText.text =(cauldronSave.progressMax - cauldronSave.progress).ToString("F");
            cauldronSprite.transform.rotation = Quaternion.Euler(0, 0,
                Mathf.Sin(Time.fixedTime * rotationSpeed) * (rotationDegree) * (cauldronSave.progress / cauldronSave.progressMax));
            if (cauldronSave.progress >= cauldronSave.progressMax) //TODO good place for upgrades
            {
                cauldronSave.isDone = true;
                timeLeftText.text = "!";
                UpdateButtons();
                foreach (CauldronIngredient preview in ingredientPreviews)
                {
                    preview.hopping = false;
                }
            }
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

        int potionAmount = cauldronSave.ingredientTotal / 100;//TODO good place for upgrades
        
        potions[(int)potionType] += (uint)potionAmount;
        
        potionSprite.transform.DOComplete();
        timeLeftText.text = "+" + potionAmount.ToString();
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
        CheckFueledAndReady();
        fuelButton.interactable = !cauldronSave.hasFuel && !cauldronSave.isOn;
        onButton.interactable = cauldronSave.ingredients.Count > 0 && cauldronSave.hasFuel && cauldronSave.ingredientTotal >= 100 && !cauldronSave.isOn;
        gatherButton.interactable = cauldronSave.isDone;
        removeIngredientsButton.interactable = cauldronSave.ingredients.Count > 0 && !cauldronSave.isOn;
        addIngredientButton.interactable = !cauldronSave.isOn && !cauldronSave.isDone && int.TryParse(ingredientAmountText.text, out int amount) && amount > 0;

    }

    public void NextIngredient()
    {
        currentIngredient++;
        if (currentIngredient > MushroomBlock.MushroomType.Blue)
        {
            currentIngredient = 0;
        }

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
            if (value > 0)
            {
                int alreadyHave = 0;
                if (cauldronSave.ingredients.Contains(currentIngredient))
                {
                    alreadyHave = cauldronSave.ingredientAmounts[cauldronSave.ingredients.IndexOf(currentIngredient)];
                }
                if (value > SaveSystem.instance.GetSaveFile().mushrooms[(int)currentIngredient]-alreadyHave)
                {
                    ingredientAmountText.text = (SaveSystem.instance.GetSaveFile().mushrooms[(int)currentIngredient]-alreadyHave).ToString();
                }
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
        ingredientAmountText.text = Mathf.RoundToInt(
            (SaveSystem.instance.GetSaveFile().mushrooms[(int)currentIngredient]-alreadyHave) * percent).ToString();
        ValidateValue();
    }
    
    public static Sprite GetPotionSprite(MushroomBlock.MushroomType type)
    {
        return Resources.Load<Sprite>("Sprites/Potions/" + type + "Potion");
    }
}
[Serializable]
public class CauldronSave
{
    public float progressMax = 30f;
    public float progress = 0f;
    public bool hasFuel = false;
    public bool isOn = false;
    public bool isDone = false;
    public List<MushroomBlock.MushroomType> ingredients = new List<MushroomBlock.MushroomType>();
    public List<int> ingredientAmounts = new List<int>();
    public int ingredientTotal = 0;
}
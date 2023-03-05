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
    public float progressMax = 3f;
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

    //TODO extract this to a saveable class for save file
    public float progress = 0f;
    public bool hasFuel = false;
    public bool isOn = false;
    public bool isDone = false;
    public List<MushroomBlock.MushroomType> ingredients = new List<MushroomBlock.MushroomType>();
    public List<int> ingredientAmounts = new List<int>();
    public int ingredientTotal = 0;

    void Start()
    {
        fuelButton.interactable = true;
        onButton.interactable = false;
        gatherButton.interactable = false;
        foreach (var renderer in fire)
        {
            renderer.enabled = false;
        }

        foreach (var renderer in wood)
        {
            renderer.enabled = false;
        }

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

        foreach (CauldronIngredient preview in ingredientPreviews)
        {
            preview.Disable();
        }
        ingredientBar.SetAmounts(ingredientAmounts, ingredients);
        UpdateIngredient();
        CheckIngredientButton();
    }

    void Update()
    {

    }

    public void AddFuel()
    {
        hasFuel = true;
        foreach (var renderer in wood)
        {
            renderer.enabled = true;
            renderer.transform.DOPunchScale(Vector3.one * punchSize, 0.5f, 1, 0.5f);
        }

        fuelButton.interactable = false;
        CheckFueledAndReady();

    }

    private void CheckFueledAndReady()
    {
        ingredientTotal = 0;
        foreach (int amount in ingredientAmounts)
        {
            ingredientTotal += amount;
        }
        onButton.interactable = ingredients.Count > 0 && hasFuel && ingredientTotal >= 100;
        removeIngredientsButton.interactable = ingredients.Count > 0;
    }

    public void TurnOn()
    {
        if (!hasFuel) return;
        isOn = true;
        foreach (var renderer in fire)
        {
            renderer.enabled = true;
            renderer.transform.DOPunchScale(Vector3.one * punchSize, 0.5f, 1, 0.5f);
        }

        foreach (CauldronIngredient preview in ingredientPreviews)
        {
            preview.hopping = true;
        }
        
        foreach (MushroomBlock.MushroomType mushroomType in ingredients)
        {
            SaveSystem.instance.GetSaveFile().mushrooms[(int)mushroomType] -= (uint)ingredientAmounts[ingredients.IndexOf(mushroomType)];
        }
        removeIngredientsButton.interactable = false;
        onButton.interactable = false;
        CheckIngredientButton();
    }

    private void CheckIngredientButton()
    {
        addIngredientButton.interactable = !isOn && !isDone && int.TryParse(ingredientAmountText.text, out int amount) && amount > 0;
    }

    public void TurnOff()
    {
        isOn = false;
        foreach (var renderer in fire)
        {
            renderer.enabled = false;
        }

        cauldronSprite.transform.DOKill();
        cauldronSprite.transform.DORotate(Vector3.zero, 0.5f);

    }

    public void RemoveFuel()
    {
        hasFuel = false;
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
        
        if (ingredients.Contains(ingredient))
        {
            ingredientAmounts[ingredients.IndexOf(ingredient)] += amount;
        }
        else
        {
            ingredients.Add(ingredient);
            ingredientAmounts.Add(amount);
            
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
                ingredientPreviews[ingredients.IndexOf(ingredient)].Enable();
                ingredientPreviews[ingredients.IndexOf(ingredient)].SetIngredient(ingredient);
            };
            ingredientSpriteTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).onComplete += () =>
            {
                ingredientPool.Release(ingredientSprite);
            };
        };

        CheckFueledAndReady();
        progressMax = baseBrewTime + (baseBrewTime * (ingredientTotal-100) * 0.01f*additionalBrewRatio);//TODO good place for upgrades
        ingredientBar.SetAmounts(ingredientAmounts, ingredients);
        timeLeftText.text =(progressMax).ToString("F");
    }

    public void RemoveIngredient(MushroomBlock.MushroomType ingredient, int amount = 1)
    {
        if (ingredients.Contains(ingredient))
        {
            ingredientAmounts[ingredients.IndexOf(ingredient)] -= amount;
            if (ingredientAmounts[ingredients.IndexOf(ingredient)] <= 0)
            {
                ingredients.Remove(ingredient);
                ingredientAmounts.Remove(ingredientAmounts[ingredients.IndexOf(ingredient)]);
            }
        }
    }

    public void ChangeScreen()
    {
        if (!isOn)
        {
            RemoveAllIngredients();
        }
    }
    
    public void RemoveAllIngredients()
    {
        ingredients.Clear();
        ingredientAmounts.Clear();
        ingredientBar.SetAmounts(ingredientAmounts, ingredients);
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
        progressMask.transform.localPosition = new Vector3(0, maskOffset * (progress / progressMax), 0);
    }

    private void ProgressState()
    {
        if (hasFuel && isOn && !isDone)
        {
            progress += Time.fixedDeltaTime * progressSpeed; //TODO good place for upgrades
            timeLeftText.text =(progressMax - progress).ToString("F");
            cauldronSprite.transform.rotation = Quaternion.Euler(0, 0,
                Mathf.Sin(Time.fixedTime * rotationSpeed) * (rotationDegree) * (progress / progressMax));
            if (progress >= progressMax) //TODO good place for upgrades
            {
                isDone = true;
                timeLeftText.text = "!";
                gatherButton.interactable = true;
                foreach (CauldronIngredient preview in ingredientPreviews)
                {
                    preview.hopping = false;
                }
            }
        }
    }

    public void TakePotion()
    {
        //TODO add potion to inventory
        
        potionSprite.enabled = true;
        
        int random = Random.Range(0, ingredientTotal);
        int current = 0;
        for (int i = 0; i < ingredients.Count; i++)
        {
            current += ingredientAmounts[i];
            if (random < current)
            {
                potionSprite.sprite = GetPotionSprite(ingredients[i]);
                break;
            }
        }

        int potionAmount = ingredientTotal / 100;//TODO good place for upgrades
        
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
        isDone = false;
        progress = 0f;
        ingredients.Clear();
        ingredientAmounts.Clear();
        gatherButton.interactable = false;
        fuelButton.interactable = true;
        CheckIngredientButton();
        foreach (var ingredientPreview in ingredientPreviews)
        {
            ingredientPreview.Disable();
        }
        ingredientBar.SetAmounts(ingredientAmounts, ingredients);
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
                if (ingredients.Contains(currentIngredient))
                {
                    alreadyHave = ingredientAmounts[ingredients.IndexOf(currentIngredient)];
                }
                if (value > SaveSystem.instance.GetSaveFile().mushrooms[(int)currentIngredient]-alreadyHave)
                {
                    ingredientAmountText.text = (SaveSystem.instance.GetSaveFile().mushrooms[(int)currentIngredient]-alreadyHave).ToString();
                }
                CheckIngredientButton();
                return;
            }
        }

        ingredientAmountText.text = "0";
        CheckIngredientButton();
    }

    public void SetPercent(float percent)
    {
        int alreadyHave = 0;
        if (ingredients.Contains(currentIngredient))
        {
            alreadyHave = ingredientAmounts[ingredients.IndexOf(currentIngredient)];
        }
        ingredientAmountText.text = Mathf.RoundToInt(
            (SaveSystem.instance.GetSaveFile().mushrooms[(int)currentIngredient]-alreadyHave) * percent).ToString();
        ValidateValue();
    }
    
    public Sprite GetPotionSprite(MushroomBlock.MushroomType type)
    {
        return Resources.Load<Sprite>("Sprites/Potions/" + type + "Potion");
    }
}
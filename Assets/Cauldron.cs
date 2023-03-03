using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Cauldron : MonoBehaviour
{
    public SpriteRenderer cauldronSprite;
    public SpriteRenderer progressSprite;
    public SpriteRenderer[] fire;
    public SpriteRenderer[] wood;
    public Button fuelButton;
    public Button onButton;
    public Button gatherButton;
    public Button addIngredientButton;
    public SpriteRenderer potionSprite;
    public SpriteMask progressMask;
    public float progress = 0f;
    public float progressSpeed = 1f;
    public float progressMax = 3f;
    public float maskOffset = 2.25f;
    public float rotationSpeed = 5f;
    public float rotationDegree = 35f;
    public float punchSize = 1.5f;
    public SpriteRenderer ingredientSprite;

    //TODO extract this to a saveable class for save file
    public bool hasFuel = false;
    public bool isOn = false;
    public bool isDone = false;
    public List<MushroomBlock.MushroomType> ingredients = new List<MushroomBlock.MushroomType>();
    public List<int> ingredientAmounts = new List<int>();
    
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
        onButton.interactable = ingredients.Count > 0 && hasFuel;
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
        onButton.interactable = false;
        addIngredientButton.interactable = false;
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

    public void TestAddIngredient()
    {
        AddIngredient(MushroomBlock.MushroomType.Brown);
    }
      
    public void AddIngredient(MushroomBlock.MushroomType ingredient, int amount = 1)
    {
        ingredientSprite.sprite = MushroomBlock.GetMushroomSprite(ingredient);
        Transform ingredientSpriteTransform;
        (ingredientSpriteTransform = ingredientSprite.transform).DOKill();
        ingredientSpriteTransform.localScale = Vector3.zero;
        ingredientSpriteTransform.localPosition = new Vector3(0,0,-0.5f);
        ingredientSpriteTransform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutElastic).onComplete += () =>
        {
            ingredientSpriteTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
            ingredientSpriteTransform.DOLocalMoveY(-4f, 0.5f).SetEase(Ease.InBack);
            cauldronSprite.transform.DOPunchPosition(Vector3.down, 0.5f, 1, 0.5f).SetDelay(0.5f);
        };
        if (ingredients.Contains(ingredient))
        {
            ingredientAmounts[ingredients.IndexOf(ingredient)]+=amount;
        }
        else
        {
            ingredients.Add(ingredient);
            ingredientAmounts.Add(amount);
        }
        CheckFueledAndReady();
        
    }
    
    public void RemoveIngredient(MushroomBlock.MushroomType ingredient, int amount = 1)
    {
        if (ingredients.Contains(ingredient))
        {
            ingredientAmounts[ingredients.IndexOf(ingredient)]-=amount;
            if (ingredientAmounts[ingredients.IndexOf(ingredient)] <= 0)
            {
                ingredients.Remove(ingredient);
                ingredientAmounts.Remove(ingredientAmounts[ingredients.IndexOf(ingredient)]);
            }
        }
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
            progress += Time.fixedDeltaTime * progressSpeed;//TODO good place for upgrades
            cauldronSprite.transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.fixedTime * rotationSpeed) * (rotationDegree)* (progress / progressMax));
            if (progress >= progressMax)//TODO good place for upgrades
            {
                isDone = true;
                TurnOff();
                RemoveFuel();
                gatherButton.interactable = true;
            }
        }
    }

    public void TakePotion()
    {
        //TODO add potion to inventory
        potionSprite.enabled = true;
        potionSprite.transform.DOScale(Vector3.one * 1.5f, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            potionSprite.transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InBack).OnComplete(() =>
            {
                potionSprite.enabled = false;
            });
        });
        potionSprite.transform.DOLocalJump(potionSprite.transform.localPosition, 3f, 1, 1.5f).SetEase(Ease.OutBack);
        TurnOff();
        cauldronSprite.transform.DOPunchScale(Vector3.one * punchSize, 0.5f,1, 0.5f); 
        isDone = false;
        progress = 0f;
        ingredients.Clear();
        ingredientAmounts.Clear();
        gatherButton.interactable = false;
        fuelButton.interactable = true;
        addIngredientButton.interactable = true;
    }
}

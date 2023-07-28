using System;

public class CauldronCurrencyVisualizer : CurrencyVisualizer
{
    public override void SetCurrency(Currency currency)
    {
        this.currency = currency;
        switch (currency)
        {
            case Currency.BrownMushroom:
                if (SaveSystem.save.cauldronSave.isOn)
                {
                    text.text = SaveSystem.save.stats.mushrooms[0].ToString("N0");
                    break;
                }

                uint brownMushrooms = SaveSystem.save.stats.mushrooms[0];
                if (SaveSystem.save.cauldronSave.ingredients.Contains(MushroomBlock.MushroomType.Brown))
                {
                    int saveIndex = SaveSystem.save.cauldronSave.ingredients.IndexOf(MushroomBlock.MushroomType.Brown);
                    brownMushrooms -= (uint)SaveSystem.save.cauldronSave.ingredientAmounts[saveIndex];
                }

                text.text = brownMushrooms.ToString("N0");
                break;
            case Currency.RedMushroom:
                if (SaveSystem.save.cauldronSave.isOn)
                {
                    text.text = SaveSystem.save.stats.mushrooms[1].ToString("N0");
                    break;
                }

                uint redMushrooms = SaveSystem.save.stats.mushrooms[1];
                if (SaveSystem.save.cauldronSave.ingredients.Contains(MushroomBlock.MushroomType.Red))
                {
                    int saveIndex = SaveSystem.save.cauldronSave.ingredients.IndexOf(MushroomBlock.MushroomType.Red);
                    redMushrooms -= (uint)SaveSystem.save.cauldronSave.ingredientAmounts[saveIndex];
                }

                text.text = redMushrooms.ToString("N0");
                break;
            case Currency.BlueMushroom:
                if (SaveSystem.save.cauldronSave.isOn)
                {
                    text.text = SaveSystem.save.stats.mushrooms[2].ToString("N0");
                    break;
                }

                uint blueMushrooms = SaveSystem.save.stats.mushrooms[2];
                if (SaveSystem.save.cauldronSave.ingredients.Contains(MushroomBlock.MushroomType.Blue))
                {
                    int saveIndex = SaveSystem.save.cauldronSave.ingredients.IndexOf(MushroomBlock.MushroomType.Blue);
                    blueMushrooms -= (uint)SaveSystem.save.cauldronSave.ingredientAmounts[saveIndex];
                }

                text.text = blueMushrooms.ToString("N0");
                break;
            case Currency.Spore:
                text.text = SaveSystem.save.stats.spores.ToString("N0");
                break;
            case Currency.SkillPoint:
                text.text = SaveSystem.save.stats.skillPoints.ToString("N0");
                break;
            case Currency.BrownPotion:
                text.text = SaveSystem.save.marketSave.potionsCount[0].ToString("N0");
                break;
            case Currency.RedPotion:
                text.text = SaveSystem.save.marketSave.potionsCount[1].ToString("N0");
                break;
            case Currency.BluePotion:
                text.text = SaveSystem.save.marketSave.potionsCount[2].ToString("N0");
                break;
            case Currency.Coin:
                text.text = SaveSystem.save.marketSave.coins.ToString("N0");
                break;
            case Currency.PlinkoBall:
                text.text = SaveSystem.save.plinkoSave.balls.ToString("N0");
                break;
            case Currency.Collectible:
                text.text = SaveSystem.GetCollectionMultiplier().ToString("N0");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(currency), currency, null);
        }
    }
}
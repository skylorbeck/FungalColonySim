using System;

public class UpgradeToolTipData : ToolTipData
{
    public enum PurchaseLimit
    {
        One,
        Multiple,
    }

    public PurchaseLimit purchaseLimit = PurchaseLimit.Multiple;

    public void OnEnable()
    {
        switch (purchaseLimit)
        {
            case PurchaseLimit.One:
                tooltipHeader = "One Purchase";
                break;
            case PurchaseLimit.Multiple:
                tooltipHeader = "Multiple Purchases";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlinkoPrizeAwarder : MonoBehaviour
{
        public CollectionItemSaveData currentPrize;
        public PrizeType prizeType;
        public void UpdatePrize()
        {
                if (prizeType!=PrizeType.Collectible) return;
                currentPrize = GameMaster.instance.Hivemind.collectionShelf.GenerateSaveData();
        }

        public void Start()
        {
                UpdatePrize();
        }

        public void OnTriggerEnter2D(Collider2D col)
        {
                if (col.gameObject.CompareTag("PlinkoBall"))
                {
                        switch (prizeType)
                        {
                                case PrizeType.Collectible:
                                        GameMaster.instance.Hivemind.plinkoMachine.AwardCollectible();
                                        break;
                                case PrizeType.NonCollectible:
                                        GameMaster.instance.Hivemind.plinkoMachine.AwardNonCollectible();
                                        break;
                                default:
                                        throw new ArgumentOutOfRangeException();
                        }
                }
        }
        
        public enum PrizeType
        {
                Collectible,
                NonCollectible,
        }
}
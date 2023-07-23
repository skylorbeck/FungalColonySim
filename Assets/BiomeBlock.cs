using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class BiomeBlock : Block
{
    [SerializeField] public Biome biome;
    private Biome biomePrevious;

    void Start()
    {
        UpdateSprite();
    }

    protected override void Update()
    {
        if (biome != biomePrevious)
        {
            biomePrevious = biome;
            UpdateSprite();
        }

        base.Update();
    }

    void FixedUpdate()
    {
    }

    private void UpdateSprite()
    {
        float sum = blockPos.x + blockPos.y + blockPos.z;
        string path = "Sprites/Blocks/" + biome + Mathf.FloorToInt(Mathf.Abs(sum % 3));
        this.name = "BiomeBlock " + biome + " " + blockPos;
        spriteRenderer.sprite = Resources.Load<Sprite>(path);
    }

    public void PlaceBlock(int3 blockPos, Biome biome, bool isInstant = true)
    {
        PlaceBlock(blockPos, isInstant);
        this.biome = biome;
        biomePrevious = biome;
        UpdateSprite();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        //TODO harvest the mushroom above
        base.OnPointerClick(eventData);
    }
}

public enum Biome
{
    Rock,
    Dirt,
    Grass,
    RockyGrass,
}
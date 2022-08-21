using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AnttiStarterKit.Extensions;

public class Flasher : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> sprites;
    [SerializeField] private List<LineRenderer> lines;
    [SerializeField] private List<SpriteRenderer> spritesNeedingShader;
    [SerializeField] private Material whiteMaterial;

    [SerializeField] private float duration = 0.1f;
    
    private List<Color> spriteColors, lineColors;
    private List<Material> defaultMaterials;

    private void Start()
    {
        spriteColors = sprites.Select(s => s.color).ToList();
        lineColors = lines.Select(l => l.startColor).ToList();
        if (spritesNeedingShader.Any())
        {
            defaultMaterials = spritesNeedingShader.Select(s => s.material).ToList();
        }
    }

    public void Flash()
    {
        Colorize(Color.white);
        ChangeMaterials(whiteMaterial);
        this.StartCoroutine(() =>
        {
            Colorize();
            ChangeMaterials();
        }, duration);
    }
    
    private void ChangeMaterials()
    {
        spritesNeedingShader.ForEach(s => s.material = defaultMaterials[spritesNeedingShader.IndexOf(s)]);
    }

    private void ChangeMaterials(Material material)
    {
        spritesNeedingShader.ForEach(s => s.material = material);
    }

    private void Colorize(Color color)
    {
        sprites.ForEach(s => s.color = color);
        lines.ForEach(l => l.startColor = l.endColor = color);
    }

    private void Colorize()
    {
        sprites.ForEach(s => s.color = spriteColors[sprites.IndexOf(s)]);
        lines.ForEach(l => l.startColor = l.endColor = lineColors[lines.IndexOf(l)]);
    }
}
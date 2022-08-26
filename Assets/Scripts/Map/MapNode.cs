using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace Map
{
    public class MapNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private LineRenderer linePrefab;
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private Color dimColor = Color.black;
        [SerializeField] private Color activeColor = Color.white;
        [SerializeField] private Pulsate pulsate;
        [SerializeField] private GameObject outline;
        [SerializeField] private SpriteRenderer icon;
        [SerializeField] private List<Sprite> icons;

        private readonly List<MapNode> connections = new();
        private readonly List<MapNode> backConnections = new();

        private readonly List<LineRenderer> lines = new();

        private int x, y;
        private bool canPick;
        private MapIcon iconType;
        
        public WorldMap WorldMap { get; set; }
    
        public bool IsDone => backConnections.Any();
        public bool HasConnections => connections.Any();

        public void ConnectTo(MapNode node)
        {
            var line = Instantiate(linePrefab, transform);
            line.SetPosition(0, transform.position);
            line.SetPosition(1, node.transform.position);
        
            lines.Add(line);
        
            connections.Add(node);
            node.backConnections.Add(this);
        }

        public void Dim()
        {
            Colorize(dimColor);
        }

        public void Activate()
        {
            Colorize(activeColor);
            connections.ForEach(c => c.Open());
            MakeActive();
        }

        private void MakeActive()
        {
            sprite.sortingOrder += 10;
            lines.ForEach(l => l.sortingOrder += 5);
            ShowIcon();
            
            connections.ForEach(c => c.MakeActive());
        }

        private void Open()
        {
            canPick = true;
            pulsate.enabled = true;
        }

        private void Colorize(Color color)
        {
            sprite.color = color;

            lines.ForEach(l =>
            {
                l.startColor = color;
                l.endColor = color;
            });

            connections.ForEach(c => c.Colorize(color));
        }

        public void SetupPosition(int xPos, int yPos)
        {
            x = xPos;
            y = yPos;
        }

        public void Pick()
        {
            if (!canPick) return;
            MapState.Instance.x = x;
            MapState.Instance.y = y;
            
            WorldMap.OnPick(this);
            this.StartCoroutine(() => SceneChanger.Instance.ChangeScene(GetScene()), 1.5f);
        }

        private string GetScene()
        {
            return iconType switch
            {
                MapIcon.None => "Map",
                MapIcon.Fight => "Main",
                MapIcon.Boss => "Main",
                MapIcon.Star => "Chest",
                MapIcon.Shop => "Shop",
                MapIcon.Unknown => "Chest",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private string GetMysteryScene()
        {
            if (Random.value < 0.1f)
            {
                StateManager.Instance.ExtraBoss = true;
                return "Main";
            }
            
            return Random.value switch
            {
                < 0.25f => "Shop",
                < 0.5f => "Chest",
                _ => "Main"
            };
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!canPick) return;
            outline.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!canPick) return;
            outline.SetActive(false);
        }

        public void SetType(MapIcon type)
        {
            iconType = type;
            icon.sprite = icons[(int)type];
        }
        
        public void HideIcon()
        {
            icon.gameObject.SetActive(false);
        }

        public void ShowIcon()
        {
            icon.gameObject.SetActive(true);
        }

        public void DisablePick()
        {
            canPick = false;
        }
    }
}

public enum MapIcon
{
    None,
    Fight,
    Boss,
    Star,
    Shop,
    Unknown
}
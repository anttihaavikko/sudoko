using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using UnityEngine;
using UnityEngine.EventSystems;

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

        private readonly List<MapNode> connections = new();
        private readonly List<MapNode> backConnections = new();

        private readonly List<LineRenderer> lines = new();

        private int x, y;
        private bool canPick;
    
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
            SceneChanger.Instance.ChangeScene("Main");
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
    }
}
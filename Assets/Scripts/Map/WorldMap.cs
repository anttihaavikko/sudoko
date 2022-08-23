using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Map
{
    public class WorldMap : MonoBehaviour
    {
        [SerializeField] private MapNode nodePrefab;
        [SerializeField] private Transform currentMarker, pickLine;

        private const float HorizontalStep = 3.5f;
        private const float VerticalStep = 2.25f;
        private const int Levels = 7;

        private readonly List<List<MapNode>> nodes = new();

        private void Start()
        {
            var state = MapState.Instance;

            if (state.x == Levels - 1)
            {
                state.NextWorld();
            }
            
            state.Seed();
            
            GenerateNodes();
            ConnectNodes();

            nodes[0].Random().Dim();
            var node = nodes[state.x][state.y];
            node.Activate();

            currentMarker.position = node.transform.position;
            pickLine.position = pickLine.position.WhereX(nodes[state.x + 1].First().transform.position.x); 
            
            Random.InitState(System.Environment.TickCount);
        }

        private void GenerateNodes()
        {
            for (var ix = 0; ix < Levels; ix++)
            {
                var level = new List<MapNode>();

                var perLevel = NodeCountFor(ix);

                for (var iy = 0; iy < perLevel; iy++)
                {
                    var x = -Levels * 0.5f * HorizontalStep + HorizontalStep * 0.5f + ix * HorizontalStep;
                    var y = perLevel * 0.5f * VerticalStep - VerticalStep * 0.5f - iy * VerticalStep;
                    var p = new Vector3(x, y, 0);

                    var node = AddNode(p + transform.position);
                    level.Add(node);
                    node.SetupPosition(ix, iy);
                    
                    var t = node.transform;

                    if (ix == 0)
                    {
                        t.position += Vector3.left * HorizontalStep * 0.5f;
                    }

                    if (ix == Levels - 1)
                    {
                        t.localScale *= 1.75f;
                        t.position += Vector3.right * HorizontalStep * 0.5f;
                    }
                }

                nodes.Add(level);
            }
        }

        private void ConnectNodes()
        {
            for (var i = 0; i < nodes.Count - 1; i++)
            {
                nodes[i].First().ConnectTo(nodes[i + 1].First());
                nodes[i].Last().ConnectTo(nodes[i + 1].Last());
            
                foreach (var node in nodes[i])
                {
                    if (!node.HasConnections)
                    {
                        node.ConnectTo(nodes[i + 1].Random());
                    }

                    while (i > 0 && !node.IsDone)
                    {
                        nodes[i - 1].Random().ConnectTo(node);
                    }
                }
            }
        }

        private int NodeCountFor(int level)
        {
            if (level is 0 or Levels - 1) return 1;
            if (level == 1) return 2;
            return Random.Range(2, 6);
        }

        public MapNode AddNode(Vector3 pos)
        {
            var node = Instantiate(nodePrefab, transform);
            node.transform.position = pos;
            return node;
        }
    }
}
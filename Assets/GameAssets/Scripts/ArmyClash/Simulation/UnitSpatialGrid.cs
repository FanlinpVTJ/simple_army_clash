using System;
using System.Collections.Generic;
using Scellecs.Morpeh;
using SimpleArmyClash.Ecs;
using UnityEngine;

namespace SimpleArmyClash.Simulation
{
    public sealed class UnitSpatialGrid : IUnitSpatialIndex
    {
        private const float MINIMUM_CELL_SIZE = 1f;

        private readonly Dictionary<Vector3Int, int> _cellLookup;
        private readonly CellComparer _horizontalComparer;
        private readonly CellComparer _verticalComparer;
        private readonly int[] _armyRoots = { -1, -1 };
        private Entry[] _entries;
        private Cell[] _cells;
        private Node[] _nodes;
        private int[] _orderedCells;
        private int _entryCount;
        private int _cellCount;
        private int _nodeCount;

        public float MaximumRadius { get; private set; }
        public long CandidateChecks { get; private set; }
        public long NodeVisits { get; private set; }

        private struct Entry
        {
            public Entity Entity;
            public Vector3 Position;
            public int Identifier;
            public int Next;
        }

        private struct Cell
        {
            public Vector3Int Coordinate;
            public Vector3 Minimum;
            public Vector3 Maximum;
            public int Head;
        }

        private struct Node
        {
            public Vector3 Minimum;
            public Vector3 Maximum;
            public int CellIndex;
            public int Left;
            public int Right;
        }

        private struct NearestCandidate
        {
            public Entity Entity;
            public int Identifier;
            public float SquaredDistance;
            public bool Found;
        }

        private sealed class CellComparer : IComparer<int>
        {
            private readonly UnitSpatialGrid _grid;
            private readonly bool _horizontal;

            public CellComparer(UnitSpatialGrid grid, bool horizontal)
            {
                _grid = grid;
                _horizontal = horizontal;
            }

            public int Compare(int first, int second)
            {
                Vector3Int firstCoordinate = _grid._cells[first].Coordinate;
                Vector3Int secondCoordinate = _grid._cells[second].Coordinate;
                int comparison = _horizontal
                    ? firstCoordinate.x.CompareTo(secondCoordinate.x)
                    : firstCoordinate.z.CompareTo(secondCoordinate.z);

                if (comparison != 0)
                {
                    return comparison;
                }

                return _horizontal
                    ? firstCoordinate.z.CompareTo(secondCoordinate.z)
                    : firstCoordinate.x.CompareTo(secondCoordinate.x);
            }
        }

        public UnitSpatialGrid(int capacity)
        {
            capacity = Math.Max(1, capacity);
            _cellLookup = new Dictionary<Vector3Int, int>(capacity);
            _entries = new Entry[capacity];
            _cells = new Cell[capacity];
            _nodes = new Node[capacity * 2];
            _orderedCells = new int[capacity];
            _horizontalComparer = new CellComparer(this, true);
            _verticalComparer = new CellComparer(this, false);
        }

        public void Rebuild(UnitWorld units, float meleeReach)
        {
            EnsureCapacity(units.UnitCount);
            _cellLookup.Clear();
            _entryCount = 0;
            _cellCount = 0;
            _nodeCount = 0;
            MaximumRadius = 0f;
            CandidateChecks = 0;
            NodeVisits = 0;

            foreach (Entity entity in units.LivingUnits)
            {
                if (units.Health.Get(entity).Current > 0)
                {
                    MaximumRadius = Mathf.Max(MaximumRadius, units.Positions.Get(entity).Radius);
                }
            }

            float cellSize = Mathf.Max(MINIMUM_CELL_SIZE, MaximumRadius * 2f + meleeReach);

            foreach (Entity entity in units.LivingUnits)
            {
                if (units.Health.Get(entity).Current <= 0)
                {
                    continue;
                }

                ref UnitComponent unit = ref units.Units.Get(entity);
                Add(entity, unit.Identifier, unit.ArmyIndex, units.Positions.Get(entity).Value, cellSize);
            }

            int orderedCount = 0;

            for (int armyIndex = 0; armyIndex < _armyRoots.Length; armyIndex++)
            {
                int start = orderedCount;

                for (int i = 0; i < _cellCount; i++)
                {
                    if (_cells[i].Coordinate.y == armyIndex)
                    {
                        _orderedCells[orderedCount++] = i;
                    }
                }

                _armyRoots[armyIndex] = start == orderedCount ? -1 : BuildTree(start, orderedCount - start);
            }
        }

        public Entity FindNearestEnemy(Vector3 position, int armyIndex, out bool hasTarget)
        {
            NearestCandidate nearest = new NearestCandidate
            {
                Identifier = int.MaxValue,
                SquaredDistance = float.PositiveInfinity
            };
            int root = _armyRoots[1 - armyIndex];

            if (root >= 0)
            {
                FindNearest(root, position, ref nearest);
            }

            hasTarget = nearest.Found;
            return nearest.Entity;
        }

        public void CollectNeighbors(Vector3 position, float radius, List<Entity> results)
        {
            results.Clear();
            float squaredRadius = radius * radius;

            for (int i = 0; i < _armyRoots.Length; i++)
            {
                if (_armyRoots[i] >= 0)
                {
                    Collect(_armyRoots[i], position, squaredRadius, results);
                }
            }
        }

        private void EnsureCapacity(int capacity)
        {
            if (capacity <= _entries.Length)
            {
                return;
            }

            int newCapacity = Math.Max(capacity, _entries.Length * 2);
            Array.Resize(ref _entries, newCapacity);
            Array.Resize(ref _cells, newCapacity);
            Array.Resize(ref _nodes, newCapacity * 2);
            Array.Resize(ref _orderedCells, newCapacity);
        }

        private void Add(Entity entity, int identifier, int armyIndex, Vector3 position, float cellSize)
        {
            Vector3Int coordinate = new Vector3Int(
                Mathf.FloorToInt(position.x / cellSize), armyIndex, Mathf.FloorToInt(position.z / cellSize));

            if (!_cellLookup.TryGetValue(coordinate, out int cellIndex))
            {
                cellIndex = _cellCount++;
                _cellLookup.Add(coordinate, cellIndex);
                _cells[cellIndex] = new Cell
                {
                    Coordinate = coordinate,
                    Minimum = position,
                    Maximum = position,
                    Head = -1
                };
            }

            ref Cell cell = ref _cells[cellIndex];
            cell.Minimum = Vector3.Min(cell.Minimum, position);
            cell.Maximum = Vector3.Max(cell.Maximum, position);
            _entries[_entryCount] = new Entry
            {
                Entity = entity,
                Identifier = identifier,
                Position = position,
                Next = cell.Head
            };
            cell.Head = _entryCount++;
        }

        private int BuildTree(int start, int count)
        {
            int nodeIndex = _nodeCount++;
            Cell first = _cells[_orderedCells[start]];
            Node node = new Node
            {
                Minimum = first.Minimum,
                Maximum = first.Maximum,
                CellIndex = -1,
                Left = -1,
                Right = -1
            };

            for (int i = start + 1; i < start + count; i++)
            {
                Cell cell = _cells[_orderedCells[i]];
                node.Minimum = Vector3.Min(node.Minimum, cell.Minimum);
                node.Maximum = Vector3.Max(node.Maximum, cell.Maximum);
            }

            if (count == 1)
            {
                node.CellIndex = _orderedCells[start];
            }
            else
            {
                bool horizontal = node.Maximum.x - node.Minimum.x >= node.Maximum.z - node.Minimum.z;
                Array.Sort(_orderedCells, start, count, horizontal ? _horizontalComparer : _verticalComparer);
                int leftCount = count / 2;
                node.Left = BuildTree(start, leftCount);
                node.Right = BuildTree(start + leftCount, count - leftCount);
            }

            _nodes[nodeIndex] = node;
            return nodeIndex;
        }

        private void FindNearest(int nodeIndex, Vector3 position, ref NearestCandidate nearest)
        {
            NodeVisits++;
            ref Node node = ref _nodes[nodeIndex];

            if (DistanceToBounds(position, node) > nearest.SquaredDistance)
            {
                return;
            }

            if (node.CellIndex >= 0)
            {
                int entryIndex = _cells[node.CellIndex].Head;

                while (entryIndex >= 0)
                {
                    ref Entry entry = ref _entries[entryIndex];
                    CandidateChecks++;
                    float squaredDistance = SquaredDistance(position, entry.Position);

                    if (squaredDistance < nearest.SquaredDistance
                        || squaredDistance == nearest.SquaredDistance && entry.Identifier < nearest.Identifier)
                    {
                        nearest.Entity = entry.Entity;
                        nearest.Identifier = entry.Identifier;
                        nearest.SquaredDistance = squaredDistance;
                        nearest.Found = true;
                    }

                    entryIndex = entry.Next;
                }

                return;
            }

            int first = node.Left;
            int second = node.Right;

            if (DistanceToBounds(position, _nodes[second]) < DistanceToBounds(position, _nodes[first]))
            {
                first = node.Right;
                second = node.Left;
            }

            FindNearest(first, position, ref nearest);
            FindNearest(second, position, ref nearest);
        }

        private void Collect(int nodeIndex, Vector3 position, float squaredRadius, List<Entity> results)
        {
            NodeVisits++;
            ref Node node = ref _nodes[nodeIndex];

            if (DistanceToBounds(position, node) > squaredRadius)
            {
                return;
            }

            if (node.CellIndex >= 0)
            {
                int entryIndex = _cells[node.CellIndex].Head;

                while (entryIndex >= 0)
                {
                    ref Entry entry = ref _entries[entryIndex];
                    CandidateChecks++;

                    if (SquaredDistance(position, entry.Position) <= squaredRadius)
                    {
                        results.Add(entry.Entity);
                    }

                    entryIndex = entry.Next;
                }

                return;
            }

            Collect(node.Left, position, squaredRadius, results);
            Collect(node.Right, position, squaredRadius, results);
        }

        private float DistanceToBounds(Vector3 position, Node node)
        {
            float horizontal = Mathf.Max(node.Minimum.x - position.x, 0f);
            horizontal = Mathf.Max(horizontal, position.x - node.Maximum.x);
            float vertical = Mathf.Max(node.Minimum.z - position.z, 0f);
            vertical = Mathf.Max(vertical, position.z - node.Maximum.z);
            return horizontal * horizontal + vertical * vertical;
        }

        private float SquaredDistance(Vector3 first, Vector3 second)
        {
            float horizontal = first.x - second.x;
            float vertical = first.z - second.z;
            return horizontal * horizontal + vertical * vertical;
        }
    }
}

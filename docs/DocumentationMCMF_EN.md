# Assignment Module: Min-Cost Max-Flow (MCMF) Algorithm - Strict Constraints Version

## 1. Problem Description

* **Problem:** Assigning dwarves to mines with the lowest possible total cost (distance). The goal is to find an arrangement where every employed dwarf covers the minimum possible distance to work.

* **Strict Constraints:** Assignments must perfectly match the dwarf's preferences (profession). If preferred mines are at full capacity, or if a dwarf has no preferences at all, they are not assigned to any mine (they remain unemployed).

* **Algorithms:** The core concept of the Ford-Fulkerson algorithm (for maximizing flow) combined with the Bellman-Ford algorithm to find the minimum-cost path in the residual network.

## 2. Mathematical Modeling (Residual Network)

* **Source and Sink:** Artificially added vertices required for flow algorithms. The Source connects to all dwarves, and all mines connect to the Sink.

* **Nodes:** Represent real-world domain entities: Dwarf and Mine.

* **Edges and Capacities:** Edges represent connections between entities. An edge between a dwarf and a mine is created **strictly** if the mine's resource is explicitly listed in the dwarf's preferences.

* **Costs:** The cost is the value paid for traversing an edge. In this model, it represents the actual physical distance between the dwarf's home and the mine (scaled up during calculations to avoid floating-point precision issues).

## 3. Algorithm Execution

1. **Initialization:** Building the residual network based on input data, rigorously omitting any non-preferred connections.

2. **Pathfinding:** Using the Bellman-Ford algorithm to find the cheapest augmenting path from the Source to the Sink, bypassing fully saturated edges.

3. **Flow Update and Backward Edges:** After finding an optimal path, the algorithm pushes a unit of flow through it. Simultaneously, **backward edges** with an inverted (negative) cost are made available. This allows the algorithm to "undo" a dwarf's assignment in subsequent iterations if another candidate is found for whom that specific mine slot is globally cheaper. This is the foundation of resource conflict resolution.

## 4. Architecture and System Components

* `McmfMapper`: A class responsible for mapping business models to graph objects.

* `ResidualNetwork`: A structure holding the graph state and capacities. Its constructor is responsible for the strict filtering of dwarf preferences.

* `MinCostMaxFlowProblem`: The core engine orchestrating the algorithm's loop. It returns the total cost, maximum flow (number of employed dwarves), and a final list of optimal assignments (`AssignmentDto`).

## 5. Computational Complexity

* **Single Iteration Complexity:** Finding the shortest path using the Bellman-Ford algorithm in a graph with no negative-weight cycles takes $O(V \cdot E)$ time.

* **Total Complexity:** Dependent on the total flow $F$ (the final number of assigned dwarves). The algorithm operates in a worst-case time of $O(F \cdot V \cdot E)$.

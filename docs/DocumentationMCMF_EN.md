# Assignment Module: Min-Cost Max-Flow (MCMF) Algorithm - Penalty Version (Soft Constraints)

## 1. Problem Description

* **Problem**: Assigning dwarves to mines with the lowest possible total cost. The goal is to find an arrangement that minimizes the travel distance for each dwarf. However, strict priority is given to assignments that match the dwarf's preferences (profession).

* **Optimization**: Maintaining maximum flow (employing the highest possible number of dwarves). If preferred mines are full, the algorithm assigns unemployed dwarves to the nearest available mines (ignoring preferences), applying an appropriate penalty for this fallback assignment.

* **Algorithms**: The core concept of the Ford-Fulkerson algorithm (for maximizing flow) combined with the Bellman-Ford algorithm for finding the minimum-cost path in the residual network.

## 2. Mathematical Modeling (Residual Network)

* **Source and Sink**: Artificially added vertices required for flow algorithms. The Source connects to all dwarves, and all mines connect to the Sink.

* **Nodes**: Represent real-world entities: Dwarf and Mine.
Edges and Capacities: Edges represent connections between entities in the graph. Capacity defines how many units (dwarves) can pass through a given edge (e.g., the Mine -> Sink edge has a capacity equal to the number of slots in the mine).

* **Costs and Penalties**: The cost is the value paid for traversing an edge (actual physical distance). The NON_PREFERRED_PENALTY enforces adherence to preferences. It is added to the edge cost (Dwarf -> Mine) when a given mineral is not on the dwarf's preference list. This ensures Bellman-Ford always prioritizes cheaper, preferred connections first.

## 3. Algorithm Execution

* **Initialization**: Building the residual network based on input data.

* **Pathfinding**: Using the Bellman-Ford algorithm to find the cheapest augmenting path from the Source to the Sink, bypassing fully saturated edges.

* **Flow Update and Backward Edges**: After finding a path, the algorithm pushes a unit of flow through it (reducing available capacity). Simultaneously, the algorithm makes the same flow value available on backward edges, which have an inverted (negative) cost. This provides the mathematical mechanism to "undo" a dwarf's assignment to a mine in subsequent iterations if another candidate is found for whom that spot is more optimal. This is the primary conflict resolution mechanism.

## 4. Architecture and System Components

* `McmfMapper`: A class responsible for mapping domain (business) models to graph objects tailored for the algorithm.
* `ResidualNetwork`: A structure that holds the graph state, vertices, edges, and current capacities at any given moment.
* `MinCostMaxFlowProblem`: The core engine orchestrating the algorithm's loop. It searches for augmenting paths with minimal cost and calculates the maximum flow and raw minimum cost (distances combined with applied penalties).

## 5. Computational Complexity
* **Single Iteration Complexity**: Finding the shortest path using the Bellman-Ford algorithm takes $O(V \cdot E)$ time.
* **Total Complexity**: Dependent on the total flow F (the number of finally assigned dwarves). The entire algorithm executes in a worst-case time of $O(F \cdot V \cdot E)$.

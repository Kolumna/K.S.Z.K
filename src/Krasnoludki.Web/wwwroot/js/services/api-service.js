const MapApiService = {
  async _sendRequest(handlerName, payload) {
    const res = await fetch(`?handler=${handlerName}`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
      },
      body: JSON.stringify(payload),
    });

    if (!res.ok) {
      const err = await res.json().catch(() => ({}));
      throw new Error(err.message || `Błąd serwera HTTP: ${res.status}`);
    }

    return await res.json();
  },

  async calculateConvexHull(pointsArray) {
    if (!pointsArray || pointsArray.length < 3) {
      throw new Error("Required at least 3 points to calculate convex hull.");
    }
    return await this._sendRequest("CalculateGraham", pointsArray);
  },

  async calculateMatching(matchingPayload) {
    if (
      !matchingPayload ||
      !matchingPayload.dwarves ||
      !matchingPayload.mines
    ) {
      throw new Error("Invalid payload for matching algorithm.");
    }
    console.log("Sending matching payload to API:", matchingPayload);
    return await this._sendRequest("CalculateMatching", matchingPayload);
  },

  async calculateSegmentTree(dwarfesForRmq) {
    if (!dwarfesForRmq || dwarfesForRmq.length < 3) {
      throw new Error("Required at least 3 points to calculate convex hull.");
    }
    return await this._sendRequest("CalculateSegmentTree", dwarfesForRmq);
  },

  async calculateMinCost(minCostPayload) {
    if (
      !minCostPayload ||
      !minCostPayload.dwarves ||
      !minCostPayload.mines
    ) {
      throw new Error("Invalid payload for min cost algorithm.");
    }
    return await this._sendRequest("CalculateMinCost", minCostPayload);
  }
};

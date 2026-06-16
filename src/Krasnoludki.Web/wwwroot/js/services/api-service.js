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
      throw new Error("Wymagane co najmniej 3 punkty do obliczenia otoczki.");
    }
    return await this._sendRequest("CalculateGraham", pointsArray);
  },

  async calculateMatching(matchingPayload) {
    if (
      !matchingPayload ||
      !matchingPayload.dwarves ||
      !matchingPayload.mines
    ) {
      throw new Error("Nieprawidłowa treść żądania dla algorytmu dopasowywania.");
    }
    console.log("Wysyłanie treści żądania dopasowywania do API:", matchingPayload);
    return await this._sendRequest("CalculateMatching", matchingPayload);
  },

  async calculateSegmentTree(dwarfesForRmq) {
    return await this._sendRequest("CalculateSegmentTree", dwarfesForRmq);
  },

  async calculateMinCost(minCostPayload) {
    if (
      !minCostPayload ||
      !minCostPayload.dwarves ||
      !minCostPayload.mines
    ) {
      throw new Error("Nieprawidłowa treść żądania dla algorytmu minimalnego kosztu.");
    }
    return await this._sendRequest("CalculateMinCost", minCostPayload);
  }
};

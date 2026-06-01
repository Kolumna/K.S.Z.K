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
      throw new Error(
        err.message || `Błąd serwera HTTP: ${res.status}`,
      );
    }

    return await res.json();
  },

  // Otoczka wypukła
  async calculateConvexHull(pointsArray) {
    if (!pointsArray || pointsArray.length < 3) {
      throw new Error("Required at least 3 points to calculate convex hull.");
    }
    return await this._sendRequest("CalculateGraham", pointsArray);
  },
};

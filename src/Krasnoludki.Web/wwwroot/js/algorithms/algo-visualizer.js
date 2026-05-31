const algorithmResults =
  typeof ALGORITHM_RESULTS !== "undefined" ? ALGORITHM_RESULTS : {};

async function runAlgorithm(algorithmType) {
  if (!algorithmType || algorithmType === "") {
    return;
  }

  const btn = document.getElementById("algo-run-button");
  btn.disabled = true;
  btn.innerText = "Trwają obliczenia...";

  try {
    let res;

    switch (algorithmType) {
      case "convexHull": // Otoczka wypukła
        const pointsPayload = INITIAL_NODES.map((n) => ({ x: n.x, y: n.y }));

        res = await MapApiService.calculateConvexHull(pointsPayload);

        if (res.success) {
          drawConvexHullOverlay(res.data);
        }
        break;
      default:
        throw new Error("Nieznany typ algorytmu: " + algorithmType);
    }

    if (res && res.success) {
      console.log("Jest git");
    } else if (res) {
      alert("Błąd: " + res.message);
    }
  } catch (err) {
    console.error("Error while running algorithm:", err);
    alert("Wystąpił błąd podczas wykonywania algorytmu: " + err.message);
  } finally {
    btn.disabled = false;
    btn.innerText = "Zapisz";
  }
}

function drawConvexHullOverlay(hullPoints) {
  const canvas = document.getElementById("algoCanvas");
  const ctx = canvas.getContext("2d");

  ctx.beginPath();
  ctx.strokeStyle = "rgba(231, 76, 60, 0.8)";
  ctx.lineWidth = 4;
  ctx.setLineDash([5, 5]);

  console.log(hullPoints);

  hullPoints.forEach((point, index) => {
    if (index === 0) {
      ctx.moveTo(point.x, point.y);
    } else {
      ctx.lineTo(point.x, point.y);
    }
  });

  ctx.closePath();
  ctx.stroke();
  ctx.setLineDash([]);
}

function loadAlgorithmResults() {
  const selectedAlgorithm = new URLSearchParams(window.location.search).get(
    "algorithm",
  );

  console.log("Selected algorithm from URL:", selectedAlgorithm);
  if (algorithmResults.convexHull && selectedAlgorithm === "convexHull") {
    drawConvexHullOverlay(algorithmResults.convexHull.hullPoints);
  }
}

document.addEventListener("DOMContentLoaded", () => {
  // setupCanvas();
  loadAlgorithmResults();
});

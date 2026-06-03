const algorithmResults =
  typeof ALGORITHM_RESULTS !== "undefined" ? ALGORITHM_RESULTS : {};

async function runAlgorithm(algorithmType) {
  if (!algorithmType || algorithmType === "") {
    return;
  }

  const btn = document.getElementById("algo-run-button");
  btn.disabled = true;
  btn.innerText = "Trwają obliczenia...";

  console.log("INITIAL_NODES", INITIAL_NODES);

  try {
    let res;

    switch (algorithmType) {
      case "convexHull":
        const pointsPayload = INITIAL_NODES.map((n) => ({ x: n.x, y: n.y }));

        res = await MapApiService.calculateConvexHull(pointsPayload);

        if (res.success) {
          drawConvexHullOverlay(res.data);
        }
        break;
      case "matching":
        const nodesWithCorrectIds = INITIAL_NODES.map((node, index) => ({
          ...node,
          id: index + 1,
        }));

        const dwarves = nodesWithCorrectIds
          .filter((n) => n.type === "dwarf")
          .map((n) => ({
            pointId: n.id,
            x: n.x,
            y: n.y,
            preferredMinerals: n.minerals,
            voiceLoudness: n.loudness,
          }));
        const mines = nodesWithCorrectIds
          .filter((n) => n.type === "mine")
          .map((n) => ({
            pointId: n.id,
            x: n.x,
            y: n.y,
            resource: n.minerals[0],
            capacity: n.capacity,
          }));
        const matchingPayload = { dwarves, mines };

        console.log("Payload JSON:", JSON.stringify(matchingPayload, null, 2));

        console.log("INITIAL_NODES", INITIAL_NODES);

        console.log("Sending matching payload:", matchingPayload);

        res = await MapApiService.calculateMatching(matchingPayload);

        console.log("Matching API response:", res.data);

        if (res.success) {
          drawDwarfAssignments(res.data);
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

function drawDwarfAssignments(matchingData) {
  const canvas = document.getElementById("algoCanvas");
  const ctx = canvas.getContext("2d");

  console.log("Matching data received:", matchingData);

  ctx.clearRect(0, 0, canvas.width, canvas.height);

  matchingData.assignments.forEach((assignment) => {
    const dwarf = INITIAL_NODES.find(
      (n) => n.type === "dwarf" && n.id === Number(assignment.dwarfId),
    );
    const mine = INITIAL_NODES.find(
      (n) => n.type === "mine" && n.id === Number(assignment.mineId),
    );

    if (dwarf && mine) {
      ctx.beginPath();
      ctx.strokeStyle = "#4ade80";
      ctx.lineWidth = 2;
      ctx.setLineDash([6, 3]);
      ctx.moveTo(dwarf.x, dwarf.y);
      ctx.lineTo(mine.x, mine.y);
      ctx.stroke();
      ctx.setLineDash([]);
    } else {
      console.warn(
        `Could not find dwarf ${assignment.dwarfId} or mine ${assignment.mineId}`,
      );
    }
  });

  INITIAL_NODES.filter((n) => n.type === "dwarf").forEach((dwarf) => {
    ctx.beginPath();
    ctx.arc(dwarf.x, dwarf.y, 10, 0, Math.PI * 2);
    ctx.fillStyle = "#3b82f6";
    ctx.fill();
  });

  INITIAL_NODES.filter((n) => n.type === "mine").forEach((mine) => {
    const size = 20;
    ctx.beginPath();
    ctx.rect(mine.x - size / 2, mine.y - size / 2, size, size);
    ctx.fillStyle = "#f59e0b";
    ctx.fill();
  });
}

function loadAlgorithmResults() {
  const selectedAlgorithm = new URLSearchParams(window.location.search).get(
    "algorithm",
  );

  console.log("Selected algorithm from URL:", selectedAlgorithm);
  if (algorithmResults.convexHull && selectedAlgorithm === "convexHull") {
    drawConvexHullOverlay(algorithmResults.convexHull.hullPoints);
  } else if (algorithmResults.matching && selectedAlgorithm === "matching") {
    drawDwarfAssignments(algorithmResults.matching);
  }
}

document.addEventListener("DOMContentLoaded", () => {
  // setupCanvas();
  loadAlgorithmResults();
});

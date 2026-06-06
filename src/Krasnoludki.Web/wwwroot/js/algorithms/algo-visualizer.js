const algorithmResults =
  typeof ALGORITHM_RESULTS !== "undefined" ? ALGORITHM_RESULTS : {};

const canvas = document.getElementById("algoCanvas");
const ctx = canvas.getContext("2d");

let rmqSelectedL = -1;
let rmqSelectedR = -1;

let rmqSelectionBox = { minX: -1, maxX: -1, minY: -1, maxY: -1 };

let isDragging = false;
let dragStart = { x: 0, y: 0 };
let dragEnd = { x: 0, y: 0 };

function getCanvasCoords(event) {
  const rect = canvas.getBoundingClientRect();
  return {
    x: (event.clientX - rect.left) * (canvas.width / rect.width),
    y: (event.clientY - rect.top) * (canvas.height / rect.height),
  };
}

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
        const dwarves = INITIAL_NODES.filter((n) => n.type === "dwarf").map(
          (n) => ({
            pointId: n.id,
            x: n.x,
            y: n.y,
            preferredMinerals: n.minerals,
            voiceLoudness: n.loudness,
          }),
        );
        const mines = INITIAL_NODES.filter((n) => n.type === "mine").map(
          (n) => ({
            pointId: n.id,
            x: n.x,
            y: n.y,
            resource: n.minerals[0],
            capacity: n.capacity,
          }),
        );
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
      case "rmq":
        const dwarvesForRmq = INITIAL_NODES.filter(
          (n) => n.type === "dwarf",
        ).map((n) => ({
          pointId: n.id,
          x: n.x,
          y: n.y,
          voiceLoudness: n.loudness,
        }));
        console.log("Dwarves for RMQ:", dwarvesForRmq);
        res = await MapApiService.calculateSegmentTree(dwarvesForRmq);

        console.log(res);

        if (res.success) {
          drawLoudestDwarf(res.data.loudestDwarfId);
        }
        break;
      case "minCost":
        const mcmfDwarves = INITIAL_NODES.filter((n) => n.type === "dwarf").map(
          (n, i) => ({
            pointId: n.id,
            x: n.x,
            y: n.y,
            preferredMinerals: n.minerals,
            voiceLoudness: n.loudness ?? 0,
          }),
        );

        const mcmfMines = INITIAL_NODES.filter((n) => n.type === "mine").map(
          (n, i) => ({
            pointId: n.id,
            x: n.x,
            y: n.y,
            resource: n.minerals[0],
            capacity: n.capacity ?? 1,
          }),
        );

        res = await MapApiService.calculateMinCost({
          dwarves: mcmfDwarves,
          mines: mcmfMines,
        });

        if (res.success) drawMinCostResult(res.data);
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

  if (algorithmResults.convexHull && selectedAlgorithm === "convexHull") {
    drawConvexHullOverlay(algorithmResults.convexHull.hullPoints);
  } else if (algorithmResults.matching && selectedAlgorithm === "matching") {
    drawDwarfAssignments(algorithmResults.matching);
  } else if (algorithmResults.segmentTree && selectedAlgorithm === "rmq") {
    drawLoudestDwarf(algorithmResults.segmentTree.loudestDwarfId);
  } else if (algorithmResults.minCost && selectedAlgorithm === "minCost") {
    drawMinCostResult(algorithmResults.minCost);
  }
}

function getSelectedAlgorithm() {
  return new URLSearchParams(window.location.search).get("algorithm");
}

function drawDwarvesForRmq() {
  ctx.clearRect(0, 0, canvas.width, canvas.height);

  INITIAL_NODES.filter((n) => n.type === "mine").forEach((mine) => {
    const size = 20;
    ctx.beginPath();
    ctx.rect(mine.x - size / 2, mine.y - size / 2, size, size);
    ctx.fillStyle = "#f59e0b";
    ctx.fill();
  });

  const rmqDwarves = INITIAL_NODES.filter((node) => node.type === "dwarf").sort(
    (a, b) => a.x - b.x,
  );

  rmqDwarves.forEach((dwarf, index) => {
    let isSelected = false;

    if (isDragging) {
      const minX = Math.min(dragStart.x, dragEnd.x);
      const maxX = Math.max(dragStart.x, dragEnd.x);
      const minY = Math.min(dragStart.y, dragEnd.y);
      const maxY = Math.max(dragStart.y, dragEnd.y);

      isSelected =
        dwarf.x >= minX &&
        dwarf.x <= maxX &&
        dwarf.y >= minY &&
        dwarf.y <= maxY;
    } else if (rmqSelectionBox.minX !== -1) {
      isSelected =
        dwarf.x >= rmqSelectionBox.minX &&
        dwarf.x <= rmqSelectionBox.maxX &&
        dwarf.y >= rmqSelectionBox.minY &&
        dwarf.y <= rmqSelectionBox.maxY;
    }

    if (isSelected) {
      ctx.beginPath();
      ctx.fillStyle = "rgba(230, 126, 34, 0.3)";
      ctx.arc(dwarf.x, dwarf.y, 25, 0, 2 * Math.PI);
      ctx.fill();
    }

    ctx.beginPath();
    ctx.fillStyle = isSelected ? "#e67e22" : "#34495e";
    ctx.arc(dwarf.x, dwarf.y, 15, 0, 2 * Math.PI);
    ctx.fill();

    ctx.strokeStyle = isSelected ? "#f1c40f" : "#2c3e50";
    ctx.lineWidth = isSelected ? 3 : 1.5;
    ctx.stroke();

    ctx.fillStyle = isSelected ? "#f1c40f" : "#bdc3c7";
    ctx.font = isSelected ? "bold 15px Georgia" : "13px Georgia";
    ctx.textAlign = "center";
    ctx.fillText(`[${dwarf.id}]`, dwarf.x, dwarf.y + 32);
  });

  if (isDragging) {
    ctx.save();
    ctx.beginPath();
    ctx.fillStyle = "rgba(241, 196, 15, 0.15)";
    ctx.strokeStyle = "#f1c40f";
    ctx.lineWidth = 1.5;
    ctx.setLineDash([4, 4]);

    const width = dragEnd.x - dragStart.x;
    const height = dragEnd.y - dragStart.y;

    ctx.fillRect(dragStart.x, dragStart.y, width, height);
    ctx.strokeRect(dragStart.x, dragStart.y, width, height);
    ctx.restore();
  }

  if (
    algorithmResults.segmentTree &&
    algorithmResults.segmentTree.loudestDwarfId
  ) {
    drawLoudestDwarf(algorithmResults.segmentTree.loudestDwarfId);
  }
}

function drawLoudestDwarf(dwarfId) {
  const dwarf = INITIAL_NODES.find(
    (n) => n.type === "dwarf" && Number(n.id) === Number(dwarfId),
  );

  if (dwarf) {
    ctx.beginPath();
    ctx.fillStyle = "rgba(241, 196, 15, 0.5)";
    ctx.arc(dwarf.x, dwarf.y, 30, 0, Math.PI * 2);
    ctx.fill();
    ctx.beginPath();
    ctx.fillStyle = "#f39c12";
    ctx.arc(dwarf.x, dwarf.y, 20, 0, Math.PI * 2);
    ctx.fill();
    ctx.strokeStyle = "#d35400";
    ctx.lineWidth = 3;
    ctx.stroke();
  }
}

canvas.addEventListener("mousemove", function (e) {
  if (!isDragging) return;

  const coords = getCanvasCoords(e);
  dragEnd.x = coords.x;
  dragEnd.y = coords.y;

  drawDwarvesForRmq();
});

canvas.addEventListener("mousedown", function (e) {
  const selectedAlgorithm = new URLSearchParams(window.location.search).get(
    "algorithm",
  );
  if (selectedAlgorithm !== "rmq") return;

  rmqSelectedL = -1;
  rmqSelectedR = -1;
  rmqLoudestDwarfId = null;
  rmqSelectionBox = { minX: -1, maxX: -1, minY: -1, maxY: -1 };

  isDragging = true;
  const coords = getCanvasCoords(e);
  dragStart.x = coords.x;
  dragStart.y = coords.y;
  dragEnd.x = coords.x;
  dragEnd.y = coords.y;

  drawDwarvesForRmq();
});

canvas.addEventListener("mouseup", function (e) {
  if (!isDragging) return;
  isDragging = false;

  const rmqDwarves = INITIAL_NODES.filter((node) => node.type === "dwarf").sort(
    (a, b) => a.x - b.x,
  );

  const minX = Math.min(dragStart.x, dragEnd.x);
  const maxX = Math.max(dragStart.x, dragEnd.x);
  const minY = Math.min(dragStart.y, dragEnd.y);
  const maxY = Math.max(dragStart.y, dragEnd.y);

  rmqSelectionBox = { minX, maxX, minY, maxY };

  let selectedIndices = [];
  rmqDwarves.forEach((dwarf, index) => {
    if (
      dwarf.x >= minX &&
      dwarf.x <= maxX &&
      dwarf.y >= minY &&
      dwarf.y <= maxY
    ) {
      selectedIndices.push(index);
    }
  });

  if (selectedIndices.length > 0) {
    rmqSelectedL = Math.min(...selectedIndices);
    rmqSelectedR = Math.max(...selectedIndices);
  }

  drawDwarvesForRmq();
});

document.addEventListener("DOMContentLoaded", () => {
  // setupCanvas();
  loadAlgorithmResults();

  if (getSelectedAlgorithm() === "rmq") {
    drawDwarvesForRmq();
  }
  if(getSelectedAlgorithm() === "minCost"){
    drawMinCostResult(algorithmResults.minCost);
  }
});

function drawAllNodes() {
  INITIAL_NODES.forEach((node) => {
    const x = node.x;
    const y = node.y;

    if (node.type === "dwarf") {
      ctx.beginPath();
      ctx.arc(x, y, 14, 0, Math.PI * 2);
      ctx.fillStyle = "#1a3a5a";
      ctx.strokeStyle = "#4a90c0";
      ctx.lineWidth = 2;
      ctx.fill();
      ctx.stroke();
      ctx.fillStyle = "#80c0f0";
    } else {
      const s = 12;
      ctx.beginPath();
      ctx.rect(x - s, y - s, s * 2, s * 2);
      ctx.fillStyle = "#2a1a08";
      ctx.strokeStyle = "#c07030";
      ctx.lineWidth = 2;
      ctx.fill();
      ctx.stroke();
      ctx.fillStyle = "#e8c060";
    }
  });
}

function drawMinCostResult(data) {
  console.log("Min cost result:", data);
  ctx.clearRect(0, 0, canvas.width, canvas.height);

  const dwarves = INITIAL_NODES.filter((n) => n.type === "dwarf");
  const mines = INITIAL_NODES.filter((n) => n.type === "mine");

  dwarves.forEach((d) => {
    mines.forEach((m) => {
      if (!d.minerals.some((min) => m.minerals.includes(min))) return;
      ctx.beginPath();
      ctx.strokeStyle = "rgba(60,55,40,0.2)";
      ctx.lineWidth = 1;
      ctx.setLineDash([4, 4]);
      ctx.moveTo(d.x, d.y);
      ctx.lineTo(m.x, m.y);
      ctx.stroke();
      ctx.setLineDash([]);
    });
  });

  data.assignments.forEach((a) => {
    const dwarf = dwarves.find((d) => Number(d.id) === Number(a.dwarfId));
    const mine = mines.find((m) => Number(m.id) === Number(a.mineId));

    if (!dwarf || !mine) {
      console.warn(`Nie znaleziono: dwarf=${a.dwarfId} mine=${a.mineId}`);
      return;
    }

    const dist = Math.sqrt(
      Math.pow(dwarf.x - mine.x, 2) + Math.pow(dwarf.y - mine.y, 2),
    );
    const ratio = Math.min(dist / 500, 1);
    const r = Math.round(ratio * 200);
    const g = Math.round((1 - ratio) * 170 + 80);
    const color = a.isPenalized ? "#a060c0" : `rgb(${r},${g},50)`;

    ctx.beginPath();
    ctx.strokeStyle = color;
    ctx.lineWidth = a.isPenalized ? 1.5 : 2.5;
    ctx.setLineDash(a.isPenalized ? [4, 4] : []);
    ctx.moveTo(dwarf.x, dwarf.y);
    ctx.lineTo(mine.x, mine.y);
    ctx.stroke();
    ctx.setLineDash([]);

    const mx = (dwarf.x + mine.x) / 2;
    const my = (dwarf.y + mine.y) / 2;
    ctx.fillStyle = color;
    ctx.font = "9px Georgia";
    ctx.textAlign = "center";
    ctx.fillText(dist.toFixed(0), mx, my - 4);
  });

  drawAllNodes();
}

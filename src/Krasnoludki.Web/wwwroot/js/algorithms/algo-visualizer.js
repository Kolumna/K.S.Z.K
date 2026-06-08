const algorithmResults =
  typeof ALGORITHM_RESULTS !== "undefined" ? ALGORITHM_RESULTS : {};

const canvas = document.getElementById("algoCanvas");
const ctx = canvas.getContext("2d");

let selectedDwarfs = [];

const camera = {
  x: 0,
  y: 0,
  zoom: 1,
  minZoom: 0.2,
  maxZoom: 5,
};

let isPanning = false;
let panStart = { x: 0, y: 0 };

function screenToWorld(sx, sy) {
  return {
    x: (sx - camera.x) / camera.zoom,
    y: (sy - camera.y) / camera.zoom,
  };
}

function applyCamera() {
  ctx.setTransform(camera.zoom, 0, 0, camera.zoom, camera.x, camera.y);
}

function resetTransform() {
  ctx.setTransform(1, 0, 0, 1, 0, 0);
}

function drawScene(drawFn) {
  resetTransform();
  ctx.clearRect(0, 0, canvas.width, canvas.height);

  applyCamera();
  drawFn();

  resetTransform();
}

let rmqSelectionBox = { minX: -1, maxX: -1, minY: -1, maxY: -1 };
let isDragging = false;
let dragStart = { x: 0, y: 0 };
let dragEnd = { x: 0, y: 0 };

function setupCanvas() {
  const dpr = window.devicePixelRatio || 1;
  const rect = canvas.getBoundingClientRect();
  canvas.width = rect.width * dpr;
  canvas.height = rect.height * dpr;
  ctx.scale(dpr, dpr);
  canvas.style.width = `${rect.width}px`;
  canvas.style.height = `${rect.height}px`;
}

function getCanvasCoords(event) {
  const rect = canvas.getBoundingClientRect();
  return {
    x: (event.clientX - rect.left) * (canvas.width / rect.width),
    y: (event.clientY - rect.top) * (canvas.height / rect.height),
  };
}

function getCW() {
  return canvas.getBoundingClientRect().width;
}
function getCH() {
  return canvas.getBoundingClientRect().height;
}

function drawNodeGraphics(x, y, type) {
  ctx.beginPath();
  ctx.arc(x, y, 20, 0, Math.PI * 2);
  ctx.fillStyle =
    type === "dwarf" ? "#8B4513" : window.MINERAL_COLORS?.[type] || "#888";
  ctx.fill();
  ctx.strokeStyle = "#000";
  ctx.lineWidth = 2;
  ctx.stroke();

  const label = type.charAt(0).toUpperCase();
  ctx.fillStyle = "white";
  ctx.font = "bold 14px Arial";
  ctx.textAlign = "center";
  ctx.textBaseline = "middle";
  ctx.fillText(label, x, y);
}

function drawDwarf(node, options = {}) {
  const {
    highlight = false,
    highlightColor = "#62e662",
    isLoudest = false,
    showIndex = false,
    index = 0,
    showLoudness = false,
  } = options;

  const x = node.x;
  const y = node.y;

  if (highlight) {
    ctx.beginPath();
    ctx.arc(x, y, 26, 0, Math.PI * 2);
    ctx.fillStyle = `${highlightColor}`;
    ctx.fill();
  }

  drawNodeGraphics(x, y, node.type);

  ctx.fillStyle = "#fff";
  ctx.font = "16px Arial";
  node.minerals.forEach((mineral, i) => {
    ctx.fillText(mineral, x - 48, y + 24 + i * 18);
  });

  if (showIndex) {
    ctx.fillStyle = "#fff";
    ctx.font = "16px Arial";
    ctx.fillText(`id: ${index}`, x, y + 24);
  }

  if (showLoudness && node.loudness) {
    ctx.fillStyle = "#fff";
    ctx.font = "16px Arial";
    ctx.textBaseline = "bottom";
    ctx.fillText(`${node.loudness}dB`, x, y - 17);

    if (isLoudest) {
      ctx.fillStyle = "#5fff5f";
    }
  }
}

function drawMine(node, options = {}) {
  const { highlight = false, highlightColor = "#c8a030" } = options;
  const x = node.x;
  const y = node.y;
  const s = 12;

  if (highlight) {
    ctx.fillStyle = `${highlightColor}33`;
    ctx.fillRect(x - s - 6, y - s - 6, (s + 6) * 2, (s + 6) * 2);
  }

  drawNodeGraphics(x, y, node.type);

  if (node.minerals && node.minerals[0]) {
    ctx.fillStyle = "#fff";
    ctx.font = "16px Arial";
    ctx.fillText(node.minerals[0], x, y + s + 14);
  }
}

function drawAllNodes(options = {}) {
  INITIAL_NODES.forEach((node) => {
    if (node.type === "dwarf") drawDwarf(node, options.dwarf || {});
    else drawMine(node, options.mine || {});
  });
}

async function runAlgorithm(algorithmType) {
  if (!algorithmType) return;

  const btn = document.getElementById("algo-run-button");
  btn.disabled = true;
  btn.innerText = "Trwają obliczenia...";

  try {
    let res;

    switch (algorithmType) {
      case "convexHull": {
        const payload = INITIAL_NODES.map((n) => ({ x: n.x, y: n.y }));
        res = await MapApiService.calculateConvexHull(payload);
        if (res.success) drawConvexHull(res.data);
        break;
      }
      case "matching": {
        const payload = buildMatchingPayload();
        res = await MapApiService.calculateMatching(payload);
        if (res.success) {
          drawMatching(res.data);
        }
        break;
      }
      case "minCost": {
        const payload = buildMatchingPayload();
        res = await MapApiService.calculateMinCost(payload);
        if (res.success) drawMinCost(res.data);
        break;
      }
      case "rmq": {
        console.log("Zaznaczeni krasnoludki do RMQ:", selectedDwarfs);
        const payload = selectedDwarfs.map((s) => ({
          ...s,
          pointId: Number(s.id),
          voiceLoudness: s.loudness ?? 0,
        }));
        res = await MapApiService.calculateSegmentTree(payload);
        if (res.success) {
          drawRMQ();
          drawLoudestDwarf(res.data.loudestDwarfId);
        }
        break;
      }
      default:
        throw new Error("Nieznany algorytm: " + algorithmType);
    }

    if (res && !res.success) alert("Błąd: " + res.message);
  } catch (err) {
    console.error(err);
    alert("Wystąpił błąd podczas wykonywania algorytmu: " + err.message);
  } finally {
    btn.disabled = false;
    btn.innerText = "Uruchom";
  }
}

function buildMatchingPayload() {
  return {
    dwarves: INITIAL_NODES.filter((n) => n.type === "dwarf").map((n) => ({
      pointId: n.id,
      x: n.x,
      y: n.y,
      preferredMinerals: n.minerals,
      voiceLoudness: n.loudness ?? 0,
    })),
    mines: INITIAL_NODES.filter((n) => n.type === "mine").map((n) => ({
      pointId: n.id,
      x: n.x,
      y: n.y,
      resource: n.minerals[0],
      capacity: n.capacity ?? 1,
    })),
  };
}

function drawConvexHull(hullPoints) {
  drawScene(() => {
    drawAllNodes();

    if (!hullPoints || hullPoints.length < 3) return;

    ctx.beginPath();
    hullPoints.forEach((p, i) => {
      i === 0 ? ctx.moveTo(p.x, p.y) : ctx.lineTo(p.x, p.y);
    });
    ctx.closePath();

    ctx.fillStyle = "rgba(192,112,48,0.07)";
    ctx.strokeStyle = "#c07030";
    ctx.lineWidth = 2.5;
    ctx.setLineDash([8, 4]);
    ctx.fill();
    ctx.stroke();
    ctx.setLineDash([]);

    hullPoints.forEach((p) => {
      ctx.beginPath();
      ctx.arc(p.x, p.y, 5, 0, Math.PI * 2);
      ctx.fillStyle = "#c07030";
      ctx.fill();
    });
    showConvexHullStats(hullPoints);
  });
}

function showConvexHullStats(data) {
  console.log("Dane do statystyk ConvexHull:", data);
  const statsDiv = document.querySelector(".algo-stats");
  if (!statsDiv) return;

  statsDiv.classList.remove("disabled");

  const nodesCount = data.length;
  const perimeter = data.reduce((sum, p, i) => {
    const next = data[(i + 1) % data.length];
    return (
      sum + Math.sqrt(Math.pow(p.x - next.x, 2) + Math.pow(p.y - next.y, 2))
    );
  }, 0);

  statsDiv.innerHTML = `
    <p>Ilość wierzchołków: <strong>${nodesCount}</strong></p>
    <p>Obwód: <strong>${perimeter.toFixed(2)}</strong></p>
  `;
}

function drawMatching(data) {
  drawScene(() => {
    const dwarves = INITIAL_NODES.filter((n) => n.type === "dwarf");
    const mines = INITIAL_NODES.filter((n) => n.type === "mine");

    dwarves.forEach((d) => {
      mines.forEach((m) => {
        if (!d.minerals.some((min) => m.minerals.includes(min))) return;
        ctx.beginPath();
        ctx.strokeStyle = "rgba(60,55,40,0.15)";
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
      if (!dwarf || !mine) return;

      ctx.beginPath();
      ctx.strokeStyle = "#6aaa6a";
      ctx.lineWidth = 2.5;
      ctx.moveTo(dwarf.x, dwarf.y);
      ctx.lineTo(mine.x, mine.y);
      ctx.stroke();
    });

    drawAllNodes();
    showMatchingStats(data);
  });
}

function showMatchingStats(data) {
  console.log("Dane do statystyk przydziału:", data);
  const statsDiv = document.querySelector(".algo-stats");

  if (!statsDiv) return;
  statsDiv.classList.remove("disabled");

  const assignedDwarves = data.assignments.length;
  const unassignedDwarves =
    INITIAL_NODES.filter((n) => n.type === "dwarf").length - assignedDwarves;
  const edgesCount = data.assignments.length;

  statsDiv.innerHTML = `
    <p>Przydzielono: <strong>${assignedDwarves}</strong></p>
    <p>Nieprzydzielono: <strong>${unassignedDwarves}</strong></p>
    <p>Ilość krawędzi grafu: <strong>${edgesCount}</strong></p>
  `;
}

function drawMinCost(data) {
  drawScene(() => {
    const dwarves = INITIAL_NODES.filter((n) => n.type === "dwarf");
    const mines = INITIAL_NODES.filter((n) => n.type === "mine");

    dwarves.forEach((d) => {
      mines.forEach((m) => {
        if (!d.minerals.some((min) => m.minerals.includes(min))) return;
        ctx.beginPath();
        ctx.strokeStyle = "rgba(60,55,40,0.15)";
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
      if (!dwarf || !mine) return;

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
      ctx.fillStyle = "#fff";
      ctx.font = "18px Arial";
      ctx.textAlign = "center";
      ctx.textBaseline = "middle";
      ctx.fillText(dist.toFixed(0), mx, my - 6);
    });

    drawAllNodes();
    showMinCostStats(data);
  });
}

function showMinCostStats(data) {
  console.log("Dane do statystyk minCost:", data);
  const statsDiv = document.querySelector(".algo-stats");

  if (!statsDiv) return;
  statsDiv.classList.remove("disabled");

  const sumOfDistances = data.realCost;
  const dwarfsWithPenalties = data.penalizedCount;
  const maxFlow = data.maxFlow;
  const dwarfsCount = data.employedCount;

  statsDiv.innerHTML = `
    <p>Suma odległości: <strong>${sumOfDistances.toFixed(2)}</strong></p>
    <p>Krasnoludki z karą: <strong>${dwarfsWithPenalties}</strong></p>
    <p>Max flow: <strong>${maxFlow}</strong></p>
    <p>Przydzielone krasnoludki: <strong>${dwarfsCount}</strong></p>
  `;
}

function drawRMQ() {
  drawScene(() => {
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    const rmqDwarves = INITIAL_NODES.filter((n) => n.type === "dwarf").sort(
      (a, b) => a.x - b.x,
    );

    INITIAL_NODES.filter((n) => n.type === "mine").forEach((m) => drawMine(m));

    rmqDwarves.forEach((dwarf, index) => {
      const inBox =
        rmqSelectionBox.minX !== -1 &&
        dwarf.x >= rmqSelectionBox.minX &&
        dwarf.x <= rmqSelectionBox.maxX &&
        dwarf.y >= rmqSelectionBox.minY &&
        dwarf.y <= rmqSelectionBox.maxY;

      const inDrag =
        isDragging &&
        dwarf.x >= Math.min(dragStart.x, dragEnd.x) &&
        dwarf.x <= Math.max(dragStart.x, dragEnd.x) &&
        dwarf.y >= Math.min(dragStart.y, dragEnd.y) &&
        dwarf.y <= Math.max(dragStart.y, dragEnd.y);

      const isSelected = inBox || inDrag;

      drawDwarf(dwarf, {
        highlight: isSelected,
        highlightColor: "#c8a030",
        showIndex: true,
        index,
        showLoudness: true,
      });
    });

    if (isDragging) {
      ctx.save();
      ctx.fillStyle = "rgba(241,196,15,0.1)";
      ctx.strokeStyle = "#f1c40f";
      ctx.lineWidth = 1.5;
      ctx.setLineDash([4, 4]);
      ctx.fillRect(
        dragStart.x,
        dragStart.y,
        dragEnd.x - dragStart.x,
        dragEnd.y - dragStart.y,
      );
      ctx.strokeRect(
        dragStart.x,
        dragStart.y,
        dragEnd.x - dragStart.x,
        dragEnd.y - dragStart.y,
      );
      ctx.restore();
    }

    if (algorithmResults.segmentTree?.loudestDwarfId) {
      drawLoudestDwarf(algorithmResults.segmentTree.loudestDwarfId);
      showRMQStats(algorithmResults.segmentTree);
    }
  });
}

function showRMQStats(data) {
  console.log("Dane do statystyk RMQ:", data);
  const statsDiv = document.querySelector(".algo-stats");

  if (!statsDiv) return;
  statsDiv.classList.remove("disabled");

  const loudestDwarf = INITIAL_NODES.find(
    (n) => n.type === "dwarf" && Number(n.id) === Number(data.loudestDwarfId),
  );

  statsDiv.innerHTML = `
    <p>Najgłośniejszy krasnolud: <strong>${loudestDwarf ? loudestDwarf.id : "Nie znaleziono"}</strong></p>
    <p>Głośność: <strong>${loudestDwarf ? loudestDwarf.loudness : "Nie znaleziono"}</strong></p>
  `;
}

function drawLoudestDwarf(dwarfId) {
  const dwarf = INITIAL_NODES.find(
    (n) => n.type === "dwarf" && Number(n.id) === Number(dwarfId),
  );
  if (!dwarf) return;

  drawDwarf(dwarf, {
    highlight: true,
    highlightColor: "#6aaa6a",
    isLoudest: true,
    showLoudness: true,
  });
}

function loadAlgorithmResults() {
  const algo = new URLSearchParams(window.location.search).get("algorithm");

  switch (algo) {
    case "convexHull":
      if (algorithmResults.convexHull)
        drawConvexHull(algorithmResults.convexHull.hullPoints);
      else drawAllNodes();
      break;
    case "matching":
      if (algorithmResults.matching) drawMatching(algorithmResults.matching);
      else drawAllNodes();
      break;
    case "minCost":
      if (algorithmResults.minCost) drawMinCost(algorithmResults.minCost);
      else drawAllNodes();
      break;
    case "rmq":
      drawRMQ();
      break;
    default:
      drawAllNodes();
  }
}

canvas.addEventListener("mousemove", (e) => {
  if (!isDragging) return;
  const coords = getCanvasCoords(e);
  dragEnd = { ...coords };
  drawRMQ();
});

document.addEventListener("DOMContentLoaded", () => {
  setupCanvas();
  loadAlgorithmResults();
});

window.addEventListener("resize", () => {
  setupCanvas();
  loadAlgorithmResults();
});

canvas.addEventListener(
  "wheel",
  (e) => {
    e.preventDefault();

    const rect = canvas.getBoundingClientRect();
    const mouseX = (e.clientX - rect.left) * (canvas.width / rect.width);
    const mouseY = (e.clientY - rect.top) * (canvas.height / rect.height);

    const zoomFactor = e.deltaY < 0 ? 1.1 : 0.9;
    const newZoom = Math.min(
      camera.maxZoom,
      Math.max(camera.minZoom, camera.zoom * zoomFactor),
    );

    camera.x = mouseX - (mouseX - camera.x) * (newZoom / camera.zoom);
    camera.y = mouseY - (mouseY - camera.y) * (newZoom / camera.zoom);
    camera.zoom = newZoom;

    loadAlgorithmResults();
  },
  { passive: false },
);

canvas.addEventListener("mousedown", (e) => {
  const algo = new URLSearchParams(window.location.search).get("algorithm");

  if (e.button === 1 || (e.button === 0 && e.altKey)) {
    e.preventDefault();
    isPanning = true;
    panStart = { x: e.clientX - camera.x, y: e.clientY - camera.y };
    canvas.style.cursor = "grab";
    return;
  }

  if (algo === "rmq" && e.button === 0 && !e.altKey) {
    rmqSelectionBox = { minX: -1, maxX: -1, minY: -1, maxY: -1 };
    isDragging = true;
    const coords = getCanvasCoords(e);
    const world = screenToWorld(coords.x, coords.y);
    dragStart = { ...world };
    dragEnd = { ...world };
    drawRMQ();
  }
});

canvas.addEventListener("mousemove", (e) => {
  if (isPanning) {
    camera.x = e.clientX - panStart.x;
    camera.y = e.clientY - panStart.y;
    loadAlgorithmResults();
    return;
  }

  if (!isDragging) return;
  const coords = getCanvasCoords(e);
  const world = screenToWorld(coords.x, coords.y);
  dragEnd = { ...world };
  drawRMQ();
});

canvas.addEventListener("mouseup", (e) => {
  if (isPanning) {
    isPanning = false;
    canvas.style.cursor = "default";
    return;
  }

  if (!isDragging) return;
  isDragging = false;

  const minX = Math.min(dragStart.x, dragEnd.x);
  const maxX = Math.max(dragStart.x, dragEnd.x);
  const minY = Math.min(dragStart.y, dragEnd.y);
  const maxY = Math.max(dragStart.y, dragEnd.y);
  rmqSelectionBox = { minX, maxX, minY, maxY };

  const rmqDwarves = INITIAL_NODES.filter((n) => n.type === "dwarf").sort(
    (a, b) => a.x - b.x,
  );

  const selected = rmqDwarves
    .map((d, i) => ({ d, i }))
    .filter(
      ({ d }) => d.x >= minX && d.x <= maxX && d.y >= minY && d.y <= maxY,
    );

  console.log(
    "Zaznaczeni krasnoludki:",
    selected.map(({ d }) => d),
  );

  selectedDwarfs = selected.map(({ d }) => d);

  if (selected.length > 0) {
    const indices = selected.map((s) => s.i);
    const L = Math.min(...indices);
    const R = Math.max(...indices);
    console.log(`Zaznaczono przedział [${L}, ${R}]`);

    const lSel = document.getElementById("rmq-l");
    const rSel = document.getElementById("rmq-r");
    if (lSel) lSel.value = L;
    if (rSel) rSel.value = R;
  }

  drawRMQ();
});

canvas.addEventListener("wheel", (e) => e.preventDefault(), { passive: false });

function zoomToCenter(factor) {
  const W = getCW();
  const H = getCH();

  const centerX = W / 2;
  const centerY = H / 2;

  const newZoom = Math.min(
    camera.maxZoom,
    Math.max(camera.minZoom, camera.zoom * factor),
  );

  camera.x = centerX - (centerX - camera.x) * (newZoom / camera.zoom);
  camera.y = centerY - (centerY - camera.y) * (newZoom / camera.zoom);
  camera.zoom = newZoom;

  loadAlgorithmResults();
}

function zoomIn() {
  zoomToCenter(1.2);
}
function zoomOut() {
  zoomToCenter(1 / 1.2);
}
function resetZoom() {
  camera.x = 0;
  camera.y = 0;
  camera.zoom = 1;
  loadAlgorithmResults();
}

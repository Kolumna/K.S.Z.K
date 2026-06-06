const algorithmResults =
  typeof ALGORITHM_RESULTS !== "undefined" ? ALGORITHM_RESULTS : {};

const canvas = document.getElementById("algoCanvas");
const ctx = canvas.getContext("2d");

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

function drawDwarf(node, options = {}) {
  const {
    highlight = false,
    highlightColor = "#6aaa6a",
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
    ctx.fillStyle = `${highlightColor}33`;
    ctx.fill();
  }

  ctx.beginPath();
  ctx.arc(x, y, 10, 0, Math.PI * 2);
  ctx.fillStyle = isLoudest ? "#1a4a2a" : "#1a3a5a";
  ctx.strokeStyle = isLoudest ? "#6aaa6a" : "#4a90c0";
  ctx.lineWidth = highlight ? 2.5 : 2;
  ctx.fill();
  ctx.stroke();

  // ctx.fillStyle = "#fff";
  // ctx.font = "16px Arial";
  // ctx.fillText(`id: ${index}`, x, y + 24);

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
      ctx.fillStyle = "#6aaa6a";
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

  ctx.beginPath();
  ctx.rect(x - s, y - s, s * 2, s * 2);
  ctx.fillStyle = "#2a1a08";
  ctx.strokeStyle = "#c07030";
  ctx.lineWidth = 2;
  ctx.fill();
  ctx.stroke();

  if (node.minerals && node.minerals[0]) {
    ctx.fillStyle = "#3a3528";
    ctx.font = "8px Arial";
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
        if (res.success) drawMatching(res.data);
        break;
      }
      case "minCost": {
        const payload = buildMatchingPayload();
        res = await MapApiService.calculateMinCost(payload);
        if (res.success) drawMinCost(res.data);
        break;
      }
      case "rmq": {
        const payload = INITIAL_NODES.filter((n) => n.type === "dwarf").map(
          (n) => ({
            pointId: n.id,
            x: n.x,
            y: n.y,
            voiceLoudness: n.loudness ?? 50,
          }),
        );
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
  ctx.clearRect(0, 0, canvas.width, canvas.height);
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
}

function drawMatching(data) {
  ctx.clearRect(0, 0, canvas.width, canvas.height);

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
}

function drawMinCost(data) {
  ctx.clearRect(0, 0, canvas.width, canvas.height);

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
}

function drawRMQ() {
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
  }
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

canvas.addEventListener("mousedown", (e) => {
  const algo = new URLSearchParams(window.location.search).get("algorithm");
  if (algo !== "rmq") return;

  rmqSelectionBox = { minX: -1, maxX: -1, minY: -1, maxY: -1 };
  isDragging = true;
  const coords = getCanvasCoords(e);
  dragStart = { ...coords };
  dragEnd = { ...coords };
  drawRMQ();
});

canvas.addEventListener("mousemove", (e) => {
  if (!isDragging) return;
  const coords = getCanvasCoords(e);
  dragEnd = { ...coords };
  drawRMQ();
});

canvas.addEventListener("mouseup", (e) => {
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

document.addEventListener("DOMContentLoaded", () => {
  setupCanvas();
  loadAlgorithmResults();
});

window.addEventListener("resize", () => {
  setupCanvas();
  loadAlgorithmResults();
});

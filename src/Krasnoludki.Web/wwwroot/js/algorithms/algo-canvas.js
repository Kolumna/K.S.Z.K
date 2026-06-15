let canvas, ctx;
let _dirty = false;
let _canvasRect = null;

let selectedDwarfs = [];
let rmqSelectionBox = { minX: -1, maxX: -1, minY: -1, maxY: -1 };
let isDragging = false;
let dragStart = { x: 0, y: 0 };
let dragEnd = { x: 0, y: 0 };

const mineralsNames = {
  Silver: "Srebro",
  Gold: "Złoto",
  Quartz: "Kwarc",
  Coal: "Węgiel",
  Uranium: "Uran",
};

let hoveredNode = null;

const camera = {
  x: 0,
  y: 0,
  zoom: 1,
  minZoom: 0.05,
  maxZoom: 5,
};

let isPanning = false;
let panStart = { x: 0, y: 0 };

function scheduleRedraw() {
  _dirty = true;
}

function getCanvasRect() {
  if (!_canvasRect) _canvasRect = canvas.getBoundingClientRect();
  return _canvasRect;
}

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

function getNodeAt(x, y) {
  if (typeof INITIAL_NODES === "undefined" || !INITIAL_NODES) return null;

  for (let i = INITIAL_NODES.length - 1; i >= 0; i--) {
    const node = INITIAL_NODES[i];
    const dx = x - node.x;
    const dy = y - node.y;

    if (Math.sqrt(dx * dx + dy * dy) <= 22) {
      return node;
    }
  }
  return null;
}

function drawScene(drawFn) {
  resetTransform();
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  applyCamera();
  drawFn();
  if (hoveredNode) {
    drawTooltip(hoveredNode);
  }
  resetTransform();
}

function setupCanvas() {
  if (!canvas) return;
  const dpr = window.devicePixelRatio || 1;
  const rect = getCanvasRect();
  canvas.width = rect.width * dpr;
  canvas.height = rect.height * dpr;
  ctx.scale(dpr, dpr);
  canvas.style.width = `${rect.width}px`;
  canvas.style.height = `${rect.height}px`;
}

function getCanvasCoords(event) {
  const rect = getCanvasRect();
  return {
    x: (event.clientX - rect.left) * (canvas.width / rect.width),
    y: (event.clientY - rect.top) * (canvas.height / rect.height),
  };
}

function getCW() {
  return getCanvasRect().width;
}
function getCH() {
  return getCanvasRect().height;
}

function drawNodeGraphics(x, y, type) {
  ctx.beginPath();
  if (type === "dwarf") {
    ctx.arc(x, y, 20, 0, Math.PI * 2);
  } else if (type === "mine") {
    ctx.rect(x - 18, y - 18, 36, 36);
  }

  ctx.fillStyle = type === "dwarf" ? "#1A3A5A" : "#3A2010";
  ctx.fill();
  ctx.strokeStyle = "#000";
  ctx.lineWidth = 2;
  ctx.stroke();

  ctx.fillStyle = "white";
  ctx.font = "bold 14px Arial";
  ctx.textAlign = "center";
  ctx.textBaseline = "middle";
  ctx.fillText(type.charAt(0).toUpperCase(), x, y);
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

  const { x, y } = node;

  if (highlight) {
    ctx.beginPath();
    ctx.arc(x, y, 26, 0, Math.PI * 2);
    ctx.fillStyle = highlightColor;
    ctx.fill();
  }

  drawNodeGraphics(x, y, node.type);

  // ctx.fillStyle = "#fff";
  // ctx.font = "16px Arial";
  // (node.preferredMinerals ?? []).forEach((mineral, i) => {
  //   ctx.fillText(mineral, x - 48, y + 24 + i * 18);
  // });

  // if (showIndex) {
  //   ctx.fillStyle = "#fff";
  //   ctx.font = "16px Arial";
  //   ctx.textAlign = "center";
  //   ctx.textBaseline = "middle";
  //   ctx.fillText(`id: ${index}`, x, y + 24);
  // }

  if (showLoudness && node.voiceLoudness) {
    const text = `${node.voiceLoudness} dB`;

    ctx.save();

    ctx.font = "bold 13px 'Segoe UI', Arial, sans-serif";

    const textWidth = ctx.measureText(text).width;
    const paddingX = 8;
    const height = 22;

    const badgeY = y - 32;

    ctx.beginPath();

    if (isLoudest) {
      ctx.fillStyle = "#1A3320";
      ctx.strokeStyle = "#5FFF5F";
      ctx.lineWidth = 1.5;

      ctx.shadowColor = "#5FFF5F";
      ctx.shadowBlur = 8;
    } else {
      ctx.fillStyle = "#2B2D31";

      ctx.shadowColor = "rgba(0, 0, 0, 0.6)";
      ctx.shadowBlur = 4;
      ctx.shadowOffsetY = 2;
    }

    ctx.roundRect(
      x - textWidth / 2 - paddingX,
      badgeY - height / 2,
      textWidth + paddingX * 2,
      height,
      height / 2,
    );
    ctx.fill();

    if (isLoudest) {
      ctx.stroke();
    }

    ctx.shadowBlur = 0;
    ctx.shadowOffsetY = 0;

    ctx.fillStyle = isLoudest ? "#5FFF5F" : "#FFFFFF";
    ctx.textAlign = "center";
    ctx.textBaseline = "middle";

    ctx.fillText(text, x, badgeY + 1);

    ctx.restore();
  }
}

function drawMine(node, options = {}) {
  const { highlight = false, highlightColor = "#c8a030" } = options;
  const { x, y } = node;
  const s = 12;

  if (highlight) {
    ctx.fillStyle = `${highlightColor}33`;
    ctx.fillRect(x - s - 6, y - s - 6, (s + 6) * 2, (s + 6) * 2);
  }

  drawNodeGraphics(x, y, node.type);

  if (node.minerals?.[0]) {
    ctx.fillStyle = "#fff";
    ctx.font = "16px Arial";
    ctx.textAlign = "center";
    ctx.textBaseline = "middle";
    ctx.fillText(node.minerals[0], x, y + s + 14);
  }
}

function drawAllNodes(options = {}) {
  INITIAL_NODES.forEach((node) => {
    if (node.type === "dwarf") drawDwarf(node, options.dwarf || {});
    else drawMine(node, options.mine || {});
  });
}

function drawLoudestDwarf(dwarfId) {
  const dwarf = INITIAL_NODES.find(
    (n) => n.type === "dwarf" && Number(n.pointId) === Number(dwarfId),
  );
  if (!dwarf) return;

  drawDwarf(dwarf, {
    highlight: true,
    highlightColor: "#6aaa6a",
    isLoudest: true,
    showLoudness: true,
  });
}

function drawRMQ() {
  drawScene(() => {
    let boxMinX, boxMaxX, boxMinY, boxMaxY;
    let shouldDrawBox = false;

    if (isDragging) {
      boxMinX = Math.min(dragStart.x, dragEnd.x);
      boxMaxX = Math.max(dragStart.x, dragEnd.x);
      boxMinY = Math.min(dragStart.y, dragEnd.y);
      boxMaxY = Math.max(dragStart.y, dragEnd.y);
      shouldDrawBox = true;
    } else if (rmqSelectionBox.minX !== -1) {
      boxMinX = rmqSelectionBox.minX;
      boxMaxX = rmqSelectionBox.maxX;
      boxMinY = rmqSelectionBox.minY;
      boxMaxY = rmqSelectionBox.maxY;
      shouldDrawBox = true;
    }

    const rmqDwarves = INITIAL_NODES.filter((n) => n.type === "dwarf").sort(
      (a, b) => a.x - b.x,
    );

    INITIAL_NODES.filter((n) => n.type === "mine").forEach((m) => drawMine(m));

    rmqDwarves.forEach((dwarf, index) => {
      const isSelected =
        shouldDrawBox &&
        dwarf.x >= boxMinX &&
        dwarf.x <= boxMaxX &&
        dwarf.y >= boxMinY &&
        dwarf.y <= boxMaxY;

      drawDwarf(dwarf, {
        highlight: isSelected,
        highlightColor: "#735e08",
        showIndex: true,
        index,
        showLoudness: true,
      });
    });

    if (shouldDrawBox) {
      const width = boxMaxX - boxMinX;
      const height = boxMaxY - boxMinY;

      ctx.save();

      const fillGradient = ctx.createLinearGradient(
        boxMinX,
        boxMinY,
        boxMinX,
        boxMaxY,
      );
      fillGradient.addColorStop(0, "rgba(241, 196, 15, 0.05)");
      fillGradient.addColorStop(1, "rgba(241, 196, 15, 0.2)");
      ctx.fillStyle = fillGradient;
      ctx.fillRect(boxMinX, boxMinY, width, height);

      ctx.strokeStyle = "#F1C40F";
      ctx.lineWidth = 1.5;
      ctx.setLineDash([8, 6]);
      ctx.shadowColor = "#F1C40F";
      ctx.shadowBlur = 6;
      ctx.strokeRect(boxMinX, boxMinY, width, height);

      ctx.setLineDash([]);
      ctx.lineWidth = 3;
      ctx.shadowBlur = 0;
      const cornerSize = Math.min(15, width / 4, height / 4);

      ctx.beginPath();
      ctx.moveTo(boxMinX, boxMinY + cornerSize);
      ctx.lineTo(boxMinX, boxMinY);
      ctx.lineTo(boxMinX + cornerSize, boxMinY);
      ctx.moveTo(boxMaxX - cornerSize, boxMinY);
      ctx.lineTo(boxMaxX, boxMinY);
      ctx.lineTo(boxMaxX, boxMinY + cornerSize);
      ctx.moveTo(boxMaxX, boxMaxY - cornerSize);
      ctx.lineTo(boxMaxX, boxMaxY);
      ctx.lineTo(boxMaxX - cornerSize, boxMaxY);
      ctx.moveTo(boxMinX + cornerSize, boxMaxY);
      ctx.lineTo(boxMinX, boxMaxY);
      ctx.lineTo(boxMinX, boxMaxY - cornerSize);
      ctx.stroke();

      ctx.restore();
    }

    if (algorithmResults.segmentTree?.loudestDwarfId) {
      drawLoudestDwarf(algorithmResults.segmentTree.loudestDwarfId);
      showRMQStats(algorithmResults.segmentTree);
    }
  });
}

function fitCameraToNodes() {
  if (!canvas || !INITIAL_NODES?.length) return;

  let minX = Infinity,
    minY = Infinity;
  let maxX = -Infinity,
    maxY = -Infinity;

  INITIAL_NODES.forEach(({ x, y }) => {
    if (x < minX) minX = x;
    if (x > maxX) maxX = x;
    if (y < minY) minY = y;
    if (y > maxY) maxY = y;
  });

  if (minX === maxX) {
    minX -= 50;
    maxX += 50;
  }
  if (minY === maxY) {
    minY -= 50;
    maxY += 50;
  }

  const mapWidth = maxX - minX;
  const mapHeight = maxY - minY;
  const rect = getCanvasRect();
  const padding = 40;

  const requiredZoom = Math.min(
    (rect.width - padding * 2) / mapWidth,
    (rect.height - padding * 2) / mapHeight,
  );

  camera.zoom = Math.max(
    camera.minZoom,
    Math.min(requiredZoom, camera.maxZoom),
  );
  camera.x = rect.width / 2 - (minX + mapWidth / 2) * camera.zoom;
  camera.y = rect.height / 2 - (minY + mapHeight / 2) * camera.zoom;
}

function zoomToCenter(factor) {
  if (!canvas) return;
  const W = getCW(),
    H = getCH();
  const newZoom = Math.min(
    camera.maxZoom,
    Math.max(camera.minZoom, camera.zoom * factor),
  );
  camera.x = W / 2 - (W / 2 - camera.x) * (newZoom / camera.zoom);
  camera.y = H / 2 - (H / 2 - camera.y) * (newZoom / camera.zoom);
  camera.zoom = newZoom;
  scheduleRedraw();
}

function zoomIn() {
  zoomToCenter(1.2);
}
function zoomOut() {
  zoomToCenter(1 / 1.2);
}
function resetZoom() {
  if (!canvas) return;
  camera.x = 0;
  camera.y = 0;
  camera.zoom = 1;
  scheduleRedraw();
}

window.addEventListener("resize", () => {
  if (!canvas) return;
  _canvasRect = null;
  setupCanvas();
  scheduleRedraw();
});

function renderLoop() {
  if (_dirty) {
    loadAlgorithmResults();
    _dirty = false;
  }
  requestAnimationFrame(renderLoop);
}

function drawTooltip(node) {
  if (!node) return;

  ctx.save();

  const lines = [
    `${node.type === "dwarf" ? "Krasnoludek" : "Kopalnia"} (Id: ${node.pointId})`,
  ];

  if (node.type === "dwarf") {
    lines.push(
      `Minerały: ${node.preferredMinerals?.map((m) => mineralsNames[m]).join(", ") || "Brak"}`,
    );
    lines.push(`Głośność: ${node.voiceLoudness || 0}`);
  } else {
    lines.push(
      `Zasób: ${node.resource ? mineralsNames[node.resource] : "Brak"}`,
    );
    lines.push(`Pojemność: ${node.capacity || 0}`);
  }

  ctx.font = "14px monospace";

  let maxWidth = 0;
  lines.forEach((line) => {
    const width = ctx.measureText(line).width;
    if (width > maxWidth) maxWidth = width;
  });

  const padding = 10;
  const boxWidth = maxWidth + padding * 2;
  const boxHeight = lines.length * 20 + padding;

  const tooltipX = node.x + 20;
  const tooltipY = node.y - boxHeight - 10;

  ctx.fillStyle = "rgba(0, 0, 0, 0.8)";
  ctx.strokeStyle = "#ffffff";
  ctx.lineWidth = 1;
  ctx.beginPath();
  ctx.roundRect(tooltipX, tooltipY, boxWidth, boxHeight, 6);
  ctx.fill();
  ctx.stroke();

  ctx.fillStyle = "#ffffff";
  ctx.textAlign = "left";
  ctx.textBaseline = "top";
  lines.forEach((line, index) => {
    ctx.fillText(line, tooltipX + padding, tooltipY + padding + index * 18);
  });

  ctx.restore();
}

function loadAlgorithmResults() {
  console.log(algorithmResults);
  switch (currentAlgo) {
    case "convexHull":
      if (algorithmResults.convexHull)
        drawConvexHull(algorithmResults.convexHull.hullPoints);
      else
        drawScene(() => {
          drawAllNodes();

          if (hoveredNode) {
            drawTooltip(hoveredNode);
          }
        });
      break;
    case "matching":
      if (algorithmResults.matching) drawMatching(algorithmResults.matching);
      else
        drawScene(() => {
          drawAllNodes();

          if (hoveredNode) {
            drawTooltip(hoveredNode);
          }
        });
      break;
    case "minCost":
      if (algorithmResults.minCost) drawMinCost(algorithmResults.minCost);
      else
        drawScene(() => {
          drawAllNodes();

          if (hoveredNode) {
            drawTooltip(hoveredNode);
          }
        });
      break;
    case "rmq":
      drawRMQ();
      break;
    default:
      drawScene(() => {
        drawAllNodes();

        if (hoveredNode) {
          drawTooltip(hoveredNode);
        }
      });
  }
}

function drawConvexHull(hullPoints) {
  drawScene(() => {
    if (!hullPoints || hullPoints.length < 3) {
      drawAllNodes();
      return;
    }

    const hullColor = "#C07030";

    ctx.save();

    ctx.beginPath();
    hullPoints.forEach((p, i) => {
      i === 0 ? ctx.moveTo(p.x, p.y) : ctx.lineTo(p.x, p.y);
    });
    ctx.closePath();

    const centerX =
      hullPoints.reduce((sum, p) => sum + p.x, 0) / hullPoints.length;
    const centerY =
      hullPoints.reduce((sum, p) => sum + p.y, 0) / hullPoints.length;

    const maxDist = Math.max(
      ...hullPoints.map((p) => Math.hypot(p.x - centerX, p.y - centerY)),
    );

    const fillGradient = ctx.createRadialGradient(
      centerX,
      centerY,
      0,
      centerX,
      centerY,
      maxDist,
    );
    fillGradient.addColorStop(0, "rgba(192, 112, 48, 0.02)");
    fillGradient.addColorStop(0.7, "rgba(192, 112, 48, 0.08)");
    fillGradient.addColorStop(1, "rgba(192, 112, 48, 0.15)");

    ctx.fillStyle = fillGradient;
    ctx.fill();
    ctx.restore();

    ctx.save();

    ctx.beginPath();
    hullPoints.forEach((p, i) => {
      i === 0 ? ctx.moveTo(p.x, p.y) : ctx.lineTo(p.x, p.y);
    });
    ctx.closePath();

    ctx.strokeStyle = hullColor;
    ctx.lineWidth = 3.5;
    ctx.setLineDash([12, 6]);
    ctx.lineJoin = "round";

    ctx.shadowColor = hullColor;
    ctx.shadowBlur = 12;

    ctx.stroke();
    ctx.restore();

    hullPoints.forEach((p) => {
      ctx.save();

      ctx.beginPath();
      ctx.arc(p.x, p.y, 7, 0, Math.PI * 2);
      ctx.fillStyle = "#1A1A1D";
      ctx.shadowColor = "rgba(0,0,0,0.5)";
      ctx.shadowBlur = 4;
      ctx.shadowOffsetY = 2;
      ctx.fill();

      ctx.lineWidth = 2;
      ctx.strokeStyle = hullColor;
      ctx.stroke();

      ctx.beginPath();
      ctx.arc(p.x, p.y, 2.5, 0, Math.PI * 2);
      ctx.fillStyle = "#FFFFFF";

      ctx.shadowColor = "#FFFFFF";
      ctx.shadowBlur = 5;
      ctx.fill();

      ctx.restore();
    });

    drawAllNodes();
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
  console.log("Dane do rysowania matching:", data);
  drawScene(() => {
    const dwarves = DWARF_NODES;
    const mines = MINE_NODES;

    ctx.save();

    data.assignments.forEach((a) => {
      const dwarf = dwarves.find(
        (d) => Number(d.pointId) === Number(a.dwarfId),
      );
      const mine = mines.find((m) => Number(m.pointId) === Number(a.mineId));

      if (!dwarf || !mine) return;

      const gradient = ctx.createLinearGradient(
        dwarf.x,
        dwarf.y,
        mine.x,
        mine.y,
      );
      gradient.addColorStop(0, "#4ADE80");
      gradient.addColorStop(1, "#059669");

      ctx.beginPath();
      ctx.strokeStyle = gradient;

      ctx.lineWidth = 3.5;

      ctx.shadowColor = "#4ADE80";
      ctx.shadowBlur = 10;
      ctx.shadowOffsetX = 0;
      ctx.shadowOffsetY = 0;

      ctx.moveTo(dwarf.x, dwarf.y);
      ctx.lineTo(mine.x, mine.y);
      ctx.stroke();
    });

    ctx.restore();

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
    data.assignments.forEach((a) => {
      const dwarf = DWARF_NODES.find(
        (d) => Number(d.pointId) === Number(a.dwarfId),
      );
      const mine = MINE_NODES.find(
        (m) => Number(m.pointId) === Number(a.mineId),
      );

      if (!dwarf || !mine) return;

      const dist = Math.hypot(dwarf.x - mine.x, dwarf.y - mine.y);

      const ratio = Math.min(dist / 500, 1);

      const hue = 120 - ratio * 120;
      const lineColor = `hsla(${hue}, 80%, 45%, 0.85)`;

      ctx.save();

      ctx.beginPath();
      ctx.strokeStyle = lineColor;
      ctx.lineWidth = 3;

      ctx.shadowColor = lineColor;
      ctx.shadowBlur = 6;

      ctx.moveTo(dwarf.x, dwarf.y);
      ctx.lineTo(mine.x, mine.y);
      ctx.stroke();

      const mx = (dwarf.x + mine.x) / 2;
      const my = (dwarf.y + mine.y) / 2;
      const text = dist.toFixed(0);

      ctx.font = "bold 13px 'Segoe UI', Arial, sans-serif";

      const textWidth = ctx.measureText(text).width;
      const paddingX = 8;
      const paddingY = 4;
      const height = 22;

      ctx.fillStyle = "#2B2D31";
      ctx.shadowColor = "rgba(0, 0, 0, 0.5)";
      ctx.shadowBlur = 4;
      ctx.shadowOffsetY = 2;

      ctx.beginPath();
      ctx.roundRect(
        mx - textWidth / 2 - paddingX,
        my - height / 2,
        textWidth + paddingX * 2,
        height,
        height / 2,
      );
      ctx.fill();

      ctx.shadowColor = "transparent";
      ctx.fillStyle = "#FFFFFF";
      ctx.textAlign = "center";
      ctx.textBaseline = "middle";
      ctx.fillText(text, mx, my + 1);

      ctx.restore();
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
  const maxFlow = data.maxFlow;
  const dwarfsCount = data.employedCount;

  statsDiv.innerHTML = `
    <p>Suma odległości: <strong>${sumOfDistances.toFixed(2)}</strong></p>
    <p>Max flow: <strong>${maxFlow}</strong></p>
    <p>Przydzielone krasnoludki: <strong>${dwarfsCount}</strong></p>
  `;
}

function drawRMQ() {
  drawScene(() => {
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

  const loudestDwarf = DWARF_NODES.find(
    (d) => Number(d.pointId) === Number(data.loudestDwarfId),
  );

  statsDiv.innerHTML = `
    <p>Najgłośniejszy krasnolud: <strong>${loudestDwarf ? loudestDwarf.pointId : "Nie znaleziono"}</strong></p>
    <p>Głośność: <strong>${loudestDwarf ? loudestDwarf.voiceLoudness : "Nie znaleziono"}</strong></p>
  `;
}

function drawLoudestDwarf(dwarfId) {
  const dwarf = INITIAL_NODES.find(
    (n) => n.type === "dwarf" && Number(n.pointId) === Number(dwarfId),
  );
  if (!dwarf) return;

  drawDwarf(dwarf, {
    highlight: true,
    highlightColor: "#6aaa6a",
    isLoudest: true,
    showLoudness: true,
  });
}

document.addEventListener("DOMContentLoaded", () => {
  canvas = document.getElementById("algoCanvas");
  console.log("Canvas element:", canvas);
  if (!canvas) return;

  ctx = canvas.getContext("2d");

  setupCanvas();
  fitCameraToNodes();
  scheduleRedraw();
  renderLoop();
  loadAlgorithmResults();

  canvas.addEventListener(
    "wheel",
    (e) => {
      e.preventDefault();
      const rect = getCanvasRect();
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
      scheduleRedraw();
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
      scheduleRedraw();
    }
  });

  canvas.addEventListener("mousemove", (e) => {
    if (isPanning) {
      camera.x = e.clientX - panStart.x;
      camera.y = e.clientY - panStart.y;
      scheduleRedraw();
      return;
    }

    const coords = getCanvasCoords(e);
    const world = screenToWorld(coords.x, coords.y);

    if (isDragging) {
      dragEnd = { ...world };
      scheduleRedraw();
    }

    const newHoveredNode = getNodeAt(world.x, world.y);

    if (hoveredNode !== newHoveredNode) {
      hoveredNode = newHoveredNode;
      scheduleRedraw();
    }
  });

  canvas.addEventListener("mouseup", (e) => {
    if (isPanning) {
      isPanning = false;
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

    selectedDwarfs = selected.map(({ d }) => d);

    let leftIndex = null;
    let rightIndex = null;

    if (selected.length > 0) {
      const indices = selected.map((s) => s.i);
      leftIndex = Math.min(...indices);
      rightIndex = Math.max(...indices);

      const lSel = document.getElementById("rmq-l");
      const rSel = document.getElementById("rmq-r");
      if (lSel) lSel.value = leftIndex;
      if (rSel) rSel.value = rightIndex;
    }

    document.dispatchEvent(
      new CustomEvent("rmqSelectionReady", {
        detail: {
          minX,
          maxX,
          minY,
          maxY,
          leftIndex,
          rightIndex,
          selectedDwarfs,
        },
      }),
    );

    scheduleRedraw();
  });

  canvas.addEventListener("mouseleave", function () {
    hoveredNode = null;
    scheduleRedraw();
  });
});

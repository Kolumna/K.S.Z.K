let canvas, ctx;
let _dirty = false;
let _canvasRect = null;

let selectedDwarfs = [];
let rmqSelectionBox = { minX: -1, maxX: -1, minY: -1, maxY: -1 };
let isDragging = false;
let dragStart = { x: 0, y: 0 };
let dragEnd = { x: 0, y: 0 };

const camera = {
  x: 0, y: 0, zoom: 1,
  minZoom: 0.05, maxZoom: 5,
};

let isPanning = false;
let panStart = { x: 0, y: 0 };

function scheduleRedraw() { _dirty = true; }

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

function drawScene(drawFn) {
  resetTransform();
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  applyCamera();
  drawFn();
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

function getCW() { return getCanvasRect().width; }
function getCH() { return getCanvasRect().height; }

function drawNodeGraphics(x, y, type) {
  ctx.beginPath();
  ctx.arc(x, y, 20, 0, Math.PI * 2);
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

  ctx.fillStyle = "#fff";
  ctx.font = "16px Arial";
  (node.preferredMinerals ?? []).forEach((mineral, i) => {
    ctx.fillText(mineral, x - 48, y + 24 + i * 18);
  });

  if (showIndex) {
    ctx.fillStyle = "#fff";
    ctx.font = "16px Arial";
    ctx.textAlign = "center";
    ctx.textBaseline = "middle";
    ctx.fillText(`id: ${index}`, x, y + 24);
  }

  if (showLoudness && node.voiceLoudness) {
    ctx.fillStyle = isLoudest ? "#5fff5f" : "#fff";
    ctx.font = "16px Arial";
    ctx.textAlign = "center";
    ctx.textBaseline = "bottom";
    ctx.fillText(`${node.voiceLoudness}dB`, x, y - 17);
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
    const rmqDwarves = INITIAL_NODES
      .filter((n) => n.type === "dwarf")
      .sort((a, b) => a.x - b.x);

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

      drawDwarf(dwarf, {
        highlight: inBox || inDrag,
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
      ctx.fillRect(dragStart.x, dragStart.y, dragEnd.x - dragStart.x, dragEnd.y - dragStart.y);
      ctx.strokeRect(dragStart.x, dragStart.y, dragEnd.x - dragStart.x, dragEnd.y - dragStart.y);
      ctx.restore();
    }

    if (algorithmResults.segmentTree?.loudestDwarfId) {
      drawLoudestDwarf(algorithmResults.segmentTree.loudestDwarfId);
    }
  });
}

function fitCameraToNodes() {
  if (!canvas || !INITIAL_NODES?.length) return;

  let minX = Infinity, minY = Infinity;
  let maxX = -Infinity, maxY = -Infinity;

  INITIAL_NODES.forEach(({ x, y }) => {
    if (x < minX) minX = x; if (x > maxX) maxX = x;
    if (y < minY) minY = y; if (y > maxY) maxY = y;
  });

  if (minX === maxX) { minX -= 50; maxX += 50; }
  if (minY === maxY) { minY -= 50; maxY += 50; }

  const mapWidth = maxX - minX;
  const mapHeight = maxY - minY;
  const rect = getCanvasRect();
  const padding = 40;

  const requiredZoom = Math.min(
    (rect.width - padding * 2) / mapWidth,
    (rect.height - padding * 2) / mapHeight,
  );

  camera.zoom = Math.max(camera.minZoom, Math.min(requiredZoom, camera.maxZoom));
  camera.x = rect.width / 2 - (minX + mapWidth / 2) * camera.zoom;
  camera.y = rect.height / 2 - (minY + mapHeight / 2) * camera.zoom;
}

function zoomToCenter(factor) {
  if (!canvas) return;
  const W = getCW(), H = getCH();
  const newZoom = Math.min(camera.maxZoom, Math.max(camera.minZoom, camera.zoom * factor));
  camera.x = W / 2 - (W / 2 - camera.x) * (newZoom / camera.zoom);
  camera.y = H / 2 - (H / 2 - camera.y) * (newZoom / camera.zoom);
  camera.zoom = newZoom;
  scheduleRedraw();
}

function zoomIn() { zoomToCenter(1.2); }
function zoomOut() { zoomToCenter(1 / 1.2); }
function resetZoom() {
  if (!canvas) return;
  camera.x = 0; camera.y = 0; camera.zoom = 1;
  scheduleRedraw();
}

window.addEventListener("resize", () => {
  if (!canvas) return;
  _canvasRect = null;
  setupCanvas();
  scheduleRedraw();
});

document.addEventListener("DOMContentLoaded", () => {
  canvas = document.getElementById("algoCanvas");
  if (!canvas) return;

  ctx = canvas.getContext("2d");

  canvas.addEventListener("wheel", (e) => {
    e.preventDefault();
    const rect = getCanvasRect();
    const mouseX = (e.clientX - rect.left) * (canvas.width / rect.width);
    const mouseY = (e.clientY - rect.top) * (canvas.height / rect.height);
    const zoomFactor = e.deltaY < 0 ? 1.1 : 0.9;
    const newZoom = Math.min(camera.maxZoom, Math.max(camera.minZoom, camera.zoom * zoomFactor));
    camera.x = mouseX - (mouseX - camera.x) * (newZoom / camera.zoom);
    camera.y = mouseY - (mouseY - camera.y) * (newZoom / camera.zoom);
    camera.zoom = newZoom;
    scheduleRedraw();
  }, { passive: false });

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
    if (!isDragging) return;
    const coords = getCanvasCoords(e);
    const world = screenToWorld(coords.x, coords.y);
    dragEnd = { ...world };
    scheduleRedraw();
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

    const rmqDwarves = INITIAL_NODES
      .filter((n) => n.type === "dwarf")
      .sort((a, b) => a.x - b.x);

    const selected = rmqDwarves
      .map((d, i) => ({ d, i }))
      .filter(({ d }) => d.x >= minX && d.x <= maxX && d.y >= minY && d.y <= maxY);

    selectedDwarfs = selected.map(({ d }) => d);

    if (selected.length > 0) {
      const indices = selected.map((s) => s.i);
      const lSel = document.getElementById("rmq-l");
      const rSel = document.getElementById("rmq-r");
      if (lSel) lSel.value = Math.min(...indices);
      if (rSel) rSel.value = Math.max(...indices);
    }

    scheduleRedraw();
  });
});
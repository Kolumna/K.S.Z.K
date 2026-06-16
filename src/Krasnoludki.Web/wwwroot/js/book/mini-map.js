const nodes = typeof INITIAL_NODES !== "undefined" ? INITIAL_NODES : {};

const canvas = document.getElementById("miniMap");
const ctx = canvas.getContext("2d");

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

function renderMap() {
  drawScene(() => {
    nodes.forEach((node) => {
      console.log("Node:", node);
      if (node.type === "dwarf") {
        ctx.fillStyle = "#1A3A5A";
      } else {
        ctx.fillStyle = "#3A2010";
      }
      const x = node.x;
      const y = node.y;

      ctx.beginPath();
      if (node.type === "dwarf") {
        ctx.arc(x, y, 7, 0, 2 * Math.PI);
      } else {
        ctx.fillRect(x, y, 12, 12);
      }

      ctx.fill();
    });

    console.log(nodes);
  });
}

document.addEventListener("DOMContentLoaded", () => {
  setupCanvas();
  fitCameraToNodes();
  renderMap();
});

window.addEventListener("resize", () => {
  setupCanvas();
  renderMap();
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
  },
  { passive: false },
);

canvas.addEventListener("mousedown", (e) => {
  if (e.button === 1 || (e.button === 0 && e.altKey)) {
    e.preventDefault();
    isPanning = true;
    panStart = { x: e.clientX - camera.x, y: e.clientY - camera.y };
    canvas.style.cursor = "grabbing";
    return;
  }
});

canvas.addEventListener("mousemove", (e) => {
  if (isPanning) {
    e.preventDefault();
    camera.x = e.clientX - panStart.x;
    camera.y = e.clientY - panStart.y;
    renderMap();
  } else {
    const coords = getCanvasCoords(e);
    const world = screenToWorld(coords.x, coords.y);
    dragEnd = { ...world };
  }
});

canvas.addEventListener("mouseup", () => {
  if (isPanning) {
    isPanning = false;
    canvas.style.cursor = "default";
  }
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

    renderMap();
  },
  { passive: false },
);

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
}

function fitCameraToNodes() {
  if (!nodes || nodes.length === 0) return;

  let minX = Infinity;
  let minY = Infinity;
  let maxX = -Infinity;
  let maxY = -Infinity;

  nodes.forEach((node) => {
    if (node.x < minX) minX = node.x;
    if (node.y < minY) minY = node.y;
    if (node.x > maxX) maxX = node.x;
    if (node.y > maxY) maxY = node.y;
  });

  if (minX === maxX) { minX -= 50; maxX += 50; }
  if (minY === maxY) { minY -= 50; maxY += 50; }

  const mapWidth = maxX - minX;
  const mapHeight = maxY - minY;
  const rect = canvas.getBoundingClientRect();

  const padding = 40;

  const zoomX = (rect.width - padding * 2) / mapWidth;
  const zoomY = (rect.height - padding * 2) / mapHeight;

  let requiredZoom = Math.min(zoomX, zoomY);

  camera.zoom = Math.max(camera.minZoom, Math.min(requiredZoom, camera.maxZoom));

  const centerX = minX + mapWidth / 2;
  const centerY = minY + mapHeight / 2;

  camera.x = (rect.width / 2) - (centerX * camera.zoom);
  camera.y = (rect.height / 2) - (centerY * camera.zoom);
}
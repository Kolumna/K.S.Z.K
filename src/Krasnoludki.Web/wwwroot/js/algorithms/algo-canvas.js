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

const mineSvgRaw = (
  iconColor = "white",
) => `<svg width="64" height="64" viewBox="0 0 64 64" fill="none" xmlns="http://www.w3.org/2000/svg">
<rect width="64" height="64" rx="8" fill="#3A2010"/>
<path d="M44.4116 14.0002L14 15.2675V19.0539L46.3168 17.811L44.4116 14V14.0002ZM42.8905 19.4251L40.3881 19.5214L41.1929 34.8119L44.3656 34.1774L42.8905 19.4251ZM31.7685 19.8529L30.2551 19.9113L30.0334 21.1959L27.8462 22.7389L27.8569 22.8102L27.8345 22.8063L27.5824 24.266L28.0916 24.3541L28.4716 26.8553L31.0664 27.3034L32.2632 25.0743L32.7723 25.1623L33.0243 23.7025L33.0019 23.6987L33.0361 23.6351L31.4931 21.4479L31.7685 19.8529ZM25.3954 20.0981L23.5175 20.1704L20.2694 24.678L20.1301 27.405L25.3954 20.0982V20.0981ZM19.0074 20.3438L16.4337 20.4428L14.0703 42.8948L17.8278 43.4489L19.0074 20.3439V20.3438ZM30.6777 21.8166L31.8675 23.5027L28.9916 23.0061L30.6777 21.8166ZM28.9894 24.509L31.3652 24.9192L30.7882 25.9943L29.1728 25.7153L28.9894 24.509ZM47.1521 35.1307L40.8621 36.3888L37.1598 35.1545L34.7975 37.5166H48.7425L47.1521 35.1307ZM25.7519 35.3173L24.9767 37.0228H23.6282V38.5041H24.3034L23.7048 39.8208H21.8178V41.3021H23.0316L22.3582 42.7833H20.1719V44.2646H21.6849L20.5628 46.7334H18.526V48.2146H19.8894L19.1684 49.8008L20.5169 50.4139L21.5165 48.2146H33.3192L32.7637 46.7334H22.1898L23.3121 44.2646H31.838L31.2825 42.7833H23.9854L24.6585 41.3021H30.7268L30.1713 39.8208H25.3319L25.9305 38.5041H29.6776L29.3075 37.5166H32.7027L33.1965 37.0228H26.6038L27.1004 35.9303L25.7519 35.3173ZM31.445 38.9979L34.7664 47.8554C35.2619 47.1772 36.0623 46.7334 36.9596 46.7334C37.9402 46.7334 38.8055 47.2632 39.2831 48.05H43.8528C44.3304 47.2632 45.1958 46.7334 46.1764 46.7334C47.0342 46.7334 47.8032 47.1391 48.3022 47.7673L50.4945 38.9979H31.445ZM36.9596 48.2146C36.2692 48.2146 35.7252 48.7586 35.7252 49.449C35.7252 50.1395 36.2692 50.6834 36.9596 50.6834C37.6501 50.6834 38.194 50.1395 38.194 49.449C38.194 48.7586 37.6501 48.2146 36.9596 48.2146ZM46.1764 48.2146C45.486 48.2146 44.942 48.7586 44.942 49.449C44.942 50.1395 45.486 50.6834 46.1764 50.6834C46.8668 50.6834 47.4108 50.1395 47.4108 49.449C47.4108 48.7586 46.8668 48.2146 46.1764 48.2146Z" fill="${iconColor}"/>
</svg>
`;

const dwarfSvgRaw = (
  iconColor = "white",
) => `<svg width="64" height="64" viewBox="0 0 64 64" fill="none" xmlns="http://www.w3.org/2000/svg">
<rect width="64" height="64" rx="8" fill="#1A3A5A"/>
<path d="M41.1438 14C46.7145 21.8296 41.5412 23.337 38.3314 25.1069C37.7764 23.8995 37.139 22.8045 36.284 21.7771C40.1838 20.1197 40.8888 16.5124 41.1438 14ZM21.8548 14C22.1098 16.5124 22.8147 20.1197 26.7145 21.7771C25.8596 22.8045 25.2221 23.8995 24.6671 25.1069C21.4498 23.337 16.2901 21.8296 21.8548 14ZM31.4993 19.6322C34.8966 21.7021 36.1715 23.712 37.379 26.7418C33.7867 27.6193 29.2194 27.6193 25.6196 26.7418C26.827 23.712 28.102 21.7021 31.4993 19.6322ZM39.5389 27.6268C39.9363 28.3992 41.3838 33.229 41.3988 34.5114C41.0463 34.1439 40.6863 33.7689 40.3338 33.394L39.8388 33.8289C39.7563 31.909 38.8939 29.8692 37.9564 28.0168C38.4439 27.8968 39.0439 27.7693 39.5389 27.6268ZM23.4597 27.6268C23.9247 27.7543 24.6071 27.9193 25.0421 28.0168C24.1122 29.8692 23.2422 31.909 23.1672 33.8289L22.6648 33.394L21.5998 34.5114C20.8124 33.5815 23.0622 28.3992 23.4597 27.6268ZM36.479 28.2867L37.0715 30.0716C35.3616 30.5591 33.8017 31.0166 31.8968 31.5716L32.3842 32.8465C32.8042 32.7265 33.2392 32.6065 33.6667 32.479C33.8842 32.914 34.3791 33.199 34.9191 33.199C35.6841 33.199 36.2915 32.659 36.2915 31.999C36.2915 31.9015 36.284 31.8116 36.2615 31.7216L37.5065 31.3691L38.5489 34.4964L35.8415 37.0688L34.7466 35.0439H28.2595L27.157 37.0688L24.4497 34.4964L25.4921 31.3691C25.8971 31.4816 26.3021 31.5941 26.6995 31.7141C26.6695 31.8041 26.662 31.9015 26.662 31.999C26.662 32.659 27.2695 33.199 28.042 33.199C28.5894 33.199 29.0844 32.9065 29.3019 32.4715C29.7444 32.599 30.1794 32.719 30.6143 32.8465L31.1093 31.5716C29.3694 31.0616 27.7795 30.6041 25.9271 30.0716L26.527 28.2867C30.1044 29.1192 32.8942 29.1192 36.479 28.2867ZM39.7188 35.3964L45.0001 40.9236H41.4438L42.8237 44.5234H38.5114L39.6138 46.4433L36.8015 45.6258L31.4993 50.2381L26.1971 45.6258L23.3922 46.4433L24.4947 44.5234H20.1824L21.5548 40.9236H18L23.2872 35.3964L27.592 39.1387L29.2344 36.2738H33.7867L35.4291 39.1387L39.7188 35.3964ZM32.8792 37.3238H30.1194V38.6737H32.8792V37.3238Z" fill="${iconColor}"/>
</svg>
`;

const imageCache = {};

function getCachedImage(type, color) {
  const key = `${type}_${color}`;

  if (imageCache[key]) {
    return imageCache[key];
  }

  const img = new Image();
  const svgRaw = type === "dwarf" ? dwarfSvgRaw(color) : mineSvgRaw(color);
  img.src = "data:image/svg+xml;utf8," + encodeURIComponent(svgRaw);

  img.onload = () => redrawAll();

  imageCache[key] = img;
  return img;
}

function drawNodeGraphics(x, y, type) {
  const size = 44;
  const offset = size / 2;
  const img = getCachedImage(type);

  ctx.save();
  ctx.shadowColor = "rgba(0, 0, 0, 0.5)";
  ctx.shadowBlur = 8;
  ctx.shadowOffsetY = 4;

  ctx.beginPath();
  if (type === "dwarf") {
    ctx.arc(x, y, 20, 0, Math.PI * 2);
  } else if (type === "mine") {
    ctx.rect(x - 18, y - 18, 36, 36);
  }

  if (img && img.complete && img.naturalWidth !== 0) {
    ctx.drawImage(img, x - offset, y - offset, size, size);
  } else {
    ctx.fillStyle = type === "dwarf" ? "#1A3A5A" : "#3A2010";
    ctx.fill();
    ctx.strokeStyle = "#000";
    ctx.lineWidth = 2;
    ctx.stroke();
  }

  // ctx.fillStyle = "white";
  // ctx.font = "bold 14px Arial";
  // ctx.textAlign = "center";
  // ctx.textBaseline = "middle";
  // ctx.fillText(type.charAt(0).toUpperCase(), x, y);

  ctx.restore();
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
    ctx.arc(x, y, 60, 0, Math.PI * 2);
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
      // ctx.strokeStyle = "#5FFF5F";
      ctx.lineWidth = 1.5;

      // ctx.shadowColor = "#5FFF5F";
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
    highlightColor: "#5FFF5F",
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

  const nodeRadiusAllowance = 50;
  minX -= nodeRadiusAllowance;
  maxX += nodeRadiusAllowance;
  minY -= nodeRadiusAllowance;
  maxY += nodeRadiusAllowance;

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

  const padding = 80;

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

    const selected = rmqDwarves.filter(
      (d) => d.x >= minX && d.x <= maxX && d.y >= minY && d.y <= maxY,
    );

    let leftPointId = null;
    let rightPointId = null;

    if (selected.length > 0) {
      leftPointId = selected[0].pointId;
      rightPointId = selected[selected.length - 1].pointId;

      const lSel = document.getElementById("rmq-l");
      const rSel = document.getElementById("rmq-r");

      if (lSel) lSel.value = leftPointId;
      if (rSel) rSel.value = rightPointId;
    }

    console.log("Zaznaczone krasnoludki:", selected);

    document.dispatchEvent(
      new CustomEvent("rmqSelectionReady", {
        detail: {
          minX,
          maxX,
          minY,
          maxY,
          leftPointId,
          rightPointId,
          selectedDwarfs: selected,
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

const canvas = document.getElementById("mapCanvas");
const ctx = canvas.getContext("2d");

const mineralsColors = {
  Silver: "#C0C0C0",
  Gold: "#FFD700",
  Quartz: "#E0E0E0",
  Coal: "#0f1212",
  Uranium: "#4E9A06",
};

const mineralsNames = {
  Silver: "Srebro",
  Gold: "Złoto",
  Quartz: "Kwarc",
  Coal: "Węgiel",
  Uranium: "Uran",
};

const sleep = (ms) => new Promise((resolve) => setTimeout(resolve, ms));

let nodes =
  typeof INITIAL_NODES !== "undefined" && Array.isArray(INITIAL_NODES)
    ? structuredClone(INITIAL_NODES)
    : [];

document.addEventListener("DOMContentLoaded", () => {
  setupCanvas();
  if (nodes.length > 0) {
    redrawAll();
  }
});

const camera = {
  x: 0,
  y: 0,
  zoom: 1,
  minZoom: 0.2,
  maxZoom: 5,
};

let isPanning = false;
let panStart = { x: 0, y: 0 };
let hoveredNode = null;

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

let pendingCoords = null;
let draggedNode = null;
let isDragging = false;
let hasUnsavedChanges = false;

function markAsChanged() {
  if (!hasUnsavedChanges) {
    hasUnsavedChanges = true;
    document.getElementById("unsavedChangesWarning").style.display = "flex";
    console.log(INITIAL_NODES);
    console.log(INITIAL_NODES.length);
    if (INITIAL_NODES && INITIAL_NODES.length > 0) {
      document.getElementById("updateButton").disabled = false;
    } else {
      document.getElementById("saveButton").disabled = false;
    }
  }
}

function setupCanvas() {
  const dpr = window.devicePixelRatio || 1;
  const rect = canvas.getBoundingClientRect();
  canvas.width = rect.width * dpr;
  canvas.height = rect.height * dpr;
  ctx.scale(dpr, dpr);
  canvas.style.width = `${rect.width}px`;
  canvas.style.height = `${rect.height}px`;
}
setupCanvas();

function redrawAll() {
  redrawCanvas();
  redrawList();
}

function redrawCanvas() {
  drawScene(() => {
    // nodes.forEach((node, i) => {
    //   if (i > 0) {
    //     drawConnection(nodes[i - 1].x, nodes[i - 1].y, node.x, node.y);
    //   }
    // });

    nodes.forEach((node) => {
      const minerals =
        node.type === "dwarf"
          ? (node.preferredMinerals ?? [])
          : node.resource
            ? [node.resource]
            : [];

      drawNodeGraphics(node.x, node.y, node.type, minerals, node.pointId);
    });

    if (hoveredNode && !isDragging) {
      drawTooltip(hoveredNode);
    }
  });
}

function redrawList() {
  const list = document.getElementById("pointsList");
  list.innerHTML = "";

  nodes.forEach((node) => {
    const item = document.createElement("div");

    item.className = "point-item";
    item.innerHTML = `
            <div>
                <span>${node.type === "dwarf" ? "Krasnoludek" : "Kopalnia"} <small style="font-size: 12px; font-family: var(--font-mono)">(Id: ${node.pointId})</small></span>
            </div>
            <div style="display: flex; gap: 8px; align-items: center;">
            <button onclick="cameraGoTo(${node.pointId})" title="Przejdź do punktu">
               <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512" style="height: 20px; width: 20px;"><g class="" style="" transform="translate(0,0)"><path d="M249.334 22.717c-18.64 2.424-35.677 23.574-37.043 51.49v.02a74.612 74.612 0 0 0-.097 3.59c0 16.362 5.658 30.827 13.942 40.818l10.127 12.213-15.592 2.933c-10.75 2.025-18.622 7.702-25.373 16.978-2.285 3.14-4.384 6.707-6.31 10.62-57.54-6.44-97.91-21.06-97.91-37.952 0-17.363 42.647-31.983 102.75-37.97a90.295 90.295 0 0 1-.323-7.636v-.002c0-.84.024-1.674.047-2.51-96.43 6.77-167.298 29.15-167.3 55.71-.002 25.33 64.462 46.86 154.074 54.67-.19.742-.394 1.465-.576 2.216-2.36 9.72-4.05 20.22-5.268 31.03-.01 0-.02 0-.03.002-.418 3.653-.78 7.34-1.095 11.046l.05-.005c-1.316 15.777-1.772 31.88-1.893 46.95h35.894l2.115 28.4c-68.24-4.994-118.444-21.004-118.444-39.843 0-13.243 24.83-24.89 63.27-32.33.3-4.056.66-8.115 1.076-12.162-76.42 9.353-129.17 29.168-129.172 52.086-.002 28.17 79.71 51.643 185.098 56.768l5.94 79.77c10.5 2.648 24.84 4.162 39.017 4.068 13.79-.092 27.235-1.71 36.45-4l5.263-79.846c105.308-5.14 184.935-28.605 184.935-56.76 0-23.013-53.196-42.895-130.13-52.2.304 4.02.557 8.047.755 12.07 38.883 7.43 63.965 19.17 63.965 32.536 0 18.84-49.804 34.85-117.908 39.844l1.87-28.402h34.18c-.012-15.113-.127-31.27-1.033-47.094.01 0 .02.002.032.004a406.307 406.307 0 0 0-.782-10.986l-.02-.002c-.94-11.157-2.367-21.984-4.546-31.967-.09-.405-.184-.803-.275-1.206 89.518-7.826 153.893-29.344 153.893-54.656 0-26.787-72.076-49.332-169.77-55.887.025.895.053 1.788.053 2.688 0 2.5-.104 4.97-.304 7.407 61.19 5.836 104.61 20.61 104.61 38.2 0 16.805-39.633 31.355-96.524 37.848-2.01-4.283-4.26-8.15-6.762-11.505-6.83-9.167-15.063-14.81-27.14-16.682l-15.913-2.47 10.037-12.59c6.928-8.69 11.912-20.715 13.057-34.268h.002c.163-1.95.25-3.93.25-5.938 0-.77-.022-1.532-.048-2.29-.015-.48-.033-.958-.057-1.434h-.002c-1.48-29.745-20.507-51.3-41.076-51.3-2.528 0-3.966-.087-4.03-.08h-.003zM194.54 355.822c-97.11 6.655-168.573 29.11-168.573 55.8 0 31.932 102.243 57.815 228.367 57.815S482.7 443.555 482.7 411.623c0-26.608-71.02-49.004-167.67-55.736l-.655 9.93c60.363 6.055 103.074 20.956 103.074 38.394 0 22.81-73.032 41.298-163.12 41.298-90.088 0-163.12-18.49-163.12-41.297 0-17.533 43.18-32.502 104.07-38.493l-.74-9.895z" fill="#fff" fill-opacity="1"/></g></svg>
            </button><button onclick="deleteNode(${node.pointId})" title="Usuń">
                <svg viewBox="0 0 512 512" style="height: 20px; width: 20px;">
                    <path d="M199 103v50h-78v30h270v-30h-78v-50H199zm18 18h78v32h-78v-32zm-79.002 80 30.106 286h175.794l30.104-286H137.998zm62.338 13.38.64 8.98 16 224 .643 8.976-17.956 1.283-.64-8.98-16-224-.643-8.976 17.956-1.283zm111.328 0 17.955 1.284-.643 8.977-16 224-.64 8.98-17.956-1.284.643-8.977 16-224 .64-8.98zM247 215h18v242h-18V215z" fill="#fff"></path>
                </svg>
            </button></div>`;
    list.appendChild(item);
  });
}

function cameraGoTo(pointId, targetZoom = 2) {
  const node = nodes.find((n) => n.pointId === pointId);
  if (node) {
    camera.zoom = Math.min(camera.maxZoom, Math.max(camera.minZoom, targetZoom));

    const rect = canvas.getBoundingClientRect();
    const centerX = rect.width / 2;
    const centerY = rect.height / 2;

    camera.x = centerX - node.x * camera.zoom;
    camera.y = centerY - node.y * camera.zoom;

    redrawAll();
  }
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

const oreSvgRaw = `<svg width="36" height="36" viewBox="0 0 36 36" fill="none" xmlns="http://www.w3.org/2000/svg">
<rect x="0.5" y="0.5" width="35" height="35" rx="7.5" fill="white" stroke="black"/>
<path d="M22.3598 29.9998L16.1917 29.8933L10.4841 21.8432L11.2395 19.1408L13.0951 19.6344L15.2077 21.8463L14.2885 18.6956L15.5428 16.835L13.2933 11.3605L15.5648 7.17962L19.3764 8.36194L21.8107 12.5959L20.2597 12.859L18.2349 11.0164L19.4225 13.5456L19.3872 15.5118L18.0142 16.4708L19.8534 16.5384L22.3343 17.7926L23.8472 16.3258L25.8725 17.3286L26.4328 19.3061L25.4345 18.5807L21.3212 20.6839L18.9541 19.874L19.2625 18.4452L17.012 18.6175L18.2525 19.2444L17.7531 20.4517L20.8072 22.098L21.8775 26.2774L21.0645 28.105L22.6114 27.0336L26.1282 26.5369L22.3599 30L22.3598 29.9998ZM14.0179 29.8137L11.2036 29.6928L12.6593 26.3247L14.4685 28.9398L14.0179 29.8137ZM9.45131 27.8645L8.86825 26.0293L7 25.4924L8.83431 24.9088L9.37169 23.0407L9.95527 24.8748L11.8235 25.4126L9.98921 25.9958L9.45141 27.8644L9.45131 27.8645ZM22.7639 26.117L21.6291 21.6848L25.4201 19.633L28.245 22.7547L27.2628 25.5032L22.7639 26.117ZM13.1452 18.7985L10.0335 17.9769L8.96534 14.3376L12.4418 12.3669L14.3857 16.7938L13.1452 18.7985V18.7985ZM22.2018 16.7477L20.1819 15.7131L20.2647 13.6682L22.5594 13.1464L23.9672 15.2142L22.2019 16.7476L22.2018 16.7477ZM25.2984 14.2623L24.5494 11.872L22.1214 11.1637L24.5109 10.4132L25.2192 7.98558L25.9692 10.3752L28.3972 11.0844L26.0076 11.8335L25.2984 14.2623H25.2984ZM21.4353 10.6011L19.9268 7.61417L18.4023 7.1953L19.5939 5L21.1364 5.75134L22.0752 8.88458L21.4353 10.6011H21.4353Z" fill="black"/>
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

function drawNodeGraphics(
  x,
  y,
  type,
  minerals = [],
  isDraggedNode = false,
  pointId = null,
) {
  const size = 44;
  const offset = size / 2;

  // const color = minerals.includes("Uranium") ? "#35ff43" : "white";

  const img = getCachedImage(type);

  ctx.save();
  ctx.shadowColor = "rgba(0, 0, 0, 0.5)";
  ctx.shadowBlur = 8;
  ctx.shadowOffsetY = 4;

  if (img && img.complete && img.naturalWidth !== 0) {
    ctx.drawImage(img, x - offset, y - offset, size, size);
  } else {
    ctx.beginPath();
    ctx.arc(x, y, 20, 0, Math.PI * 2);
    ctx.fillStyle = type === "dwarf" ? "#8B4513" : "#888";
    ctx.fill();
    ctx.stroke();
  }

  if (minerals.length > 0) {
    for (let i = 0; i < minerals.length; i++) {
      ctx.beginPath();
      ctx.arc(x - 15 + i * 20, y + 35, 6, 0, Math.PI * 2);
      ctx.fillStyle = mineralsColors[minerals[i]] || "#000";
      ctx.fill();
      ctx.stroke();
    }
  }

  ctx.restore();
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

function getNodeAt(x, y) {
  for (let i = nodes.length - 1; i >= 0; i--) {
    const node = nodes[i];
    const dx = x - node.x;
    const dy = y - node.y;

    if (Math.sqrt(dx * dx + dy * dy) <= 22) {
      return node;
    }
  }
  return null;
}

function confirmNode() {
  if (!pendingCoords) return;

  let selectedTypes = [];

  if (MapState.mode === "mine") {
    const val = document.getElementById("nodeTypeSelect").value;
    selectedTypes = [val];
  } else {
    const checkboxes = document.querySelectorAll(
      "#mineralsCheckboxGroup input:checked",
    );
    selectedTypes = Array.from(checkboxes).map((cb) => cb.value);
  }

  nodes.push({
    pointId: nodes.length + 1,
    x: pendingCoords.x,
    y: pendingCoords.y,
    type: MapState.mode,
    capacity:
      MapState.mode === "mine"
        ? parseInt(document.getElementById("capacityInput").value) || 0
        : undefined,
    preferredMinerals: MapState.mode === "dwarf" ? selectedTypes : undefined,
    voiceLoudness:
      MapState.mode === "dwarf"
        ? parseInt(document.getElementById("loudnessInput").value) || 0
        : undefined,
    resource: MapState.mode === "mine" ? selectedTypes[0] : undefined,
  });

  markAsChanged();
  redrawAll();
  closeAllModals();
}

function deleteNode(index) {
  nodes = nodes.filter((n) => n.pointId !== index);
  markAsChanged();
  redrawAll();
}

function closeAllModals() {
  document.getElementById("dwarfModal").style.display = "none";
  document.getElementById("mineModal").style.display = "none";

  document
    .querySelectorAll("#mineralsCheckboxGroup input")
    .forEach((cb) => (cb.checked = false));

  pendingCoords = null;
}

function cancelNode() {
  closeAllModals();
}
function closeModal() {
  closeAllModals();
}

function showLoading(message = "Zapisywanie mapy...") {
  const overlay = document.getElementById("loading-overlay");
  overlay.querySelector("span").textContent = message;
  overlay.style.display = "flex";
}

function hideLoading() {
  document.getElementById("loading-overlay").style.display = "none";
}

async function saveMapBtn() {
  const scenarioId = document.getElementById("currentScenarioId").value;

  const token = document.querySelector(
    'input[name="__RequestVerificationToken"]',
  ).value;

  try {
    showLoading("Zapisywanie mapy...");
    console.log("Wysyłanie danych do serwera:", {
      scenarioId,
      nodes,
    });

    const res = await fetch("?handler=SaveHoffApi", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        scenarioId,
        nodes: JSON.stringify(nodes),
      }),
    });

    if (!res.ok) {
      hideLoading();
      throw new Error("Błąd podczas generowania pliku .hoff");
    }

    showLoading("Wczytywanie scenariusza...");
    await sleep(1000);
    window.location.reload();
  } catch (err) {
    hideLoading();
    console.error(err);
    alert("Nie udało się zapisać mapy: " + err.message);
  }
}

async function updateMapBtn() {
  const scenarioId = document.getElementById("currentScenarioId").value;

  const token = document.querySelector(
    'input[name="__RequestVerificationToken"]',
  ).value;

  try {
    showLoading("Zapisywanie mapy...");

    console.log("Wysyłanie danych do serwera:", {
      scenarioId,
      nodes,
    });

    const res = await fetch("?handler=UpdateHoffApi", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        scenarioId,
        nodes: JSON.stringify(nodes),
      }),
    });

    if (!res.ok) {
      hideLoading();
      throw new Error("Błąd podczas generowania pliku .hoff");
    }

    showLoading("Wczytywanie scenariusza...");
    await sleep(1000);
    window.location.reload();
  } catch (err) {
    hideLoading();
    console.error(err);
    alert("Nie udało się zapisać mapy: " + err.message);
  }
}

window.addEventListener("pageshow", () => {
  hideLoading();
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

    redrawAll();
  },
  { passive: false },
);

function getCW() {
  return canvas.getBoundingClientRect().width;
}
function getCH() {
  return canvas.getBoundingClientRect().height;
}

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
  redrawAll();
}
function zoomOut() {
  zoomToCenter(1 / 1.2);
  redrawAll();
}
function resetZoom() {
  camera.x = 0;
  camera.y = 0;
  camera.zoom = 1;

  redrawAll();
}

canvas.addEventListener("mousedown", function (e) {
  const rect = canvas.getBoundingClientRect();
  const sx = e.clientX - rect.left;
  const sy = e.clientY - rect.top;

  if (e.button === 1) {
    e.preventDefault();
    isPanning = true;
    panStart = { x: sx - camera.x, y: sy - camera.y };
    canvas.style.cursor = "grabbing";
    return;
  }

  const worldPos = screenToWorld(sx, sy);
  const clickedNode = getNodeAt(worldPos.x, worldPos.y);

  if (clickedNode) {
    draggedNode = clickedNode;
    isDragging = false;
  }
});

canvas.addEventListener("mousemove", function (e) {
  const rect = canvas.getBoundingClientRect();
  const sx = e.clientX - rect.left;
  const sy = e.clientY - rect.top;

  if (isPanning) {
    camera.x = sx - panStart.x;
    camera.y = sy - panStart.y;
    redrawAll();
    return;
  }

  const worldPos = screenToWorld(sx, sy);

  if (draggedNode) {
    draggedNode.x = worldPos.x;
    draggedNode.y = worldPos.y;
    isDragging = true;
    redrawAll();
  } else {
    const newHoveredNode = getNodeAt(worldPos.x, worldPos.y);
    if (hoveredNode !== newHoveredNode) {
      hoveredNode = newHoveredNode;
      redrawAll();
    }

    canvas.style.cursor = hoveredNode ? "grab" : "default";
  }
});

canvas.addEventListener("mouseleave", function () {
  isPanning = false;
  draggedNode = null;
  isDragging = false;
  hoveredNode = null;
  redrawAll();
  canvas.style.cursor = "default";
});

canvas.addEventListener("mouseup", function (e) {
  if (isPanning) {
    isPanning = false;
    canvas.style.cursor = "default";
    return;
  }

  if (draggedNode && isDragging) {
    markAsChanged();
  }
  draggedNode = null;
  isDragging = false;
});

canvas.addEventListener("mouseleave", function () {
  isPanning = false;
  draggedNode = null;
  isDragging = false;
  canvas.style.cursor = "default";
});

canvas.addEventListener("click", function (e) {
  if (isDragging) {
    isDragging = false;
    return;
  }

  const rect = canvas.getBoundingClientRect();
  const sx = e.clientX - rect.left;
  const sy = e.clientY - rect.top;
  const worldPos = screenToWorld(sx, sy);

  if (getNodeAt(worldPos.x, worldPos.y)) {
    return;
  }

  pendingCoords = { x: worldPos.x, y: worldPos.y };

  if (MapState.mode === "dwarf") {
    document.getElementById("dwarfModal").style.display = "flex";
  } else {
    document.getElementById("mineModal").style.display = "flex";
  }
});

function loadJSON() {
  const input = document.createElement("input");
  input.type = "file";
  input.accept = "application/json";
  input.onchange = (e) => {
    const file = e.target.files[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = (event) => {
      try {
        const content = event.target.result;
        const json = JSON.parse(content);
        if (Array.isArray(json)) {
          nodes = json;
          markAsChanged();
          redrawAll();
        } else {
          alert("Nieprawidłowy format pliku JSON");
        }
      } catch (err) {
        alert("Błąd podczas wczytywania pliku JSON: " + err.message);
      }
    };

    reader.readAsText(file);
  };
  input.click();
}

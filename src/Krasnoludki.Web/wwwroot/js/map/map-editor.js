const canvas = document.getElementById("mapCanvas");
const ctx = canvas.getContext("2d");
let nodes = [];
let pendingCoords = null;

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
  redrawDebugCode();
}

function redrawCanvas() {
  ctx.clearRect(0, 0, canvas.width, canvas.height);

  // nodes.forEach((node, i) => {
  //   if (i > 0) {
  //     drawConnection(nodes[i - 1].x, nodes[i - 1].y, node.x, node.y);
  //   }
  // });

  nodes.forEach((node) => {
    drawNodeGraphics(node.x, node.y, node.type);
  });
}

function redrawList() {
  const list = document.getElementById("pointsList");
  list.innerHTML = "";

  nodes.forEach((node, index) => {
    const item = document.createElement("div");
    item.className = "point-item";
    item.innerHTML = `
            <div>
                <span>${node.type === "dwarf" ? "Krasnoludek" : "Kopalnia"} ${index + 1}</span>
            </div>
            <button onclick="deleteNode(${index})" title="Usuń">
                <svg viewBox="0 0 512 512" style="height: 20px; width: 20px;">
                    <path d="M199 103v50h-78v30h270v-30h-78v-50H199zm18 18h78v32h-78v-32zm-79.002 80 30.106 286h175.794l30.104-286H137.998zm62.338 13.38.64 8.98 16 224 .643 8.976-17.956 1.283-.64-8.98-16-224-.643-8.976 17.956-1.283zm111.328 0 17.955 1.284-.643 8.977-16 224-.64 8.98-17.956-1.284.643-8.977 16-224 .64-8.98zM247 215h18v242h-18V215z" fill="#fff"></path>
                </svg>
            </button>`;
    list.appendChild(item);
  });
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

function drawConnection(x1, y1, x2, y2) {
  ctx.beginPath();
  ctx.setLineDash([5, 5]);
  ctx.moveTo(x1, y1);
  ctx.lineTo(x2, y2);
  ctx.strokeStyle = "#444";
  ctx.lineWidth = 2;
  ctx.stroke();
  ctx.setLineDash([]);
}

canvas.addEventListener("click", function (e) {
  const rect = canvas.getBoundingClientRect();
  const x = e.clientX - rect.left;
  const y = e.clientY - rect.top;

  pendingCoords = { x, y };

  if (MapState.mode === "dwarf") {
    document.getElementById("dwarfModal").style.display = "flex";
  } else {
    document.getElementById("mineModal").style.display = "flex";
  }
});

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

    if (selectedTypes.length === 0) {
      alert("Wybierz przynajmniej jeden surowiec!");
      return;
    }
  }

  nodes.push({
    x: pendingCoords.x,
    y: pendingCoords.y,
    type: MapState.mode,
    capacity: MapState.mode === "mine" ? parseInt(document.getElementById("capacityInput").value) || 0 : undefined,
    minerals: selectedTypes,
  });

  redrawAll();
  closeAllModals();
}

function deleteNode(index) {
  nodes.splice(index, 1);
  redrawAll();
}

function closeAllModals() {
  document.getElementById("dwarfModal").style.display = "none";
  document.getElementById("mineModal").style.display = "none";
  
  document.querySelectorAll('#mineralsCheckboxGroup input').forEach(cb => cb.checked = false);
  
  pendingCoords = null;
}

function cancelNode() { closeAllModals(); }
function closeModal() { closeAllModals(); }

function redrawDebugCode() {
  const debug = document.getElementById("debugOutput");
  debug.textContent = JSON.stringify(nodes, null, 2);
}
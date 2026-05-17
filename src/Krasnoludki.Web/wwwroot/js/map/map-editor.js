const canvas = document.getElementById("mapCanvas");
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

setupCanvas();

let nodes = [];

canvas.addEventListener("click", function (e) {
  const rect = canvas.getBoundingClientRect();
  const x = (e.clientX - rect.left) * (canvas.width / rect.width);
  const y = (e.clientY - rect.top) * (canvas.height / rect.height);

  const newNode = { x: x, y: y, type: "miner" };
  nodes.push(newNode);

  if (nodes.length > 1) {
    const prev = nodes[nodes.length - 2];
    // drawConnection(prev.x, prev.y, newNode.x, newNode.y);
  }

  drawNode(x, y, MapState.mode);
});

function drawNode(x, y, type) {
  ctx.beginPath();
  ctx.arc(x, y, 30, 0, Math.PI * 2);
  ctx.fillStyle = type === "dwarf" ? "#8B4513" : MINERAL_COLORS[type] || "#888";
  ctx.strokeStyle = "#000";
  ctx.fill();
  ctx.lineWidth = 2;
  ctx.stroke();

  const label = type.charAt(0).toUpperCase();

  ctx.fillStyle = "white";
  ctx.font = "bold 16px Arial";

  ctx.strokeStyle = "#c8a030"; // Kolor bordera
  ctx.lineWidth = 2; // Grubość bordera
  ctx.stroke();

  ctx.textAlign = "center";
  ctx.textBaseline = "middle";

  ctx.fillText(label, x, y);
}

function drawConnection(x1, y1, x2, y2, color = "#000") {
  ctx.beginPath();
  ctx.lineWidth = 2;
  ctx.strokeStyle = color;

  ctx.moveTo(x1, y1);
  ctx.lineTo(x2, y2);

  ctx.stroke();
}

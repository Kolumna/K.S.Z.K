const canvas = document.getElementById("mapCanvas");
const ctx = canvas.getContext("2d");
let nodes = [];

canvas.addEventListener("click", function (e) {
  const rect = canvas.getBoundingClientRect();
  const x = (e.clientX - rect.left) * (canvas.width / rect.width);
  const y = (e.clientY - rect.top) * (canvas.height / rect.height);

  const newNode = { x: x, y: y, type: "miner" };
  nodes.push(newNode);

  if (nodes.length > 1) {
    const prev = nodes[nodes.length - 2];
    drawConnection(prev.x, prev.y, newNode.x, newNode.y);
  }

  drawNode(x, y, "miner");
});

function drawNode(x, y, type) {
  ctx.beginPath();

  ctx.arc(x, y, 20, 0, Math.PI * 2);

  ctx.fillStyle = "#fff";
  ctx.strokeStyle = "#000";

  ctx.fill();
  ctx.lineWidth = 2;
  ctx.stroke();
}

function drawConnection(x1, y1, x2, y2, color = "#000") {
  ctx.beginPath();
  ctx.lineWidth = 2;
  ctx.strokeStyle = color;

  ctx.moveTo(x1, y1);
  ctx.lineTo(x2, y2);

  ctx.stroke();
}

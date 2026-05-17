window.MapState = {
    mode: 'dwarf',
    nodes: [],
};

window.MINERAL_COLORS = {
    Gold: '#e8c060',
    Silver: '#c0c8d0',
    Coal: '#808878',
    Quartz: '#a0c0e0',
};

function setMode(m) {
  MapState.mode = m;

  document
    .getElementById("btn-dwarf")
    .classList.toggle("active", m === "dwarf");
  document.getElementById("btn-mine").classList.toggle("active", m === "mine");
  canvas.classList.remove("mode-dwarf", "mode-mine");
  canvas.classList.add(`mode-${m}`);

  // updateModeHint();
}
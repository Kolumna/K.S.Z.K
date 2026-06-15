const algorithmResults =
  typeof ALGORITHM_RESULTS !== "undefined" ? ALGORITHM_RESULTS : {};

const DWARF_NODES = INITIAL_NODES.filter((n) => n.type === "dwarf");
const MINE_NODES = INITIAL_NODES.filter((n) => n.type === "mine");

const currentAlgo = new URLSearchParams(window.location.search).get(
  "algorithm",
);

async function setRunningAlgoSession(algorithmType, isRunning) {
  await fetch("?handler=SetRunningAlgo", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ algorithmType, isRunning }),
  });
}

async function runAlgorithm(algorithmType) {
  if (!algorithmType) return;

  await setRunningAlgoSession(algorithmType, true);
  setLoadingState(algorithmType, true);

  try {
    let res;

    switch (algorithmType) {
      case "convexHull":
        res = await MapApiService.calculateConvexHull(INITIAL_NODES);
        break;
      case "matching":
        res = await MapApiService.calculateMatching({
          dwarves: DWARF_NODES,
          mines: MINE_NODES,
        });
        break;
      case "minCost":
        res = await MapApiService.calculateMinCost({
          dwarves: DWARF_NODES,
          mines: MINE_NODES,
        });

        break;
      case "rmq": {
        const from = parseInt(
          document.getElementById("rmq-from-input")?.value ?? "0",
        );
        const to = parseInt(
          document.getElementById("rmq-to-input")?.value ?? "100",
        );
        const dwarfsCompartment = DWARF_NODES.filter(
          (d) => d.pointId >= from && d.pointId <= to,
        );
        res = await MapApiService.calculateSegmentTree({
          dwarves: dwarfsCompartment,
        });
        break;
      }
      default:
        throw new Error("Nieznany algorytm: " + algorithmType);
    }

    await setRunningAlgoSession(algorithmType, false);

    if (res?.success) window.location.reload();
    else if (res) alert("Błąd: " + res.message);
  } catch (err) {
    console.error(err);
    alert("Wystąpił błąd: " + err.message);
    await setRunningAlgoSession(algorithmType, false);
  } finally {
    setLoadingState(algorithmType, false);
  }
}

function downloadResults(algorithmType) {
  const keyMap = {
    convexHull: "convexHull",
    matching: "matching",
    minCost: "minCost",
    rmq: "segmentTree",
  };

  const data = algorithmResults[keyMap[algorithmType]];

  if (!data) {
    alert("Brak wyników. Najpierw uruchom algorytm.");
    return;
  }

  const blob = new Blob([JSON.stringify(data, null, 2)], {
    type: "application/json",
  });
  const url = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = `wyniki-${algorithmType}.json`;
  a.click();
  URL.revokeObjectURL(url);
}

const setLoadingState = (algorithmType, isLoading) => {
  const btn = document.getElementById(`algo-run-button-${algorithmType}`);

  if (isLoading) {
    btn.disabled = isLoading;
    btn.textContent = "Ładowanie...";
  }
};

document.addEventListener("DOMContentLoaded", () => {
  if (Array.isArray(RUNNING_ALGOS)) {
    RUNNING_ALGOS.forEach((algo) => setLoadingState(algo, true));
  }
});

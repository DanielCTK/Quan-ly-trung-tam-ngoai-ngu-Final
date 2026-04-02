document.addEventListener("DOMContentLoaded", () => {
  const confirmModalElement = document.getElementById("confirmActionModal");
  const confirmMessageElement = document.getElementById("confirmActionMessage");
  const confirmButtonElement = document.getElementById("confirmActionButton");
  const confirmModal = confirmModalElement ? new bootstrap.Modal(confirmModalElement) : null;

  document.querySelectorAll(".confirm-action").forEach((trigger) => {
    trigger.addEventListener("click", (event) => {
      event.preventDefault();
      if (!confirmModal || !confirmMessageElement || !confirmButtonElement) return;

      confirmMessageElement.textContent = trigger.getAttribute("data-confirm-message") || "Ban co chac chan muon tiep tuc?";
      confirmButtonElement.onclick = () => {
        confirmModal.hide();
        showInlineToast("Da xac nhan thao tac mock thanh cong.", "success");
      };
      confirmModal.show();
    });
  });

  document.querySelectorAll(".mock-submit-form").forEach((form) => {
    form.addEventListener("submit", (event) => {
      event.preventDefault();
      showInlineToast("Da luu tam giao dien mock. // TODO: connect database later", "success");
    });
  });

  document.querySelectorAll(".app-chart").forEach((canvas) => {
    const raw = canvas.getAttribute("data-chart-config");
    if (!raw) return;
    const config = JSON.parse(raw);
    const colors = config.Colors?.length ? config.Colors : ["#1d4ed8"];

    new Chart(canvas, {
      type: config.ChartType || "bar",
      data: {
        labels: config.Labels,
        datasets: [{
          label: config.Title,
          data: config.Values,
          borderColor: colors[0],
          backgroundColor: config.ChartType === "line" ? "rgba(37,99,235,0.15)" : colors,
          fill: config.ChartType === "line",
          borderWidth: 2,
          borderRadius: 10,
          tension: 0.35
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: config.ChartType === "doughnut" },
          tooltip: { mode: "index", intersect: false }
        },
        scales: config.ChartType === "doughnut" ? {} : {
          x: { grid: { display: false } },
          y: { beginAtZero: true, ticks: { precision: 0 } }
        }
      }
    });
  });
});

function showInlineToast(message, type) {
  const toastContainer = document.createElement("div");
  toastContainer.className = "toast-container position-fixed bottom-0 end-0 p-3";
  toastContainer.innerHTML = `
    <div class="toast show border-0 app-toast app-toast-${type}" role="alert">
      <div class="toast-header border-0">
        <strong class="me-auto">Thong bao</strong>
        <small>vua xong</small>
        <button type="button" class="btn-close ms-2 mb-1" data-bs-dismiss="toast"></button>
      </div>
      <div class="toast-body">${message}</div>
    </div>`;
  document.body.appendChild(toastContainer);
  setTimeout(() => toastContainer.remove(), 3200);
}

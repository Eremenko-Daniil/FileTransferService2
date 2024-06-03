document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("loadLogs").addEventListener("click", function () {
        fetch("/api/Log/GetLogs")
            .then(response => response.json())
            .then(data => {
                document.getElementById("logContent").textContent = data.logs;
            })
            .catch(error => console.error("Error loading logs:", error));
    });

    document.getElementById("clearLogs").addEventListener("click", function () {
        fetch("/api/Log/ClearLogs", {
            method: "POST"
        })
            .then(response => response.json())
            .then(data => {
                document.getElementById("logContent").textContent = data.message;
            })
            .catch(error => console.error("Error clearing logs:", error));
    });
});
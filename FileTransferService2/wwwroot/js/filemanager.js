document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".file-name").forEach(function (element) {
        var fileName = element.textContent.trim();
        if (fileName.startsWith(" ")) {
            element.textContent = fileName.substring(1);
        }

        // Если это изображение, создайте элемент изображения
        var fileExt = fileName.split('.').pop().toLowerCase();
        if (["jpg", "jpeg", "png", "gif"].includes(fileExt)) {
            var img = document.createElement("img");
            img.src = `/path/to/your/images/${fileName}`;
            img.alt = fileName;
            img.classList.add("file-thumbnail");

            element.parentElement.insertBefore(img, element);
            element.style.display = "none"; // Скрыть текстовое имя файла
        }
    });
});

function allowDrop(event) {
    event.preventDefault();
}

function onDragStart(event, fileName) {
    event.dataTransfer.setData("text", fileName);
}

function onDrop(event, target) {
    event.preventDefault();
    var data = event.dataTransfer.getData("text");
    moveFile(data, target);
}

function moveFile(fileName, target) {
    fetch('/FileTransfer/MoveFile', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ fileName: fileName, target: target })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                location.reload();
            } else {
                alert('Ошибка перемещения файла: ' + data.message);
            }
        })
        .catch(error => {
            console.error('Ошибка:', error);
        });
}
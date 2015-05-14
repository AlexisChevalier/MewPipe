"use strict";

window.uploadVideoModule = window.uploadVideoModule || {};

(function(module, window) {

    var $elements = {
            dropZone: $("#uploadZone"),
            fileView: $("#fileView"),
            doneView: $("#doneView"),
            errorView: $("#errorView"),
            videoFileName: $("#videoFileName"),
            progressBarInner: $("#innerProgressBar"),
            progressBarText: $("#percent"),
            editVideoButton: $("#editVideo")
        },
        dragEnterCounter = 0,
        uploadedVideoId = null;

    function confirmExit(e) {

        e = e || window.event;

        if (e) {
            e.returnValue = "You are currently uploading a video, are you sure you want to close the window ?";
        }

        return "You are currently uploading a video, are you sure you want to close the window ?";
    }

    function onNewFile(id, file) {
        $elements.dropZone.fadeOut(300);
        $elements.fileView.fadeIn(300);

        $elements.videoFileName.text(file.name);
        window.onbeforeunload = confirmExit;
    }

    function onDragEnter(event) {
        event.preventDefault();
        dragEnterCounter++;
        $elements.dropZone.addClass("drag-over");
    }

    function onDragLeave(event) {
        dragEnterCounter--;
        if (dragEnterCounter === 0) {
            $elements.dropZone.removeClass("drag-over");
        }
    }

    function onUploadProgress(id, percent) {
        var percentStr = percent + "%";
        $elements.progressBarInner.width(percentStr);
        $elements.progressBarText.text(percentStr);
    }

    function onUploadSuccess(id, data) {
        $elements.progressBarInner.width("100%");
        $elements.fileView.fadeOut(300);
        $elements.doneView.fadeIn(300);
        uploadedVideoId = data.VideoId;
        window.onbeforeunload = null;
    }

    function onUploadError(id, message) {
        $elements.dropZone.fadeOut(300);
        $elements.errorView.fadeOut(300);
        $elements.fileView.fadeOut(300);
        $elements.doneView.fadeIn(300);
        window.onbeforeunload = null;
    }

    function onFileTypeError(file) {
        FlashMessages.setMessage("danger", message);
    }

    function onFileSizeError(file) {
        FlashMessages.setMessage("danger", message);
    }

    function getDmUploaderOptions() {
        return {
            url: module.uploadUrl,
            dataType: "json",
            allowedTypes: "video/*",
            maxFiles: 1,
            maxFileSize: 500000000,
            onInit: function() {},
            onBeforeUpload: function(id) {},
            onNewFile: onNewFile,
            onDragEnter: onDragEnter,
            onDragLeave: onDragLeave,
            onComplete: function() {},
            onUploadProgress: onUploadProgress,
            onUploadSuccess: onUploadSuccess,
            onUploadError: onUploadError,
            onFileTypeError: onFileTypeError,
            onFileSizeError: onFileSizeError,
            onFallbackMode: function(message) {
                alert("Browser not supported ! Please update your browser or contact the support !");
            }
        };
    }

    function setUploadUrl(url) {
        module.uploadUrl = url;
    };

    function init() {
        $elements.dropZone.dmUploader(getDmUploaderOptions());
    };

    function goToEditPage() {
        if (uploadedVideoId === null) {

            FlashMessages.setMessage("danger", "An unknown error occured. Please navigate manually to your videos to continue with the edition.");
            return;
        }
        window.location = "/myVideos/edit/" + uploadedVideoId;
    }

    module.init = init;
    module.setUploadUrl = setUploadUrl;

    $elements.editVideoButton.click(goToEditPage);

}(window.uploadVideoModule, window));

$(function () {
    window.uploadVideoModule.init();
})
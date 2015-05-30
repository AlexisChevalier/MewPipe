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
        $elements.dropZone.hide();
        $elements.fileView.show();

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

    function onUploadError(id, message) {
        $elements.dropZone.hide();
        $elements.errorView.show();
        $elements.fileView.hide();
        $elements.doneView.hide();
        window.onbeforeunload = null;
    }

    function onUploadSuccess(id, data) {
        uploadedVideoId = data.VideoId;
        if (!uploadedVideoId || uploadedVideoId === null || uploadedVideoId === "") {
            onUploadError(null, null);
        } else {
            $elements.progressBarInner.width("100%");
            $elements.fileView.hide();
            $elements.doneView.show();
            $elements.editVideoButton.attr("href", "/myVideos/edit/" + uploadedVideoId);
            window.onbeforeunload = null;
        }
    }

    function onFileTypeError(file) {
        FlashMessages.setMessage("danger", "The file type you selected is not accepted by MewPipe.");
    }

    function onFileSizeError(file) {
        FlashMessages.setMessage("danger", "The video file you selected is too large. MewPipe only accepts files up to 500 MB.");
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


    module.init = init;
    module.setUploadUrl = setUploadUrl;
}(window.uploadVideoModule, window));

$(function () {
    window.uploadVideoModule.init();
})
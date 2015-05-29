"use strict";

window.playerModule = window.playerModule || {};

(function (module) {

    var $elements = {
        player: $("#mewPipePlayerElement"),
        playerContainer: $("#videoPlayerContainer")
    },
    playerObject = null,
    videoDetails = null,
    videoEndpointUri = "http://videos-repository.mewpipe.local:44403/api/videosData/",
    thumbnailEndpointUri = "http://videos-repository.mewpipe.local:44403/api/thumbnailsData/";

    function detectVideoFormat() {
        var format = null;

        if (Modernizr.video) {
            if (Modernizr.video.h264) {
                format = "video/mp4";
            } else if (Modernizr.video.ogg) {
                format = "video/ogg";
            }
        }
        format = "video/mp4";
        return format;
    }

    function getVideoSources(format) {
        var sources = [];
        for (var i = 0; i < videoDetails.VideoFiles.length; i++) {
            var mimeType = videoDetails.VideoFiles[i].MimeType.HttpMimeType;
            var qualityType = videoDetails.VideoFiles[i].QualityType.Name;
            if (mimeType === format) {
                var source = videoEndpointUri + videoDetails.PublicId + "?encoding=" + mimeType + "&quality=" + qualityType;
                sources.push({
                    type: format,
                    src: source,
                    "data-res": qualityType
                });
            }
        }
        return sources;
    }
    
    function initPlayer(videoData, done) {
        videoDetails = videoData;
        var format = detectVideoFormat();
        var sources = getVideoSources(format);

        //Init html player
        var playerElement = "<video id='mewPipePlayerElement'" +
                " width='854' " +
                "height='480' " + 
                "autoplay " + 
                "controls " + 
                "poster='" + thumbnailEndpointUri + videoDetails.PublicId + "' " + 
                "class='video-js vjs-default-skin'>";

        for (var i = 0; i < sources.length; i++) {
            playerElement += "<source src='" + sources[i].src + "' type='" + sources[i].type + "' data-res='" + sources[i]["data-res"] + "' />";
        }

        playerElement += "</video>";

        $elements.playerContainer.html(playerElement);

        videojs("mewPipePlayerElement", {
            plugins: {
                resolutionSelector: {
                    default_res: "240"
                }
            }
        }, function () {
            playerObject = this;
            playerObject.src(sources);
            playerObject.on("changeRes", function () {});

            playerObject.play();

            this.on("ended", function () {});
        });
    }

    videojs.options.flash.swf = "/swf/video-js.swf";

    //THIS DISABLES THE HTML5 PLAYER
    //document.createElement("video").constructor.prototype.canPlayType = function(type){return ""};

    module.initPlayer = initPlayer;

}(window.playerModule));
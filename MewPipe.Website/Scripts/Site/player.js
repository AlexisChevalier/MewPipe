"use strict";

window.playerModule = window.playerModule || {};

(function (module) {

    var $elements = {
        player: $("#mewPipePlayerElement")
    },
    playerObject = null,
    videoDetails = null,
    videoEndpointUri = "http://videos-repository.mewpipe.local:44403/api/videosData/";

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
                var source = videoEndpointUri + videoDetails.PublicId + "?encoding=" + mimeType + "?quality=" + qualityType;
                sources.push({
                    type: format,
                    src: source,
                    "data-res": "720"
                });
                sources.push({
                    type: format,
                    src: source,
                    "data-res": "360"
                });
            }
        }
        return sources;
    }
    
    function initPlayer(videoData, done) {
        videoDetails = videoData;
        var format = detectVideoFormat();
        var sources = getVideoSources(format);

        videojs("mewPipePlayerElement", {
            plugins: {
                resolutionSelector: {
                    default_res: "240"
                }
            }
        }, function () {
            playerObject = this;
            console.log(sources);
            playerObject.src(sources);
            playerObject.on("changeRes", function () {
                console.log("Current Res is: " + player.getCurrentRes());
            });

            playerObject.play();

            this.on("ended", function () {
                console.log("awww...over so soon?");
            });
        });

        /*$elements.player.mediaelementplayer({
            type: format,
            plugins: ["flash", "silverlight"],
            features: ["playpause", "progress", "current", "duration", "volume", "fullscreen", "sourcechooser"],
            pluginPath: "/flash/",
            flashName: "flashmediaelement.swf",
            success: function (player, node) {
                playerObject = player;

                //playerObject.setSrc("http://videos-repository.mewpipe.local:44403/api/videosData/" + videoId);
                playerObject.setSrc([
                    { src: "http://videos-repository.mewpipe.local:44403/api/videosData/" + videoId + "?encoding=video/mp4&quality=low", type: 'video/mp4' },
                    { src: "http://videos-repository.mewpipe.local:44403/api/videosData/" + videoId + "?encoding=video/mp4&quality=high", type: 'video/mp4' },
                ]);

                if (typeof done === "function") {
                    done();
                }
            },
            error: function () {
                console.log("ERROR");
            }
        });*/
    }

    module.initPlayer = initPlayer;

}(window.playerModule));
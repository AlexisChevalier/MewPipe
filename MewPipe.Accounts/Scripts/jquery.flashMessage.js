var FlashMessages = (function () {
    var target,
        options = { timeout: 0, alert: "info" };


    var attach = function (selector, optionsObject) {
        target = selector;
        options = $.extend(options, optionsObject);

        if (!options.message) {
            setFlashMessageFromCookie(options);
        }
    };

    // GetStream the first alert message read from the cookie
    function setFlashMessageFromCookie() {
        $.each(new Array("Success", "Danger", "Warning", "Info"), function (i, alert) {
            var cookie = $.cookie("Flash." + alert);

            if (cookie) {
                options.message = cookie;
                options.alert = alert;

                redrawAlert();
                deleteFlashMessageCookie(alert);
                return;
            }
        });
    }

    // Delete the named flash cookie
    function deleteFlashMessageCookie(alert) {
        $.cookie("Flash." + alert, null, { path: "/" });
        $.removeCookie("Flash." + alert, { path: "/" });
    }



    function redrawAlert() {
        if (options.message && options.message) {
            $(target).addClass("alert-" + options.alert.toString().toLowerCase());

            if (typeof options.message === "string") {
                $("p", target).html("<span>" + options.message + "</span>");
            } else {
                target.empty().append(options.message);
            }
        } else {
            return;
        }

        if (target.children().length === 0) return;

        target.fadeIn().one("click", function () {
            $(this).fadeOut();
        });

        if (options.timeout > 0) {
            setTimeout(function () { target.fadeOut(); }, options.timeout);
        }
    };

    var setMessage = function (type, message) {
        options.message = message;
        options.alert = type;
        redrawAlert();
    };

    return {
        attach: attach,
        setMessage: setMessage
    };
}());
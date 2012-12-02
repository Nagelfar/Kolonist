var Kolonist;
(function (Kolonist) {
    var Util = {

        Degree2Rad: function (degree) {
            return Math.PI / 180 * degree;
        },

        nearestPow2: function (n) {
            var l = Math.log(n) / Math.LN2;
            return Math.pow(2, Math.ceil(l));
        },

        jsonPost: function (url, data) {
            return $.ajax({
                type:'POST',
                url: url,
                dataType: 'json',
                data: JSON.stringify(data),
                contentType: "application/json; charset=utf-8"
            });
        }


    };
    Kolonist.Util = Util;
})(Kolonist || (Kolonist = {}));
var Kolonist;
(function (Kolonist) {
    var Util = {

        Degree2Rad: function (degree) {
            return Math.PI / 180 * degree;
        },

        nearestPow2: function (n) {
            var l = Math.log(n) / Math.LN2;
            return Math.pow(2, Math.ceil(l));
        }

};
Kolonist.Util = Util;
})(Kolonist || (Kolonist = {}));
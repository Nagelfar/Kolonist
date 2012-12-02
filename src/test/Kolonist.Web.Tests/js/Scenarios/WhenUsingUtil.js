/// <reference path="../../../../app/kolonist.web/scripts/kolonist/util.js" />
/// <reference path="../jasmine.js"/>

describe("When calculating nearest power of two", function () {

    describe("And we start with 2", function () {
        var number = 2;
        var result = Kolonist.Util.nearestPow2(number);

        it("should be 2", function () {
            expect(result).toBe(number);
        });
    });
    
    describe("And we use 512", function () {
        var number = 512;
        var result = Kolonist.Util.nearestPow2(number);

        it("should also be 512", function () {
            expect(result).toBe(number);
        });
    });

    describe("And we use 3", function () {
        var number = 4;
        var result = Kolonist.Util.nearestPow2(number);

        it("should return 4 as the nearest number to 3", function () {
            expect(result).toBe(4);
        });
    });

    describe("and we use 600", function () {
        var number = 600;
        var result = Kolonist.Util.nearestPow2(number);

        it("should return 1024 as the nearest number to 600", function () {
            expect(result).toBe(1024);
        });
    });
});

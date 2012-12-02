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

describe("We calculating rads from degrees", function () {
    it("Should return 0 on 0 degrees", function () {
        var degrees = 0;

        var result = Kolonist.Util.Degree2Rad(degrees);

        expect(result).toBe(0);
    });

    it("should return PI on 180 degrees", function () {
        var degrees = 180;
        var result = Kolonist.Util.Degree2Rad(degrees);

        expect(result).toBeCloseTo(Math.PI);
    });

    it("should return PI/2 on 90 degrees", function () {
        var degrees = 90;
        var result = Kolonist.Util.Degree2Rad(degrees);

        expect(result).toBeCloseTo(Math.PI / 2.0);
    });
});